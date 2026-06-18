# signal-search Specification

## Purpose
TBD - created by archiving change add-trailsignal-management. Update Purpose after archive.
## Requirements
### Requirement: Buscar signals por filtros funcionales
El sistema MUST permitir que cualquier usuario autenticado, independientemente de su rol, consulte `Signal` aplicando filtros opcionales por `Tags`, `Activo`, `Descripcion` y `Tipo`, incluyendo combinaciones entre ellos.

#### Scenario: B�squeda por un filtro individual
- **WHEN** un usuario autenticado consulta signals filtrando solo por `Tipo`
- **THEN** el sistema devuelve �nicamente los `Signal` que coinciden con el tipo solicitado

#### Scenario: B�squeda por m�ltiples filtros
- **WHEN** un usuario autenticado consulta signals filtrando por `Tags`, `Activo` y `Descripcion`
- **THEN** el sistema devuelve solo los `Signal` que cumplen simult�neamente los filtros enviados

#### Scenario: B�squeda sin filtros
- **WHEN** un usuario autenticado consulta signals sin par�metros de filtro
- **THEN** el sistema devuelve el conjunto disponible de `Signal` seg�n reglas de acceso generales

