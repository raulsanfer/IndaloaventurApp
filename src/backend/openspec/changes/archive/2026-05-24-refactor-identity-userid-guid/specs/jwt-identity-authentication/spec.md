## MODIFIED Requirements

### Requirement: Identity-backed authentication
The system MUST manage users and roles using ASP.NET Identity as the authoritative identity store, and user identifiers MUST be persisted as `Guid` in `AspNetUsers` and related identity tables.

#### Scenario: User registration success
- **WHEN** a valid registration request is submitted
- **THEN** the system SHALL create an Identity user record with `Guid` primary key and apply configured password policies

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens for authenticated users and MUST validate tokens for protected endpoints, using `Guid` user identifiers serialized in claims.

#### Scenario: Successful login token issuance
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return a signed JWT containing subject and authorization claims derived from a `Guid` user identifier

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response

### Requirement: Role-based authorization
The system MUST enforce role-based access policies on endpoints marked with role requirements.

#### Scenario: Role mismatch
- **WHEN** an authenticated user without a required role calls a role-protected endpoint
- **THEN** the system SHALL return a forbidden response

#### Scenario: Phonebook write operations restricted to Admin
- **WHEN** an authenticated user without role `Admin` calls phonebook create, update, or delete endpoints
- **THEN** the system SHALL deny the operation with forbidden response semantics

#### Scenario: Phonebook read operations allowed to any authenticated role
- **WHEN** an authenticated user with any role calls phonebook read endpoints
- **THEN** the system SHALL authorize the request without requiring role `Admin`
