## MODIFIED Requirements

### Requirement: User creation by administrator
The system MUST allow an administrator to create a user with required profile fields, credentials policy validation, and role assignment.

#### Scenario: Admin creates user successfully with generated cargo id reference
- **WHEN** an administrator first creates a `Cargo` through its management API and receives the generated `Id`, then submits a valid create-user request referencing that `CargoId`
- **THEN** the system SHALL create the Identity user and persist assigned roles and cargo relationship

#### Scenario: Duplicate identity rejected
- **WHEN** an administrator submits a create-user request with an existing email or username
- **THEN** the system SHALL reject the request with a validation error

### Requirement: User update by administrator
The system MUST allow an administrator to edit managed user data and role membership.

#### Scenario: Admin updates user profile, roles, and cargo reference
- **WHEN** an administrator submits a valid update-user request for an existing user using a valid generated `CargoId`
- **THEN** the system SHALL persist profile changes, synchronize role assignments, and keep a valid cargo association

#### Scenario: Update request with unknown cargo id
- **WHEN** an administrator submits an update-user request with a `CargoId` that does not exist
- **THEN** the system SHALL reject the request with a validation outcome and SHALL not persist user changes
