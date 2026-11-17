# Design

## Context

La motivación y el alcance están en `proposal.md`. El backend usa .NET 9, SQL Server, EF Core para persistencia y Dapper en las lecturas de solicitudes propias y administrativas. El catálogo de tarifas se consulta actualmente mediante repositorio EF Core.

`TarifaLicenciaFederativa` contiene `MediaTemporada` y un índice único `(Temporada, Licencia, Categoria, MediaTemporada)`. Su configuración precarga 50 tarifas con identificadores fijos. `SolicitudLicenciaFederativa` referencia la tarifa y tiene un índice único `(UserId, Temporada)`; el handler consulta una única solicitud por esa combinación antes de insertar. El correo se envía después de persistir y su fallo no revierte el alta.

El frontend Blazor está en `../blazorapp` y tiene su propio OpenSpec. Sus modelos, servicio, cliente HTTP, formulario e histórico usan el booleano; su especificación actual es `frontend-member-federative-license-request-flow`. Este cambio local documenta la integración en `frontend-handoff.md`, sin escribir en ese otro proyecto.

## Goals / Non-Goals

**Goals:**

- Sustituir el booleano por una relación explícita sin recrear las tarifas existentes ni alterar sus importes.
- Resolver la obligatoriedad de las fechas desde el tipo real de la tarifa, sin confiar en un dato adicional enviado por el cliente.
- Admitir varias solicitudes diarias manteniendo la exclusividad conjunta de completa/media ante concurrencia.
- Mantener contratos coherentes en las lecturas Dapper, el alta EF Core y la respuesta administrativa de cambio de estado.

**Non-Goals:**

- No introducir un motor de calendarios, restricciones por solapamiento o cálculo de importes por duración.
- No añadir dependencias de librerías ni un repositorio genérico nuevo.
- No duplicar el tipo en la solicitud: se obtiene de la tarifa referenciada.

## Decisions

### 1. Tipo como catálogo nominal con identidad estable

Crear `TipoLicencia` en el dominio de licencias con `int Id` y `string Nombre`, tabla `TiposLicencias`, nombre obligatorio de hasta 80 caracteres e índice único. Reservar identidades de datos iniciales mediante constantes del dominio: `1 = Completa`, `2 = Media 01/07 - 31/12`, `3 = Día`. El orden de consulta será el de estas identidades. Las reglas de solicitud compararán identidades estables y no nombres traducibles ni subcadenas.

`TarifaLicenciaFederativa` incorporará `TipoLicenciaId` obligatorio y navegación `TipoLicencia`; la relación impedirá borrados que dejen tarifas huérfanas. La nueva unicidad será `(Temporada, TipoLicenciaId, Licencia, Categoria)`. La creación de tarifas exigirá un tipo explícito, sin asignación implícita a completa.

Se descarta un enum como único almacenamiento porque se pide una entidad nominal relacionada. También se descarta usar el nombre como clave porque mezcla presentación e identidad. No se añaden endpoints de mantenimiento; los futuros cambios de catálogo se harán mediante migraciones.

### 2. Fechas sin hora y validación condicional

Añadir `DateOnly? FechaInicio` y `DateOnly? FechaFin` a la solicitud y columnas SQL Server `date NULL`. Usar JSON `yyyy-MM-dd`; `FechaCreacionUtc` conserva su función de auditoría y su tipo actual.

La factoría de dominio recibirá la tarifa y ambas fechas. Para `Día` serán obligatorias; para los demás tipos podrán ser ambas nulas. Si se informa una cobertura, se exigirá la pareja completa, inicio menor o igual que fin y ambas fechas dentro del año de `Temporada`. Se admite inicio igual a fin para un solo día. Se propone considerar la temporada como año natural, coherentemente con el catálogo anual existente. No se agrega un límite de duración ni una prohibición de fechas pasadas.

La validación estructural de formato y pareja de fechas estará en el contrato/validador; la comprobación condicional se hará después de cargar la tarifa y se repetirá en el dominio. Los errores de fechas devolverán `400` con ProblemDetails en español, no el conflicto `409` reservado a duplicidad funcional. Se añadirán restricciones de persistencia para pareja nula/completa, orden y año cuando haya cobertura; la obligatoriedad según tipo se asegura en el flujo de escritura al depender de otra tabla.

Se descarta guardar horas UTC para una cobertura expresada por días, y rellenar automáticamente la cobertura de completa/media: generaría fechas que no aportó el usuario ni existen en el histórico.

### 3. Multiplicidad por tipo sin duplicar clasificación

