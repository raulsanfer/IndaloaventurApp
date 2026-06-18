## MODIFIED Requirements

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens only for authenticated and active users, and MUST validate tokens for protected endpoints.

#### Scenario: Successful login token issuance
- **WHEN** valid credentials are provided for an active user
- **THEN** the system SHALL return a signed JWT containing subject and authorization claims

#### Scenario: Login denied for inactive user
- **WHEN** valid credentials are provided for a user marked as inactive
- **THEN** the system SHALL reject token issuance and return an unauthorized response

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response

### Requirement: Role-based authorization
The system MUST enforce role-based access policies on endpoints marked with role requirements and MUST deny authorization for inactive users.

#### Scenario: Role mismatch
- **WHEN** an authenticated active user without a required role calls a role-protected endpoint
- **THEN** the system SHALL return a forbidden response

#### Scenario: Access denied for inactive user with token
- **WHEN** an authenticated user marked as inactive calls a protected endpoint with a structurally valid token
- **THEN** the system SHALL deny access because the user is inactive

#### Scenario: Phonebook write operations restricted to Admin
- **WHEN** an authenticated active user without role `Admin` calls phonebook create, update, or delete endpoints
- **THEN** the system SHALL deny the operation with forbidden response semantics

#### Scenario: Phonebook read operations allowed to any authenticated role
- **WHEN** an authenticated active user with any role calls phonebook read endpoints
- **THEN** the system SHALL authorize the request without requiring role `Admin`