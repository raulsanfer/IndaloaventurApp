# repository-pattern-persistence Specification

## Purpose
TBD - created by archiving change bootstrap-clean-architecture-ddd-api. Update Purpose after archive.
## Requirements
### Requirement: Repository abstractions per aggregate
The system MUST define repository interfaces in the domain or application boundary for each aggregate root requiring persistence.

#### Scenario: Aggregate repository contract
- **WHEN** a new aggregate requires persistence operations
- **THEN** the system SHALL expose a repository contract that encapsulates aggregate load and save behavior

#### Scenario: Phonebook aggregate repository contract
- **WHEN** `FichaContacto` is introduced as a new aggregate root
- **THEN** the system SHALL define a dedicated repository abstraction for create, load by `Guid`, update, delete, and list/query support required by application use cases

### Requirement: Infrastructure repository implementations
The system MUST provide Infrastructure implementations of repository contracts using EF Core for write consistency and transactional behavior.

#### Scenario: Transactional save
- **WHEN** multiple aggregate changes are committed within a command
- **THEN** repository operations SHALL persist within a single EF Core transaction scope

#### Scenario: Phonebook repository persistence mapping
- **WHEN** `FichaContacto` data is persisted through Infrastructure
- **THEN** the system SHALL map the aggregate and Value Objects to SQL Server with schema constraints that preserve domain invariants

### Requirement: Read model access isolation
The system MUST keep read-model query access separate from repository write abstractions.

#### Scenario: Query bypasses repository write path
- **WHEN** a query endpoint requests projection data
- **THEN** the system SHALL use the query data access path and SHALL not require repository aggregate materialization

