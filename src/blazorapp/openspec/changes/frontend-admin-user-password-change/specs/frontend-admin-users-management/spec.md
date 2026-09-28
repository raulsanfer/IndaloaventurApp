## MODIFIED Requirements

### Requirement: Acciones desde el registro del usuario
Cada registro listado MUST permitir iniciar la operación administrativa correspondiente desde la propia fila del usuario.

#### Scenario: Usuario listado ya es socio
- **WHEN** el usuario listado tiene `IsMember = true`
- **THEN** el sistema MUST mostrar una acción `Editar`
- **THEN** al pulsarla el sistema MUST navegar a la ficha administrativa de ese usuario

#### Scenario: Usuario listado todavía no es socio
- **WHEN** el usuario listado tiene `IsMember = false`
- **THEN** el sistema MUST mostrar una acción `Crear ficha`
- **THEN** al pulsarla el sistema MUST crear la ficha de socio y navegar después a su edición administrativa

#### Scenario: Admin inicia el cambio de contraseña desde una ficha
- **WHEN** un administrador abre la ficha administrativa de un usuario
- **THEN** el sistema MUST mostrar una acción `Cambiar contraseña` que navegue al flujo dedicado para ese `userId`
