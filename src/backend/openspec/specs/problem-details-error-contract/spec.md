# problem-details-error-contract Specification

## Purpose
TBD - created by archiving change bootstrap-clean-architecture-ddd-api. Update Purpose after archive.
## Requirements
### Requirement: RFC 7807 compliant error responses
The system MUST return errors using `application/problem+json` with ProblemDetails fields (`type`, `title`, `status`, `detail`, `instance`), and the `title` and `detail` user-facing text SHALL be in Spanish.

#### Scenario: Unhandled server error
- **WHEN** an unhandled exception occurs during request processing
- **THEN** the API SHALL return a ProblemDetails response with HTTP 500 status and Spanish user-facing text

### Requirement: Validation error normalization
The system MUST translate validation failures into structured ProblemDetails responses that include field-level error information, and validation messages exposed to clients SHALL be in Spanish.

#### Scenario: Invalid command payload
- **WHEN** a request fails command validation rules
- **THEN** the API SHALL return a ProblemDetails response with HTTP 400 status and validation error details in Spanish

### Requirement: Domain and authorization error mapping
The system MUST map known domain and authorization exceptions to deterministic HTTP status codes and ProblemDetails payloads, with user-facing text in Spanish.

#### Scenario: Domain rule conflict
- **WHEN** a domain rule rejects an operation due to business conflict
- **THEN** the API SHALL return a ProblemDetails response with an appropriate 4xx status code and Spanish user-facing detail

