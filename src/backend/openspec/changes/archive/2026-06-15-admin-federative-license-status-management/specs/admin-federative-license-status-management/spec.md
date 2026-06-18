## ADDED Requirements

### Requirement: El sistema MUST permitir que un usuario con rol Admin consulte y filtre todas las solicitudes de licencia federativa
El sistema MUST exponer una consulta autenticada restringida a rol `Admin` para recuperar todas las `SolicitudLicenciaFederativa` creadas en el sistema. La consulta SHALL admitir filtros opcionales al menos por `UserId`, `Temporada` y `Estado`, y cada resultado SHALL incluir la informacion funcional necesaria para administracion, incluyendo propietario, temporada, estado, fecha de creacion y datos de la tarifa asociada.

#### Scenario: Admin consulta todas las solicitudes sin filtros
- **WHEN** un usuario autenticado con rol `Admin` solicita el listado administrativo sin informar filtros
- **THEN** el sistema SHALL devolver la coleccion completa de solicitudes registradas

#### Scenario: Admin filtra por estado
- **WHEN** un usuario autenticado con rol `Admin` solicita el listado administrativo filtrando por un `Estado` valido
- **THEN** el sistema SHALL devolver solo las solicitudes que coinciden con ese estado

#### Scenario: Admin filtra por usuario y temporada
- **WHEN** un usuario autenticado con rol `Admin` solicita el listado administrativo informando `UserId` y `Temporada`
- **THEN** el sistema SHALL devolver solo las solicitudes pertenecientes a ese usuario y a esa temporada

#### Scenario: Usuario sin rol Admin intenta consultar el listado global
- **WHEN** un usuario autenticado sin rol `Admin` intenta acceder a la consulta administrativa de solicitudes
- **THEN** el sistema SHALL rechazar la operacion con respuesta `403 Forbidden`

### Requirement: El sistema MUST permitir que un usuario con rol Admin actualice el estado de una solicitud de licencia federativa
El sistema MUST exponer un endpoint autenticado restringido a rol `Admin` para actualizar el campo `Estado` de una `SolicitudLicenciaFederativa` existente. El contrato de actualizacion SHALL admitir unicamente valores soportados por el dominio y SHALL no permitir cambios sobre `UserId`, `Temporada` ni `TarifaLicenciaFederativaId`.

#### Scenario: Admin actualiza correctamente el estado de una solicitud
- **WHEN** un usuario autenticado con rol `Admin` envia una actualizacion valida sobre una solicitud existente de un usuario concreto
- **THEN** el sistema SHALL persistir el nuevo `Estado` y devolver la representacion actualizada de la solicitud

#### Scenario: Usuario sin rol Admin intenta actualizar el estado
- **WHEN** un usuario autenticado sin rol `Admin` intenta actualizar el estado de una solicitud de licencia federativa
- **THEN** el sistema SHALL rechazar la operacion con respuesta `403 Forbidden`

#### Scenario: Estado invalido en la solicitud de actualizacion
- **WHEN** un usuario con rol `Admin` envia un valor de estado no soportado por el dominio
- **THEN** el sistema SHALL rechazar la operacion con un error de validacion en espanol

### Requirement: El sistema MUST validar la coherencia entre el usuario indicado y la solicitud actualizada
El sistema MUST comprobar que la solicitud a editar pertenece realmente al `UserId` indicado en la ruta administrativa. Si la solicitud no existe o no corresponde a ese usuario, el sistema SHALL impedir la actualizacion sin exponer informacion inconsistente.

#### Scenario: Solicitud inexistente
- **WHEN** un usuario con rol `Admin` intenta actualizar una solicitud que no existe
- **THEN** el sistema SHALL devolver `404 Not Found`

#### Scenario: Solicitud existente pero asociada a otro usuario
- **WHEN** un usuario con rol `Admin` intenta actualizar una solicitud existente usando un `UserId` que no coincide con el propietario real
- **THEN** el sistema SHALL responder como recurso no encontrado o incoherente y SHALL no modificar la solicitud

### Requirement: El sistema MUST conservar el resto de datos funcionales de la solicitud durante la actualizacion administrativa
El sistema MUST limitar la operacion administrativa al cambio de `Estado`, preservando temporada, tarifa asociada, fecha de creacion y usuario propietario de la solicitud.

#### Scenario: Cambio administrativo de estado no altera otros datos
- **WHEN** un usuario con rol `Admin` actualiza el estado de una solicitud existente
- **THEN** el sistema SHALL mantener sin cambios `Temporada`, `TarifaLicenciaFederativaId`, `FechaCreacionUtc` y `UserId`
