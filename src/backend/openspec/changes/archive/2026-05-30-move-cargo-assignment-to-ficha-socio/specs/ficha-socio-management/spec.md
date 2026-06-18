## MODIFIED Requirements

### Requirement: El titular de la ficha MUST poder editar su propia informacion
El sistema MUST permitir a un usuario autenticado actualizar su propia `FichaSocio` y SHALL validar la consistencia basica de los datos antes de persistir, incluyendo la validez de `CargoId` cuando se informe.

#### Scenario: Edición de ficha propia con datos válidos
- **WHEN** un usuario autenticado envia una actualizacion sobre su `FichaSocio` con campos validos y `CargoId` existente
- **THEN** el sistema SHALL persistir los cambios y devolver confirmacion de actualizacion

#### Scenario: Edición con datos inválidos
- **WHEN** un usuario autenticado envia una actualizacion con campos invalidos, incompletos o `CargoId` inexistente
- **THEN** el sistema SHALL rechazar la operacion con error de validacion en espanol

#### Scenario: Edición de ficha inexistente
- **WHEN** un usuario autenticado intenta actualizar su ficha y no existe una `FichaSocio` previa para su `UserId`
- **THEN** el sistema SHALL responder que la ficha no existe

### Requirement: Solo un administrador MUST poder crear una ficha de socio
El sistema MUST permitir la creacion de una `FichaSocio` unicamente a usuarios con rol `Administrador`, indicando el `UserId` destino, y SHALL impedir crear mas de una ficha por usuario, incluyendo la validacion de `CargoId` cuando se informe.

#### Scenario: Administrador crea ficha para un usuario
- **WHEN** un usuario autenticado con rol `Administrador` solicita crear una ficha indicando un `UserId` sin ficha previa y `CargoId` existente
- **THEN** el sistema SHALL crear la `FichaSocio` y devolver la ficha creada

#### Scenario: No administrador intenta crear ficha
- **WHEN** un usuario autenticado sin rol `Administrador` solicita crear una ficha de socio
- **THEN** el sistema SHALL responder con acceso denegado

#### Scenario: Creación duplicada para el mismo usuario
- **WHEN** un usuario autenticado con rol `Administrador` solicita crear una ficha para un `UserId` que ya tiene ficha
- **THEN** el sistema SHALL rechazar la operacion por conflicto

#### Scenario: Creación con cargo inexistente
- **WHEN** un usuario autenticado con rol `Administrador` solicita crear una ficha indicando `CargoId` no existente
- **THEN** el sistema SHALL rechazar la operacion