# signal-search Specification

## Purpose
TBD - created by archiving change add-trailsignal-management. Update Purpose after archive.

## Requirements
### Requirement: Buscar signals por filtros funcionales
El sistema MUST permitir que cualquier usuario autenticado, independientemente de su rol, consulte `Signal` aplicando filtros opcionales por `Tags`, `Activo`, `Descripcion` y `Tipo`, incluyendo combinaciones entre ellos, y MUST devolver en cada resultado el bloque principal de datos con `Id`, `Latitud`, `Longitud`, `Titulo`, `Descripcion`, `Activo`, `UserIdAlta`, `FechaAlta`, `FechaModificacion`, `UserIdModificacion`, `Tipo` y `Tags`.

#### Scenario: Busqueda por un filtro individual
- **WHEN** un usuario autenticado consulta signals filtrando solo por `Tipo`
- **THEN** el sistema devuelve unicamente los `Signal` que coinciden con el tipo solicitado, incluyendo `Titulo` en cada elemento del resultado

#### Scenario: Busqueda por multiples filtros
- **WHEN** un usuario autenticado consulta signals filtrando por `Tags`, `Activo` y `Descripcion`
- **THEN** el sistema devuelve solo los `Signal` que cumplen simultaneamente los filtros enviados, incluyendo `Titulo` junto a `Descripcion`

#### Scenario: Busqueda sin filtros
- **WHEN** un usuario autenticado consulta signals sin parametros de filtro
- **THEN** el sistema devuelve el conjunto disponible de `Signal` segun reglas de acceso generales, incluyendo `Titulo` en cada `Signal` devuelta
