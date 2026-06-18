## ADDED Requirements

### Requirement: Administrator can manage user club membership flag
The system MUST allow an authenticated administrator to set whether a user is a club member through the user management flow.

#### Scenario: Admin sets user as member
- **WHEN** an authenticated administrator updates a user and sets `IsMember` to `true`
- **THEN** the system SHALL persist `IsMember` as `true` for that user

#### Scenario: Admin unsets user membership
- **WHEN** an authenticated administrator updates a user and sets `IsMember` to `false`
- **THEN** the system SHALL persist `IsMember` as `false` for that user

### Requirement: User membership flag is available in user queries
The system MUST include the user membership flag in user read models used by the API.

#### Scenario: Query user includes membership flag
- **WHEN** a user read operation is executed for an existing user
- **THEN** the response SHALL include the `IsMember` field with the persisted boolean value
