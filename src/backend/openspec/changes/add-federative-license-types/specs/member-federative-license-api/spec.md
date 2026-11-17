## MODIFIED Requirements

### Requirement: El sistema MUST permitir al usuario autenticado crear su propia solicitud de licencia federativa
El sistema MUST registrar solicitudes del usuario del token sin aceptar `UserId` del cliente. El alta SHALL recibir `Temporada`, `TarifaLicenciaFederativaId`, `FechaInicio` y `FechaFin` anulables según tipo y devolver estado `Pendiente`, cobertura, `TipoLicenciaId`, `TipoLicenciaNombre`, `Licencia`, `Categoria`, `PrecioClub` y `PrecioIndependiente`. La multiplicidad SHALL permitir varias solicitudes `Día` y una sola no diaria por usuario y temporada.

#### Scenario: Alta correcta de solicitud propia
- **WHEN** un usuario autenticado con `IsMember = true` envía una tarifa existente de la misma temporada y una cobertura válida según el tipo
- **THEN** el sistema SHALL crear la solicitud ligada al `NameIdentifier` del token y devolver `201 Created` con tipo, cobertura y estado `Pendiente`

#### Scenario: Usuario no socio intenta solicitar licencia
- **WHEN** un usuario autenticado con `IsMember = false` intenta crear una solicitud
- **THEN** el sistema SHALL rechazarla con una respuesta de autorización sin persistirla

#### Scenario: Solicitud duplicada para la misma temporada
- **WHEN** un usuario con una solicitud no diaria intenta crear otra no diaria en esa temporada
- **THEN** el sistema SHALL devolver `409 Conflict` con un mensaje en español

#### Scenario: Segunda solicitud diaria de la misma temporada
- **WHEN** un socio con una solicitud `Día` envía otra solicitud diaria válida para distintas fechas de la misma temporada
- **THEN** el sistema SHALL devolver `201 Created` con un nuevo identificador y conservar ambas solicitudes

#### Scenario: Solicitud diaria sin fechas
- **WHEN** un socio envía una tarifa `Día` sin ambas fechas requeridas
- **THEN** el sistema SHALL devolver `400 Bad Request` con ProblemDetails en español y SHALL no persistir una solicitud

#### Scenario: Solicitud no diaria sin fechas
- **WHEN** un socio envía una tarifa `Completa` o `Media` sin fechas y no tiene otra solicitud no diaria de esa temporada
- **THEN** el sistema SHALL crear la solicitud con ambas fechas nulas

#### Scenario: Tarifa inexistente o temporada incoherente
- **WHEN** el socio informa una tarifa inexistente o una tarifa de temporada distinta
- **THEN** el sistema SHALL rechazar el alta con el contrato funcional vigente y SHALL no persistir la solicitud

### Requirement: El sistema MUST permitir al usuario autenticado consultar MIS solicitudes de licencia federativa
El sistema MUST devolver exclusivamente las solicitudes del usuario autenticado, ordenadas de más reciente a más antigua con desempate determinista. Cada elemento SHALL incluir identificador, temporada, estado, creación, cobertura, `TipoLicenciaId`, `TipoLicenciaNombre`, `Licencia`, `Categoria`, `Territorio`, `PrecioClub` y `PrecioIndependiente`, sustituyendo `MediaTemporada`.

#### Scenario: Consulta del historial propio con resultados
- **WHEN** un usuario autenticado consulta su historial y tiene solicitudes
- **THEN** el sistema SHALL devolver solo sus solicitudes con temporada, estado, cobertura y detalle de tarifa y tipo

#### Scenario: Consulta del historial propio sin resultados
- **WHEN** un usuario autenticado consulta su historial sin solicitudes
- **THEN** el sistema SHALL devolver `200 OK` con una colección vacía

#### Scenario: Aislamiento entre usuarios en la consulta
- **WHEN** hay solicitudes de varios usuarios y un usuario consulta su historial
- **THEN** el sistema SHALL excluir las de cualquier otra identidad

#### Scenario: Varias solicitudes de la misma temporada
- **WHEN** un usuario tiene varias solicitudes `Día` y una no diaria en la misma temporada
- **THEN** el sistema SHALL devolverlas todas, sin agruparlas en una sola ni descartar registros por temporada

#### Scenario: Solicitudes históricas sin cobertura
- **WHEN** se consulta el historial después de migrar solicitudes antiguas
- **THEN** el sistema SHALL devolver el tipo de cada tarifa migrada y ambas fechas de cobertura nulas

### Requirement: El sistema MUST permitir al usuario autenticado consultar el detalle completo de una solicitud propia
El sistema MUST permitir consultar por identificador una solicitud del usuario autenticado. La respuesta SHALL incluir temporada, estado, creación, cobertura, `TipoLicenciaId`, `TipoLicenciaNombre`, licencia, categoría, territorio, `PrecioClub` y `PrecioIndependiente`, sustituyendo `MediaTemporada`.

#### Scenario: Detalle de una solicitud propia existente
- **WHEN** un usuario solicita el detalle de una solicitud que le pertenece
- **THEN** el sistema SHALL devolver todos sus datos y los de la tarifa, incluidos tipo y fechas de cobertura

#### Scenario: Solicitud de detalle sobre una solicitud ajena
- **WHEN** un usuario solicita una solicitud de otro usuario
- **THEN** el sistema SHALL responder como recurso no encontrado o inaccesible sin exponer datos ajenos

#### Scenario: Solicitud de detalle inexistente
- **WHEN** un usuario solicita una solicitud inexistente
- **THEN** el sistema SHALL devolver `404 Not Found`
