## MODIFIED Requirements

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens for authenticated users and MUST validate tokens for protected endpoints.

#### Scenario: Successful login token issuance
- **WHEN** valid credentials are provided for an active user
- **THEN** the system SHALL return a signed JWT containing subject and authorization claims

#### Scenario: Deactivated user login denied
- **WHEN** valid credentials are provided for a deactivated user
- **THEN** the system SHALL deny authentication and SHALL not issue a JWT token

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response
