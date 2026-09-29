# Temple development guidelines

These instructions apply to the entire Temple solution. Follow explicit task
instructions and any more specific AGENTS.md files in the area being changed.

## Reuse and NuGet packages

- Prefer existing dependencies for general-purpose functionality. Check the
  relevant Craft packages first, then other packages already used by the solution.
- Before writing a general-purpose utility or introducing another dependency,
  consider established, maintained NuGet packages that cover the need. Evaluate
  architectural fit, API suitability, framework compatibility, maintenance,
  licensing, and dependency cost; popularity alone is not sufficient.
- Prefer a small custom implementation when no suitable package exists or when
  a dependency would add disproportionate complexity. Briefly explain material
  package-versus-custom decisions in the delivery notes.
- Keep framework-specific packages in the appropriate outer layer. Do not weaken
  architectural boundaries merely to reuse a package.
- Do not replace existing libraries or upgrade unrelated packages as part of a
  narrowly scoped change.

## MVVM and presentation

- Prefer pure MVVM for WPF and Avalonia features: use bindings, commands, styles,
  templates, and triggers to connect the view to presentation state and actions.
- Use reusable attached behaviors for view mechanics when declarative XAML is
  insufficient. Check existing packages before introducing a new behavior.
- Code-behind is acceptable when it is the clearest, simplest solution for a
  particular view-only need, such as focus, rendering, or control lifecycle.
  Do not introduce elaborate abstractions solely to eliminate code-behind.
  Briefly explain the reason for choosing it.
- Never place business rules, persistence access, or application use-case logic
  in code-behind or attached behaviors. ViewModels coordinate presentation and
  invoke application operations; they do not own business rules.
- Keep UI controls and view instances out of ViewModels. Keep UI framework types
  out of Domain and Application. Presentation-specific types may live in the
  presentation layer where appropriate.
- Preserve keyboard operation, focus behavior, and mouse interaction when
  changing UI features. Modal overlays must prevent interaction with underlying
  controls and gameplay input, not merely cover them visually.

## Clean Architecture

- Adhere strictly to Clean Architecture in new and changed designs. Dependencies
  point inward; outer implementations depend on inner contracts.
- Temple.Domain owns domain entities, invariants, and business rules. It must
  remain independent of UI frameworks, EF Core, transport, and infrastructure.
- Temple.Application owns use cases and their orchestration. It depends on domain
  types and abstractions, not concrete persistence or presentation implementations.
- Temple.Persistence supplies persistence abstractions, including repositories
  and units of work. EF Core contexts, mappings, migrations, and database-specific
  behavior belong in the concrete persistence projects.
- Infrastructure implements external capabilities behind appropriate contracts.
  UI and API projects adapt user requests to application operations. Wire concrete
  implementations through dependency injection at application entry points.
- Existing violations are architectural debt, not precedents. Do not add or
  deepen such dependencies. Report relevant conflicts and make
  focused corrections needed by the task; do not perform an unrelated solution-wide
  migration without a request.

## CQRS

- Adhere strictly to command/query separation for application use cases.
  Commands change application or domain state; queries return data without
  changing business state or triggering business events.
- Follow the existing MediatR request/handler conventions where applicable,
  including FluentValidation and Result<T>. Keep domain rules in the domain
  rather than duplicating them in handlers or UI validation.
- Route business mutations through application command handling or the existing
  application orchestration for game actions. Do not bypass application boundaries
  from views, controllers, or ViewModels. View-only state need not become a MediatR
  request.
- Use DTOs/read models for query results where appropriate; do not expose EF Core
  tracking or database implementation details to callers.
- CQRS does not require separate databases, event sourcing, or a new messaging
  framework. Introduce these only for a concrete requirement.

## Local conventions and verification

- Match nearby naming, formatting, and established library conventions. Keep
  changes focused; avoid unrelated cleanup, reformatting, or framework migrations.
- Preserve the existing data-driven approach to quests, dialogues, and site data.
  Prefer extending the relevant models and assets over hardcoding content in views.
- Preserve historical/versioned persistence semantics when changing data access.
  Do not discard migrations or change database providers incidentally.
- Build the affected projects and run relevant existing tests. Add focused tests
  for changed business rules, commands, queries, and game-state transitions using
  the existing xUnit projects; avoid tests that only mirror implementation details.
- For UI changes, verify relevant interaction paths when feasible. Clearly
  distinguish build/test results from manual UI verification and disclose any
  checks that could not be completed.
