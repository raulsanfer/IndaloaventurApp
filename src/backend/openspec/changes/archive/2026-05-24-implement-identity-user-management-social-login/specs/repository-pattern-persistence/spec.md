## MODIFIED Requirements

### Requirement: Repository abstractions per aggregate
The system MUST define repository interfaces in the domain or application boundary for each aggregate root requiring persistence.

#### Scenario: Aggregate repository contract
- **WHEN** a new aggregate requires persistence operations
- **THEN** the system SHALL expose a repository contract that encapsulates aggregate load and save behavior

### Requirement: Infrastructure repository implementations
The system MUST provide Infrastructure implementations of repository contracts using EF Core for write consistency and transactional behavior.

#### Scenario: Transactional save
- **WHEN** multiple aggregate changes are committed within a command
- **THEN** repository operations SHALL persist within a single EF Core transaction scope

### Requirement: Read model access isolation
The system MUST keep read-model query access separate from repository write abstractions.

#### Scenario: Query bypasses repository write path
- **WHEN** a query endpoint requests projection data
- **THEN** the system SHALL use the query data access path and SHALL not require repository aggregate materialization

### Requirement: EF Core migration-driven schema evolution
The system MUST manage relational schema changes through EF Core migrations applied against SQL Server using the configured development connection `api_ContextConnection`.

#### Scenario: New model change creates migration artifact
- **WHEN** a persistence model change affecting schema is introduced
- **THEN** the system SHALL produce a new EF Core migration and track it in source control

#### Scenario: Development database applies migrations
- **WHEN** the development environment runs schema update workflow
- **THEN** the system SHALL apply pending migrations to the database resolved from `api_ContextConnection`
