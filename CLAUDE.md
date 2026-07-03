# CLAUDE.md – F1-Telemetry

Guidance for Claude Code working in this repository. Keep suggestions aligned with the
current repository state instead of generic defaults. This file consolidates the project
conventions from `.github/copilot-instructions.md` and `.github/instructions/csharp.instructions.md`.

---

## Git workflow

- Never run `git commit`, `git push`, create branches, or perform other Git write operations
  without explicit user approval.
- Read-only Git commands such as `git status`, `git diff`, and `git log` are allowed.
- After making changes, summarize the modifications before proposing a commit.

### Commit messages

- Subject line: maximum 80 characters, no trailing period.
- Do not write the message in the first person.
- Keep the body concise, usually 3-5 sentences.

---

## Solution overview

F1-Telemetry is a multi-project .NET and Angular solution for receiving, processing, storing,
and visualizing telemetry data from EA F1 games.

- Backend: .NET 10 (`net10.0`)
- Frontend: Angular 21, TypeScript 5.x
- Databases: Microsoft SQL Server, MySQL/MariaDB, PostgreSQL
- Observability: OpenTelemetry, metrics, tracing, structured logging
- Delivery: Docker images and Azure Pipelines
- Solution file format: `.slnx`

### Main projects

| Project | Purpose |
|---|---|
| `F1Server` | Main host and startup application |
| `F1Server.Core` | Core logic, packet contracts, enums, exceptions |
| `F1Server.Data` | DTOs, view data, shared data models |
| `F1Server.Db` | `F1ServerDbContext`, repositories, entity tables |
| `F1Server.Db.*Migrations` | Provider-specific EF Core migrations |
| `F1Server.Service` | Business logic and packet processing |
| `F1Server.Telemetry` | Telemetry logging and telemetry-specific runtime work |
| `F1Server.WebApi` | Controllers, hubs, caching, authentication, hosting |
| `F1Server.Observability` | Metrics, tracing, logging configuration |
| `F1Server.Shared` | Cross-cutting shared helpers |
| `F1Server.Tests` | MSTest-based automated tests |
| `F1ServerApp` | Angular frontend |
| `F1ReplayClient` | Replay client |
| `F1PacketTester`, `F1SessionFolderRename` | Supporting tools/utilities |

### Architectural expectations

- Keep controllers thin; move business logic into services or processors.
- Keep database access inside repositories.
- Reuse `RepositoryFactory` and `RepositoryBase` patterns instead of bypassing them.
- Follow existing packet processing flow: analyzers/factories select specialized processors.
- Preserve multi-database support when changing persistence code.
- Preserve observability hooks when changing important workflows.

---

## .NET and C# conventions

### Project configuration

- Target framework `net10.0`; nullable reference types and implicit usings enabled.
- XML documentation files are generated.
- Central package management via `Directory.Packages.props`.
- Shared files (`GlobalSuppressions.cs`, `SharedAssemblyInfo.cs`, `StyleCop.json`) are linked across projects.

### Naming

| Element | Convention | Example |
|---|---|---|
| Classes / Methods / Properties | PascalCase | `PacketAnalyzer`, `GetSessionData` |
| Interfaces | `I` + PascalCase | `ITelemetryClient` |
| Private fields | `_` + camelCase | `_logger`, `_dbContext` |
| Constants | PascalCase | `F1MeterName` |
| Parameters / locals | camelCase | `packetHeader`, `sessionData` |
| Enums (type + values) | PascalCase | `PacketTypes.CarTelemetry` |

Type suffixes: `*Controller`, `*Repository`, `*ViewData` / `*Data`, `*Exception`, `*Tests`, `*Factory`.

### File structure

- Main type name must match the file name exactly; exactly one top-level type per file.
- Classes, interfaces, enums, structs, delegates each in their own file. No helper types beside the primary type.
- File-scoped namespaces (`namespace F1Server.Core;`).
- `using` directives **outside** the namespace, grouped with blank lines:
  1. System namespaces
  2. Own project namespaces
  3. Third-party namespaces (Microsoft, etc.)

### Formatting

- 4 spaces indentation (no tabs), CRLF line endings, **no trailing newline** at end of file.
- Allman braces; braces always required, even for single-line `if`/`for`/`while`.
- `var` preferred wherever the type is obvious.
- Do **not** use primary constructors; do **not** use `this.`.
- Prefer explicit `using (...) { }` blocks (not simplified `using`).
- Language keywords over BCL types (`int`, not `Int32`).
- Collection initializers with `[]`; string interpolation; `ReadOnlySpan<byte>` for byte processing.
- **LINQ method syntax only** — never query syntax (`from … in … select`).
- Each `switch` `case` block enclosed in braces.

### Regions (used extensively)

`#endregion` is annotated with `// Name`. Typical order:
`Constants` → `Fields` → `Constructors` → `Properties` → `Static methods` → `Methods` →
`Controller methods` → `IDisposable` → interface implementations.
Within the same visibility level, static methods come before instance methods.

```csharp
#region Fields

private readonly ILogger<MyClass> _logger;

#endregion // Fields
```

### Parameter & call formatting

- One or two parameters stay on a single line.
- Wrap only when **more than two** parameters; first parameter on the same line, the rest
  aligned under the first parameter.
- Method chaining: first call on the same line, subsequent calls aligned **under the dot**.

### Null handling & boolean checks

