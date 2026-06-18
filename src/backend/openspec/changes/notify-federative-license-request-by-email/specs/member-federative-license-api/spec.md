## MODIFIED Requirements

### Requirement: El sistema MUST permitir al usuario autenticado crear su propia solicitud de licencia federativa
El sistema MUST exponer un endpoint autenticado de autoservicio para registrar una `SolicitudLicenciaFederativa` asociada al usuario del token, sin aceptar un `UserId` informado por el cliente. La solicitud SHALL incluir al menos la `Temporada` y la `TarifaLicenciaFederativaId`, y el alta resultante SHALL quedar en estado inicial `Pendiente`. Cuando la solicitud se cree correctamente, el sistema SHALL enviar un correo informativo en castellano a la direccion operativa configurada para licencias federativas incluyendo, al menos, que se ha recibido una solicitud y el email del usuario solicitante.

#### Scenario: Alta correcta de solicitud propia
- **WHEN** un usuario autenticado con claim `IsMember = true` envia una solicitud valida con una tarifa existente de la misma temporada
- **THEN** el sistema SHALL crear la solicitud ligada al `NameIdentifier` del token y devolver la representacion creada en estado `Pendiente`
- **AND** el sistema SHALL enviar un correo informativo a la direccion configurada para notificaciones de licencias federativas

#### Scenario: Usuario no socio intenta solicitar licencia
- **WHEN** un usuario autenticado con claim `IsMember = false` intenta crear una solicitud de licencia federativa
- **THEN** el sistema SHALL rechazar la operacion con una respuesta de autorizacion y no persistira ninguna solicitud

#### Scenario: Solicitud duplicada para la misma temporada
- **WHEN** un usuario autenticado intenta crear una segunda solicitud para una temporada en la que ya tiene una solicitud registrada
- **THEN** el sistema SHALL rechazar la operacion con un conflicto funcional y un mensaje en espanol

#### Scenario: Destinatario operativo por defecto
- **WHEN** el sistema usa su configuracion inicial de correo para solicitudes de licencia federativa
- **THEN** la direccion destinataria SHALL ser `club@indaloaventura.com`
