## ADDED Requirements

### Requirement: RFC 7807 compliant error responses
The system MUST return errors using `application/problem+json` with ProblemDetails fields (`type`, `title`, `status`, `detail`, `instance`).

#### Scenario: Unhandled server error
- **WHEN** an unhandled exception occurs during request processing
- **THEN** the API SHALL return a ProblemDetails response with HTTP 500 status

### Requirement: Validation error normalization
The system MUST translate validation failures into structured ProblemDetails responses that include field-level error information.

#### Scenario: Invalid command payload
- **WHEN** a request fails command validation rules
- **THEN** the API SHALL return a ProblemDetails response with HTTP 400 status and validation error details

### Requirement: Domain and authorization error mapping
The system MUST map known domain and authorization exceptions to deterministic HTTP status codes and ProblemDetails payloads.

#### Scenario: Domain rule conflict
- **WHEN** a domain rule rejects an operation due to business conflict
- **THEN** the API SHALL return a ProblemDetails response with an appropriate 4xx status code