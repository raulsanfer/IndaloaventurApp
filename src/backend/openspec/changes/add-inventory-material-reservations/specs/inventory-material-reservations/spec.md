## Purpose

Permite reservar unidades de material del club con disponibilidad fiable, devolucion confirmada y comunicacion trazable entre el socio y administracion.

## ADDED Requirements

### Requirement: Material de inventario reservable y activo

El sistema MUST incorporar el indicador booleano `Activo` a cada material de inventario. Un administrador MUST poder activarlo o desactivarlo al crear o editar el material. Los materiales existentes MUST conservarse y quedar activos al aplicar la migracion. Un material inactivo MUST quedar excluido de nuevas reservas, sin cancelar ni borrar sus reservas historicas o ya comprometidas.

El sistema MUST calcular las unidades disponibles de un material como `Cantidad` menos la suma de `CantidadSolicitada` de sus reservas en estado `Solicitada` o `Retirada`. Un alta o edicion de reserva MUST rechazar una cantidad positiva que exceda las unidades disponibles. El sistema MUST impedir que una actualizacion administrativa reduzca `Cantidad` por debajo de las unidades comprometidas por reservas solicitadas o retiradas.

#### Scenario: Reserva parcial de existencias disponibles
- **WHEN** un material activo tiene cinco unidades, una reserva solicitada compromete dos y un socio solicita tres unidades para un periodo compatible
- **THEN** el sistema acepta la segunda reserva y deja cero unidades disponibles para nuevas reservas

#### Scenario: Rechazo por unidades insuficientes o material inactivo
- **WHEN** un socio solicita mas unidades de las disponibles o solicita cualquier cantidad de un material inactivo
- **THEN** el sistema rechaza la operacion como conflicto sin crear ni modificar una reserva

#### Scenario: Desactivacion conserva compromisos existentes
- **WHEN** un administrador desactiva un material que tiene reservas solicitadas o retiradas
- **THEN** el sistema conserva esas reservas y rechaza solamente nuevas solicitudes para ese material

### Requirement: Registro de reserva de un unico material

El sistema MUST registrar cada reserva con un identificador, el socio propietario, un unico material, `CantidadSolicitada`, `FechaSolicitudUtc`, `FechaRetiradaPrevista`, `FechaRetirada`, `FechaDevolucionEstimada`, `FechaDevolucionEfectiva`, observaciones del socio, observacion administrativa de cancelacion, estado y las confirmaciones de devolucion del socio y de administracion.

`FechaSolicitudUtc` MUST asignarse automaticamente. `CantidadSolicitada`, `FechaRetiradaPrevista` y `FechaDevolucionEstimada` MUST ser obligatorias al solicitar; la cantidad MUST ser positiva y la fecha estimada de devolucion MUST ser igual o posterior a la retirada prevista. Las fechas efectivas de retirada y devolucion MUST permanecer vacias hasta que administracion registre los hechos correspondientes.

#### Scenario: Solicitud valida con fechas previstas
- **WHEN** un socio solicita una cantidad disponible de un material activo con retirada prevista y devolucion estimada validas
- **THEN** el sistema crea una reserva en estado `Solicitada`, asigna la fecha de solicitud y no informa fechas efectivas

#### Scenario: Fechas o cantidad invalidas
- **WHEN** una solicitud contiene cantidad cero o negativa, o una devolucion estimada anterior a la retirada prevista
- **THEN** el sistema rechaza la solicitud con un error de validacion en Espanol y no altera la disponibilidad

### Requirement: Ciclo de vida controlado de la reserva

El sistema MUST limitar el estado de una reserva a `Solicitada`, `Retirada`, `Entregada` o `Cancelada`. Toda reserva nueva MUST iniciar en `Solicitada`.

Solo administracion MUST poder pasar una reserva de `Solicitada` a `Retirada`, registrando `FechaRetirada`. El estado `Entregada` MUST representar que el material ha vuelto al club: solo puede alcanzarse desde `Retirada` cuando consten las confirmaciones de devolucion del socio y de administracion, y administracion registre `FechaDevolucionEfectiva`. La fecha efectiva de devolucion MUST no ser anterior a la fecha efectiva de retirada.

Una reserva `Solicitada` puede pasar a `Cancelada` por su socio propietario o por administracion. Una reserva `Retirada` solo puede pasar a `Cancelada` por administracion y MUST incluir una observacion administrativa que explique la cancelacion. `Entregada` y `Cancelada` MUST ser estados finales.

#### Scenario: Retirada registrada por administracion
- **WHEN** un administrador entrega el material solicitado al socio
- **THEN** el sistema registra la fecha efectiva de retirada y cambia la reserva de `Solicitada` a `Retirada`

#### Scenario: Devolucion confirmada por ambas partes
- **WHEN** una reserva retirada tiene la confirmacion del socio, la confirmacion administrativa y una fecha efectiva de devolucion valida
- **THEN** el sistema cambia su estado a `Entregada` y sus unidades dejan de contar como comprometidas

#### Scenario: Cancelacion de material ya retirado
- **WHEN** un administrador cancela una reserva en estado `Retirada` sin aportar observacion administrativa
- **THEN** el sistema rechaza la cancelacion y conserva la reserva en estado `Retirada`

### Requirement: Autoservicio limitado al propietario socio

El sistema MUST permitir crear reservas solo a usuarios autenticados que sean socios reales (`IsMember=true`) y dispongan de una ficha de socio existente. El rol tecnico `Member` por si solo MUST ser insuficiente.

El socio MUST poder consultar exclusivamente sus propias reservas. Mientras el estado sea `Solicitada`, el socio propietario MUST poder editar el material, la cantidad, las fechas previstas y sus observaciones, siempre que la disponibilidad siga siendo valida, o cancelarla de forma logica. Tras la retirada, el socio MUST no poder modificar esos datos ni cancelar la reserva.

