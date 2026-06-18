## ADDED Requirements

### Requirement: User management admin endpoints are regression-tested under authorization rules
The system MUST keep automated regression tests for admin-only user-management endpoints protected by JWT and role-based authorization.

#### Scenario: Admin access remains allowed
- **WHEN** an authenticated user with role `Admin` calls `api/users` management endpoints
- **THEN** automated tests SHALL confirm the request is authorized and endpoint-specific success codes are returned for valid payloads

#### Scenario: Non-admin access remains blocked
- **WHEN** an authenticated user without role `Admin` calls `api/users` management endpoints
- **THEN** automated tests SHALL confirm forbidden response semantics
