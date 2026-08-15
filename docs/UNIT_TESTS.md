# How to create Unit Tests for F1-Telemetry

## Overview

This document describes how unit tests are written in this repository. It is binding
for both human contributors and AI agents: new tests must follow the conventions
below, and existing tests are the reference implementation — when in doubt, look at a
neighboring test file in `F1Server.Tests` before inventing a new pattern.

## Unit tests vs. other test types

1. **Unit tests**

   A unit test exercises an individual component or method in isolation — a packet
   converter, an analyzer, a service, a cache. Most of `F1Server.Tests` falls into this
   category: packet byte arrays in, parsed objects or `null` out, with no database
   involved.

2. **Integration-style tests**

   Some tests exercise the repository/EF Core layer together with the packet
   processors (for example `PacketProcessor{Year}Tests`, `ChampionshipServiceTests`,
   `RepositoryCacheTests`). These run against the database configured by
   `TestInitializer` (`F1SERVER_DATABASE_TYPE=99`, an in-memory EF Core provider) rather
   than mocking the data layer, so the tests also verify the wiring between processors,
   repositories, and the `F1ServerDbContext`.

3. **Load tests**

   F1-Telemetry does not currently have load tests.

## Why unit test?

- **Fast feedback.** Unit tests run in milliseconds and don't require a running EA F1
  game or a live UDP feed.
- **Protection against regression.** Every EA F1 game release (2019–2026 so far)
  changes packet layouts; the full suite guards existing game-version support while new
  versions are added.
- **Executable documentation.** A well-named test tells the reader what a converter
  does for a given input without needing to read its implementation.
- **Less coupled code.** Code that is hard to unit test is usually a sign of tight
  coupling; writing the test first tends to produce better-decoupled designs.

## Test stack

| Concern | Tooling |
| --- | --- |
| Test framework | MSTest (`[TestClass]`, `[TestMethod]`, `[DataRow]`) |
| Assertions | MSTest `Assert` / `CollectionAssert` APIs only — no FluentAssertions |
| Mocking | None. The project has no mocking framework; exercise real objects (`PacketAnalyzer`, processors, repositories) instead of introducing NSubstitute or Moq |
| EF Core | `Microsoft.EntityFrameworkCore.InMemory`, wired up through `RepositoryFactory` with `F1SERVER_DATABASE_TYPE=99` set in `TestInitializer` |
| Code coverage | `coverlet.collector`, collected via `dotnet test --collect:"XPlat Code Coverage"` |

Do not introduce xUnit, NUnit, FluentAssertions, or a mocking library — the project
standardizes on MSTest and MSTest's own `Assert`/`CollectionAssert` APIs, exercising
real collaborators wherever possible.

## Where tests live

- All tests live in the single `F1Server.Tests` project, referenced from
  `F1Server.slnx`.
- Tests are grouped by concern into folders: per-game-version packet tests under
  `F1-2019` … `F1-2026`, cache tests under `Cache`, championship tests under
  `Championships`, observability tests under `Observability`, shared test data and
  helpers under `Data`.
- Sample raw packets used as test fixtures live in `SampleData\*.packet` and are copied
  to the test output directory (`CopyToOutputDirectory`); read them with
  `File.ReadAllBytes(Path.Combine("SampleData", fileName))`.
- One test file per type under test, named `{TypeUnderTest}Tests.cs` (for example
  `PacketLengthValidationTests.cs`, `ChampionshipServiceTests.cs`). When a single
  packet type has version-specific behavior, split by game version instead of growing
  one file indefinitely (for example `PacketCarStatus2025Tests.cs`).
- `TestInitializer.cs` runs once per test assembly via `[AssemblyInitialize]`; it
  configures the in-memory database and calls `TestData.PrepareDatabase()` to seed
  baseline data. Do not duplicate that setup in individual test classes.

## Naming your tests

Test method names are a single PascalCase identifier with no underscores, built from
three parts, concatenated directly — this is a binding project rule enforced by the
`RH4103` analyzer, which rejects underscores in member names:

- The name of the **type or method** being tested.
- The **scenario** under which it's being tested.
- The **expected behavior** when the scenario is invoked.

**Examples from this codebase:**

```csharp
public void PacketHeaderCheckGameVersionReturns2025()
public void GetCarStatusTruncatedPacketReturnsNull(string fileName)
public void GetLapPositionsDataManipulatedLapCountReturnsObject()
public void GetLapPositionsDataFirstCarPositionIsNotShiftedByLapStartIndex(int gameVersion, int headerSize, int lapPositionSize)
```

