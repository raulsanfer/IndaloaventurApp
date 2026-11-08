## Purpose

Define a consistent and safe frontend experience for delegated inventory management, while preserving the backend as the final authorization authority.

## ADDED Requirements

### Requirement: Session-based inventory management capability
The system MUST determine whether the authenticated session can manage inventory exclusively from the current session claims and member condition. The capability MUST be true for `Admin` regardless of `IsMember`, and for `InventoryManager` only when `IsMember` is true. A user-list row, a route parameter, or other client-supplied state MUST NOT be used as the authorization source for the current session.

#### Scenario: Administrator who is not a member opens inventory administration
- **WHEN** the current session has `Admin` and `IsMember` is false
- **THEN** the system MUST treat the session as able to manage inventory

#### Scenario: Member delegated as inventory manager opens inventory administration
- **WHEN** the current session has `InventoryManager` and `IsMember` is true
- **THEN** the system MUST treat the session as able to manage inventory

#### Scenario: Delegated user without member condition opens inventory administration
- **WHEN** the current session has `InventoryManager` and `IsMember` is false
- **THEN** the system MUST treat the session as unable to manage inventory

### Requirement: Delegated navigation and direct-route protection
The system MUST expose the inventory-management and administrative-reservations entries and routes to a session that can manage inventory. It MUST keep user administration and all other administrative areas exclusive to `Admin`. When a session cannot manage inventory, it MUST not display those entries and MUST redirect a direct visit to either protected route without initializing its administrative data.

#### Scenario: Delegated member opens Settings
- **WHEN** a member session with `InventoryManager` opens Settings
- **THEN** the system MUST display entries for inventory management and administrative material reservations
- **THEN** the system MUST NOT display the users-management entry or other unrelated administrative entries

#### Scenario: Non-delegated session visits an inventory-management route directly
- **WHEN** an authenticated session that cannot manage inventory visits an inventory-management or administrative-reservations route
- **THEN** the system MUST redirect to Settings
- **THEN** the system MUST NOT request administrative inventory or reservation data

### Requirement: Delegated inventory and administrative-reservation operations
For a session that can manage inventory, the system MUST make available and execute the existing create, edit, delete, listing, detail, pickup, cancellation, and return-confirmation operations for inventory categories, statuses, materials, and administrative material reservations. For a session that cannot manage inventory, it MUST hide or disable those write controls and MUST prevent their request from being issued.

#### Scenario: Delegated member manages a material catalogue entry
- **WHEN** a member session with `InventoryManager` opens the inventory-management area
- **THEN** the system MUST display the category, status, and material write controls
- **THEN** a completed write MUST use the existing inventory endpoint and refresh the affected data from the server

#### Scenario: Delegated member processes an administrative reservation
- **WHEN** a member session with `InventoryManager` opens administrative material reservations
- **THEN** the system MUST allow listing and detail retrieval and the valid pickup, cancellation, and administrative return-confirmation actions
- **THEN** each completed action MUST refresh the affected reservation data from the server

#### Scenario: User without the capability attempts a write through rendered client state
- **WHEN** a session without inventory-management capability invokes an inventory or administrative-reservation write action
- **THEN** the system MUST not issue the administrative request
- **THEN** the system MUST leave the backend authorization decision unchanged

### Requirement: Delegation does not alter reservation ownership
The system MUST preserve the member-owned semantics of `/api/inventario-reservas/me` routes. `InventoryManager` alone MUST NOT provide access to another member's personal reservations or replace the member and ownership conditions required by those routes.

#### Scenario: Inventory manager accesses personal-reservation navigation
- **WHEN** a session has `InventoryManager` but does not satisfy the existing member-owned reservation conditions
- **THEN** the system MUST NOT expose another user's personal reservation data or actions
- **THEN** the system MUST continue to use only the current identity for personal reservation routes

### Requirement: Safe authorization failure handling for inventory management
The system MUST clear the local session and navigate to sign-in after an inventory-related `401 Unauthorized`. After a `403 Forbidden`, it MUST preserve the local session, stop the failed action, hide or disable the unavailable action when applicable, and present a permissions message without exposing server internals.

#### Scenario: Revoked token receives unauthorized response
- **WHEN** an inventory or administrative-reservation request returns `401 Unauthorized`
- **THEN** the system MUST clear the persisted local session
- **THEN** the system MUST navigate to sign-in

#### Scenario: Session receives forbidden response
- **WHEN** an inventory or administrative-reservation request returns `403 Forbidden`
- **THEN** the system MUST keep the session locally authenticated
- **THEN** the system MUST not retry the failed operation automatically and MUST show a permissions error
