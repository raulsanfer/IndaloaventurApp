## ADDED Requirements

### Requirement: Identity-backed authentication
The system MUST manage users and roles using ASP.NET Identity as the authoritative identity store.

#### Scenario: User registration success
- **WHEN** a valid registration request is submitted
- **THEN** the system SHALL create an Identity user record and apply configured password policies

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens for authenticated users and MUST validate tokens for protected endpoints.

#### Scenario: Successful login token issuance
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return a signed JWT containing subject and authorization claims

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response

### Requirement: Role-based authorization
The system MUST enforce role-based access policies on endpoints marked with role requirements.

#### Scenario: Role mismatch
- **WHEN** an authenticated user without a required role calls a role-protected endpoint
- **THEN** the system SHALL return a forbidden response