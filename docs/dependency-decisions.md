# Dependency decisions

Most NuGet packages used by F1-Telemetry are stable releases from Microsoft or
from the upstream project itself. A few references deliberately deviate from
that rule: three OpenTelemetry instrumentation packages only exist as
pre-releases, the MySQL provider is a community fork, and the code analyzer is
still on a beta line.

This document records those decisions, the reason each one is accepted, the risk
it carries, and the condition under which it should be revisited. All versions
are pinned centrally in `Directory.Packages.props`.

## Overview

| Package | Version | Why it deviates | Decision |
| --- | --- | --- | --- |
| `OpenTelemetry.Instrumentation.EntityFrameworkCore` | `1.17.0-beta.1` | Upstream never published a stable release | Accepted |
| `OpenTelemetry.Instrumentation.EventCounters` | `1.17.0-alpha.1` | Upstream never published a stable release | Accepted |
| `OpenTelemetry.Instrumentation.Process` | `1.17.0-rc.1` | Upstream never published a stable release | Accepted |
| `Microting.EntityFrameworkCore.MySql` | `10.0.10` | Pomelo has no EF Core 10 build | Accepted, revisit when Pomelo ships EF Core 10 |
| `Reihitsu.Analyzer` | `1.0.0-beta6` | Latest published version of the analyzer | Accepted, build-time only |

## Pre-release OpenTelemetry instrumentation

`F1Server.Observability` registers three instrumentation packages that the
OpenTelemetry .NET contrib repository has never released as stable. `alpha`,
`beta` and `rc` are the only channels these components have ever shipped on, so
"upgrade to the stable version" is not an available option — the alternative
would be to drop the signals or to hand-write the equivalent meters.

| Package | Used for | Registered in |
| --- | --- | --- |
| `OpenTelemetry.Instrumentation.EntityFrameworkCore` | Database command spans on the trace provider | `ObservabilityConfiguration.ConfigureTracing` |
| `OpenTelemetry.Instrumentation.Process` | Process CPU and memory metrics | `ObservabilityConfiguration.ConfigureMetrics` |
| `OpenTelemetry.Instrumentation.EventCounters` | `Microsoft.Data.SqlClient` and `Microsoft.EntityFrameworkCore` event counters as metrics | `ObservabilityConfiguration.ConfigureMetrics` |

The versions are kept in step with the stable OpenTelemetry SDK and exporters,
which are on `1.17.0`. Mixing an older instrumentation line with a newer SDK is
the more likely source of breakage than the pre-release status itself, so these
three packages are bumped together with the SDK rather than left behind.

Risk assessment:

- The packages are referenced by `F1Server.Observability` only. Packet
  processing, persistence and the Web API do not depend on them.
- Observability is opt-in at runtime: without `F1SERVER_OTLP_TARGET` and a
  configured endpoint, none of the instrumentation is even initialized.
- Versions are pinned exactly through central package management, so a new
  pre-release is never picked up implicitly.
- A breaking API change surfaces at build time, not in production.

Revisit when: any of the three components publishes a stable release, or a
signal it provides becomes available in the stable OpenTelemetry SDK.

## MySQL provider: Microting instead of Pomelo

`F1Server.Db` and `F1Server.Db.MySqlMigrations` use
`Microting.EntityFrameworkCore.MySql`, a maintained fork of
`Pomelo.EntityFrameworkCore.MySql`, rather than Pomelo itself.

The reason is framework alignment: the solution targets `net10.0` with EF Core
`10.0.10`, and Pomelo's newest published release is `9.0.0` for EF Core 9. The
fork publishes `10.0.x` releases that track EF Core 10, and — unlike the
OpenTelemetry packages above — these are stable versions, not pre-releases.

Risk assessment:

- The provider is a fork of an established project and keeps the Pomelo API, so
  the migration project and the `UseMySql` call are unchanged.
- The dependency is confined to `F1Server.Db` and the MySQL migrations project.
  MSSQL and PostgreSQL support is unaffected.
- Switching back is a package reference change; the provider-specific migration
  history stays valid because the generated SQL comes from the same code base.

Revisit when: Pomelo publishes an EF Core 10 release. At that point the fork is
no longer needed and the reference should move back to the upstream package.

## Reihitsu.Analyzer beta

`Reihitsu.Analyzer` enforces the formatting and naming conventions described in
`CLAUDE.md`; a clean build must show zero `RH####` warnings. `1.0.0-beta6` is
the newest version published, so there is no stable release to move to.

The reference is declared once in `Directory.Build.props` with
`PrivateAssets=all`, which makes it a build-time-only dependency: it is not part
of any published assembly, NuGet package or Docker image, and it cannot affect
runtime behaviour. A regression in the analyzer costs build warnings, nothing
more.

Revisit when: `Reihitsu.Analyzer` reaches `1.0.0` stable.

## Not applicable: StyleCop.Analyzers

StyleCop is not referenced by this solution. Code analysis is covered by
`Reihitsu.Analyzer`, the built-in .NET analyzers configured through
`.editorconfig`, and SonarQube (suppressions with justifications live in
`GlobalSuppressions.cs`). No `SA####` rule is active or suppressed anywhere in
the repository.

## Keeping this document current

When a version in `Directory.Packages.props` changes, check whether the entry
here still holds — in particular whether a pre-release dependency has finally
reached a stable release, in which case the reference should move to it and the
entry should be removed from this document.
