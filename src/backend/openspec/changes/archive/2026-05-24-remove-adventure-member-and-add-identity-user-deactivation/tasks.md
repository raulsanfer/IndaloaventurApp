## 1. Remove AdventureMember Capability

- [x] 1.1 Remove `AdventureMember` domain entity and related abstractions/usages in Domain and Application layers.
- [x] 1.2 Remove `AdventureMember` infrastructure persistence artifacts (EF mappings, repositories, configurations, migrations only if change-specific).
- [x] 1.3 Remove `AdventureMember` API endpoints/contracts and update routing/DI registrations accordingly.
- [x] 1.4 Remove or rewrite tests tied to `AdventureMember` behavior so no test depends on that entity.

## 2. Identity User Lifecycle Management

- [x] 2.1 Ensure Identity-backed user management endpoints keep list/create flows working after `AdventureMember` removal.
- [x] 2.2 Implement administrator-only user deactivation endpoint/command with logical (non-physical) persistence semantics.
- [x] 2.3 Implement administrator-only user reactivation endpoint/command to restore active authentication state.
- [x] 2.4 Ensure authorization policies enforce admin role/permission on deactivate/reactivate operations.

## 3. Authentication Enforcement

- [x] 3.1 Update login/authentication flow to reject deactivated users before JWT issuance.
- [x] 3.2 Standardize API response behavior for deactivated login attempts (unauthorized/error contract alignment).

## 4. Verification and Documentation

- [x] 4.1 Add/update unit and integration tests for deactivate/reactivate and deactivated-login-denied scenarios.
- [x] 4.2 Run full test suite (`dotnet test`) and fix regressions.
- [x] 4.3 Update API documentation/changelog to mark `AdventureMember` removal as breaking and document Identity lifecycle endpoints.
