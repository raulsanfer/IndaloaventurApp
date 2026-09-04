## 1. DocFX Foundation

- [x] 1.1 Create the `docs/` folder with `docfx.json`, root `toc.yml` and `index.md`, and verify the files exist with the expected navigation entry points.
- [x] 1.2 Configure DocFX metadata for `IndaloaventurApp.SharedUI`, `IndaloaventurApp.Web.Client` and `IndaloaventurApp.Web`, and verify `docfx docs/docfx.json` can discover the configured projects or reports actionable metadata errors.
- [x] 1.3 Document local build and preview commands in the docs landing page or a development page, and verify the commands are visible from the generated site navigation.

## 2. Architecture Documentation

- [x] 2.1 Add architecture pages for overview and project structure, and verify they explain the responsibilities of `IndaloaventurApp.Web`, `IndaloaventurApp.Web.Client`, `IndaloaventurApp.SharedUI` and `IndaloaventurApp.Frontend.Tests`.
- [x] 2.2 Add infrastructure pages for dependency injection, authentication/session, HTTP API clients, localization, PWA/navigation and styling, and verify each page references concrete folders or representative files from the repo.
- [x] 2.3 Add a SharedUI/component-boundaries page, and verify it documents Razor/code-behind partial conventions, service boundaries, model usage and SCSS styling rules.
- [x] 2.4 Add a testing strategy page, and verify it describes bUnit/xUnit usage, shared test doubles and feature-based test organization.

## 3. Feature Documentation

- [x] 3.1 Add feature pages for auth, club, licenses, members/profile, signals, food alerts, settings/admin and PWA/navigation, and verify each page is linked from `docs/features/toc.yml` or the root TOC.
- [x] 3.2 For each feature page, document purpose, main components, services/clients, relevant endpoints or API contracts, loading/error states and related tests, and verify at least one concrete code reference appears per feature.
- [x] 3.3 Cross-link feature pages to architecture pages where useful, and verify DocFX reports no broken internal links.

## 4. API Reference

- [x] 4.1 Enable XML documentation output for selected frontend projects where needed, and verify `dotnet build IndaloaventurApp.sln` still succeeds.
- [x] 4.2 Add or improve XML comments on the most relevant public component models, service abstractions and API client contracts, and verify generated API pages render meaningful summaries.
- [x] 4.3 Exclude frontend test implementation types from generated API reference unless explicitly needed as examples, and verify the API reference navigation stays focused on runtime/frontend projects.

## 5. Validation

- [x] 5.1 Run `docfx docs/docfx.json` and verify the static documentation site is generated under `docs/_site/`.
- [x] 5.2 Run `docfx docs/docfx.json --serve` or document why serving could not be executed in the current environment, and verify the local preview URL loads the documentation site.
- [x] 5.3 Run `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj` and verify the documentation changes did not introduce new frontend test failures; record any pre-existing failures separately.
