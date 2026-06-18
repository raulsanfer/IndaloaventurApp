## 1. Solution Bootstrap and Architecture Boundaries

- [x] 1.1 Create .NET 9 solution structure with `Domain`, `Application`, `Infrastructure`, and `API` projects and enforce inward dependency direction.
- [x] 1.2 Add architecture validation tests to ensure forbidden project references are blocked.
- [x] 1.3 Establish shared conventions for namespaces, folder layout, and dependency injection composition root.

## 2. Domain and Repository Contracts

- [x] 2.1 Define initial aggregate root(s), entities, and value objects with explicit invariant checks.
- [x] 2.2 Create repository interfaces for aggregate persistence at domain/application boundary.
- [x] 2.3 Add domain-focused unit tests verifying invariant enforcement and repository contract expectations.

## 3. CQRS Application Layer

- [x] 3.1 Implement CQRS abstractions (commands, queries, handlers) and request dispatching mechanism.
- [x] 3.2 Add pipeline behaviors for validation and cross-cutting error handling.
- [x] 3.3 Implement one end-to-end vertical slice (create command + read query) to validate CQRS separation.

## 4. Infrastructure Persistence with EF Core and Dapper

- [x] 4.1 Configure EF Core SQL Server context, mappings, migrations, and unit-of-work transaction behavior for commands.
- [x] 4.2 Implement repository concrete classes in Infrastructure using EF Core.
- [x] 4.3 Implement query data access with Dapper and dedicated query connection factory/read models.
- [x] 4.4 Add integration tests proving command path uses EF Core transaction semantics and query path uses Dapper projections.

## 5. Security with ASP.NET Identity and JWT

- [x] 5.1 Configure ASP.NET Identity user/role store, password policy, and persistence integration.
- [x] 5.2 Implement authentication endpoints (register/login) with JWT issuance including claims and expiration.
- [x] 5.3 Configure authorization policies and role-protected controller actions.
- [x] 5.4 Add authentication/authorization integration tests for success, unauthorized, and forbidden scenarios.

## 6. API Controllers and ProblemDetails Contract

- [x] 6.1 Implement classic ASP.NET Core MVC controllers (no Minimal APIs) for the reference vertical slice and auth endpoints.
- [x] 6.2 Add centralized ProblemDetails mapping for validation, domain, authorization, and unhandled exceptions.
- [x] 6.3 Add API tests validating `application/problem+json` payload shape and status code mappings.

## 7. Verification and Readiness

- [x] 7.1 Run full test suite and fix failing tests until green.
- [x] 7.2 Validate all OpenSpec scenarios map to automated tests or documented manual checks.
- [x] 7.3 Update task checkboxes only after verification and prepare change for `/opsx:apply` implementation flow.
