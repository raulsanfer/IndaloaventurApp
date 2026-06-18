# wordpress-post-detail-query Specification

## Purpose
TBD - created by archiving change add-wordpress-post-detail-by-slug. Update Purpose after archive.
## Requirements
### Requirement: Authenticated users can read a WordPress post by slug
The system MUST allow an authenticated user to retrieve the full detail of a WordPress post by providing its `slug`, using the backend WordPress integration as the source of truth.

#### Scenario: Existing slug returns post detail
- **WHEN** an authenticated user requests a WordPress post using an existing `slug`
- **THEN** the system SHALL return the post detail including `Id`, `Slug`, `Titulo`, `Resumen`, `Contenido`, `Enlace` and `FechaPublicacionUtc`

### Requirement: Missing WordPress slug returns stable not-found semantics
The system MUST return a stable not-found response when the requested WordPress `slug` does not exist.

#### Scenario: Unknown slug
- **WHEN** an authenticated user requests a WordPress post with a `slug` that does not exist in WordPress
- **THEN** the system SHALL return a not-found response with user-facing detail in Spanish

