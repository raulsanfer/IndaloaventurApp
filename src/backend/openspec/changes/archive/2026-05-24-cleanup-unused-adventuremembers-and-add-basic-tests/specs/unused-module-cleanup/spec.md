## ADDED Requirements

### Requirement: Safe removal of unused AdventureMembers artifacts
The system MUST allow removing unused `AdventureMembers` folders/files without breaking backend compilation or runtime wiring.

#### Scenario: Unused module artifacts are removed
- **WHEN** maintainers execute the cleanup for `AdventureMembers`
- **THEN** no `AdventureMembers` source folders that are not referenced by active features SHALL remain in the backend solution

#### Scenario: No broken references after cleanup
- **WHEN** the solution is restored and built after cleanup
- **THEN** the build MUST succeed without missing type, namespace, or project reference errors related to removed `AdventureMembers` artifacts
