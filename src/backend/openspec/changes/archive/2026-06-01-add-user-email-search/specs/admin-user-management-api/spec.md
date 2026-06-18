## ADDED Requirements

### Requirement: User listing by administrator
The system MUST allow an administrator to list managed users and optionally filter the result by email without changing the response contract.

#### Scenario: Admin lists all users without filter
- **WHEN** an authenticated administrator requests `GET /api/users` without the `email` query parameter, or with it empty
- **THEN** the system SHALL return the complete managed-user collection ordered by email ascending

#### Scenario: Admin filters users by partial email
- **WHEN** an authenticated administrator requests `GET /api/users` with an `email` query parameter containing part of an email address
- **THEN** the system SHALL return only users whose email matches that value case-insensitively

#### Scenario: Admin searches email with no matches
- **WHEN** an authenticated administrator requests `GET /api/users` with an `email` query parameter that matches no users
- **THEN** the system SHALL return `200 OK` with an empty collection
