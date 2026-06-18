## MODIFIED Requirements

### Requirement: Opción administrativa Cargos restringida a Admin
El sistema MUST mostrar dentro del bloque `Administración` las opciones administrativas disponibles únicamente a usuarios con rol `Admin`.

#### Scenario: Opciones administrativas visibles para Admin
- **WHEN** el usuario autenticado tiene rol `Admin` y el bloque `Administración` se renderiza
- **THEN** el sistema MUST mostrar la opción `Cargos` como acceso operativo
- **THEN** el sistema MUST mostrar la opción `Usuarios` como acceso operativo

#### Scenario: Opciones administrativas ocultas para no Admin
- **WHEN** el usuario autenticado no tiene rol `Admin`
- **THEN** el sistema MUST no mostrar la opción `Cargos`
- **THEN** el sistema MUST no mostrar la opción `Usuarios`
