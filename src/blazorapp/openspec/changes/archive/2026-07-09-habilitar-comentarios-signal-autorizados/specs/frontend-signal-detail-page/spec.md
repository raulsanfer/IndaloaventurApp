## ADDED Requirements

### Requirement: La pagina de detalle MUST mostrar la accion de comentar dentro del bloque de comentarios solo a perfiles autorizados
La pagina de detalle MUST mostrar una accion visible `Anadir comentario` dentro del bloque `Comentarios` unicamente para usuarios autenticados con rol `Admin` o con rol `Member` y claim `IsMember = true`.

#### Scenario: Accion visible para Admin
- **WHEN** un usuario autenticado con rol `Admin` visualiza el detalle de una signal
- **THEN** el sistema MUST mostrar la accion `Anadir comentario` dentro de la seccion `Comentarios`
- **AND** el sistema MUST mantener el resto del detalle sin cambios de navegacion

#### Scenario: Accion visible para Member con IsMember verdadero
- **WHEN** un usuario autenticado con rol `Member` y claim `IsMember = true` visualiza el detalle de una signal
- **THEN** el sistema MUST mostrar la accion `Anadir comentario` dentro de la seccion `Comentarios`

#### Scenario: Accion oculta para perfiles no autorizados
- **WHEN** un usuario autenticado que no cumple la regla de permiso visualiza el detalle de una signal
- **THEN** el sistema MUST no mostrar la accion `Anadir comentario`
- **AND** el sistema MUST seguir mostrando la lista de comentarios en modo de consulta
