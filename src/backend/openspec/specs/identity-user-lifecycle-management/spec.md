# identity-user-lifecycle-management Specification

## Purpose
TBD - created by archiving change remove-adventure-member-and-add-identity-user-deactivation. Update Purpose after archive.
## Requirements
### Requirement: Administrator can deactivate users logically
The system MUST allow an administrator to deactivate an Identity user without physically deleting the user record.

#### Scenario: Admin deactivates an active user
- **WHEN** an authenticated administrator requests user deactivation for an active user
- **THEN** the system SHALL persist the user as inactive in Identity and SHALL keep the user record for future reactivation

### Requirement: Deactivated users are blocked from authentication
The system MUST deny authentication for deactivated users.

#### Scenario: Deactivated user attempts login
- **WHEN** valid credentials are submitted for a deactivated user
- **THEN** the system SHALL reject authentication and SHALL not issue a JWT access token

### Requirement: Administrator can reactivate users
The system MUST allow an administrator to reactivate a previously deactivated Identity user.

#### Scenario: Admin reactivates a deactivated user
- **WHEN** an authenticated administrator requests reactivation for a deactivated user
- **THEN** the system SHALL persist the user as active in Identity so the user can authenticate again

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

### Requirement: User membership flag is available in user queries
The system MUST include the user membership flag in user read models used by the API.

#### Scenario: Query user includes membership flag
- **WHEN** a user read operation is executed for an existing user
- **THEN** the response SHALL include the `IsMember` field with the persisted boolean value

### Requirement: Identity user MUST soportar vinculo univoco con FichaSocio
El sistema MUST soportar la vinculacion univoca de cada usuario de Identity con una entidad `FichaSocio` mediante `UserId`, preservando la identidad del usuario como fuente de autenticacion/autorizacion, y SHALL reflejar `IsMember = true` cuando exista una ficha de socio activa creada para ese usuario.

#### Scenario: Usuario activo con vinculo de ficha
- **WHEN** un usuario de Identity existe en el sistema
- **THEN** el sistema SHALL poder resolver su `FichaSocio` por `UserId` sin ambiguedad

#### Scenario: Usuario con ficha creada queda marcado como socio
- **WHEN** administracion crea una `FichaSocio` para un usuario de Identity sin ficha previa
- **THEN** el sistema SHALL mantener el vinculo por `UserId` y SHALL persistir `IsMember` como `true`

