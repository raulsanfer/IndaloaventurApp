## ADDED Requirements

### Requirement: Ficha socio read allows missing profile
The system MUST treat the absence of a ficha de socio as a valid empty state in read flows used by "Mi Cuenta".

#### Scenario: User without ficha opens Mi Cuenta
- **WHEN** an authenticated user requests their own ficha and no ficha exists
- **THEN** the system SHALL return a successful response with empty ficha payload and SHALL NOT throw an exception

### Requirement: Authorization is still enforced for ficha reads
The system MUST keep existing authorization checks for ficha de socio queries.

#### Scenario: Unauthorized user attempts third-party read
- **WHEN** a non-admin user requests ficha data for a different user
- **THEN** the system SHALL reject the request with forbidden access behavior
