## Why

The current codebase still includes the `AdventureMember` entity and related behaviors even though user management is now based on ASP.NET Identity. This duplicates responsibilities and increases maintenance cost. We need a single user model and an admin-controlled logical deactivation flow that blocks login without deleting historical user records.

## What Changes

- Remove `AdventureMember` domain entity and all related application, infrastructure, API, and test artifacts.
- Keep user management exclusively on ASP.NET Identity (list/create workflows remain in Identity-based endpoints).
- Add administrator capability to deactivate and reactivate users through logical status (no physical delete).
- Enforce that deactivated users cannot authenticate and therefore cannot obtain JWT access tokens.
- **BREAKING**: Remove API/domain contracts and persistence behavior tied to `AdventureMember`.

## Capabilities

### New Capabilities
- `identity-user-lifecycle-management`: Administrative user lifecycle operations in Identity, including deactivate/reactivate with logical (non-physical) deletion semantics.

### Modified Capabilities
- `jwt-identity-authentication`: Authentication behavior changes to deny login when an Identity user is deactivated.

## Impact

- Affected code: Domain (`AdventureMember` aggregate), application use cases/handlers, infrastructure mappings/repositories, API controllers/endpoints, and tests linked to `AdventureMember`.
- Affected API: removal of `AdventureMember` endpoints/contracts; user lifecycle endpoints under Identity gain deactivate/reactivate behavior.
- Affected persistence: user status is stored and managed in Identity user data; no physical user deletion for lifecycle deactivation.
- Affected security: JWT issuance path must reject deactivated users.
