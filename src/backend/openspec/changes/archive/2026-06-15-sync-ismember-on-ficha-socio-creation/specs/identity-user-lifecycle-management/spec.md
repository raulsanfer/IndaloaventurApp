## MODIFIED Requirements

### Requirement: Administrator can manage user club membership flag
The system MUST allow an authenticated administrator to set whether a user is a club member through the user management flow, and SHALL keep that flag synchronized when an administrative member record creation makes the user an active club member.

#### Scenario: Admin sets user as member
- **WHEN** an authenticated administrator updates a user and sets `IsMember` to `true`
- **THEN** the system SHALL persist `IsMember` as `true` for that user

#### Scenario: Admin unsets user membership
- **WHEN** an authenticated administrator updates a user and sets `IsMember` to `false`
- **THEN** the system SHALL persist `IsMember` as `false` for that user

#### Scenario: Member record creation marks user as member
- **WHEN** an authenticated administrator creates a `FichaSocio` for an existing user and the operation completes successfully
- **THEN** the system SHALL persist `IsMember` as `true` for that user

### Requirement: Identity user MUST soportar vinculo univoco con FichaSocio
El sistema MUST soportar la vinculacion univoca de cada usuario de Identity con una entidad `FichaSocio` mediante `UserId`, preservando la identidad del usuario como fuente de autenticacion/autorizacion, y SHALL reflejar `IsMember = true` cuando exista una ficha de socio activa creada para ese usuario.

#### Scenario: Usuario activo con vinculo de ficha
- **WHEN** un usuario de Identity existe en el sistema
- **THEN** el sistema SHALL poder resolver su `FichaSocio` por `UserId` sin ambiguedad

#### Scenario: Usuario con ficha creada queda marcado como socio
- **WHEN** administracion crea una `FichaSocio` para un usuario de Identity sin ficha previa
- **THEN** el sistema SHALL mantener el vinculo por `UserId` y SHALL persistir `IsMember` como `true`
