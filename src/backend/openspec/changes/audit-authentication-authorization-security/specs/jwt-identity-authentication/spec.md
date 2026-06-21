## MODIFIED Requirements

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens for authenticated users and MUST explicitly validate token signature, issuer, audience, expiration, and active user state for protected endpoints, and authentication failure messages exposed to clients SHALL be in Spanish. Authentication responses MUST expose the persisted `IsMember` state of the authenticated user, and issued tokens MUST include a stable `IsMember` claim serialized as `true` or `false`.

#### Scenario: Successful login token issuance
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return a signed JWT containing subject, authorization claims, and the `IsMember` claim with the persisted user value

#### Scenario: Successful login response includes membership flag
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return `LoginResponse.IsMember` with the same boolean value emitted in the `IsMember` claim

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response with Spanish user-facing detail

#### Scenario: Expired token is rejected
- **WHEN** a request to a protected endpoint includes a token whose expiration time has passed
- **THEN** the system SHALL reject the token and SHALL not execute the protected operation

#### Scenario: Token of deactivated user is rejected
- **WHEN** a request to a protected endpoint includes a previously valid token that belongs to a user later deactivated in Identity
- **THEN** the system SHALL reject the request and SHALL not authorize access to the endpoint

## ADDED Requirements

### Requirement: Credential login MUST resist repeated failed attempts
The system MUST track repeated failed credential submissions for password-based login and SHALL temporarily block further token issuance for the affected user identity after the configured failure threshold is exceeded.

#### Scenario: Consecutive invalid passwords trigger temporary lockout
- **WHEN** a client submits invalid credentials repeatedly for the same existing user until the configured threshold is reached
- **THEN** the system SHALL deny subsequent password login attempts for that user for the configured lockout window and SHALL not issue a JWT

#### Scenario: Locked-out user still receives no token with valid password
- **WHEN** a user is under an active authentication lockout window and then submits the correct password
- **THEN** the system SHALL still reject the login attempt until the lockout window expires or the account is administratively re-enabled
