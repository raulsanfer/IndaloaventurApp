## Context

See `proposal.md` for motivation. The repository is a .NET 9 Blazor solution with separate projects for shared UI components, the hosted web application/client, and frontend tests. Current OpenSpec files describe product behavior, but there is no dedicated developer documentation site that explains the architecture and operational conventions of the frontend.

The documentation should be useful even before the API reference is complete. Conceptual Markdown pages are the primary source of truth for architecture and feature flows; generated C# reference pages are supporting material.

## Goals / Non-Goals

**Goals:**

- Add a `docs/` documentation project based on DocFX.
- Provide a navigable technical portal with architecture, infrastructure, feature and API reference sections.
- Document the frontend structure in terms of responsibilities: Blazor app shell, client pages, shared UI components, services, API clients, models, resources, styles and tests.
- Configure local commands to build and preview the documentation.
- Enable XML documentation generation for selected frontend projects so DocFX can render API reference pages.
- Include a reusable implementation prompt in this change so `/opsx:apply` can generate the documentation consistently.

**Non-Goals:**

- Do not redesign application UI or runtime navigation.
- Do not expose the documentation site as a user-facing route inside the Blazor app unless a later change explicitly requests it.
- Do not document every private method or every file.
- Do not replace OpenSpec. OpenSpec remains the change/spec record; DocFX becomes developer-facing technical documentation.
- Do not require generated API reference to be perfect before conceptual documentation is useful.

## Decisions

### Use DocFX as a standalone static documentation project

Create a `docs/` folder with a `docfx.json` configuration and Markdown content. The generated output should go to `docs/_site/`.

Rationale: DocFX supports Markdown plus .NET XML comments and produces static HTML. Keeping it standalone avoids coupling documentation runtime to the Blazor app.

Alternative considered: embed documentation pages directly in Blazor. That would add runtime routes and application surface area for content that is primarily developer-facing.

### Treat Markdown as the architecture source of truth

Create conceptual pages first:

```text
docs/
  index.md
  toc.yml
  docfx.json
  architecture/
    overview.md
    project-structure.md
    dependency-injection.md
    authentication-session.md
    http-api-clients.md
    shared-ui.md
    styling.md
    testing-strategy.md
  features/
    auth.md
    club.md
    licenses.md
    members-profile.md
    signals.md
    food-alerts.md
    settings-admin.md
    pwa-navigation.md
```

Rationale: The most valuable documentation for this app is the mental model: what belongs where, how state flows, and how features are wired. API reference alone does not explain those boundaries.

Alternative considered: rely mainly on generated reference docs. That is easy to produce but weak for explaining architecture and feature ownership.

### Generate API reference from selected projects only

Configure DocFX metadata for these projects:

- `IndaloaventurApp.SharedUI/IndaloaventurApp.SharedUI.csproj`
- `IndaloaventurApp.Web/IndaloaventurApp.Web.Client/IndaloaventurApp.Web.Client.csproj`
- `IndaloaventurApp.Web/IndaloaventurApp.Web/IndaloaventurApp.Web.csproj`

Exclude or avoid prioritizing `IndaloaventurApp.Frontend.Tests` from API reference. Tests should be documented conceptually in `architecture/testing-strategy.md`, not exposed as product API.

Rationale: The shared UI and web/client projects describe the frontend surface. Tests are useful as examples and verification, but not as API reference.

Alternative considered: include the full solution. That can make docs noisy and expose implementation/test-only types.

### Enable XML comments without forcing full XML coverage immediately

Add XML documentation output to relevant `.csproj` files. If missing-public-member warnings become too noisy, suppress `1591` initially and track incremental improvement in tasks.

Rationale: The site should become usable quickly while allowing progressive improvement of XML comments on public types.

Alternative considered: require all public members to have XML comments before landing the docs. That increases scope and could turn a documentation infrastructure change into a broad code-commenting effort.

### Keep documentation validation command explicit

Document these local commands:

```powershell
dotnet tool update -g docfx
dotnet restore
docfx docs/docfx.json
docfx docs/docfx.json --serve
```

If global tools are not preferred later, this can move to a local tool manifest in a follow-up.

### Implementation prompt for `/opsx:apply`

Use this prompt when applying the change:

```text
Implement the OpenSpec change `agregar-documentacion-tecnica-docfx`.

Build a DocFX-based technical documentation site for the Blazor frontend. Treat Markdown conceptual docs as the primary documentation and generated C# API reference as supporting material.

Create `docs/` with `docfx.json`, `toc.yml`, `index.md`, architecture pages and feature pages. The documentation must explain:
- overall Blazor architecture and project responsibilities;
- repository structure;
- dependency injection and service registration;
- authentication/session persistence;
- HTTP API client pattern and error handling;
- SharedUI component boundaries, Razor/code-behind convention and localization;
- styling strategy with SCSS;
- test strategy and feature-based test organization;
- feature flows for auth, club, licenses, members/profile, signals, food alerts, settings/admin, PWA/navigation.

Configure DocFX API metadata for `IndaloaventurApp.SharedUI`, `IndaloaventurApp.Web.Client` and `IndaloaventurApp.Web`. Enable XML documentation generation in those projects if required. Do not include frontend test types in generated API reference unless needed for a testing page example.

Use concrete references to existing folders, representative components, services, clients and tests. Avoid documenting every file. Prefer stable architecture explanations and maintenance guidance.

Verify with:
- `dotnet restore`
- `docfx docs/docfx.json`

If DocFX is not installed, report the exact install command and continue with all repository files prepared.
```

## Risks / Trade-offs

- [Risk] Generated API reference can be noisy if too many public implementation details are included -> Mitigation: start with selected frontend projects and add DocFX filter rules later if needed.
- [Risk] XML documentation warnings can create a large immediate cleanup task -> Mitigation: enable XML output while suppressing missing-comment warnings initially, then improve public comments incrementally.
- [Risk] Documentation can drift from code -> Mitigation: include maintenance rules and a build command that can be run in CI later.
- [Risk] DocFX metadata generation may fail if workloads or SDK resolution are inconsistent -> Mitigation: document `dotnet restore`, use explicit project paths, and keep conceptual Markdown valuable even if API reference needs tuning.

## Migration Plan

1. Add the `docs/` DocFX project and conceptual Markdown pages.
2. Enable XML documentation output for selected frontend projects.
3. Configure DocFX metadata and table of contents.
4. Build the documentation locally.
5. Fix broken links, metadata errors or missing project references.
6. Optionally add CI validation in a later change.

Rollback is simple: remove `docs/` and revert XML documentation project settings if the generated site is not adopted.

## Open Questions

- Should the generated `_site` be published anywhere initially, or only built locally?
- Should DocFX be installed as a global tool, local tool manifest, or CI-only dependency in a later hardening change?
