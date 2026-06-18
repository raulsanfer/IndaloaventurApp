## MODIFIED Requirements

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens for authenticated users and MUST validate tokens for protected endpoints, and authentication failure messages exposed to clients SHALL be in Spanish. Authentication responses MUST expose the persisted `IsMember` state of the authenticated user, and issued tokens MUST include a stable `IsMember` claim serialized as `true` or `false`.

#### Scenario: Successful login token issuance
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return a signed JWT containing subject, authorization claims, and the `IsMember` claim with the persisted user value

#### Scenario: Successful login response includes membership flag
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return `LoginResponse.IsMember` with the same boolean value emitted in the `IsMember` claim

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response with Spanish user-facing detail
