## ADDED Requirements

### Requirement: Administrator can deactivate users logically
The system MUST allow an administrator to deactivate an Identity user without physically deleting the user record.

#### Scenario: Admin deactivates an active user
- **WHEN** an authenticated administrator requests user deactivation for an active user
- **THEN** the system SHALL persist the user as inactive in Identity and SHALL keep the user record for future reactivation

### Requirement: Deactivated users are blocked from authentication
The system MUST deny authentication for deactivated users.

#### Scenario: Deactivated user attempts login
- **WHEN** valid credentials are submitted for a deactivated user
- **THEN** the system SHALL reject authentication and SHALL not issue a JWT access token

### Requirement: Administrator can reactivate users
The system MUST allow an administrator to reactivate a previously deactivated Identity user.

#### Scenario: Admin reactivates a deactivated user
- **WHEN** an authenticated administrator requests reactivation for a deactivated user
- **THEN** the system SHALL persist the user as active in Identity so the user can authenticate again
