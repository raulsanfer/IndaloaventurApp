## MODIFIED Requirements

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens for authenticated users and MUST explicitly validate token signature, issuer, audience, expiration, active user state, and the current Identity security stamp for protected endpoints, and authentication failure messages exposed to clients SHALL be in Spanish. Authentication responses MUST expose the persisted `IsMember` state of the authenticated user, and issued tokens MUST include a stable `IsMember` claim serialized as `true` or `false`.

#### Scenario: Successful login token issuance
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return a signed JWT containing subject, authorization claims, the current Identity security stamp, and the `IsMember` claim with the persisted user value

#### Scenario: Successful login response includes membership flag
- **WHEN** valid user credentials are provided
- **THEN** the response SHALL include `LoginResponse.IsMember` with the same boolean value emitted in the `IsMember` claim

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response with Spanish user-facing detail

#### Scenario: Expired token is rejected
- **WHEN** a request to a protected endpoint includes a token whose expiration time has passed
- **THEN** the system SHALL reject the token and SHALL not execute the protected operation

#### Scenario: Token of deactivated user is rejected
- **WHEN** a request to a protected endpoint includes a previously valid token that belongs to a user later deactivated in Identity
- **THEN** the system SHALL reject the request and SHALL not authorize access to the endpoint

#### Scenario: Token with stale security stamp is rejected
- **WHEN** a request to a protected endpoint includes a token whose security stamp no longer matches the Identity user
- **THEN** the system SHALL reject the token and SHALL not authorize access to the endpoint
