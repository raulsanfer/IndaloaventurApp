## MODIFIED Requirements

### Requirement: El sistema MUST permitir que un usuario con rol Admin consulte y filtre todas las solicitudes de licencia federativa
El sistema MUST restringir a `Admin` la consulta global de solicitudes con filtros opcionales por `UserId`, `Temporada` y `Estado`. Cada resultado SHALL incluir propietario, temporada, estado, creación, `FechaInicio`, `FechaFin`, `TipoLicenciaId`, `TipoLicenciaNombre`, `Licencia`, `Categoria`, `Territorio`, `PrecioClub` y `PrecioIndependiente`, sustituyendo `MediaTemporada`.

#### Scenario: Admin consulta todas las solicitudes sin filtros
- **WHEN** un usuario autenticado con rol `Admin` consulta el listado sin filtros
- **THEN** el sistema SHALL devolver todas las solicitudes con tipo y cobertura

#### Scenario: Admin filtra por estado
- **WHEN** un administrador consulta indicando un estado válido
- **THEN** el sistema SHALL devolver solo solicitudes de ese estado

#### Scenario: Admin filtra por usuario y temporada
- **WHEN** un administrador consulta indicando usuario y temporada
- **THEN** el sistema SHALL devolver todas las solicitudes coincidentes, incluidas varias solicitudes `Día` del mismo usuario

#### Scenario: Usuario sin rol Admin intenta consultar el listado global
- **WHEN** un usuario autenticado sin rol `Admin` accede al listado global
- **THEN** el sistema SHALL devolver `403 Forbidden`

#### Scenario: Listado administrativo con solicitudes antiguas
- **WHEN** un administrador consulta solicitudes anteriores a la migración
- **THEN** el sistema SHALL mostrar su tipo migrado con ambas fechas de cobertura nulas

### Requirement: El sistema MUST permitir que un usuario con rol Admin actualice el estado de una solicitud de licencia federativa
El sistema MUST restringir a `Admin` el cambio de estado de una solicitud existente. El contrato SHALL admitir únicamente estados soportados y SHALL no permitir modificar usuario, temporada, tarifa, tipo ni cobertura. La respuesta SHALL incluir el tipo y las fechas conservadas junto con el nuevo estado.

#### Scenario: Admin actualiza correctamente el estado de una solicitud
- **WHEN** un administrador envía un estado válido para una solicitud existente del usuario indicado
- **THEN** el sistema SHALL persistir el nuevo estado y devolver la solicitud con tipo y cobertura

#### Scenario: Usuario sin rol Admin intenta actualizar el estado
- **WHEN** un usuario autenticado sin rol `Admin` intenta cambiar el estado
- **THEN** el sistema SHALL devolver `403 Forbidden`

#### Scenario: Estado invalido en la solicitud de actualizacion
- **WHEN** un administrador envía un estado no soportado
- **THEN** el sistema SHALL rechazarlo con un error de validación en español

### Requirement: El sistema MUST conservar el resto de datos funcionales de la solicitud durante la actualizacion administrativa
El sistema MUST limitar el cambio administrativo al estado, conservando `Temporada`, `TarifaLicenciaFederativaId`, `FechaCreacionUtc`, `UserId`, `FechaInicio`, `FechaFin` y el tipo derivado de la tarifa.

#### Scenario: Cambio administrativo de estado no altera otros datos
- **WHEN** un administrador cambia el estado de una solicitud existente
- **THEN** el sistema SHALL conservar temporada, tarifa, creación, usuario, cobertura y tipo

#### Scenario: Cambio de estado de una solicitud diaria entre varias
- **WHEN** un administrador cambia el estado de una solicitud `Día` de un usuario que tiene otras en la misma temporada
- **THEN** el sistema SHALL modificar exclusivamente el estado de la solicitud identificada y conservar intactas las demás
