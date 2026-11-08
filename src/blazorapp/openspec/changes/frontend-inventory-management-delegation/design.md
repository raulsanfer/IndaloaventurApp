## Context

See `proposal.md` for the motivation and the delta specifications for the behavioural contract. The current frontend already has `AuthSession` with roles and `IsMember`, separate routes for inventory and administrative reservations, and typed clients that return stable errors for `401` and `403`. Those routes, `SettingsView`, and their tests currently recognise only `Admin`.

`GET /api/users` already supplies `userId`, `email`, `isMember`, and the complete roles collection. The backend accepts `PUT /api/users/{userId}` with `email`, `isMember`, and roles, replies with `204 No Content`, and invalidates tokens when roles or membership change. The existing users list is the only administration surface that represents both members and non-members; the member-file editor cannot present a non-member with an inconsistent inherited role.

The active changes `frontend-inventory-management` and `frontend-admin-material-reservations` supply the areas that this proposal authorizes. The backend's `delegate-inventory-management` change already applies the matching server-side policy, so frontend checks are defence-in-depth and experience controls only.

## Goals / Non-Goals

**Goals:**

- Establish one readable client-side capability that exactly mirrors the backend eligibility rule and is used consistently at Settings and route boundaries.
- Keep role assignment in the existing user-management list so administrators can inspect and normalize every managed user, including non-members.
- Make token revocation, `401`, and `403` outcomes deterministic without optimistic permission or role updates.
- Preserve the existing shared-component, typed-client, localization, and global-SCSS conventions.

**Non-Goals:**

- Do not create a client-side authorization mechanism, a new permission claim, a new user-management route, or a role-management endpoint.
- Do not change the API policy, member-owned reservation routes, other Settings permissions, persisted session format, or the inventory/reservation business rules.
- Do not merge the member-file update and role update into a multi-endpoint save operation; they have independent API contracts and failure modes.

## Decisions

### Add a capability method to `AuthSession`

Add a public `CanManageInventory()` method that returns `IsInRole("Admin") || (IsMember && IsInRole("InventoryManager"))`. Settings, both route guards, and presentation decisions will use this method rather than reproduce the boolean expression or derive the current user's permission from `ManagedUserItem`.

`AuthSession` already owns role checks such as member-reservation and licence capabilities, so it is the narrowest reusable location and preserves its case-insensitive role semantics. It also keeps a non-member administrator valid.

Alternatives considered:

- Repeat the expression in each page and component: rejected because the member condition could be omitted in one of the guards.
- Obtain the current user's roles from `GET /api/users`: rejected because that call is Admin-only and can be stale relative to the JWT.
- Add a client-side permission service: rejected because there is only one compound rule and a second state holder would increase synchronization risk.

### Split Settings entries by responsibility while sharing the capability

Keep the current Admin-only Settings group for users and unrelated administration. Add a distinct inventory-management group that is visible whenever `CanManageInventory()` is true and contains only inventory and administrative-reservation links. Pass both `IsAdmin` and `CanManageInventory` from the Settings page to the shared Settings view. Update the inventory and reservation page guards to allow the new capability before any child component initializes or invokes its service.

This gives a delegated user exactly the two relevant entry points instead of an Admin-labelled area with inaccessible controls, while direct-route protection remains fail-closed.

Alternatives considered:

- Keep a single Admin group and add the two links for inventory managers: rejected because it misleadingly exposes an administrative group to a non-admin and complicates future permissions.
- Allow the route but hide only buttons: rejected because it would initialize privileged administrative lists for an ineligible session.

### Edit the delegated role in the managed-user list with an explicit save

Extend the existing row for each `ManagedUserItem` with a local inventory-role editor and a save action. Its model starts from the response roles, toggles only the canonical `InventoryManager` role, and sends `PUT /api/users/{userId}` through a new documented `UpdateManagedUserAsync` service operation. The request body contains only the API contract fields (`email`, `isMember`, and the full roles collection); the route carries the user identifier.

For a non-member, the checkbox remains disabled. A legacy row whose roles contain `InventoryManager` shows an explanatory inactive state, and its save/normalization action removes that role while leaving every unrelated role untouched. On `204`, no body is read; the current filtered or unfiltered list is reloaded to obtain the confirmed state. The user-table editor remains separate from the member-file form, avoiding a partial cross-endpoint save.

If the updated `userId` equals the current session user ID and the update changed roles or `IsMember`, call `SignOutAsync` and navigate to sign-in after the successful `204`. For any other target, show the re-login notice after list refresh.

Alternatives considered:

- Put the control solely in `AdminMemberProfileView`: rejected because the normal user-management flow does not open that view for non-members, so it cannot display or normalize the required legacy state.
- Replace the managed roles collection with a list containing only `InventoryManager`: rejected because it would silently remove `Admin`, `Member`, or future roles.
- Optimistically change the row after `204`: rejected because there is no response representation and server normalization remains authoritative.

### Centralize status-to-session handling at feature boundaries

Keep the typed clients responsible for mapping HTTP status codes to stable `ServiceError` codes. Extend the inventory-management component's existing error flow to handle `auth.session_invalid` and `inventory.forbidden` like the reservations component already handles `auth.session_invalid` and `reservations.forbidden`. Extend the users client and list editor with equivalent handling for its update operation.

A `401` clears persisted local state and forces sign-in. A `403` keeps the session, stops the failed operation, prevents further corresponding write attempts in the affected view, and shows a localized permissions message. No client can refresh JWT claims locally; the confirmed path after an identity change is signing in again.

Alternatives considered:

- Sign out on every `403`: rejected because it turns a valid, lower-privilege session into an authentication failure.
- Automatically retry after `403` or use role data from the edited row: rejected because neither establishes new authorization and could repeat an unauthorized action.
- Deserialize a response after `204`: rejected because the response intentionally has no body.

### Preserve existing transport and styling boundaries

The new update request and XML-documented service method live in SharedUI, while its `HttpClient` implementation remains in Web.Client. New labels, notice, success, and error messages use shared resource keys. The list-row and Settings-group presentation reuses the existing global SCSS partial structure; no inline or component-scoped styles and no extra packages are introduced.

## Risks / Trade-offs

- [Backend and frontend eligibility diverge later] → Keep the expression small and covered by an Admin/non-member, delegated-member, delegated-non-member matrix; the backend remains the authority and `403` safely stops the feature.
- [A stale token or an administrator's own update invalidates the current token] → React to `401` by clearing local session, and deliberately sign out the editor of their own roles/member condition following success.
- [A legacy non-member retains `InventoryManager`] → Surface it as inactive and provide an idempotent normalization save that preserves unrelated roles.
- [The active inventory and reservation changes have not yet been archived] → Implement against their established components and endpoints, keep this change limited to authorization and user-assignment concerns, and archive only after their behaviour is integrated and verified.
- [A `403` occurs after a route initially passed its client guard] → Preserve the session but disable the affected privileged operation and avoid revealing response internals or stale administrative data.

## Migration Plan

1. Merge or make available the backend policy and the inventory/reservation frontend components on which this change depends.
2. Deploy the capability and route/navigation updates together with the user-role editor and client update operation.
3. Verify the role matrix with feature tests and the full frontend test project; then confirm an administrator can grant the role and that the delegate obtains access only after a fresh sign-in.
4. To roll back, remove the delegated Settings group and route allowance and hide the user-role editor. Existing `InventoryManager` assignments remain harmless because server authorization remains authoritative and can be reverted separately by an administrator.
