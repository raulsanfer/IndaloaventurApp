## ADDED Requirements

### Requirement: Delegated inventory role assignment from user administration
The user-management interface MUST remain available only to `Admin`. In the existing edit flow, it MUST show an administrator a control labelled `Gestor/a de inventario` whose state is bound to the presence of `InventoryManager` in the managed user's roles. The control MUST preserve every role not managed by this control when it prepares the complete role collection for update.

#### Scenario: Administrator grants inventory management to a member
- **WHEN** an administrator marks `Gestor/a de inventario` for a managed user with `IsMember` true and saves
- **THEN** the system MUST send `PUT /api/users/{userId}` with the managed user's current email, `IsMember`, and the complete roles collection including `InventoryManager`
- **THEN** the system MUST preserve roles other than `InventoryManager`

#### Scenario: Administrator removes inventory management from a member
- **WHEN** an administrator clears `Gestor/a de inventario` for a managed member and saves
- **THEN** the system MUST send the complete roles collection without `InventoryManager`
- **THEN** the system MUST preserve all unrelated roles

### Requirement: Member condition constrains delegated-role editing
The system MUST allow `Gestor/a de inventario` to be activated only for a managed user with `IsMember` true. If the managed user is not a member, the system MUST render the control disabled and MUST remove `InventoryManager` from the outgoing roles collection on save, including for a legacy inconsistent user that already has that role.

#### Scenario: Administrator views a non-member with a legacy inventory-manager role
- **WHEN** the managed user has `IsMember` false and the returned roles include `InventoryManager`
- **THEN** the system MUST render `Gestor/a de inventario` disabled
- **THEN** the system MUST explain that the permission is inactive until the member condition is restored

#### Scenario: Administrator saves a non-member with a legacy inventory-manager role
- **WHEN** an administrator saves a managed user whose `IsMember` is false and whose loaded roles include `InventoryManager`
- **THEN** the system MUST send the complete roles collection without `InventoryManager`

### Requirement: Confirmed user-update and session-revocation experience
The system MUST treat `204 No Content` from `PUT /api/users/{userId}` as a successful update without attempting to deserialize a response body, and MUST reload the managed user before presenting the confirmed role state. After a successful change to roles or member condition, it MUST tell the administrator that the managed user must sign in again. If the administrator changes their own roles or member condition, the system MUST clear that administrator's session and navigate to sign-in because the current token is invalidated.

#### Scenario: Administrator updates another user's delegated role
- **WHEN** `PUT /api/users/{userId}` returns `204 No Content` for another user
- **THEN** the system MUST reload the managed user state
- **THEN** the system MUST inform the administrator that the changed user must sign in again for the new claims to apply

#### Scenario: Administrator updates their own roles or member condition
- **WHEN** the administrator successfully updates their own roles or member condition
- **THEN** the system MUST clear the local session and navigate to sign-in

### Requirement: Safe update authorization failures in user management
The system MUST clear the local session and navigate to sign-in when the user-update operation returns `401 Unauthorized`. When it returns `403 Forbidden`, it MUST retain the local session, preserve unsaved context where safe, and show a permissions error without claiming that the update succeeded.

#### Scenario: User update token is no longer valid
- **WHEN** the managed-user update returns `401 Unauthorized`
- **THEN** the system MUST clear the local session and navigate to sign-in

#### Scenario: User update is forbidden
- **WHEN** the managed-user update returns `403 Forbidden`
- **THEN** the system MUST preserve the local session and display a permissions error
- **THEN** the system MUST NOT change the visible managed-user roles as though the update had succeeded
