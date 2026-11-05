## Context

Vease `proposal.md` y `specs/inventory-material-reservations/spec.md`. El cambio activo `add-inventory-item-management` incorpora `InventarioMaterial` con cantidad y lecturas CQRS, pero excluye expresamente prestamos y reservas. El proyecto usa .NET 9, SQL Server, EF Core para escritura, Dapper para lecturas, MediatR, FluentValidation, ProblemDetails en Espanol y una matriz de autorizacion. La politica `ClubMember` ya representa `IsMember=true`; no debe sustituirse por el rol tecnico `Member`.

`InventarioEstado` es un catalogo nominal sin semantica de disponibilidad. El servicio de correo actual persiste primero y trata los fallos de envio como no bloqueantes, registrandolos mediante logging.

## Goals / Non-Goals

**Goals:**

- Mantener una asignacion de unidades consistente incluso cuando se solicitan de forma simultanea.
- Conservar el historial funcional de la reserva y las reglas de propiedad sin permitir que el socio gestione una retirada.
- Mantener material retirado del catalogo de nuevas reservas mediante una baja logica, no un borrado.
- Reutilizar los mecanismos de autenticacion, correo y manejo de errores existentes.

**Non-Goals:**

- No se implementan pantallas de frontend, calendario visual, recordatorios automaticos ni prestamos recurrentes.
- No se descomponen lotes sin numero de serie en activos individuales ni se registran movimientos de almacen fuera de una reserva.
- No se introducen aprobaciones previas a la solicitud ni pagos, depositos, sanciones, fotografias o partes de danos.
- No se garantiza reintento automatico de correo en esta fase; los fallos quedaran registrados despues de persistir la operacion.

## Decisions

### Modelo de reserva y fechas

Se creara el agregado `ReservaMaterial` con `Guid` como identificador, `UserId`, `InventarioMaterialId`, `CantidadSolicitada`, `FechaSolicitudUtc`, `FechaRetiradaPrevista`, `FechaRetirada`, `FechaDevolucionEstimada`, `FechaDevolucionEfectiva`, `Observaciones`, `ObservacionAdministracion`, `Estado`, `ConfirmacionDevolucionSocio` y `ConfirmacionDevolucionAdmin`.

La solicitud se registra con marca UTC porque es un evento de auditoria; retirada y devolucion se modelan como `DateOnly` porque el negocio ha definido fechas y no franjas horarias. La devolucion estimada se valida contra la retirada prevista y la efectiva contra la retirada efectiva. Se mantiene una observacion administrativa separada para no sobrescribir el comentario inicial del socio al cancelar una retirada.

Se descarta reutilizar `InventarioMaterial` como la reserva: un material representa existencias y puede tener multiples reservas a lo largo del tiempo, mientras la reserva necesita propietario, cantidades, fechas y ciclo de vida propios.

### Estados y confirmaciones

El enum cerrado sera `Solicitada`, `Retirada`, `Entregada` y `Cancelada`. `Entregada` significa que el material ha sido devuelto al inventario del club; `Retirada` significa entregado al socio. La reserva permanece en `Retirada` mientras llega una o ambas confirmaciones. Solo el administrador registra la retirada y la fecha efectiva de devolucion; el socio solamente puede establecer su propia confirmacion durante la devolucion.

Las transiciones y permisos se aplicaran dentro del agregado y se repetirán en los handlers para producir respuestas funcionales claras. No se permitira borrado fisico: el DELETE de autoservicio se traducira en cancelacion desde `Solicitada`.

### Activo y disponibilidad por unidades

`InventarioMaterial` recibira `Activo`, obligatorio y con valor predeterminado `true` para los registros existentes. `Activo=false` bloquea nuevas solicitudes, pero no modifica ni libera reservas existentes.

La disponibilidad sera `Cantidad - SUM(CantidadSolicitada)` de reservas `Solicitada` y `Retirada`. Al editar una reserva se excluira su cantidad actual antes de validar el nuevo material o cantidad. La cantidad total del material no podra disminuir por debajo de la suma comprometida. Un material serializado ya tiene cantidad uno, por lo que la misma regla limita naturalmente una reserva a una unidad.

