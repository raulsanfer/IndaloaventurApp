## 1. Session capability and Settings navigation

- [x] 1.1 Add the XML-documented `AuthSession` capability for `Admin OR (InventoryManager AND IsMember)` and verify unit tests cover an Admin non-member, a delegated member, a delegated non-member, and a user without either role.
- [x] 1.2 Extend the Settings page and shared Settings view with a dedicated inventory-management group while keeping unrelated administration Admin-only; add localized resource keys and global SCSS only as needed, and verify component tests show exactly the two delegated entries and no user-management entry.
- [x] 1.3 Replace the Admin-only guards of the inventory and administrative-reservations routes with the session capability before component initialization; verify delegated members load both routes and ineligible or anonymous sessions redirect without any inventory or reservation request.

## 2. Delegated inventory and reservation experience

- [x] 2.1 Apply the common capability to the inventory-management presentation so a delegated member can use all existing category, status, and material write actions while an ineligible session cannot invoke a write; verify bUnit tests exercise both paths and that successful writes refresh server data.
- [x] 2.2 Apply the common capability to administrative material reservations and preserve the existing personal-reservation ownership flow; verify a delegated member can list, view, collect, cancel, and confirm return, while `me` routes remain bound to the current owner and member condition.
- [x] 2.3 Complete the inventory-management error flow for `auth.session_invalid` and `inventory.forbidden`, aligned with the reservation flow; verify `401` clears persisted session and redirects to sign-in, and `403` keeps the session, prevents further affected operations, and exposes only a localized permission message.

## 3. Administrator assignment of the delegated role

- [x] 3.1 Add the XML-documented managed-user update request and `IAdminUserManagementService` operation, then implement `PUT /api/users/{userId}` in the typed client with the complete `email`, `isMember`, and `roles` body; verify HTTP-client tests assert verb, route, bearer header, body preservation, `204` without deserialization, and safe mappings for `400`, `401`, `403`, network, timeout, and invalid payload failures.
- [x] 3.2 Extend each existing user-management row with the localized `Gestor/a de inventario` editor and explicit save/normalization action; preserve unrelated roles, allow enabling only for members, and show disabled inactive legacy state for non-members; verify component tests for grant, removal, role preservation, disabled non-member, and normalization of a legacy `InventoryManager` role.
- [x] 3.3 After a successful role update, reload the current filtered or complete user list and notify an administrator that the target must sign in again; when the target is the current administrator, clear the local session and navigate to sign-in. Verify bUnit tests for `204` refresh, own-account sign-out, update `401` sign-out, and update `403` session preservation with no optimistic role change.

## 4. Documentation, security regression, and acceptance validation

- [x] 4.1 Add negative and end-to-end component/client coverage for the full authorization matrix, direct-route no-load guarantee, stale-token `401`, retained-session `403`, and unchanged `/me` reservation isolation; verify all security-sensitive scenarios in both delta specs are represented by passing tests.
- [x] 4.2 Review `docs/features/settings-admin.md` and `docs/architecture` for the new delegated boundary, update conceptual documentation and public XML comments where required, and verify documentation with `docfx docs/docfx.json`; do not commit generated documentation output.
- [x] 4.3 Run `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj` and `openspec validate frontend-inventory-management-delegation --strict`, resolve regressions attributable to this change, and mark implementation tasks complete only after their stated verification passes.