La respuesta del usuario confirma varias solicitudes `Día` por temporada. Se mantendrá como máximo una solicitud no diaria, conjunta entre completa y media, contando también canceladas como hasta ahora. Las diarias podrán coexistir con ella y entre sí. No se introduce una regla adicional de rechazo por intervalos iguales o solapados en este alcance.

El índice global único `(UserId, Temporada)` pasará a ser no único y servirá para consultar y bloquear el conjunto del usuario y temporada. Se sustituirá `GetByUserIdAndTemporadaAsync`, que presupone un resultado único, por consulta del conjunto y comprobación de existencia de solicitudes no diarias.

Para no perder la protección concurrente se extenderá el repositorio/abstracción de escritura con una operación de alta transaccional. En SQL Server se abrirá una transacción serializable y se leerá el rango de solicitudes del usuario y temporada con `UPDLOCK, HOLDLOCK`, respaldado por el índice, incluso si está vacío. Dentro de la misma transacción se cargarán los tipos necesarios, se comprobará la regla y se guardará la solicitud. Todo alta, diaria o no, pasará por esta operación; la ausencia de solicitudes previas no podrá saltarse el bloqueo.

El correo se enviará una vez confirmado el commit. Un conflicto no generará correo ni una solicitud parcial. La consulta de tipos y las lecturas del histórico no requieren este bloqueo.

Se descarta mantener el índice actual porque prohíbe la multiplicidad solicitada. Un índice filtrado por tipo no puede consultar la tabla de tarifas; duplicar `TipoLicenciaId` en cada solicitud exigiría integridad adicional. La transacción conserva una única fuente de clasificación, a costa de serializar las altas de un usuario y temporada. Toda escritura futura deberá respetar la misma operación; una inserción SQL directa podría saltarse esta regla de aplicación.

### 4. Contratos HTTP y lecturas

| Operación | Contrato propuesto |
| --- | --- |
| `GET /api/licencias-federativas/tipos` | Autenticado, cualquier rol; lista ordenada con `Id`, `Nombre`. |
| `GET /api/licencias-federativas/tarifas` | Filtros opcionales `temporada`, `tipoLicenciaId`; cada tarifa incorpora `TipoLicenciaId`, `TipoLicenciaNombre`. |
| `POST /api/licencias-federativas/me/solicitudes` | Mantiene `ClubMember`; recibe `Temporada`, `TarifaLicenciaFederativaId`, `FechaInicio?`, `FechaFin?`. |
| `GET .../me/solicitudes` y `GET .../me/solicitudes/{id}` | Mantienen aislamiento por identidad; incorporan tipo y fechas, sin colapsar varias solicitudes de una temporada. |
| `GET .../admin/solicitudes` | Mantiene `Admin` y filtros actuales; incorpora tipo y fechas. |
| `PUT .../admin/users/{userId}/solicitudes/{id}/estado` | Conserva un cuerpo limitado al estado; devuelve tipo y fechas sin alterarlos. |

Los DTOs de tarifas, solicitudes y solicitudes administrativas sustituirán el booleano por `TipoLicenciaId` y `TipoLicenciaNombre`; los de solicitud añadirán fechas. El tipo del alta se deriva de `TarifaLicenciaFederativaId`, sin pedir al cliente otra clasificación ni precio.

Las consultas Dapper de `GetMisSolicitudes`, `GetMiSolicitud` y `GetAdminSolicitudes` harán join con `TiposLicencias` y proyectarán las fechas nuevas. La respuesta del alta y el mapeo de cambio de estado cargarán también la navegación de tipo. Revisar específicamente la materialización de `DateOnly?` con Dapper y los proveedores de pruebas; si requiere adaptación, se centralizará la conversión y se probará contra SQL Server.

El orden de tarifas será temporada descendente, tipo por identificador, licencia, categoría, territorio e identificador de tarifa. Las solicitudes propias tendrán fecha de creación descendente e identificador como desempate.

El filtro `mediaTemporada` se retirará y su presencia se rechazará explícitamente con `400` y mensaje de sustitución. ASP.NET ignora parámetros desconocidos por defecto; esta comprobación evita devolver por error el catálogo completo a clientes antiguos. Los `tipoLicenciaId` no positivos o mal formados darán `400`; un identificador positivo sin coincidencias devolverá colección vacía.

Se descarta devolver un booleano calculado porque clasificaría incorrectamente `Día` como una de las variantes antiguas. La publicación requiere actualizar el cliente; no se introduce una capa de compatibilidad indefinida.

### 5. Selección y precio en el cliente

La lectura de tipos permite mostrar los tres tipos incluso si `Día` aún no dispone de tarifas. El cliente consultará tarifas tras elegir temporada y tipo, deduplicará licencias y categorías y resolverá un único identificador. El importe seguirá siendo `PrecioClub`; `PrecioIndependiente` se conserva como dato independiente.

