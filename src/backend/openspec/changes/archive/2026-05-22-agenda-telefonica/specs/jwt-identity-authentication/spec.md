## MODIFIED Requirements

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