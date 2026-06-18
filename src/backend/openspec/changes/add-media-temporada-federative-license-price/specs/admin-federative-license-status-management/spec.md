## MODIFIED Requirements

### Requirement: El sistema MUST permitir que un usuario con rol Admin consulte y filtre todas las solicitudes de licencia federativa
El sistema MUST exponer una consulta autenticada restringida a rol `Admin` para recuperar todas las `SolicitudLicenciaFederativa` creadas en el sistema. La consulta SHALL admitir filtros opcionales al menos por `UserId`, `Temporada` y `Estado`, y cada resultado SHALL incluir la informacion funcional necesaria para administracion, incluyendo propietario, temporada, estado, fecha de creacion y datos de la tarifa asociada, con `PrecioClub`, `PrecioClubMediaTemporada` y `PrecioIndependiente`.

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
