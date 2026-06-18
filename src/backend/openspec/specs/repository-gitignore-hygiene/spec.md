# repository-gitignore-hygiene Specification

## Purpose
TBD - created by archiving change add-root-gitignore-and-sensitive-file-rules. Update Purpose after archive.
## Requirements
### Requirement: Root-level gitignore policy
The repository MUST define a root-level `.gitignore` that centralizes ignore rules for build outputs, local tooling artifacts, and temporary files.

#### Scenario: Build artifacts excluded
- **WHEN** a developer builds or tests the solution locally
- **THEN** generated artifacts such as `bin/`, `obj/`, and test result folders SHALL remain untracked by git

### Requirement: Sensitive local file protection
The repository MUST ignore common local-only sensitive files and secret-bearing artifacts that are not intended for version control.

#### Scenario: Local secrets are not tracked
- **WHEN** a developer creates local secret/configuration files matching configured sensitive patterns
- **THEN** those files SHALL be ignored and MUST NOT appear as staged changes by default

### Requirement: Safe source/config retention
The `.gitignore` policy MUST avoid excluding required source files and versioned baseline configuration needed by the project.

#### Scenario: Source files remain trackable
- **WHEN** a developer adds or modifies valid source code or required versioned config files
- **THEN** git SHALL continue detecting those changes as trackable files

