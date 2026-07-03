# GitHub Copilot Instructions for F1-Telemetry

These instructions apply to the F1-Telemetry solution. Keep suggestions aligned with the current repository state instead of generic defaults.

---

## Git workflow

- Never run `git commit`, `git push`, create branches, or perform other Git write operations without explicit user approval
- Read-only Git commands such as `git status`, `git diff`, and `git log` are allowed
- After making changes, summarize the modifications before proposing a commit

### Commit messages

- Subject line: maximum 80 characters
- Do not end the subject line with a period
- Do not write the message in the first person
- Keep the body concise, usually 3-5 sentences

---

## Solution overview

F1-Telemetry is a multi-project .NET and Angular solution for receiving, processing, storing, and visualizing telemetry data from EA F1 games.

- Backend: .NET 10
- Frontend: Angular 21
- Databases: Microsoft SQL Server, MySQL/MariaDB, PostgreSQL
- Observability: OpenTelemetry, metrics, tracing, structured logging
- Delivery: Docker images and Azure Pipelines

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

- Keep controllers thin and move business logic into services or processors
- Keep database access inside repositories
- Reuse `RepositoryFactory` and `RepositoryBase` patterns instead of bypassing them
- Follow existing packet processing flow: analyzers/factories select specialized processors
- Preserve multi-database support when changing persistence code
- Preserve observability hooks when changing important workflows

---

## .NET and C# conventions

### Project configuration

- Target framework is `net10.0`
- Nullable reference types are enabled
- Implicit usings are enabled in the solution
- XML documentation files are generated
- Central package management is used via `Directory.Packages.props`
- Shared files such as `GlobalSuppressions.cs`, `SharedAssemblyInfo.cs`, and `StyleCop.json` are linked across projects
- The solution file format is `.slnx`

### Naming

- Types, methods, properties, and enums use PascalCase
- Private fields use `_camelCase`
- Interfaces use `I` prefixes
- Repository classes end with `Repository`
- Controllers end with `Controller`
- DTO and projection classes typically end with `Data` or `ViewData`
- Test classes end with `Tests`

### Formatting and structure

- Use file-scoped namespaces
- Keep `using` directives outside the namespace
- Group `using` directives by system, project, then third-party namespaces
- Use 4 spaces for indentation and Allman braces
- Prefer `var` where the type is obvious
- Do not use primary constructors
- Do not use `this.`
- Prefer explicit `using (...) { }` blocks
- Regions are used extensively and should match the surrounding file style, for example:
  - `#region Fields`
  - `#region Constructors`
  - `#region Properties`
  - `#region Methods`
  - `#region Controller methods`
  - `#endregion // Fields`

### Documentation

- Add XML documentation to all public members
- Match the existing codebase style by documenting most non-public members as well, except private fields
- Do not add `<remarks>`
- Every method with a return type must have a `<returns>` tag
- Use `/// <inheritdoc/>` for interface implementations where appropriate
- Keep documentation in English

### Coding style

- Prefer constructor injection with private readonly fields
- Use `ILogger<T>` instead of console output
- Prefer structured logging
- Use `condition == false` instead of `!condition` when touching code that follows this project style
- Prefer `is null` and `is not null` for reference checks
- Use `string.IsNullOrEmpty(...)` or `string.IsNullOrWhiteSpace(...)` for string checks
- Keep async I/O operations async end-to-end
- Avoid broad catches and silent fallbacks unless an existing surrounding pattern requires them

---

## Data access and EF Core

### Repository pattern

- Create repositories through `RepositoryFactory`
- Dispose repository factories with `using`
- Base classes are `RepositoryBase` and `RepositoryBase<TQueryable, TEntity>`
- Typical access pattern:

```csharp
using (var dbFactory = RepositoryFactory.CreateInstance())
{
    var sessionRepository = dbFactory.GetRepository<SessionRepository>();
}
```

### Entity Framework Core

- `F1ServerDbContext` lives in `F1Server.Db`
- Migrations are split by provider:
  - `F1Server.Db.MsSqlMigrations`
  - `F1Server.Db.MySqlMigrations`
  - `F1Server.Db.PostgreSqlMigrations`
- Keep provider-specific migration histories in sync when schema changes require it

### Naming Convention

