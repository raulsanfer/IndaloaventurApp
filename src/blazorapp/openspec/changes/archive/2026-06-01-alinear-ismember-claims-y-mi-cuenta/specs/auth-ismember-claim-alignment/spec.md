## ADDED Requirements

### Requirement: IsMember MUST formar parte de la identidad autenticada
El sistema MUST propagar el valor de `AspNetUser.IsMember` como parte de la identidad autenticada del usuario para que pueda ser consumido de forma consistente por backend y frontend.

#### Scenario: Emisión del claim de membresía
- **WHEN** un usuario se autentica correctamente
- **THEN** el backend MUST emitir un claim de identidad que represente `IsMember`
- **AND** el valor del claim MUST corresponder con `AspNetUser.IsMember`

### Requirement: El frontend MUST poblar AuthSession.IsMember desde la autenticación validada
El sistema MUST mantener `AuthSession.IsMember` como proyección local del estado autenticado del usuario.

#### Scenario: Usuario autenticado como socio
- **WHEN** el usuario inicia sesión y la autenticación indica `IsMember = true`
- **THEN** el frontend MUST guardar `AuthSession.IsMember = true`

#### Scenario: Usuario autenticado como no socio
- **WHEN** el usuario inicia sesión y la autenticación indica `IsMember = false`
- **THEN** el frontend MUST guardar `AuthSession.IsMember = false`

### Requirement: Mi Cuenta MUST tolerar usuarios autenticados no socios
El sistema MUST permitir que un usuario autenticado con `IsMember = false` abra `Mi Cuenta` sin que la ausencia de ficha de socio se trate como error bloqueante.

#### Scenario: Render sin ficha para usuario no socio
- **WHEN** un usuario autenticado con `IsMember = false` navega a `Mi Cuenta`
- **THEN** el sistema MUST no mostrar mensaje de error por falta de ficha de socio
- **AND** el sistema MUST renderizar las secciones no dependientes de `Profile`

### Requirement: Los bloques exclusivos de socio MUST ocultarse para no socios
El sistema MUST ocultar cualquier bloque de `Mi Cuenta` que dependa de la condición de socio cuando `IsMember = false`.

#### Scenario: Ocultación de bloques de socio
- **WHEN** `MyAccount` se renderiza para un usuario con `IsMember = false`
- **THEN** el sistema MUST ocultar `MemberCargoBadge`
- **AND** el sistema MUST ocultar `Ficha Socio`
- **AND** el sistema MUST ocultar `Licencias Federativas`
