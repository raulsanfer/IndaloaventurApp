## MODIFIED Requirements

### Requirement: Identity-backed authentication
The system MUST manage users and roles using ASP.NET Identity as the authoritative identity store, and MUST initialize configured baseline roles plus a default administrator account in development environments.

#### Scenario: User registration success
- **WHEN** a valid registration request is submitted
- **THEN** the system SHALL create an Identity user record and apply configured password policies

#### Scenario: Development bootstrap of roles and admin user
- **WHEN** the application starts in development with Identity persistence enabled
- **THEN** the system SHALL ensure baseline roles and the configured default administrator account exist idempotently

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens for authenticated users and MUST validate tokens for protected endpoints, including users authenticated through social providers.

#### Scenario: Successful login token issuance
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return a signed JWT containing subject and authorization claims

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response

#### Scenario: Social login token issuance
- **WHEN** a social authentication flow is successfully validated
- **THEN** the system SHALL return a signed JWT using the same authorization claim model as credential login

### Requirement: Role-based authorization
The system MUST enforce role-based access policies on endpoints marked with role requirements, and management endpoints MUST require the `Admin` role.

#### Scenario: Role mismatch
- **WHEN** an authenticated user without a required role calls a role-protected endpoint
- **THEN** the system SHALL return a forbidden response

#### Scenario: Admin-only policy on user management routes
- **WHEN** an authenticated user without role `Admin` calls a user-management route
- **THEN** the system SHALL return a forbidden response and MUST NOT execute the handler
