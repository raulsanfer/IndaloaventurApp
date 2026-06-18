## MODIFIED Requirements

### Requirement: Infrastructure repository implementations
The system MUST provide Infrastructure implementations of repository contracts using EF Core for write consistency and transactional behavior, including Identity persistence mapped with `Guid` keys for users, roles, and relationship tables.

#### Scenario: Transactional save
- **WHEN** multiple aggregate changes are committed within a command
- **THEN** repository operations SHALL persist within a single EF Core transaction scope

#### Scenario: Phonebook repository persistence mapping
- **WHEN** `FichaContacto` data is persisted through Infrastructure
- **THEN** the system SHALL map the aggregate and Value Objects to SQL Server with schema constraints that preserve domain invariants

#### Scenario: Identity persistence mapping with Guid keys
- **WHEN** Identity users and roles are persisted through Infrastructure
- **THEN** the system SHALL map `Guid` primary/foreign keys consistently across `AspNetUsers`, `AspNetRoles`, claims, logins, roles assignment, and tokens
