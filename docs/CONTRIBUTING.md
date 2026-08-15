# Contributing

## Getting started

### Machine setup

To begin you'll need Git and the .NET SDK. If you plan to touch the Angular frontend,
you'll also need Node.js.

The `F1-Telemetry` repository uses Git as its source control system. If you haven't
already installed it, you can download it [here](https://git-scm.com/downloads) or,
if you prefer a GUI-based approach, try [GitHub Desktop](https://desktop.github.com/).

Once Git is installed, you'll also need the .NET SDK matching the version targeted by
the solution (currently `net10.0`). Instructions and downloads for your preferred OS
can be found [here](https://dotnet.microsoft.com/download).

The service connects to a relational database at runtime (PostgreSQL, MariaDB/MySQL or
Microsoft SQL Server) and applies EF Core migrations automatically on startup. A local
instance of any one of these (native or via Docker) is enough for backend development;
see [`README.md`](../README.md) for a `docker-compose` example.

For frontend work, install a current Node.js LTS release matching the `F1ServerApp`
Angular/TypeScript toolchain (Angular 22, TypeScript 6.x).

Format checks rely on `reihitsu-format`, a .NET tool. Install it once with:

```shell
dotnet tool install -g Reihitsu.Cli
```

> [!IMPORTANT]
> The above steps are a one-time setup for your machine and do not need to be repeated
> after the initial configuration.

### Cloning the repository

Now that your machine is set up, you can clone the `F1-Telemetry` repository. Open a
terminal and run this command:

```shell
git clone https://github.com/LarsLaskowski/F1-Telemetry.git
```

Cloning via SSH:

```shell
git clone git@github.com:LarsLaskowski/F1-Telemetry.git
```

### Building the backend

The solution also contains a Windows-only WPF client (`F1ReplayClient`) and the Angular
`.esproj` (`F1ServerApp`), so a solution-wide restore fails on non-Windows machines.
Restore and build the backend entry points explicitly instead; the `F1Server` project
graph covers all backend libraries and is the same set the Dockerfile and CI build:

```shell
dotnet restore F1Server/F1Server.csproj
dotnet restore F1Server.Tests/F1Server.Tests.csproj
reihitsu-format ./
dotnet build F1Server/F1Server.csproj -c Release --no-restore
dotnet build F1Server.Tests/F1Server.Tests.csproj -c Release --no-restore
```

On Windows, `dotnet build F1Server.slnx` builds the full solution, including
`F1ReplayClient` and `F1ServerApp`.

### Building the frontend

```shell
cd F1ServerApp
npm install
npm start
```

### Running tests

```shell
dotnet test F1Server.Tests/F1Server.Tests.csproj -c Release --no-build --logger trx --collect:"XPlat Code Coverage"
```

For detailed rules on how unit tests should be structured and named, see
[`UNIT_TESTS.md`](UNIT_TESTS.md).

### Submitting a pull request

If you'd like to contribute by fixing a bug, implementing a feature, or even correcting
typos in the documentation, you'll need to submit a pull request.

Before submitting a pull request, be sure to [rebase](https://www.atlassian.com/git/tutorials/merging-vs-rebasing)
your branch onto the current `main`. Do not use `git merge` or the *merge* button
provided by GitHub.

For PR naming use the following convention: `[area] Description` (no period at the end).

- For the area, use the affected project or feature (for example `Data`, `WebApi`,
  `Telemetry`, `Observability`, `Frontend`).
- For the description, do not reference an issue number in there. A clear, short
  summary of what the change entails is enough; there is room to elaborate in the
  description.

When a PR is related to an issue, use the `Closes #issuenumber` syntax so the issue
links to the PR automatically and closes when the PR is merged.

Follow the PR template in [`.github/pull_request_template.md`](../.github/pull_request_template.md).

## Code style

Detailed C# code-style rules (naming, regions, formatting, XML docs, null handling) are
documented in [`.github/instructions/csharp.instructions.md`](../.github/instructions/csharp.instructions.md)
and are binding for all contributions. Run `reihitsu-format ./` before opening a pull
request; a clean build must show zero `RH####` warnings.

Frontend conventions (kebab-case file names, PascalCase component/service classes,
avoiding `any`) are documented in [`CLAUDE.md`](../CLAUDE.md).

## Stability policy

An essential consideration in every pull request is its impact on the system. Avoid
introducing unnecessary breaking changes, performance or functional regressions, or
negative impacts on usability. Preserve multi-database support and observability hooks
when changing persistence code or important workflows.

## Reporting security issues

Do not report security vulnerabilities through public GitHub issues. See
[`SECURITY.md`](../SECURITY.md) for the private reporting process.

## License

By contributing to this project, you agree that your contributions will be licensed
under the same [MIT License](../LICENSE.md) that covers the project.
