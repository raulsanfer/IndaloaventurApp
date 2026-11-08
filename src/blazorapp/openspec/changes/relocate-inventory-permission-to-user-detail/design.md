## Context

See `proposal.md` for the motivation and the delta specification for the behavioural contract. The users list currently owns the `InventoryManager` checkbox, its role-merging logic, the call to `PUT /api/users/{userId}`, and the associated session/error handling. The administrative member-detail view already loads the managed-user representation alongside the member profile and is the existing destination for editing a member.

The update endpoint accepts the managed user's email, member condition, and complete role collection, returns `204 No Content`, and invalidates affected tokens when roles change. Server-side authorization remains the authority; interface checks and feedback are defence in depth.

## Goals / Non-Goals

**Goals:**

- Put the focused inventory-permission editor in the administrative member-detail experience without changing its API contract.
- Preserve unrelated roles and the existing confirmed-state, session-revocation, and authorization-failure behaviour.
- Keep the listing limited to discovery and navigation actions.

**Non-Goals:**

- Do not add the future inventory-permission filter to the users list.
- Do not introduce a new route, permission model, endpoint, dependency, or client-side authorization source.
- Do not combine a member-profile save with a role update; the two requests have different contracts and failure modes.

## Decisions

### Move the editor as an independent section of the administrative member detail

The member detail will use its loaded `ManagedUserItem` to render a clearly labelled inventory-permission section with its own selection and save action. It will remain independent from the profile `EditForm`, so a failed role change cannot submit profile changes (or vice versa). The list will retain only its existing badges, navigation, creation, and search behaviour; all inventory-role state, controls, and update feedback will be removed from it.

The detail route represents a member, which naturally aligns the editor with the existing `InventoryManager` member eligibility condition. The section will only be available once both the managed-user and member-profile data load successfully.

Alternatives considered:

- Keep a compact checkbox in the list and duplicate it in the detail: rejected because it leaves two mutation surfaces with ambiguous confirmed state.
- Add a new user-detail route usable for non-members: rejected because it expands navigation and administration scope beyond relocating the existing control.
- Include the permission in the member-profile `EditForm`: rejected because it would blend unrelated endpoint contracts and create partial-save ambiguity.

### Reuse the established complete-role update and reload flow

The detail will build a new roles collection by removing only case-insensitive `InventoryManager` values from the loaded roles and adding the canonical role only when selected. It will submit the existing update request with the currently loaded managed-user identity fields. After `204`, it will reload the managed-user state (and retain or refresh the member profile as necessary) before showing success; it will never deserialize the empty response or apply an optimistic role change.

When a profile save can change data also required by the managed-user update contract, the detail will refresh the managed-user representation before enabling a later permission save. This avoids sending stale identity data from a long-lived edit screen.

Alternatives considered:

- Send a roles collection containing only `InventoryManager`: rejected because it can silently remove `Admin`, `Member`, or future roles.
- Reuse the list model after navigation: rejected because it can be stale and would make route state a source of privilege-related data.
- Render the new state immediately after `204`: rejected because the server is authoritative and the response contains no representation.

### Carry the update failure and token-revocation rules to the new boundary

The detail will map the existing typed-client error results to localized feedback. A successful own-account role change signs out and redirects. A `401` similarly clears local state and redirects; a `403` preserves the session, leaves the confirmed role unchanged, and disables or stops the failed submission for the current interaction. The page continues to be Admin-only and the server reauthorizes every operation.

This addresses OWASP A01 by keeping authorization server-side and never treating the displayed user ID or checkbox state as proof of access. It also addresses A08 and A10 by handling the no-content contract and authorization failures explicitly rather than trusting client state or exposing internals.

Alternatives considered:

- Treat a `403` as sign-out: rejected because authorization failure does not prove the session is invalid.
- Refresh local claims after a role update: rejected because only a new authentication token can supply confirmed claims.
- Retry automatically after a failed request: rejected because it could repeat an unauthorized or conflicting mutation.

### Preserve shared presentation conventions and focused regression coverage

Existing shared resource keys and global SCSS conventions will be reused or extended in the established resource and SCSS files. The implementation will not add inline or component-scoped styles. Tests will move the view-level success, preservation, self-update, `401`, and `403` cases from the list surface to the detail surface, while keeping HTTP-client contract coverage intact.

## Risks / Trade-offs

- [Administrators lose in-list bulk-like visibility of the control] → The list stays focused and the proposed later filter can address discovery separately without coupling it to mutation.
- [The detail has independently loaded user and profile data] → Render and enable the section only after the managed-user state is confirmed, and reload it after relevant saves.
- [A role update revokes the active token] → Use the established re-authentication notice and self-update sign-out flow; treat `401` and `403` distinctly.
- [UI relocation is mistaken for access control] → Keep Admin route protection, submit through the existing authorized API, and retain negative authorization tests.

## Migration Plan

1. Deploy the list cleanup and member-detail permission section together; no persisted-data migration or API deployment is required.
2. Run the focused component/client tests, the full frontend test project, strict OpenSpec validation, and DocFX verification if documentation changes.
3. To roll back, redeploy the prior presentation while retaining the existing endpoint and role data; the backend authorization policy remains unchanged throughout.
