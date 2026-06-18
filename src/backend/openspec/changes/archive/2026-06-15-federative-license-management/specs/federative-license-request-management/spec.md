## ADDED Requirements

### Requirement: El sistema MUST gestionar una solicitud de licencia federativa por usuario y temporada
El sistema MUST disponer de una entidad `SolicitudLicenciaFederativa` asociada a un `UserId`, a una `Temporada` y a una tarifa concreta del catalogo federativo. La solicitud SHALL persistir su estado de negocio.

#### Scenario: Registro de solicitud valida
- **WHEN** existe un usuario del sistema y se registra una solicitud de licencia para una temporada con una tarifa catalogada valida
- **THEN** el sistema SHALL persistir una `SolicitudLicenciaFederativa` vinculada a ese `UserId`, a esa `Temporada` y a la tarifa seleccionada

### Requirement: El sistema MUST conservar el historico de solicitudes por usuario
El sistema MUST mantener persistidas las solicitudes de licencias realizadas por un usuario en temporadas o anios de vigencia anteriores, aunque el catalogo incorpore nuevas vigencias anuales.

#### Scenario: Consulta de solicitudes historicas tras nueva vigencia
- **WHEN** el sistema dispone de solicitudes de un usuario asociadas a una tarifa de una temporada anterior y se cargan nuevas tarifas de otra temporada
- **THEN** las solicitudes historicas SHALL seguir existiendo y vinculadas a la tarifa con la que fueron registradas

### Requirement: La solicitud MUST iniciar y mantenerse con estados controlados
El sistema MUST limitar el estado de una solicitud a `Pendiente`, `Confirmada` o `Cancelada`, y toda nueva solicitud SHALL crearse en estado `Pendiente`.

#### Scenario: Alta inicial de solicitud
- **WHEN** se crea una nueva solicitud de licencia federativa valida
- **THEN** el sistema SHALL asignar el estado inicial `Pendiente`

#### Scenario: Persistencia de estado invalido
- **WHEN** un flujo de aplicacion intenta persistir una solicitud con un estado distinto de `Pendiente`, `Confirmada` o `Cancelada`
- **THEN** el sistema SHALL rechazar la operacion antes de confirmar persistencia

### Requirement: El sistema MUST impedir mas de una solicitud por usuario y temporada
El sistema MUST garantizar que un mismo `UserId` no tenga mas de una `SolicitudLicenciaFederativa` para la misma `Temporada`.

#### Scenario: Solicitud duplicada para la misma temporada
- **WHEN** un flujo intenta registrar una segunda solicitud para el mismo `UserId` y la misma `Temporada`
- **THEN** el sistema SHALL rechazar la nueva solicitud y SHALL conservar la ya existente

### Requirement: La solicitud MUST referenciar una tarifa del catalogo de la misma temporada
El sistema MUST validar que la tarifa elegida por una solicitud pertenece al catalogo de la misma `Temporada` de la solicitud.

#### Scenario: Tarifa de temporada coherente
- **WHEN** una solicitud de temporada `2026` referencia una tarifa catalogada de `2026`
- **THEN** el sistema SHALL aceptar la relacion entre solicitud y tarifa

#### Scenario: Tarifa de temporada distinta
- **WHEN** una solicitud referencia una tarifa catalogada de una temporada distinta a la informada en la solicitud
- **THEN** el sistema SHALL rechazar la operacion por incoherencia de datos