Test class names follow `{TypeUnderTest}Tests` or `Packet{PacketType}{Year}Tests` for
per-game-version packet coverage.

## Arranging your tests

Follow Arrange, Act, Assert without labeling the sections with comments — a blank line
before the act and before the assert block is enough to separate them, consistent with
the blank-line rules in
[`.github/instructions/csharp.instructions.md`](../.github/instructions/csharp.instructions.md).

```csharp
[TestMethod]
[DataRow("F1-2025-CarStatus.packet")]
[DataRow("F1-2026-CarStatus.packet")]
public void GetCarStatusFullPacketReturnsObject(string fileName)
{
    var packetHeader = GetPacketHeader(fileName, out var packetContent);

    var packetAnalyzer = new PacketAnalyzer();

    var carStatus = packetAnalyzer.GetCarStatus(packetHeader, packetContent);

    Assert.IsNotNull(carStatus, $"Full size car status packet {fileName} must produce an object!");
}
```

## Always include an assertion message

Every `Assert.*` call must include a message explaining what the assertion
guarantees — not what it checks mechanically, but why it matters:

```csharp
Assert.IsNull(carStatus, $"Truncated car status packet {fileName} must not produce an object!");
```

Prefer the specific MSTest `Assert`/`CollectionAssert` member over `Assert.IsTrue` /
`Assert.IsFalse` wrapping a boolean expression, e.g. `Assert.Contains(expected,
collection, message)` instead of `Assert.IsTrue(collection.Contains(expected),
message)` — SonarQube flags the latter.

For async exception checks, use `Assert.ThrowsExactlyAsync<T>(...)` (MSTest 4.x;
`ThrowsExceptionAsync` no longer exists — only `Throws`, `ThrowsAsync`,
`ThrowsExactly`, `ThrowsExactlyAsync` are available).

## Write minimally passing tests

Use the simplest input that exercises the behavior under test — a synthetic packet
header built with the smallest byte array that triggers the scenario, or a truncated
`Span` of a real sample packet, rather than a full round-trip through the UDP
listener. Minimal tests stay resilient to unrelated changes elsewhere and keep the
focus on behavior rather than implementation details.

## Avoid logic in tests

Do not add `if`, `for`, `while`, or `switch` statements inside a test body. When
multiple inputs must be checked against the same behavior, use MSTest's `[DataRow]`
on a single parameterized `[TestMethod]` instead of writing conditional logic:

```csharp
[TestMethod]
[DataRow(2025, ConstData.F12025HeaderSize)]
[DataRow(2026, ConstData.F12026HeaderSize)]
public void GetLapPositionsDataTruncatedPacketReturnsNull(int gameVersion, int headerSize)
{
    var packetHeader = CreatePacketHeader(gameVersion, headerSize);

    var packetAnalyzer = new PacketAnalyzer();

    var lapPositions = packetAnalyzer.GetLapPositionsData(packetHeader, new byte[TruncatedPacketLength]);

    Assert.IsNull(lapPositions, $"Truncated F1 {gameVersion} lap positions packet must not produce an object!");
}
```

## Prefer helper methods over constructor setup

MSTest constructs a fresh test class instance per test, so shared state is already
isolated. Even so, factor repeated setup into a private (or private static) helper
method rather than a constructor, following the `#region Static methods` /
`#region Methods` ordering from
[`.github/instructions/csharp.instructions.md`](../.github/instructions/csharp.instructions.md):

```csharp
#region Static methods

/// <summary>
/// Reads a sample packet file and parses its packet header
/// </summary>
/// <param name="fileName">Name of the sample packet file</param>
/// <param name="packetContent">Raw content of the sample packet file</param>
/// <returns>Parsed packet header</returns>
private static PacketHeader GetPacketHeader(string fileName, out byte[] packetContent)
{
    packetContent = File.ReadAllBytes(Path.Combine("SampleData", fileName));

    var receivedData = new ReceivedPacketData();

    receivedData.SetRawData(packetContent);

    Assert.IsNotNull(receivedData.PacketHeader, $"Header of {fileName} could not be parsed!");

    return receivedData.PacketHeader;
}

#endregion // Static methods
```

**Why?** All setup relevant to a test stays visible from the call site, and there is
no risk of over-setting-up state that later tests then depend on.

