## MODIFIED Requirements

### Requirement: Infrastructure repository implementations
The system MUST provide Infrastructure implementations of repository contracts using EF Core for write consistency and transactional behavior. When an aggregate requires hybrid persistence, Infrastructure SHALL coordinate relational metadata persistence with the corresponding external storage backend.

#### Scenario: Transactional save
- **WHEN** multiple aggregate changes are committed within a command
- **THEN** repository operations SHALL persist within a single EF Core transaction scope

#### Scenario: Phonebook repository persistence mapping
- **WHEN** `FichaContacto` data is persisted through Infrastructure
- **THEN** the system SHALL map the aggregate and Value Objects to SQL Server with schema constraints that preserve domain invariants

#### Scenario: Signal repository with hybrid image persistence
- **WHEN** a `Signal` is created or updated with associated images
- **THEN** the system SHALL persist `Signal` metadata in SQL Server and SHALL coordinate the image references with the configured filesystem storage backend
