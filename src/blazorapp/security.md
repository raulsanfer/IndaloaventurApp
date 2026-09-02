# Security Development Rules

This file is the security skill for this repository. Every planning, implementation, review, and validation task MUST consider these rules before changing application behavior.

Baseline: OWASP Top 10:2025 for web application risks.
Reference: https://owasp.org/Top10/2025/

## How To Use This Skill

- Read this file before implementing any user-facing feature, API call, authentication flow, data persistence change, dependency update, file upload/download feature, logging change, or deployment/configuration change.
- During planning, identify which OWASP categories are touched and add explicit security tasks when risk is non-trivial.
- During implementation, prefer secure framework defaults over custom security code.
- During review, verify the checklist at the end of this file and call out any residual risk.
- During validation, add or update tests for authorization, validation, error handling, and security-sensitive regressions whenever behavior changes.

## A01: Broken Access Control

- Enforce authorization on the server side for every protected operation; UI hiding is not access control.
- Use deny-by-default policies for pages, API endpoints, components, commands, and data queries that expose protected data.
- Check object-level access before reading, updating, deleting, exporting, or listing records.
- Never trust route parameters, query strings, form fields, hidden inputs, client state, or local storage as proof of access.
- Avoid direct object reference exposure unless authorization is enforced for each referenced object.
- Keep administrator, club manager, member, and anonymous behaviors explicitly separated.
- Add tests for unauthorized, wrong-role, wrong-owner, and expired-session cases when access rules change.

## A02: Security Misconfiguration

- Keep secure defaults in configuration files and document every required production override.
- Do not enable detailed errors, developer exception pages, debug endpoints, Swagger write access, seed credentials, or test-only behavior in production.
- Set security headers where applicable, including content type protection, frame restrictions, referrer policy, and a content security policy when the UI can support it.
- Restrict CORS to known origins; never use broad wildcard origins with credentials.
- Configure cookies with `HttpOnly`, `Secure`, and appropriate `SameSite` settings.
- Keep environment-specific secrets and URLs outside source control.
- Review deployment, hosting, and logging settings when adding external integrations or changing authentication.

## A03: Software Supply Chain Failures

- Prefer established dependencies already used by the repository before adding new packages.
- Add a dependency only when it clearly reduces implementation risk or complexity.
- Pin versions through the normal project package mechanism and avoid unreviewed floating versions.
- Check package source, maintenance status, license, and transitive impact before adding or upgrading dependencies.
- Do not run untrusted build scripts, copied commands, or generated code without inspection.
- Remove unused packages when replacing implementations.
- Run dependency restore/build/test commands after package changes and document unresolved advisories.

## A04: Cryptographic Failures

- Never implement custom cryptography, hashing, token signing, password storage, or random ID generation.
- Use platform cryptography APIs and framework identity primitives.
- Protect sensitive data in transit with HTTPS and secure cookie settings.
- Do not log passwords, tokens, session IDs, reset links, API keys, personal identifiers, or sensitive club/member data.
- Store secrets in environment variables, user secrets, secret managers, or deployment configuration, never in repository files.
- Use strong random values for security tokens and avoid predictable identifiers for sensitive workflows.
- Review data retention and masking when adding exports, reports, or diagnostics.

## A05: Injection

- Use parameterized queries, LINQ providers, ORM APIs, or safe command builders for data access.
- Never concatenate untrusted input into SQL, shell commands, LDAP queries, XPath, templates, URLs, headers, or JavaScript.
- Validate input by allow-listing expected formats, ranges, lengths, and enum values.
- Encode output according to context: HTML, attribute, JavaScript, URL, JSON, CSV, or Markdown.
- Avoid rendering raw HTML from user input. If unavoidable, sanitize with a proven library and document the allowed tags/attributes.
- Treat uploaded filenames, MIME types, metadata, CSV cells, and imported text as untrusted.
- Add tests for malicious input when parsing, searching, filtering, importing, or rendering user-controlled values.

## A06: Insecure Design

- Define abuse cases, trust boundaries, and failure modes before implementing sensitive workflows.
- Keep business rules on the server side when they affect permissions, payments, membership state, identity, or data integrity.
- Use least privilege for users, services, storage, and external API credentials.
- Add rate limiting, throttling, or lockout controls to workflows that can be abused repeatedly.
- Make destructive or sensitive actions explicit, auditable, and reversible where practical.
- Design privacy into features by limiting collection, exposure, retention, and sharing of personal data.
- Prefer simple flows with clear authorization and validation points over clever stateful client-side workflows.

## A07: Authentication Failures

- Use the existing authentication and identity stack; do not create parallel login/session mechanisms.
- Require strong server-side validation for login, password reset, registration, invitation, and account recovery flows.
- Protect authentication tokens and session cookies from client-side script access.
- Invalidate or rotate credentials after password reset, role changes, account disablement, or suspicious activity when supported.
- Avoid account enumeration through error messages, timing differences, or recovery responses.
- Require re-authentication or additional confirmation for high-risk account changes.
- Test expired, revoked, malformed, replayed, and missing authentication state when auth behavior changes.

## A08: Software Or Data Integrity Failures

- Verify authorization and validation before accepting imported data, webhooks, callbacks, or client-submitted state.
- Do not deserialize untrusted data into executable, polymorphic, or privileged types.
- Validate file type, size, structure, and content before storing or processing uploads.
- Protect update, migration, and background-job paths from tampering and partial failure.
- Use integrity checks, signatures, or trusted channels for external callbacks and critical data transfers when available.
- Keep audit trails for security-sensitive data changes.
- Treat cached, offline, or client-synchronized data as stale and untrusted until revalidated.

## A09: Security Logging And Alerting Failures

- Log authentication failures, authorization failures, suspicious validation failures, sensitive data changes, and administrative actions.
- Include enough context to investigate events without exposing secrets or excessive personal data.
- Use structured logs for security-relevant events where possible.
- Never swallow security exceptions silently.
- Avoid returning internal exception details to users; log diagnostic details server side.
- Ensure logs distinguish expected validation failures from unexpected system failures.
- Review monitoring or alerting needs when adding privileged workflows, imports, exports, or integrations.

## A10: Mishandling Of Exceptional Conditions

- Fail closed for authorization, authentication, validation, configuration, and integrity errors.
- Use centralized exception handling where practical and avoid inconsistent ad hoc error responses.
- Do not expose stack traces, connection strings, filesystem paths, SQL text, tokens, or internal identifiers in user-facing errors.
- Keep cleanup and rollback behavior explicit for multi-step operations.
- Handle timeouts, retries, cancellation, concurrency conflicts, and partial writes deliberately.
- Avoid exception-driven control flow for normal validation paths.
- Add regression tests for failure paths when a feature depends on external services, persistence, uploads, or background work.

## Repository Security Checklist

- Access control: protected operations enforce server-side authorization and object-level checks.
- Input handling: untrusted input is validated, constrained, and encoded for its output context.
- Secrets: no secrets, tokens, credentials, or sensitive personal data are committed or logged.
- Configuration: development-only behavior cannot run in production by accident.
- Dependencies: new or upgraded packages are justified, reviewed, restored, built, and tested.
- Errors: user-facing failures are safe, while server logs keep enough diagnostic context.
- Data integrity: imports, uploads, callbacks, and client-submitted state are verified before use.
- Tests: meaningful security-sensitive paths include negative tests, not only happy paths.
