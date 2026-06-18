## Context

The backend currently has two overlapping user concepts: ASP.NET Identity users and the `AdventureMember` entity. Identity is already the canonical authentication/authorization source, so `AdventureMember` introduces duplication and potential inconsistency. The requested behavior requires that administrators can deactivate users without deleting them and that deactivated users are blocked from login.

## Goals / Non-Goals

**Goals:**
- Consolidate user management on ASP.NET Identity only.
- Remove `AdventureMember` code paths and related tests.
- Ensure administrators can deactivate and reactivate users using logical status.
- Ensure deactivated users cannot authenticate and cannot receive JWT tokens.

**Non-Goals:**
- Reworking the full authentication architecture beyond deactivation checks.
- Physical deletion of users from the identity store.
- Introducing a new authorization model beyond existing admin role policies.

## Decisions

1. Use Identity user status as single source of truth
- Decision: Represent user lifecycle state in the Identity user record (e.g., lockout/status flag) and stop persisting lifecycle data in `AdventureMember`.
- Rationale: Prevents divergence between two user models and simplifies maintenance.
- Alternative considered: Keep `AdventureMember` for profile/lifecycle. Rejected due to duplication and unclear ownership.

2. Enforce logical deactivation at authentication boundary
- Decision: Login flow validates active status before JWT issuance; inactive users are rejected with unauthorized semantics.
- Rationale: Guarantees deactivated users are blocked consistently before token generation.
- Alternative considered: Allow token issuance and block downstream requests only. Rejected due to security and UX inconsistency.

3. Keep admin-only lifecycle operations
- Decision: Deactivate/reactivate actions are restricted to administrator-authorized endpoints.
- Rationale: Lifecycle control is a privileged operation and must remain explicit.
- Alternative considered: Self-deactivation by end users. Rejected as out of scope for this change.

4. Remove AdventureMember surface area end-to-end
- Decision: Remove domain entity, application handlers, persistence mappings, API endpoints, and tests specific to `AdventureMember`.
- Rationale: Full removal avoids dead code and accidental future coupling.
- Alternative considered: Soft-deprecate and keep unused paths temporarily. Rejected because the system already runs on Identity and this creates maintenance debt.

## Risks / Trade-offs

- [Risk] Existing clients may still call `AdventureMember` endpoints -> Mitigation: mark as breaking change in proposal/spec and update API documentation/changelog.
- [Risk] Inactive check could be bypassed if not centralized in auth flow -> Mitigation: enforce check directly in login/token issuance path and add tests.
- [Risk] Removing entity may break unrelated tests with hidden dependencies -> Mitigation: run test suites and replace fixtures with Identity-based equivalents.

## Migration Plan

1. Remove `AdventureMember` API/application/domain/infrastructure artifacts and related tests.
2. Add/confirm admin endpoints for deactivate/reactivate in Identity user management.
3. Add inactive-user check in authentication flow before JWT issuance.
4. Update integration/unit tests for admin lifecycle and inactive login denial.
5. Update API docs and run full test suite.

Rollback strategy:
- Revert this change set and redeploy previous version if production issues appear.
- No destructive data migration is required because user records remain in Identity.

## Open Questions

- Which Identity status mechanism is canonical for deactivation (`LockoutEnabled/LockoutEnd`, custom `IsActive`, or equivalent existing field) in the current codebase?
- Should deactivated login attempts return generic unauthorized only, or include a domain-specific error code for admin observability?
