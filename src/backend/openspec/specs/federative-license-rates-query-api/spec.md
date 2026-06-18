# federative-license-rates-query-api Specification

## Purpose
TBD - created by archiving change federative-license-rates-query-api. Update Purpose after archive.
## Requirements
### Requirement: El sistema MUST permitir consultar el catalogo de tarifas de licencias federativas a cualquier usuario autenticado
El sistema MUST exponer un endpoint autenticado para recuperar el catalogo de `TarifaLicenciaFederativa` sin exigir un rol especifico. La respuesta SHALL incluir, para cada tarifa, al menos `Id`, `Temporada`, `Licencia`, `Categoria`, `Territorio`, `PrecioClub` y `PrecioIndependiente`.

#### Scenario: Consulta satisfactoria del catalogo completo
- **WHEN** un usuario autenticado de cualquier rol consulta el catalogo de tarifas sin filtros
- **THEN** el sistema SHALL devolver `200 OK` con la coleccion completa de tarifas federativas disponibles

#### Scenario: Usuario anonimo intenta consultar el catalogo
- **WHEN** un cliente sin autenticacion JWT intenta consultar el catalogo de tarifas
- **THEN** el sistema SHALL rechazar la peticion con una respuesta de autenticacion

### Requirement: El sistema MUST permitir filtrar las tarifas federativas por temporada
El sistema MUST aceptar un filtro opcional por `Temporada` en la consulta del catalogo. Cuando el filtro este presente, la respuesta SHALL contener exclusivamente tarifas de esa temporada y mantener un orden determinista.

#### Scenario: Consulta filtrada por temporada existente
- **WHEN** un usuario autenticado consulta el catalogo indicando una `Temporada` con tarifas registradas
- **THEN** el sistema SHALL devolver solo las tarifas asociadas a esa temporada

#### Scenario: Consulta filtrada por temporada sin tarifas
- **WHEN** un usuario autenticado consulta el catalogo indicando una `Temporada` sin tarifas registradas
- **THEN** el sistema SHALL devolver `200 OK` con una coleccion vacia

