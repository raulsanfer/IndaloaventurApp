## ADDED Requirements

### Requirement: El sistema MUST permitir al usuario autenticado crear su propia solicitud de licencia federativa
El sistema MUST exponer un endpoint autenticado de autoservicio para registrar una `SolicitudLicenciaFederativa` asociada al usuario del token, sin aceptar un `UserId` informado por el cliente. La solicitud SHALL incluir al menos la `Temporada` y la `TarifaLicenciaFederativaId`, y el alta resultante SHALL quedar en estado inicial `Pendiente`.

#### Scenario: Alta correcta de solicitud propia
- **WHEN** un usuario autenticado con claim `IsMember = true` envia una solicitud valida con una tarifa existente de la misma temporada
- **THEN** el sistema SHALL crear la solicitud ligada al `NameIdentifier` del token y devolver la representacion creada en estado `Pendiente`

#### Scenario: Usuario no socio intenta solicitar licencia
- **WHEN** un usuario autenticado con claim `IsMember = false` intenta crear una solicitud de licencia federativa
- **THEN** el sistema SHALL rechazar la operacion con una respuesta de autorizacion y no persistira ninguna solicitud

#### Scenario: Solicitud duplicada para la misma temporada
- **WHEN** un usuario autenticado intenta crear una segunda solicitud para una temporada en la que ya tiene una solicitud registrada
- **THEN** el sistema SHALL rechazar la operacion con un conflicto funcional y un mensaje en espanol

### Requirement: El sistema MUST permitir al usuario autenticado consultar MIS solicitudes de licencia federativa
El sistema MUST exponer una consulta autenticada de "mis solicitudes" que recupere exclusivamente las solicitudes del usuario autenticado, ordenadas de la mas reciente a la mas antigua. Cada elemento devuelto SHALL incluir el identificador de solicitud, temporada, estado, fecha de creacion y la informacion funcional de la tarifa asociada necesaria para el cliente.

#### Scenario: Consulta del historial propio con resultados
- **WHEN** un usuario autenticado consulta sus solicitudes y existen registros asociados a su identidad
- **THEN** el sistema SHALL devolver solo sus solicitudes con temporada, estado y detalle de la tarifa asociada

#### Scenario: Consulta del historial propio sin resultados
- **WHEN** un usuario autenticado consulta sus solicitudes y no existe ninguna solicitud asociada a su identidad
- **THEN** el sistema SHALL devolver `200 OK` con una coleccion vacia

#### Scenario: Aislamiento entre usuarios en la consulta
- **WHEN** existen solicitudes de multiples usuarios en el sistema y un usuario autenticado consulta "mis solicitudes"
- **THEN** el sistema SHALL excluir cualquier solicitud perteneciente a otra identidad

### Requirement: El sistema MUST permitir al usuario autenticado consultar el detalle completo de una solicitud propia
El sistema MUST exponer una consulta autenticada por identificador para recuperar una solicitud concreta del usuario autenticado. La respuesta SHALL incluir todos los datos funcionales de la solicitud y de la tarifa asociada, incluyendo al menos temporada, estado, fecha de creacion, licencia, categoria, territorio, `PrecioClub` y `PrecioIndependiente`.

#### Scenario: Detalle de una solicitud propia existente
- **WHEN** un usuario autenticado solicita el detalle de una solicitud que le pertenece
- **THEN** el sistema SHALL devolver la informacion completa de esa solicitud y su tarifa asociada

#### Scenario: Solicitud de detalle sobre una solicitud ajena
- **WHEN** un usuario autenticado solicita el detalle de una solicitud existente que pertenece a otro usuario
- **THEN** el sistema SHALL responder como recurso no encontrado o inaccesible sin exponer datos del otro usuario

#### Scenario: Solicitud de detalle inexistente
- **WHEN** un usuario autenticado solicita una solicitud que no existe
- **THEN** el sistema SHALL devolver `404 Not Found`