- The **first** migration is always named `InitialCreate`
- All subsequent migrations are named `Update` followed by an incrementing number: `Update1`, `Update2`, `Update3`, etc.
- **File names** are clean without timestamps: `InitialCreate.cs`, `Update1.cs`, etc.
- The `[Migration]` attribute **must keep the auto-generated timestamp** prefix (e.g. `[Migration("20260406161452_InitialCreate")]`) — EF Core tooling requires the `yyyyMMddHHmmss_Name` format internally
- Each migration consists of:
  - `{Name}.cs`
  - `{Name}.Designer.cs`
  - `F1ServerDbContextModelSnapshot.cs`

### Migration command

Run migrations against the matching provider project and use `F1Server` as the startup project, for example:

```bash
dotnet ef migrations add Update14 --project F1Server.Db.MsSqlMigrations --startup-project F1Server
```

When creating a new migration:

1. Determine the next number by checking existing `Update*.cs` files in the `Migrations` folder
2. Run: `dotnet ef migrations add UpdateN --project src/SeriesOverwatch.Data --startup-project src/SeriesOverwatch`
3. Rename the generated files to remove the timestamp prefix (e.g. `20260406161452_Update1.cs` → `Update1.cs`)
4. **Do NOT** change the `[Migration]` attribute or class name — keep them as generated
5. Verify both `.cs` and `.Designer.cs` files exist and are consistent

---

## Web API and hosting

- API controllers live in `F1Server.WebApi\Controllers`
- Use `[ApiController]` and `[Route("api/[controller]")]`
- Keep controller actions async where I/O is involved
- SignalR hubs live in `F1Server.WebApi\Hubs`
- Web hosting configuration lives in `F1Server.WebApi\WebHosting.cs`
- Keep Swagger, CORS, authentication, SignalR, and cache wiring consistent with existing setup
- The application uses `HybridCache` and `IMemoryCache`; reuse existing cache patterns instead of inventing new ones

### Authentication

- Cookie authentication is configured in `WebHosting`
- External providers currently include Google, Facebook, and Microsoft
- Credentials are read from Docker secrets or environment variables

---

## Observability

- Observability code lives in `F1Server.Observability`
- Reuse existing metrics and tracing abstractions such as `AppMetrics`
- Prefer extending existing counters, gauges, and histograms over introducing duplicate instruments
- Keep OpenTelemetry configuration aligned with existing endpoint and target settings

---

## Frontend conventions

- Frontend code lives in `F1ServerApp`
- Angular uses TypeScript 5.x and Angular 21 packages
- The app already mixes modern Angular bootstrap APIs with existing module-based structure; follow the surrounding pattern in the files you touch
- Keep file names in kebab-case
- Keep component and service class names in PascalCase
- Avoid `any`; prefer explicit types and shared interfaces from the app's data models
- Reuse the existing SignalR service for live updates
- Keep API contracts aligned with backend `Data` and `ViewData` models

---

## Testing

- Test project: `F1Server.Tests`
- Test framework: MSTest
- Assertions: MSTest `Assert` APIs only
- Do not introduce FluentAssertions
- Coverage tooling: `coverlet.collector`
- Test initialization uses `[AssemblyInitialize]` in `TestInitializer`

### Naming

- Test class: `{Feature}Tests`
- Test method: `{Class}_{Scenario}_{ExpectedResult}`

Example:

```csharp
[TestMethod]
public void PacketHeader_CheckGameVersion_Returns2025()
```

### Assertions

- Always provide assert messages
- For async exception checks, use `Assert.ThrowsExceptionAsync<T>(...)`

---

## Common change patterns

### Adding or changing backend behavior

1. Update core/data/entity types where needed
2. Update repositories for persistence changes
3. Update services or processors for business logic
4. Update Web API controllers or hubs only for transport concerns
5. Extend observability if the change affects important runtime behavior
6. Update MSTest coverage

### Adding or changing frontend behavior

1. Update or add typed models in the Angular app
2. Reuse existing services for HTTP or SignalR communication
3. Keep UI state and transformation logic close to the feature component
4. Keep backend and frontend contract names synchronized

---

## What to avoid

- Do not apply guidance from unrelated projects such as SeriesOverwatch
- Do not switch the test stack away from MSTest
- Do not document the project as `.NET 9`, `xUnit`, or Angular standalone-only
- Do not collapse multi-database support into a single provider assumption
- Do not bypass repositories with ad-hoc database access in higher layers
