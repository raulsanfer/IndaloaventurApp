## ADDED Requirements

### Requirement: Buscar signals por filtros funcionales
El sistema MUST permitir que cualquier usuario autenticado, independientemente de su rol, consulte `Signal` aplicando filtros opcionales por `Tags`, `Activo`, `Descripcion` y `Tipo`, incluyendo combinaciones entre ellos.

#### Scenario: Búsqueda por un filtro individual
- **WHEN** un usuario autenticado consulta signals filtrando solo por `Tipo`
- **THEN** el sistema devuelve únicamente los `Signal` que coinciden con el tipo solicitado

#### Scenario: Búsqueda por múltiples filtros
- **WHEN** un usuario autenticado consulta signals filtrando por `Tags`, `Activo` y `Descripcion`
- **THEN** el sistema devuelve solo los `Signal` que cumplen simultáneamente los filtros enviados

#### Scenario: Búsqueda sin filtros
- **WHEN** un usuario autenticado consulta signals sin parámetros de filtro
- **THEN** el sistema devuelve el conjunto disponible de `Signal` según reglas de acceso generales