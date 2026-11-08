## 1. Simplify the administrative users list

- [x] 1.1 Remove the `InventoryManager` selector, update action, role-selection state, and permission-specific feedback from `AdminUsersManagementView`, while retaining user search, member-file navigation, member-file creation, badges, and federation-license navigation; verify `AdminUsersManagementViewTests` proves the list does not render or submit the inventory-permission control.
- [x] 1.2 Remove update-only dependencies and obsolete localized/style references from the list without affecting the shared managed-user update client; verify the frontend solution builds with no unused-reference or Razor compilation errors.

## 2. Manage the permission in the member detail

- [x] 2.1 Add an independent, localized inventory-permission section to `AdminMemberProfileView` that initializes from the loaded managed-user roles and updates only `InventoryManager` in the complete role collection; verify bUnit tests cover grant, withdrawal, role preservation, and no optimistic role change.
- [x] 2.2 After a successful no-content update, reload the managed-user state and show the re-authentication notice; if profile edits can affect update-contract data, refresh that state before a later permission save. Verify bUnit tests cover the confirmed refresh and use of current managed-user data.
- [x] 2.3 Carry the safe update paths to the detail: sign out and redirect after a successful own-role change or `401`, retain the session and show a localized error after `403`, and prevent duplicate submissions; verify bUnit tests cover each path and retain the existing typed-client `204`/status-mapping regression coverage.
- [x] 2.4 Add or relocate shared resource keys and global SCSS only where required by the new detail section, following repository conventions with no inline or component-scoped styles; verify the rendered component has accessible section, label, and feedback semantics.

## 3. Documentation, security, and acceptance validation

- [x] 3.1 Review whether the change affects `docs/architecture` or `docs/features`, update documentation if needed, and verify with `docfx docs/docfx.json`.
- [x] 3.2 Review the OWASP A01, A08, and A10 behaviours for the relocated role update, including server-side authorization, preserved unrelated roles, `401`, `403`, and self-update token revocation; verify the negative component tests in this change pass.
- [x] 3.3 Run `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj` and `openspec validate relocate-inventory-permission-to-user-detail --strict`, resolve regressions attributable to this change, and mark implementation tasks complete only after the stated verification passes.
