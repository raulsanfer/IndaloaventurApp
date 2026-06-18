## ADDED Requirements

### Requirement: Social provider token login
The system MUST accept social provider authentication input, validate it against the configured provider, and resolve or create the corresponding local user account.

#### Scenario: Existing user logs in with social provider
- **WHEN** a valid social authentication token is received for an external identity already linked to a local account
- **THEN** the system SHALL authenticate the local user and continue token issuance

#### Scenario: First-time social login creates linkage
- **WHEN** a valid social authentication token is received for an external identity with no local linkage
- **THEN** the system SHALL create or link a local user according to provisioning rules

### Requirement: Unified JWT after social authentication
The system MUST issue the same local JWT contract for social logins as for credential-based logins.

#### Scenario: Social login returns local JWT
- **WHEN** social authentication succeeds
- **THEN** the system SHALL return a signed JWT with local subject and authorization claims

### Requirement: Social authentication failure handling
The system MUST reject invalid or expired external authentication artifacts and MUST NOT issue a local JWT.

#### Scenario: Invalid social token
- **WHEN** the provided social token cannot be validated by the configured provider
- **THEN** the system SHALL return an unauthorized response without creating a session
