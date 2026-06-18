## ADDED Requirements

### Requirement: Consultar una signal por identificador
El sistema MUST permitir que cualquier usuario autenticado recupere la informacion principal de una `Signal` existente mediante su `Id`, devolviendo los mismos campos funcionales expuestos en la consulta de busqueda: `Id`, `Latitud`, `Longitud`, `Descripcion`, `Activo`, `UserIdAlta`, `FechaAlta`, `FechaModificacion`, `UserIdModificacion`, `Tipo` y `Tags`.

#### Scenario: Consulta correcta de una signal existente
- **WHEN** un usuario autenticado solicita `GET /api/signals/{id}` con el identificador de una `Signal` existente
- **THEN** el sistema devuelve `200 OK` con la informacion principal de esa `Signal`

#### Scenario: Consulta rechazada para una signal inexistente
- **WHEN** un usuario autenticado solicita `GET /api/signals/{id}` con un identificador que no corresponde a ninguna `Signal`
- **THEN** el sistema devuelve `404 Not Found`

#### Scenario: Consulta rechazada sin autenticacion
- **WHEN** un usuario no autenticado solicita `GET /api/signals/{id}`
- **THEN** el sistema devuelve `401 Unauthorized`