El socio propietario MUST poder registrar exclusivamente su propia confirmacion de devolucion cuando la reserva este en estado `Retirada`; esa operacion no le concede capacidad para cambiar estado ni fechas efectivas.

#### Scenario: Socio consulta y modifica su solicitud pendiente
- **WHEN** un socio con ficha consulta o edita una de sus reservas en estado `Solicitada`
- **THEN** el sistema muestra solo sus reservas y acepta el cambio si conserva una cantidad disponible y fechas validas

#### Scenario: Usuario sin condicion de socio o recurso ajeno
- **WHEN** un usuario con `IsMember=false`, sin ficha de socio, o propietario de otra reserva intenta crear, consultar o modificar una reserva ajena
- **THEN** el sistema deniega la operacion sin exponer los datos de la reserva legitima

#### Scenario: Socio intenta alterar una reserva retirada
- **WHEN** el propietario intenta editar, cancelar o cambiar las fechas de una reserva en estado `Retirada`
- **THEN** el sistema rechaza la operacion y conserva los datos registrados

### Requirement: Gestion administrativa de reservas

El sistema MUST permitir a administracion consultar todas las reservas y gestionar las transiciones de retirada, cancelacion y entrega. La consulta administrativa MUST incluir socio, material, cantidad, fechas, estado y ambas confirmaciones. Administracion MUST poder registrar su confirmacion de devolucion solamente para una reserva retirada y no puede alterar una reserva finalizada.

#### Scenario: Administracion confirma la devolucion pendiente
- **WHEN** un administrador registra la confirmacion administrativa y la fecha efectiva de devolucion de una reserva retirada cuya confirmacion del socio ya existe
- **THEN** el sistema completa la reserva como `Entregada`

#### Scenario: Usuario no administrador intenta gestionar una reserva ajena
- **WHEN** un usuario autenticado sin rol `Admin` invoca una operacion administrativa de retirada, entrega o cancelacion posterior
- **THEN** el sistema responde con autorizacion insuficiente y no modifica la reserva

### Requirement: Disponibilidad consistente bajo concurrencia

El sistema MUST evaluar y confirmar la disponibilidad de unidades como una unica operacion atomica al crear o editar una reserva, registrar una retirada o devolver/cancelar unidades, y al modificar la cantidad total de un material. Dos operaciones concurrentes MUST no poder comprometer en conjunto mas unidades que las existentes.

#### Scenario: Solicitudes concurrentes sobre las ultimas unidades
- **WHEN** dos socios solicitan simultaneamente cantidades que por separado caben en las ultimas unidades disponibles pero juntas las exceden
- **THEN** el sistema confirma como maximo una de las operaciones que excederia el limite y rechaza la otra como conflicto

### Requirement: Notificaciones de trazabilidad de reservas

El sistema MUST intentar enviar una notificacion al correo del socio y al destinatario administrativo configurable cuando se cree una reserva, cambie su estado, se registre la confirmacion del socio o se registre la confirmacion administrativa. El mensaje MUST incluir como minimo identificador de reserva, material, cantidad, socio, estado, fechas relevantes y la observacion administrativa cuando corresponda.

Las plantillas MUST usar asuntos genericos equivalentes a `Solicitud de reserva #{id}: {material}`, `Actualizacion de reserva #{id}: {estado}`, `Confirmacion de devolucion del socio #{id}` y `Confirmacion administrativa de devolucion #{id}`. El envio de correo MUST ejecutarse despues de persistir la operacion; un fallo de entrega MUST no revertir el cambio de reserva y MUST quedar registrado para diagnostico.

#### Scenario: Alta de reserva notificada
- **WHEN** el sistema persiste una nueva reserva solicitada
- **THEN** intenta enviar al socio y al destinatario administrativo la notificacion de solicitud con sus datos esenciales

#### Scenario: Cambio o confirmacion notificado
- **WHEN** una reserva cambia de estado o una de las partes registra su confirmacion de devolucion
- **THEN** el sistema intenta enviar a ambos destinatarios el evento y su estado actualizado

#### Scenario: Fallo del proveedor de correo
- **WHEN** el proveedor de correo falla despues de que una operacion de reserva se haya persistido
- **THEN** el sistema conserva la reserva modificada y registra el fallo sin exponer detalles internos al cliente

### Requirement: Contrato HTTP y autorizacion de reservas

El sistema MUST exponer rutas de autoservicio bajo `/api/inventario-reservas/me` para crear, listar, obtener detalle, editar solicitudes, cancelar solicitudes y confirmar la devolucion propia. Estas rutas MUST exigir la condicion de socio real y aislar la identidad efectiva a la del token.

El sistema MUST exponer rutas administrativas bajo `/api/inventario-reservas/admin` para listar, obtener detalle, registrar retirada, cancelar, confirmar la devolucion administrativa y completar la entrega. Estas rutas MUST exigir el rol `Admin`. Las operaciones correctas MUST responder con `200`, `201` o `204` segun corresponda; los errores de validacion, inexistencia, conflicto y autorizacion MUST usar el contrato de errores de la API en Espanol. Toda ruta MUST constar en la matriz de autorizacion.

#### Scenario: Reserva HTTP creada por socio real
- **WHEN** un socio real crea una reserva valida mediante la ruta de autoservicio
- **THEN** la API devuelve `201 Created` con el identificador y una localizacion al detalle propio

#### Scenario: Endpoint protegido de reserva
- **WHEN** un cliente anonimo, un usuario sin condicion de socio o un usuario sin rol administrativo llama a una ruta de reservas que no le corresponde
- **THEN** la API rechaza la llamada con el codigo de autorizacion adecuado y no revela datos de reservas ajenas
