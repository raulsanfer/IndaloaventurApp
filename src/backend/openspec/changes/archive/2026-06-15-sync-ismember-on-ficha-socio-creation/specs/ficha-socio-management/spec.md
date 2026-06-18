## MODIFIED Requirements

### Requirement: Solo un administrador MUST poder crear una ficha de socio
El sistema MUST permitir la creacion de una `FichaSocio` unicamente a usuarios con rol `Administrador`, indicando el `UserId` destino, y SHALL impedir crear mas de una ficha por usuario, incluyendo la validacion de `CargoId` cuando se informe. Cuando la ficha se cree correctamente, el sistema SHALL marcar `IsMember = true` en el usuario vinculado como parte del alta de socio.

#### Scenario: Administrador crea ficha para un usuario
- **WHEN** un usuario autenticado con rol `Administrador` solicita crear una ficha indicando un `UserId` sin ficha previa y `CargoId` existente
- **THEN** el sistema SHALL crear la `FichaSocio`, SHALL marcar `IsMember` como `true` para ese usuario y SHALL devolver la ficha creada

#### Scenario: No administrador intenta crear ficha
- **WHEN** un usuario autenticado sin rol `Administrador` solicita crear una ficha de socio
- **THEN** el sistema SHALL responder con acceso denegado

#### Scenario: Creacion duplicada para el mismo usuario
- **WHEN** un usuario autenticado con rol `Administrador` solicita crear una ficha para un `UserId` que ya tiene ficha
- **THEN** el sistema SHALL rechazar la operacion por conflicto y SHALL no modificar `IsMember`

#### Scenario: Creacion con cargo inexistente
- **WHEN** un usuario autenticado con rol `Administrador` solicita crear una ficha indicando `CargoId` no existente
- **THEN** el sistema SHALL rechazar la operacion y SHALL no modificar `IsMember`
