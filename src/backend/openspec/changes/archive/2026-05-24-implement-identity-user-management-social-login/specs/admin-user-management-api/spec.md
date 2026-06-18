## ADDED Requirements

### Requirement: Admin-only user management endpoints
The system MUST expose user management endpoints that are accessible only to authenticated users with role `Admin`.

#### Scenario: Non-admin denied on user management endpoint
- **WHEN** an authenticated user without role `Admin` calls a user management endpoint
- **THEN** the system SHALL return a forbidden response

#### Scenario: Admin granted on user management endpoint
- **WHEN** an authenticated user with role `Admin` calls a user management endpoint
- **THEN** the system SHALL authorize the request and execute the operation

### Requirement: User creation by administrator
The system MUST allow an administrator to create a user with required profile fields, credentials policy validation, and role assignment.

#### Scenario: Admin creates user successfully
- **WHEN** an administrator submits a valid create-user request
- **THEN** the system SHALL create the Identity user and persist assigned roles

#### Scenario: Duplicate identity rejected
- **WHEN** an administrator submits a create-user request with an existing email or username
- **THEN** the system SHALL reject the request with a validation error

### Requirement: User update by administrator
The system MUST allow an administrator to edit managed user data and role membership.

#### Scenario: Admin updates user profile and roles
- **WHEN** an administrator submits a valid update-user request for an existing user
- **THEN** the system SHALL persist profile changes and synchronize role assignments

#### Scenario: Update request for unknown user
- **WHEN** an administrator submits an update for a non-existing user identifier
- **THEN** the system SHALL return a not-found response