## Avoid multiple acts

Include a single logical action per test. When a scenario needs multiple related
outcomes checked, that's still one act followed by multiple assertions — not multiple
acts. Add a separate `[TestMethod]`, or a `[DataRow]`-parameterized test, for each
distinct scenario instead of branching within one test.

## Testing packet converters

Most packet tests follow the same truncated-vs-full-size pair:

- A **truncated** case, using a `Span` shorter than the expected packet size (see
  `TruncatedPacketLength` in `PacketLengthValidationTests`), asserting the converter
  returns `null` instead of reading past the packet end.
- A **full size** case, using either a real sample packet from `SampleData\*.packet`
  or a synthetic byte array sized from the relevant `ConstData` fields, asserting the
  converter returns a populated object.

For game versions without a checked-in sample `.packet` file, build a synthetic
header with a private `CreatePacketHeader(gameVersion, headerSize)` helper instead of
adding new binary fixtures unless the scenario specifically needs recorded game data.

## Testing against the database

Tests that need a `F1ServerDbContext` go through `RepositoryFactory`, exactly like
production code — do not construct `F1ServerDbContext` directly in a test:

```csharp
using (var dbFactory = RepositoryFactory.CreateInstance())
{
    var sessionRepository = dbFactory.GetRepository<SessionRepository>();

    // Act + Assert against the repository
}
```

`TestInitializer` already points `RepositoryFactory` at the in-memory provider for the
whole test run; individual tests only need to seed the specific rows they depend on
(see `TestData` in the `Data` folder for existing seed helpers before adding new ones).

## XML documentation on tests

Per [`.github/instructions/csharp.instructions.md`](../.github/instructions/csharp.instructions.md),
XML documentation is required on all members, including test classes and test
methods. Document what the test verifies, not what MSTest attribute it carries:

```csharp
/// <summary>
/// Test to verify that a truncated car status packet is rejected instead of reading past the packet end
/// </summary>
/// <param name="fileName">Name of the sample packet file</param>
[TestMethod]
[DataRow("F1-2025-CarStatus.packet")]
public void GetCarStatusTruncatedPacketReturnsNull(string fileName)
{
    // ...
}
```

## Usings in F1Server.Tests

`F1Server.Tests` has `<ImplicitUsings>enable</ImplicitUsings>` plus the MSTest SDK's
own global usings, so `System`, `System.Collections.Generic`, `System.IO`,
`System.Linq`, `System.Net.Http`, `System.Threading`, `System.Threading.Tasks`, and
`Microsoft.VisualStudio.TestTools.UnitTesting` are already available everywhere in
this project. Do not add explicit `using` directives for those namespaces in new test
files; only add `using` for namespaces that are not implicitly global (e.g.
`F1Server.Db.Entity`, `F1Server.Db.Entity.Repositories`).

## Code coverage

Code coverage is collected with `coverlet.collector`, already referenced by
`F1Server.Tests` — no separate installation is needed to collect coverage during
`dotnet test`.

Run the full suite with coverage collection:

```shell
dotnet test F1Server.Tests/F1Server.Tests.csproj -c Release --no-build --logger trx --collect:"XPlat Code Coverage"
```

This produces a `coverage.cobertura.xml`/`coverage.opencover.xml` file under
`F1Server.Tests/TestResults`. CI feeds the OpenCover format into SonarQube Cloud
analysis (see `.github/workflows/ci.yml`).

## Checklist for new tests

- [ ] Test class named `{TypeUnderTest}Tests` (or `Packet{PacketType}{Year}Tests` for
      per-game-version packet coverage), inside `F1Server.Tests`.
- [ ] Test method named `{TypeUnderTest}{Scenario}{ExpectedResult}` (PascalCase, no
      underscores, `Async` suffix for async tests).
- [ ] `[TestClass]` / `[TestMethod]` (MSTest), `[DataRow]` instead of in-test branching
      for multiple inputs.
- [ ] Arrange / Act / Assert, separated by blank lines, one act per test.
- [ ] Every `Assert.*` call includes an explanatory message.
- [ ] No mocking library introduced — real objects and the in-memory EF Core provider
      via `RepositoryFactory` instead.
- [ ] Shared setup factored into a private/static helper method, not a constructor.
- [ ] `#region` layout and XML docs follow
      [`.github/instructions/csharp.instructions.md`](../.github/instructions/csharp.instructions.md).
- [ ] `reihitsu-format ./` run before committing.
