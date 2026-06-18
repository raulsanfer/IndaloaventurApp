## Scenario to Test Mapping

- `clean-architecture-ddd-foundation` dependency rules -> `tests/IndaloAventurApi.Architecture.Tests/ArchitectureTests.cs`
- Domain invariant enforcement -> `tests/IndaloAventurApi.Domain.Tests/AdventureMemberTests.cs`
- JWT/Identity auth success and unauthorized access -> `tests/IndaloAventurApi.IntegrationTests/ApiIntegrationTests.cs`
- ProblemDetails for validation failures -> `tests/IndaloAventurApi.IntegrationTests/ApiIntegrationTests.cs`
- CQRS split:
  - Command path through EF Core repository + `SaveChangesAsync` -> implemented in `CreateAdventureMemberCommandHandler`
  - Query path through Dapper -> implemented in `GetAdventureMemberByIdQueryHandler`