- Use `condition == false` instead of `!condition`.
- Prefer `is null` / `is not null` for reference checks.
- Use `string.IsNullOrEmpty(...)` / `string.IsNullOrWhiteSpace(...)` for string checks.
- Null propagation (`_logger?.LogError(...)`) and coalescing (`x ??= y;`).

### Async / await

- Keep async I/O async end-to-end.
- In service, infrastructure, and data-access code append `.ConfigureAwait(false)` to awaited
  tasks unless resuming a specific context is required (avoid CA2007 regressions).

### Coding style

- Constructor injection with private readonly fields.
- Use `ILogger<T>` (not console output); prefer structured logging.
- Avoid broad catches and silent fallbacks unless a surrounding pattern requires them.
- Constructors and methods are **never** expression-bodied; properties/accessors/lambdas/operators
  may be expression-bodied when single-line.

### XML documentation

- Required on all public, internal, and private members **including private fields** and enum values.
- **No `<remarks>`** — all info goes in `<summary>`.
- Every method parameter documented with `<param>`; every method with a return type (including
  `Task`, `Task<T>`, `ValueTask`) has a `<returns>` tag.
- Use `/// <inheritdoc/>` for interface implementations. Documentation language: **English**.

---

## Data access and EF Core

- Create repositories through `RepositoryFactory`; dispose factories with `using`.
- Base classes: `RepositoryBase` and `RepositoryBase<TQueryable, TEntity>`.

```csharp
using (var dbFactory = RepositoryFactory.CreateInstance())
{
    var sessionRepository = dbFactory.GetRepository<SessionRepository>();
}
```

- `F1ServerDbContext` lives in `F1Server.Db`. Migrations are split by provider:
  `F1Server.Db.MsSqlMigrations`, `F1Server.Db.MySqlMigrations`, `F1Server.Db.PostgreSqlMigrations`.
- Keep provider-specific migration histories in sync when schema changes require it.

### Migration naming

- The **first** migration is always `InitialCreate`; subsequent ones are `Update1`, `Update2`, …
- **File names** are clean without timestamps (`Update1.cs`), but the `[Migration]` attribute
  **must keep** the auto-generated `yyyyMMddHHmmss_Name` timestamp prefix.
- Each migration consists of `{Name}.cs`, `{Name}.Designer.cs`, and the updated `F1ServerDbContextModelSnapshot.cs`.

```bash
dotnet ef migrations add Update14 --project F1Server.Db.MsSqlMigrations --startup-project F1Server
```

When adding a migration: determine the next number from existing `Update*.cs` files, generate it,
rename generated files to drop the timestamp prefix, but **do not** change the `[Migration]`
attribute or class name. Verify `.cs` and `.Designer.cs` are consistent.

---

## Web API and hosting

- Controllers in `F1Server.WebApi\Controllers`; use `[ApiController]` and `[Route("api/[controller]")]`.
- Keep controller actions async where I/O is involved.
- SignalR hubs in `F1Server.WebApi\Hubs`; hosting config in `F1Server.WebApi\WebHosting.cs`.
- Keep Swagger, CORS, authentication, SignalR, and cache wiring consistent with existing setup.
- The app uses `HybridCache` and `IMemoryCache`; reuse existing cache patterns.
- Cookie authentication configured in `WebHosting`; external providers: Google, Facebook, Microsoft.
  Credentials are read from Docker secrets or environment variables.

---

## Observability

- Code lives in `F1Server.Observability`; reuse abstractions such as `AppMetrics`.
- Prefer extending existing counters, gauges, and histograms over duplicate instruments.
- Keep OpenTelemetry configuration aligned with existing endpoint and target settings.

---

## Frontend conventions

- Frontend in `F1ServerApp`; Angular 21 / TypeScript 5.x.
- The app mixes modern Angular bootstrap APIs with module-based structure; follow the surrounding
  pattern in the files you touch.
- File names in kebab-case; component and service class names in PascalCase.
- Avoid `any`; prefer explicit types and shared interfaces.
- Reuse the existing SignalR service for live updates.
- Keep API contracts aligned with backend `Data` and `ViewData` models.

---

## Testing

- Project `F1Server.Tests`, framework **MSTest**, MSTest `Assert` APIs only. Do **not** introduce FluentAssertions.
- Coverage via `coverlet.collector`; test initialization via `[AssemblyInitialize]` in `TestInitializer`.
- Test class `{Feature}Tests`; test method `{Class}_{Scenario}_{ExpectedResult}`.
- Always provide assert messages. For async exception checks use `Assert.ThrowsExceptionAsync<T>(...)`.

```csharp
[TestMethod]
public void PacketHeader_CheckGameVersion_Returns2025()
```

---

## Common change patterns

**Backend:** update core/data/entity types → repositories → services/processors → Web API
controllers/hubs (transport only) → observability → MSTest coverage.

**Frontend:** update/add typed models → reuse existing HTTP/SignalR services → keep UI state and
transformation logic close to the feature component → keep contract names synchronized with backend.

---

## What to avoid

- Do not apply guidance from unrelated projects such as SeriesOverwatch.
- Do not switch the test stack away from MSTest.
- Do not document the project as `.NET 9`, `xUnit`, or Angular standalone-only.
- Do not collapse multi-database support into a single provider assumption.
- Do not bypass repositories with ad-hoc database access in higher layers.

---

## Suppressed analyzer rules (intentional)

SA1101, SA1116, SA1200, SA1309, SA1310, SA1413, SA1513, SA1629, SA1633, SA1642,
IDE0290, CS8618, CA1822, CA2254 are intentionally disabled — see
`.github/instructions/csharp.instructions.md` for the rationale of each.
