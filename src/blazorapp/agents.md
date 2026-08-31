# OpenSpec Agent Instructions (OpenCode)

You are an AI coding assistant operating in a repository that uses:
- **OpenSpec** for spec-driven development (`openspec/specs/` + `openspec/changes/`)

## Golden rules
- Treat `openspec/specs/` as the source of truth (current behavior).
- Treat `openspec/changes/<change-name>/` as the proposed/active change.
- Only mark tasks complete when verification (tests) passes.
- Keep changes small, deterministic, and test-backed.
- Keep technical documentation aligned with code changes when a change affects architecture, features, public contracts, services, API clients, routing, session/auth, styling conventions, or tests.

## Workflow

### 1) Plan (PRD -> OpenSpec)
When asked to plan or create specs:
1. Read `openspec/project.md` and relevant files in `openspec/specs/`.
2. Create a new change folder under `openspec/changes/<change-name>/` with:
   - `proposal.md` (why/what/scope/non-goals/risks)
   - `tasks.md` (checklist with test plan notes)
   - `specs/**/spec.md` (deltas: ADDED/MODIFIED/REMOVED)
3. Ensure requirements use MUST/SHALL and each requirement has at least one scenario.

### 2) Implement (Tasks -> Code)
When asked to implement:
1. Identify the active change folder under `openspec/changes/`.
2. Implement tasks in order from `tasks.md`.
3. Run tests frequently and fix failures.
4. If the change affects documented architecture or features, update `docs/architecture` or `docs/features` in the same implementation pass.
5. If the change affects public C# contracts, update XML comments so DocFX can regenerate the API reference.
6. Verify documentation with `docfx docs/docfx.json` when docs or public XML comments change.
7. Update the checkbox status in `tasks.md` only when verified.

### Documentation maintenance
DocFX updates generated API reference from public C# types and XML comments, but conceptual documentation must be reviewed by the agent when behavior or structure changes.

For every new OpenSpec change, include a task like:

```md
- [ ] Review whether the change affects `docs/architecture` or `docs/features`, update documentation if needed, and verify with `docfx docs/docfx.json`.
```

Commit documentation sources such as `docs/*.md`, `docs/docfx.json`, `docs/toc.yml`, and XML comments. Do not commit generated DocFX output such as `docs/_site/` or generated `docs/api/*.yml`.

### 3) Validate (Acceptance criteria)
When asked to validate:
1. Map scenarios/acceptance criteria to tests/commands.
2. Run the project test command (commonly `dotnet test` for this Blazor project).
3. Report which requirements are proven and what gaps remain.

### 4) Archive
When asked to archive:
- Prefer `openspec archive <change-name> --yes` if OpenSpec CLI is available.
- Otherwise, move the change into `openspec/archive/` and ensure `openspec/specs/` reflects the final state.

