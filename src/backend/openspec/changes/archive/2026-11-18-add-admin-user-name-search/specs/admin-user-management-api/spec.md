## MODIFIED Requirements

### Requirement: User listing by administrator
The system MUST allow an administrator to list managed users through `GET /api/users`, optionally filtering by `email` and `nombre`. Filters SHALL be trimmed, ignore case, and be absent when blank. `nombre` MUST match literal text anywhere in the linked `FichaSocio.Nombre` OR `FichaSocio.Apellidos`. Active filters SHALL combine with AND. Results SHALL preserve the response contract and be ordered by email ascending.

#### Scenario: Admin lists all users without filter
- **WHEN** an authenticated administrator requests `GET /api/users` without effective `email` or `nombre` filters
- **THEN** the system SHALL return the complete managed-user collection ordered by email ascending, including users without a `FichaSocio`

#### Scenario: Admin filters users by partial email
- **WHEN** an authenticated administrator requests `GET /api/users` with an `email` query parameter containing part of an email address and no effective `nombre` filter
- **THEN** the system SHALL return only users whose Identity email contains that value case-insensitively, including matching users without a `FichaSocio`

#### Scenario: Admin searches email with no matches
- **WHEN** an authenticated administrator requests `GET /api/users` with an `email` query parameter that matches no users
- **THEN** the system SHALL return `200 OK` with an empty collection

#### Scenario: Admin searches a fragment inside a name
- **GIVEN** a managed user has a linked ficha with `Nombre = Ana Marina` and `Apellidos = Torres Vidal`
- **WHEN** an authenticated administrator requests `GET /api/users?nombre=ari`
- **THEN** the system SHALL include that user because the fragment occurs inside `Nombre`
- **AND** the system SHALL exclude users whose linked ficha contains the fragment in neither field

#### Scenario: Admin searches a fragment inside either surname
- **GIVEN** a managed user has a linked ficha with `Nombre = Lucía` and `Apellidos = García Montes`
- **WHEN** an authenticated administrator searches with `nombre=arc` or with `nombre=ont`
- **THEN** the system SHALL include that user because the fragment occurs within the first or second surname, respectively

#### Scenario: Name search supports beginning middle end and full-field matches
- **GIVEN** a managed user has a linked ficha with `Nombre = Daniela` and `Apellidos = Morales`
- **WHEN** an authenticated administrator searches with any of `nombre=Dan`, `nombre=nie`, `nombre=ela`, `nombre=Daniela`, `nombre=Mor`, `nombre=ora`, `nombre=les`, or `nombre=Morales`
- **THEN** the system SHALL include that user without requiring an exact full-field match

#### Scenario: Name search ignores case and surrounding whitespace
- **GIVEN** a managed user has a linked ficha with `Nombre = Ana Marina`
- **WHEN** an authenticated administrator requests `GET /api/users?nombre=%20%20aRI%20%20`
- **THEN** the system SHALL include that user after trimming the filter and ignoring letter case

#### Scenario: Empty name filter preserves email filtering and full listing
- **WHEN** an authenticated administrator supplies `nombre` as an empty or whitespace-only value
- **THEN** the system SHALL apply only the effective email filter if present
- **AND** the system SHALL return the complete managed-user collection if neither filter is effective

#### Scenario: Admin combines email and name filters
- **GIVEN** one user's email and linked ficha both match the supplied filters, another user's email alone matches, and a third user's linked ficha alone matches
- **WHEN** an authenticated administrator requests `GET /api/users` with both effective `email` and `nombre` filters
- **THEN** the system SHALL return only the user satisfying both conditions

#### Scenario: User without a ficha does not match a name filter
- **GIVEN** a managed user has no linked `FichaSocio`, even if their email contains the requested name fragment
- **WHEN** an authenticated administrator supplies an effective `nombre` filter
- **THEN** the system SHALL exclude that user without failing the request

#### Scenario: Name search uses linked profile data independently of membership flag
- **GIVEN** a managed user has a matching linked `FichaSocio` and `IsMember = false`
- **WHEN** an authenticated administrator supplies the matching `nombre` filter
- **THEN** the system SHALL include the user without requiring `IsMember = true` or a particular managed-user role

#### Scenario: Admin searches name with no matches
- **WHEN** an authenticated administrator supplies an effective `nombre` filter, alone or combined with `email`, and no user satisfies all active filters
- **THEN** the system SHALL return `200 OK` with an empty collection

#### Scenario: Name search treats SQL wildcard characters as literal text
- **WHEN** an authenticated administrator searches with a `nombre` value containing `%`, `_`, or `[`
- **THEN** those characters SHALL be matched as literal text and SHALL NOT broaden the search as SQL wildcard expressions

#### Scenario: Name search preserves response contract and ordering
- **WHEN** an authenticated administrator obtains multiple results using `nombre`, alone or with `email`
- **THEN** the system SHALL return each user once, ordered by email ascending, with the existing `UserId`, `Email`, `IsMember`, and `Roles` fields

#### Scenario: Unauthenticated client attempts a name search
- **WHEN** an unauthenticated client requests `GET /api/users` with a `nombre` filter
- **THEN** the system SHALL return `401 Unauthorized`

#### Scenario: Non-admin client attempts a name search
- **WHEN** an authenticated client without administrative authorization requests `GET /api/users` with a `nombre` filter
- **THEN** the system SHALL return `403 Forbidden`
