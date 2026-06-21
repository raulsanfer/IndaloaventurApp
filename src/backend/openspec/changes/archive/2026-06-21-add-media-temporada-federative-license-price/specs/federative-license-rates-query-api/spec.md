## MODIFIED Requirements

### Requirement: El sistema MUST permitir consultar el catalogo de tarifas de licencias federativas a cualquier usuario autenticado
El sistema MUST exponer un endpoint autenticado para recuperar el catalogo de `TarifaLicenciaFederativa` sin exigir un rol especifico. La respuesta SHALL incluir, para cada tarifa, al menos `Id`, `Temporada`, `Licencia`, `Categoria`, `Territorio`, `PrecioClub`, `PrecioClubMediaTemporada`, `PrecioClubAplicable` y `PrecioIndependiente`.

#### Scenario: Consulta satisfactoria del catalogo completo
- **WHEN** un usuario autenticado de cualquier rol consulta el catalogo de tarifas sin filtros
- **THEN** el sistema SHALL devolver `200 OK` con la coleccion completa de tarifas federativas disponibles
- **AND** cada tarifa SHALL informar `PrecioClubAplicable` con el mismo valor que `PrecioClub`

#### Scenario: Usuario anonimo intenta consultar el catalogo
- **WHEN** un cliente sin autenticacion JWT intenta consultar el catalogo de tarifas
- **THEN** el sistema SHALL rechazar la peticion con una respuesta de autenticacion

### Requirement: El sistema MUST permitir filtrar las tarifas federativas por temporada
El sistema MUST aceptar un filtro opcional por `Temporada` y un parametro booleano opcional `MediaTemporada` en la consulta del catalogo. Cuando el filtro de temporada este presente, la respuesta SHALL contener exclusivamente tarifas de esa temporada y mantener un orden determinista. Cuando `MediaTemporada = true`, la respuesta SHALL resolver `PrecioClubAplicable` usando `PrecioClubMediaTemporada`; cuando `MediaTemporada = false` o no se informe, SHALL resolver `PrecioClubAplicable` usando `PrecioClub`.

#### Scenario: Consulta filtrada por temporada existente
- **WHEN** un usuario autenticado consulta el catalogo indicando una `Temporada` con tarifas registradas
- **THEN** el sistema SHALL devolver solo las tarifas asociadas a esa temporada

#### Scenario: Consulta filtrada por temporada sin tarifas
- **WHEN** un usuario autenticado consulta el catalogo indicando una `Temporada` sin tarifas registradas
- **THEN** el sistema SHALL devolver `200 OK` con una coleccion vacia

#### Scenario: Consulta de media temporada
- **WHEN** un usuario autenticado consulta el catalogo indicando `MediaTemporada = true`
- **THEN** el sistema SHALL devolver cada tarifa con `PrecioClubAplicable` igual a `PrecioClubMediaTemporada`