Las mutaciones que puedan alterar el total comprometido usaran una transaccion SQL Server serializable y un bloqueo de actualizacion sobre el material y sus reservas activas. Esto evita que dos peticiones vean las mismas unidades libres y las asignen dos veces. Se descarta una comprobacion previa sin bloqueo porque no protege frente a carreras.

### Autorizacion y consultas

Las operaciones de socio se derivaran siempre del `UserId` del token y se protegeran con `ClubMember`; el handler comprobara tambien que existe `FichaSocio` para el usuario. La comprobacion explicita evita que un claim valido sin ficha satisfaga una regla de dominio incompleta. Las operaciones administrativas requeriran `Admin` y no dependeran de titularidad.

Las escrituras emplearan repositorios EF Core para cargar el material y las reservas activas con bloqueo/transaccion. Las lecturas Dapper proyectaran DTOs separados para `me` y administracion, incluyendo la disponibilidad del material cuando sea necesaria. No se reutilizaran los agregados para respuesta HTTP.

### Persistencia e integridad

Se añadira `ReservasMaterial` con claves foraneas restrictivas a `AspNetUsers` e `InventarioMateriales`. Tendrá indices para `InventarioMaterialId + Estado` y `UserId + FechaSolicitudUtc`, y una columna de concurrencia apropiada para EF Core. La configuracion del material añadira `Activo` y el mapeo de reservas no permitira nulos en propietario, material, cantidad, solicitud, retirada prevista, devolucion estimada y estado.

La migracion asignara `Activo = true` a los materiales existentes. No se realizara borrado en cascada de usuarios o materiales con reservas, para proteger el historico.

### Notificaciones

Se creara una opcion especifica para el destinatario administrativo de reservas y un servicio de notificacion que construya versiones HTML y texto plano con las cuatro plantillas genericas definidas en la especificacion. Los handlers guardaran primero los cambios y despues invocaran el envio para socio y administracion. Los fallos se capturaran y registraran con el identificador de reserva, sin convertir una operacion ya persistida en fallo HTTP.

Se descarta reutilizar directamente el destinatario de licencias federativas porque ambas responsabilidades pueden requerir buzones distintos. Tambien se descarta un outbox con reintentos en esta fase: aportaria mayor entrega garantizada, pero requiere una operacion de background y una politica de reintento que aun no se ha definido.

### Superficie HTTP

Se utilizara la ruta base `api/inventario-reservas`. Bajo `me` se incluiran lista, detalle, creacion, actualizacion de solicitud, cancelacion y confirmacion propia. Bajo `admin` se incluiran lista, detalle, retirada, cancelacion, confirmacion administrativa y cierre de devolucion. La matriz de autorizacion registrara cada ruta como `ClubMember` o `Admin`; las pruebas cubriran anonimos, `IsMember=false`, ausencia de ficha, titularidad cruzada y rol administrativo insuficiente.

## Risks / Trade-offs

- [La suma de reservas activas se lee y actualiza simultaneamente] -> Transaccion serializable con bloqueo sobre el material para conservar la disponibilidad.
- [El administrador reduce el stock mientras hay reservas] -> Revalidar compromisos activos dentro de la misma transaccion y devolver conflicto.
- [Desactivar material podria parecer una cancelacion] -> Mantener reservas existentes sin cambios y comunicar que solo se impiden nuevas solicitudes.
- [Un correo puede fallar tras guardar] -> Registrar el fallo y no deshacer la reserva; un outbox se podra incorporar cuando se defina politica operativa de reintentos.
- [El claim del token y la ficha pueden desalinearse] -> Aplicar la politica `ClubMember` y comprobar la ficha en el caso de uso.

## Migration Plan

1. Integrar y verificar primero `add-inventory-item-management`, incluida su tabla de materiales y contratos de lectura.
2. Añadir el campo `Activo` con valor por defecto `true`, el agregado de reserva, sus claves, indices y migracion revisada.
3. Desplegar la API con las rutas y configuracion de correo de reservas; los materiales previos quedaran reservables hasta que administracion los desactive.
4. Para revertir antes de crear reservas, retirar rutas y ejecutar `Down` eliminando primero claves e indices de reservas y despues la tabla y columna. Con reservas en produccion, cualquier retirada del esquema requerira una migracion de conservacion de historico.
