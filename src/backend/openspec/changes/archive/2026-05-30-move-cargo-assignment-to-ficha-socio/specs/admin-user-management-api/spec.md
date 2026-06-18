## MODIFIED Requirements

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