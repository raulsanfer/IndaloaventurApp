## ADDED Requirements

### Requirement: Command and query segregation
The system MUST separate write operations (commands) from read operations (queries) through distinct handlers and contracts.

#### Scenario: Command execution path
- **WHEN** a state-changing API request is received
- **THEN** the request SHALL be mapped to a command handler and SHALL not execute through a query handler

#### Scenario: Query execution path
- **WHEN** a read-only API request is received
- **THEN** the request SHALL be mapped to a query handler and SHALL not execute through a command handler

### Requirement: Persistence strategy by workload
The system MUST execute command persistence through Entity Framework Core and query retrieval through Dapper.

#### Scenario: Command persistence
- **WHEN** a command updates domain state
- **THEN** the system SHALL use EF Core unit-of-work/transaction semantics for persistence

#### Scenario: Query retrieval
- **WHEN** a query requests projections or list data
- **THEN** the system SHALL execute SQL via Dapper and return read models without tracking domain entities