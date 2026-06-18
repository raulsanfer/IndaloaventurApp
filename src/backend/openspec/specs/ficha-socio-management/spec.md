# ficha-socio-management Specification

## Purpose
TBD - created by archiving changes add-ficha-socio-management and move-cargo-assignment-to-ficha-socio. Update Purpose after archive.
## Requirements
### Requirement: El sistema MUST gestionar una FichaSocio por usuario
El sistema MUST disponer de una entidad `FichaSocio` asociada de forma univoca a cada `User` mediante `UserId`, y SHALL almacenar los campos: `Nombre`, `Apellidos`, `DNI`, `Fecha_nacimiento`, `Direccion`, `Codigo_postal`, `Poblacion`, `Provincia`, `Tlf`, `Email`, `Alergias`, `Acepta_Politica_Privacidad`, `Acepta_Uso_Imagenes`, `Acepta_CobroCuenta`.

#### Scenario: Creacion o disponibilidad de ficha para usuario existente
- **WHEN** un usuario del sistema necesita consultar o editar su perfil de socio
- **THEN** el sistema SHALL operar sobre una unica `FichaSocio` vinculada por su `UserId`

### Requirement: El titular de la ficha MUST poder consultar su propia informacion
El sistema MUST permitir a un usuario autenticado consultar su propia `FichaSocio` cuando la ficha pertenece a su `UserId`.

#### Scenario: Consulta de ficha propia
- **WHEN** un usuario autenticado solicita la ficha asociada a su `UserId`
- **THEN** el sistema SHALL devolver la informacion completa de su `FichaSocio`

### Requirement: El titular de la ficha MUST poder editar su propia informacion
El sistema MUST permitir a un usuario autenticado actualizar su propia `FichaSocio` y SHALL validar la consistencia basica de los datos antes de persistir, incluyendo la validez de `CargoId` cuando se informe.

#### Scenario: Edicion de ficha propia con datos validos
- **WHEN** un usuario autenticado envia una actualizacion sobre su `FichaSocio` con campos validos y `CargoId` existente
- **THEN** el sistema SHALL persistir los cambios y devolver confirmacion de actualizacion

#### Scenario: Edicion con datos invalidos
- **WHEN** un usuario autenticado envia una actualizacion con campos invalidos, incompletos o `CargoId` inexistente
- **THEN** el sistema SHALL rechazar la operacion con error de validacion en espanol

#### Scenario: Edicion de ficha inexistente
- **WHEN** un usuario autenticado intenta actualizar su ficha y no existe una `FichaSocio` previa para su `UserId`
- **THEN** el sistema SHALL responder que la ficha no existe

### Requirement: Un administrador MUST poder consultar y editar fichas de terceros
El sistema MUST permitir que un usuario con rol `Administrador` consulte y edite `FichaSocio` de cualquier `UserId`.

#### Scenario: Administrador consulta ficha de otro usuario
- **WHEN** un usuario autenticado con rol `Administrador` solicita una ficha cuyo `UserId` no coincide con el suyo
- **THEN** el sistema SHALL devolver la ficha solicitada

#### Scenario: Administrador edita ficha de otro usuario
- **WHEN** un usuario autenticado con rol `Administrador` actualiza una ficha cuyo `UserId` no coincide con el suyo
- **THEN** el sistema SHALL persistir los cambios si la validacion es correcta

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

### Requirement: Usuarios no administradores MUST NOT acceder a fichas de terceros
El sistema MUST denegar consulta y edicion de `FichaSocio` cuando el usuario autenticado no sea propietario de la ficha y no tenga rol `Administrador`.

#### Scenario: Usuario intenta consultar ficha ajena sin permisos
- **WHEN** un usuario autenticado sin rol `Administrador` solicita una ficha de otro `UserId`
- **THEN** el sistema SHALL responder con acceso denegado

#### Scenario: Usuario intenta editar ficha ajena sin permisos
- **WHEN** un usuario autenticado sin rol `Administrador` intenta actualizar una ficha de otro `UserId`
- **THEN** el sistema SHALL responder con acceso denegado y no SHALL persistir cambios

