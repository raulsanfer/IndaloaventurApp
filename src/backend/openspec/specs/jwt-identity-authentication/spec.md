# jwt-identity-authentication Specification

## Purpose
TBD - created by archiving change bootstrap-clean-architecture-ddd-api. Update Purpose after archive.
## Requirements
### Requirement: Identity-backed authentication
The system MUST manage users and roles using ASP.NET Identity as the authoritative identity store, and any user-facing authentication/registration error message SHALL be returned in Spanish.

#### Scenario: User registration success
- **WHEN** a valid registration request is submitted
- **THEN** the system SHALL create an Identity user record and apply configured password policies

### Requirement: JWT token issuance and validation
The system MUST issue JWT access tokens for authenticated users and MUST validate tokens for protected endpoints, and authentication failure messages exposed to clients SHALL be in Spanish. Authentication responses MUST expose the persisted `IsMember` state of the authenticated user, and issued tokens MUST include a stable `IsMember` claim serialized as `true` or `false`.

#### Scenario: Successful login token issuance
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return a signed JWT containing subject, authorization claims, and the `IsMember` claim with the persisted user value

#### Scenario: Successful login response includes membership flag
- **WHEN** valid user credentials are provided
- **THEN** the system SHALL return `LoginResponse.IsMember` with the same boolean value emitted in the `IsMember` claim

#### Scenario: Protected endpoint access denied
- **WHEN** a request to a protected endpoint includes no token or an invalid token
- **THEN** the system SHALL return an unauthorized response with Spanish user-facing detail

### Requirement: Role-based authorization
The system MUST enforce role-based access policies on endpoints marked with role requirements, and authorization-denied messages exposed to clients SHALL be in Spanish.

#### Scenario: Role mismatch
- **WHEN** an authenticated user without a required role calls a role-protected endpoint
- **THEN** the system SHALL return a forbidden response with Spanish user-facing detail

### Requirement: User management admin endpoints are regression-tested under authorization rules
The system MUST keep automated regression tests for admin-only user-management endpoints protected by JWT and role-based authorization.

#### Scenario: Admin access remains allowed
- **WHEN** an authenticated user with role `Admin` calls `api/users` management endpoints
- **THEN** automated tests SHALL confirm the request is authorized and endpoint-specific success codes are returned for valid payloads

#### Scenario: Non-admin access remains blocked
- **WHEN** an authenticated user without role `Admin` calls `api/users` management endpoints
- **THEN** automated tests SHALL confirm forbidden response semantics