Cambiar temporada o tipo limpiará licencia, categoría, tarifa resuelta y precio; cambiar licencia limpiará categoría y precio. Al salir de `Día` se limpiarán sus fechas para evitar enviar cobertura residual. El formulario diario no podrá confirmar sin fechas válidas. Un catálogo vacío impedirá confirmar y explicará que no hay tarifas disponibles. Véase `frontend-handoff.md` para aceptación y rutas afectadas.

Se descarta crear una tarifa diaria por defecto sin fuente de importes. Los escenarios diarios se probarán con datos de prueba; el uso real quedará disponible cuando se carguen tarifas aprobadas.

## Risks / Trade-offs

- [EF genera actualizaciones de datos iniciales que sobrescriben precios] → Revisar el SQL y probar tarifas con importes personalizados; actualizar solo el tipo de registros existentes, sin `DeleteData`/`InsertData` que los recreen.
- [Eliminar el índice único permite duplicidades no diarias] → Encapsular comprobación e inserción en la transacción y probar carreras reales en SQL Server, incluido el rango inicialmente vacío.
- [El cliente publicado todavía usa el booleano] → Coordinar versiones de API y frontend y detener las altas durante la migración del esquema.
- [Dapper y los proveedores de pruebas manejan de forma distinta fechas sin hora] → Probar round-trip de nulos y coberturas en SQL Server, además de las pruebas HTTP con el proveedor de test.
- [El rollback pierde información diaria o multiplicidad] → Validación previa que aborte cualquier `Down` incompatible antes de cambios de esquema o datos.
- [La regla no diaria no tiene un índice único simple] → Mantener un único punto transaccional de escritura y documentar que las cargas directas de solicitudes deben respetarlo.

## Migration Plan

1. Preparar una migración EF Core descriptiva y su snapshot; no modificar migraciones históricas. Revisar el script SQL generado antes de ejecutarlo.
2. Crear `TiposLicencias` y precargar las identidades estables. Añadir `TipoLicenciaId` temporalmente anulable en tarifas y las dos fechas anulables en solicitudes.
3. Actualizar **todas** las tarifas mediante su booleano actual, sin filtrar por temporada ni por los 50 identificadores iniciales. No tocar importes ni claves primarias. Verificar que todas tienen un tipo válido antes de hacerlo obligatorio y crear la FK restrictiva.
4. Sustituir el índice único de tarifas por la nueva combinación con tipo y cambiar el índice de solicitudes a no único. Conservar las referencias a tarifas y las fechas históricas nulas.
5. Retirar el índice anterior y la columna `MediaTemporada` cuando ya exista la asignación completa. Actualizar `HasData` para instalaciones nuevas conservando los identificadores de tarifas y sin generar tarifas `Día`.
6. Probar una instalación nueva y una actualización desde la migración inmediatamente anterior en una base SQL Server aislada. En la actualización incluir tarifas completas/medias, temporadas adicionales, importes personalizados, independientes nulos y solicitudes en distintos estados; comparar todos los datos y referencias antes/después.
7. Adaptar el frontend en su proyecto y verificar el flujo completo usando tarifas diarias de prueba. Detener temporalmente las escrituras de licencias durante el despliegue coordinado; migrar y publicar API/cliente compatibles antes de reanudar.
8. El `Down` validará primero si hay tarifas de tipos no representables, fechas informadas o multiplicidad incompatible. En esos casos abortará sin cambios. Solo sobre datos compatibles reconstruirá el booleano, índices anteriores y eliminará las columnas/tablas nuevas sin borrar tarifas ni solicitudes. Para revertir con datos nuevos se preparará una migración de conservación específica.

## Verification

- Dominio: fechas obligatorias por tipo, pareja opcional, mismo día, orden y año; estados y coherencia de temporada actuales.
- Application: tipo real de la tarifa, filtros, alta diaria múltiple y convivencia, conflicto no diario y correo después del commit.
- Integración HTTP: tipos, filtros, rechazo del booleano antiguo, JSON de fechas, alta/listado/detalle y cambio administrativo, autorización y propiedad.
- SQL Server: FK, unicidad tarifaria, migración sobre datos existentes, instalación nueva, materialización Dapper, carreras diarias/no diarias y rollback compatible/bloqueado.
- Frontend: selección encadenada, limpieza de dependencias, precio, fechas, ausencia de tarifas diarias, varias solicitudes por temporada y aislamiento del historial.
- Cierre: `dotnet test IndaloAventurApi.sln`, `dotnet build IndaloAventurApi.sln --no-restore` y `openspec validate add-federative-license-types --strict`; registrar por separado cualquier prueba SQL Server omitida, sin considerarla una verificación superada.
