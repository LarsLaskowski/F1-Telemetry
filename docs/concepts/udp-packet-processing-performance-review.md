# Performance review: UDP packet ingestion and processing pipeline

Detailed performance review of the path an incoming UDP packet takes from the socket to the
database and the live/runtime data structures. The review is written so that one or more agents
can split it into individual GitHub issues and work on them independently — every finding has a
stable ID, exact code locations, an impact assessment, and a concrete recommendation.

**Scope:** `F1Server.Service` (TelemetryClient, PacketProcessor, processors, runtime data,
caches), `F1Server.Core` (header parsing, PacketAnalyzer, PacketToObject), `F1Server.Db`
(RepositoryFactory, RepositoryBase, DbContext, entities), `F1Server.Telemetry` (TelemetryWriter,
only where it is called from the packet path).

**Out of scope (per request):** `F1ServerApp` (Angular frontend), `F1Server.WebApi`
(controllers, hubs, caching for the web side).

**Review method:** static code review of the current `main` state. No code was changed. Line
numbers refer to the state at the time of this review and may drift.

---

## 1. Pipeline overview (as implemented today)

```
UDP datagram (game, up to ~60 Hz per packet type, 22-24 cars)
  │
  ▼
TelemetryClient.ReceiveCallback            (BeginReceive/EndReceive, APM pattern)
  │  - allocates byte[] per datagram (EndReceive)
  │  - ReceivedPacketData.SetRawData copies the array a second time
  │  - parses header inline (Span/Unsafe — good)
  │  - per-packet: Activity, INFO log, timer restart, 2× ConcurrentQueue.Count
  ▼
ConcurrentQueue<ReceivedPacketData> _packetQueue
  │
  ▼
ProcessPacketQueue (single background task, polls with WaitHandle.WaitOne(100))
  │
  ▼
PacketProcessor.ProcessPacket              (global lock, serial)
  │  - Task.Run per packet for the PacketReceived event
  ▼
PacketAnalyzer.GetPacketData               (new transformation object + full managed
  │                                         object graph per packet)
  ▼
ProcessorFactory.GetProcessor → specialized processor (Session, LapData, CarTelemetry,
  │                                         SessionHistory, CarStatus, …)
  ▼
Runtime data (SessionRuntimeData / ParticipantRuntimeData)
  ├── LiveSessionData / LiveDriverData      (live view of current session)
  ├── TelemetryWriter → InfluxDB            (own queue + background writer — good pattern)
  └── RepositoryFactory.CreateInstance()    (NEW DbContext per call!)
        └── EF Core → MariaDB / MsSql / PostgreSQL (SaveChanges per operation)
```

Packet frequencies (per EA spec, race with 22 cars): CarTelemetry, Motion, LapData up to 60 Hz;
CarStatus/CarDamage ~10 Hz; Session 2 Hz; SessionHistory ~20 packets/s (one car per packet,
cycled); after FinalClassification a bulk of SessionHistory packets arrives. A one-hour session
easily produces several hundred thousand datagrams.

### What already works well (do not "fix" these)

- Receive path and processing are decoupled via queues; the socket callback never touches the DB.
- Header and payload parsing is Span/`Unsafe.ReadUnaligned`-based, no BinaryReader/serializer.
- Static lookup caches (Driver/Team/Track/Nationality/Session/Participants/LapRepositoryCache)
  avoid many repeated SELECTs.
- `LapEntity` has sensible composite indexes (`ParticipantId+LapNumber`, `SessionId+LapNumber`,
  `SessionId+DbIsInvalidLapTime`); `CarTelemetries` is indexed by `LapNumberId`.
- InfluxDB live telemetry uses a batching background writer (`TelemetryWriter`) — the right
  pattern, and the model to copy for the relational writes (see PERF-04).
- Car telemetry rows are buffered in memory per lap and only written when the lap completes,
  instead of row-by-row at 60 Hz.

---

## 2. Findings

Severity scale:
- **Critical** — measurably throttles or stalls the pipeline in every session; fix first.
- **High** — significant avoidable cost in the hot path (per packet or per lap).
- **Medium** — real cost, but bounded or only in certain phases.
- **Low** — cleanup/hardening; small wins or latent risks.

### 2.1 Database layer

#### PERF-01 (Critical) — New `DbContext` per `RepositoryFactory.CreateInstance()` call, no pooling

- **Location:** `F1Server.Db/Entity/RepositoryFactory.cs:33-37, 66-69`;
  `F1Server.Db/Entity/F1ServerDbContext.cs:124-175` (`OnConfiguring`)
- **Problem:** Every `RepositoryFactory.CreateInstance()` constructs a brand-new
  `F1ServerDbContext`. `OnConfiguring` then runs per instance: `DetectServerType()` reads an
  environment variable, four more env vars are read, a provider connection-string builder is
  allocated, and `UseMySql`/`UseSqlServer`/`UseNpgsql` options are rebuilt (the very first
  context even opens an extra ADO connection just to read the server version, cached afterwards
  in a static). Factories are created in the hot path: per new/completed lap
  (`ParticipantRuntimeData.AddLap/CompleteLap/RemoveLap`), per Session packet
  (`SessionProcessor.Process`, 2 Hz), per finished-lap check in every SessionHistory packet
  (`SessionHistoryProcessor.UpdateFinishedLap`), in `SessionRuntimeData` property setters, etc.
- **Impact:** Context construction + options building + service-provider resolution thousands of
  times per session; EF's internal service provider is only cached per options hash, so this
  mostly works but still burns CPU and allocations on every call, and prevents EF from reusing
  compiled queries efficiently across short-lived contexts with per-instance options instances.
- **Recommendation:** Build `DbContextOptions<F1ServerDbContext>` **once** at startup (env vars
  cannot change while the process runs) and back `RepositoryFactory` with a
  `PooledDbContextFactory<F1ServerDbContext>`. Keep the public `RepositoryFactory` API
  (`CreateInstance()` / `GetRepository<T>()` / `Dispose`) unchanged so no caller has to change —
  `CreateInstance()` leases a pooled context, `Dispose()` returns it.
- **Suggested issue:** *Introduce DbContext pooling behind RepositoryFactory*

#### PERF-02 (Critical) — Synchronous bulk telemetry insert blocks the packet-processing thread at every lap completion

- **Location:** `F1Server.Service/Processors/CarTelemetryProcessor.cs:141-145`
  (`CompleteTelemetryData(...).GetAwaiter().GetResult()`),
  `F1Server.Service/Processors/SessionHistoryProcessor.cs:173, 186` (same pattern),
  `F1Server.Service/Runtime/ParticipantRuntimeData.cs:381-413` (`CompleteTelemetryData`)
- **Problem:** When a lap completes, the buffered `CarTelemetryEntity` rows of that lap
  (a 90-second lap at ~20 recorded telemetry frames/s is easily 1,500-2,000 rows *per car*) are
  written in a single `SaveChangesAsync`, but the call is executed sync-over-async
  (`GetAwaiter().GetResult()`) **on the single packet-processing thread** (packet processing is
  serialized by the lock in `PacketProcessor.ProcessPacket`). While the insert runs, no packet of
  any type is processed: the queue grows and the live view freezes. In a race this happens for
  every car on every lap, and around FinalClassification for many laps at once.
- **Impact:** Periodic multi-hundred-millisecond (DB-dependent: seconds) stalls of the entire
  pipeline; queue backlog; "live" data visibly lags exactly at the most interesting moment
  (lap completion).
- **Recommendation:** Decouple relational persistence from packet processing with a dedicated
  background DB-writer (bounded queue of write jobs, same pattern as `TelemetryWriter`):
  `CompleteTelemetryData`/`CompleteLap` enqueue the finished lap + its telemetry batch and return
  immediately. The writer processes jobs with a pooled context and real `await`. Ordering
  requirements are simple (per participant: lap insert before its telemetry batch) and can be
  guaranteed by a single writer task.
- **Suggested issue:** *Move lap/telemetry persistence to an async background DB writer*

#### PERF-03 (High) — Telemetry batch is written via `UpdateRange` + change tracker instead of an insert path

- **Location:** `F1Server.Service/Runtime/ParticipantRuntimeData.cs:397-406`;
  `F1Server.Db/Entity/Repositories/Base/RepositoryBase{TQueryable,TEntity}.cs:213-235`
  (`UpdateRangeAsync`)
- **Problem:** New telemetry rows (Id == 0) are persisted via `DbContext.UpdateRange`. EF marks
  key-less entities as `Added`, so it works, but semantically it is an insert, and each entity
  passes through full change tracking. In addition, before the write the code copies the queue
  (`TelemetryQueue.ToList()`) and mutates `LapNumberId` per row, and the preceding lap lookup
  (`GetQuery().FirstOrDefault(...)`) drags a 4-table auto-include join with it (see PERF-05).
- **Impact:** For thousands of rows per lap: unnecessary tracking overhead, slow multi-row
  INSERT batching, avoidable joins.
- **Recommendation:** Add an insert-only path: `AddRange` with
  `ChangeTracker.AutoDetectChangesEnabled = false` for the batch (or `ExecuteSqlRaw` multi-row
  insert). Resolve the lap Id from the in-memory `LapEntity` (it is already known after `AddLap`,
  see PERF-06) instead of re-querying it. Consider provider-neutral bulk insert only if
  measurements still show a bottleneck.
- **Suggested issue:** *Fast insert path for CarTelemetry batches*

#### PERF-04 (High) — `SessionHistoryProcessor` opens a context and can UPDATE per lap on every history packet

- **Location:** `F1Server.Service/Processors/SessionHistoryProcessor.cs:108-141` (loop over all
  laps of the car), `231-268` (`UpdateFinishedLap`: `RepositoryFactory.CreateInstance()` per lap)
- **Problem:** A SessionHistory packet contains the full lap history of one car. For every lap
  not found in `_unfinishedLaps`, `UpdateFinishedLap` runs — and it creates a
  RepositoryFactory/DbContext **before** checking whether anything changed at all. With ~20
  history packets/s and up to 100 laps per car, this creates up to ~2,000 contexts/s in the worst
  case (late race). The `LapRepositoryCache` avoids the SELECT, but not the context, and the
  change detection compares against the *cached* entity: `isInvalidLapTime` is recomputed from the
  cached entity's own times on every packet and compared to the stored flag — if
  `ValidateLapTimes` disagrees with what was stored (e.g., reference times were 0 when the lap
  was created), the same UPDATE fires again on **every** history packet for that car.
- **Impact:** Sustained per-packet DB work in the second half of a session; potential repeated
  identical UPDATEs at ~1/s per car for the rest of the session.
- **Recommendation:** (a) Hoist the change check above the factory creation — create the context
  only when a write is actually needed (one factory per *packet*, not per lap, when writes
  happen). (b) Update the cached entity's `IsInvalidLapTime` after writing so the comparison
  converges (the cache update `LapRepositoryCache.AddOrUpdate(lapDbData)` currently re-adds the
  same instance whose times were never copied from `lapData` — copy the new values onto the
  cached entity first). (c) Skip the whole per-lap loop early when
  `NumberOfLaps`/lap contents did not change since the last packet for this car (cheap
  per-car checksum/last-seen lap counter in `ParticipantRuntimeData`).
- **Suggested issue:** *SessionHistoryProcessor: create DbContext lazily and make lap-diff
  detection converge*

#### PERF-05 (High) — AutoInclude turns every lap lookup into a 4-table join

- **Location:** `F1Server.Db/Entity/F1ServerDbContext.cs:195-199` (`LapEntity.Participant`
  AutoInclude; `ParticipantEntity` auto-includes `Nationality`, `Team`, `Driver`);
  hot queries e.g. `ParticipantRuntimeData.cs:230, 391-393`, `RepositoryBase.AddOrRefresh:147`
- **Problem:** `Navigation(...).AutoInclude()` applies to *every* query for that entity,
  including the tracked `FirstOrDefault` inside `AddOrRefresh`/`Refresh` and the lap-id lookups
  in the packet path. Every lap upsert therefore joins Laps ⋈ Participants ⋈ Drivers ⋈ Teams ⋈
  Nationalities although only the lap row is needed. The same applies to
  `CarTelemetryEntity.Lap` AutoInclude for any telemetry query.
- **Impact:** Larger result sets, more hydration work and slower queries on the most frequent
  statements in the pipeline.
- **Recommendation:** Remove the AutoIncludes for `LapEntity.Participant` and
  `CarTelemetryEntity.Lap` and add explicit `Include(...)` at the few read sites that need the
  navigation (WebApi-side queries), or at minimum use `IgnoreAutoIncludes()` inside
  `RepositoryBase` write helpers (`AddOrRefresh`, `Refresh`, `Remove`) and the packet-path
  lookups.
- **Suggested issue:** *Remove/bypass EF AutoIncludes in hot lap and telemetry queries*

#### PERF-06 (High) — `AddLap` performs 3 round-trips (SELECT + INSERT + SELECT) per new lap

- **Location:** `F1Server.Service/Runtime/ParticipantRuntimeData.cs:199-241`
- **Problem:** For a new lap, `AddOrRefresh` first SELECTs (miss), then INSERTs; afterwards a
  *second* SELECT fetches the generated Id (`GetQuery().FirstOrDefault(...)?.Id`) — including the
  auto-include join from PERF-05. EF already populates the key on the tracked entity after
  `SaveChanges`; the `after` callback of `AddOrRefresh` even exposes the entity.
- **Impact:** 3 DB round-trips instead of 1 for each of the ~22 cars, every lap; runs on the
  packet-processing thread.
- **Recommendation:** Use `AddOrRefresh(..., after: e => lapData.Id = e.Id)` (or plain `Add` on a
  fresh entity when the lap is known to be new) and drop the extra SELECT. With 20+ cars starting
  a lap in the same seconds, batching all new laps of one LapData packet into one
  `SaveChanges` would reduce this further (one context per packet, see also PERF-02).
- **Suggested issue:** *Eliminate redundant round-trips in ParticipantRuntimeData.AddLap*

#### PERF-07 (Medium) — Row-by-row deletes for telemetry/lap cleanup

- **Location:** `F1Server.Service/Processors/SessionProcessor.cs:541-574`
  (`ClearPreviousSessionData`: raw `DELETE ... WHERE LapNumberId = @p0` per lap in a loop),
  `F1Server.Service/Processors/FinalClassificationProcessor.cs:119-133` (same pattern for
  invalid laps), `RepositoryBase.RemoveRange:388-415` (loads entities, removes one by one)
- **Problem:** Cleanup paths execute one DELETE statement per lap and `RemoveRange` materializes
  all entities before deleting them through the change tracker.
- **Impact:** After a reconnect to an existing session or at session end, hundreds of statements
  run serially on the processing thread.
- **Recommendation:** Set-based deletes: single
  `DELETE FROM CarTelemetries WHERE LapNumberId IN (SELECT Id FROM Laps WHERE SessionId = @p0)`
  (works on all three providers) and EF Core `ExecuteDelete()` for the lap rows. Add an
  `ExecuteDeleteAsync`-style helper to `RepositoryBase`.
- **Suggested issue:** *Set-based deletes for session cleanup paths*

#### PERF-08 (Medium) — `SessionProcessor` opens a context on every Session packet (2 Hz) even when nothing changed

- **Location:** `F1Server.Service/Processors/SessionProcessor.cs:57-86`
- **Problem:** The `using (var dbFactory = RepositoryFactory.CreateInstance())` wraps the whole
  method although the common case (session exists, no attribute/weather/assist change) performs
  no write; reads are served by `SessionRepositoryCache`/`TrackRepositoryCache`.
- **Impact:** ~2 needless context creations per second for the whole session (couples with
  PERF-01).
- **Recommendation:** Create the factory lazily in the branches that actually write
  (`CreateNewSession`, network-game refresh, attribute refresh). Cheap once PERF-01 lands, but
  still avoids leasing a context 7,000+ times per hour.
- **Suggested issue:** *SessionProcessor: lazy DbContext creation*

### 2.2 Receive path and queue worker (`TelemetryClient`)

#### PERF-09 (High) — Per-packet INFO logging in the hot path

- **Location:** `F1Server.Service/TelemetryClient.cs:373`
  (`"Received packet with length: {Length}"`), `TelemetryClient.cs:482`
  (`"Processing packet with id ..."`), `TelemetryClient.cs:813` (TCP replay path)
- **Problem:** Two `LogInformation` calls per packet. At 60 Hz × several packet types this is
  hundreds of log records per second at default log level — formatting, allocation, sink I/O.
- **Impact:** Constant CPU/alloc overhead and log noise that drowns real events; measurable at
  ingest rates.
- **Recommendation:** Downgrade to `LogTrace`/`LogDebug` guarded by `IsEnabled`, or replace with
  the already existing metrics (`PacketsReceived` counter). Use `LoggerMessage` source-generated
  logging for whatever remains.
- **Suggested issue:** *Remove per-packet INFO logging from the ingest hot path*

#### PERF-10 (High) — `ConcurrentQueue<T>.Count` (O(n) snapshot) is sampled twice per packet

- **Location:** `F1Server.Service/TelemetryClient.cs:382, 480`
  (`AppMetrics.PacketsInQueue.Record(_packetQueue.Count)`),
  `F1Server.Service/Runtime/PacketProcessor.cs:111` (`QueuedPackets => _queuedPackets?.Count`),
  read per packet at `TelemetryClient.cs:529`
- **Problem:** `ConcurrentQueue<T>.Count` walks the segment list and is not O(1). It is called on
  every enqueue *and* every dequeue, plus `PacketProcessor.QueuedPackets` per processed packet —
  exactly when the queue is large (backlog) the count gets most expensive.
- **Impact:** Superlinear cost growth under backlog — the metric makes the congestion worse.
- **Recommendation:** The class already maintains `_queuedPackets` via `Interlocked` — record
  that value instead of `_packetQueue.Count`. Same for `PacketProcessor` (maintain an
  `Interlocked` counter next to the queue). Alternatively sample the gauge on the statistics
  timer (1 value/s) instead of per packet.
- **Suggested issue:** *Replace ConcurrentQueue.Count sampling with O(1) counters*

#### PERF-11 (Medium) — Timer restart and connection bookkeeping per datagram

- **Location:** `F1Server.Service/TelemetryClient.cs:344-357` (`_timeoutTimer.Stop()/Start()`
  per packet, statistics counters), `TelemetryClient.cs:365-367`
  (`Interlocked` + `CheckPacketProcessingTaskIsRunning()` per packet)
- **Problem:** `System.Timers.Timer.Stop/Start` takes an internal lock and re-arms the underlying
  timer — per datagram. The statistics counters (`PacketsReceivedTotal++`, `...CurrentSession++`)
  are plain increments racing between UDP callback and the TCP replay task.
- **Impact:** Avoidable synchronization per packet; slightly wrong statistics under load.
- **Recommendation:** Track `_lastPacketUtcTicks` (Volatile.Write) per packet and let the
  periodically running timeout timer compare against it; increment statistics with
  `Interlocked.Increment`.
- **Suggested issue:** *Cheap per-packet bookkeeping: timeout timestamp + atomic statistics*

#### PERF-12 (Medium) — Double copy of every datagram + legacy APM receive loop

- **Location:** `F1Server.Service/TelemetryClient.cs:361-371, 398`
  (`BeginReceive`/`EndReceive`), `F1Server.Core/Data/ReceivedPacketData.cs:76-87`
  (`SetRawData` copies the array again)
- **Problem:** `EndReceive` already returns a freshly allocated `byte[]` per datagram;
  `SetRawData` then allocates a second array of the same size and copies the payload. Two
  allocations + one copy per packet (~1.5 KB avg, CarTelemetry ~1.3 KB, Motion bigger). The APM
  (`Begin/End`) pattern additionally allocates an `IAsyncResult`/callback state per datagram and
  re-registers the callback per packet.
- **Impact:** GC pressure in Gen0 proportional to packet rate; at 300-500 packets/s this is
  ~1 MB/s of avoidable garbage plus APM overhead.
- **Recommendation:** (a) Quick win: `SetRawData` can take ownership of the array received from
  `EndReceive` instead of copying (the caller never reuses it). (b) Larger: switch to a
  `Socket.ReceiveFromAsync`-based loop (`ValueTask`, `SocketAsyncEventArgs` or
  `PipeReader`-style) with `ArrayPool<byte>` buffers and return buffers after processing/logging.
  (b) requires clear buffer-lifetime rules because `ReceivedPacketData` flows into two queues
  (processing + file logging).
- **Suggested issue:** *Reduce per-datagram allocations in the UDP receive path*

#### PERF-13 (Medium) — Queue worker polls with 100 ms sleep; adds latency to "live" data

- **Location:** `F1Server.Service/TelemetryClient.cs:752-779` (`ProcessPacketQueue`,
  `cancellationToken.WaitHandle.WaitOne(100)`), same pattern `ProcessPacketLoggingQueue:861`,
  `TelemetryWriter.WriteToInflux:371` (500 ms)
- **Problem:** After draining the queue the worker sleeps 100 ms regardless of new arrivals. A
  packet arriving right after the drain waits up to 100 ms before processing even at zero load.
  For a "live" website view this is pure added latency (and it stacks with SignalR-side
  batching, out of scope here).
- **Impact:** Up to +100 ms processing latency per packet at low/medium load; +500 ms for Influx
  live data.
- **Recommendation:** Replace queue + poll with `System.Threading.Channels.Channel<T>`
  (unbounded, single reader) and `await reader.ReadAsync(ct)` — zero-latency wakeup, no spinning,
  and it also removes the hand-rolled `_isQueueWorkerRunning` flag (see PERF-14). Same for the
  logging queue and the Influx writer.
- **Suggested issue:** *Replace polling queue workers with System.Threading.Channels*

#### PERF-14 (Low) — Racy worker-start flags

- **Location:** `F1Server.Service/TelemetryClient.cs:48-50, 705-742`
  (`_isQueueWorkerRunning`, `_isLoggingQueueRunning`, check-then-set from multiple threads),
  `F1Server.Telemetry/TelemetryWriter.cs:281-297` (`EnsureWriteTaskRunning` cancels/disposes the
  CTS of a possibly still-running task before starting a new one)
- **Problem:** Non-volatile bools with check-then-set; two receive callbacks can race and start
  two workers (they then both dequeue — functionally tolerable but doubles per-packet metric
  writes), or observe a stale `true` and start none until the next packet.
- **Impact:** Sporadic duplicate workers/latency blips; hard-to-reproduce behavior.
- **Recommendation:** Solved for free by the Channels refactor (PERF-13: one long-lived reader
  task started in `StartReceiving`). Otherwise `Interlocked.CompareExchange` on an int flag.
- **Suggested issue:** covered by PERF-13 issue

### 2.3 Dispatch and processor layer

#### PERF-15 (High) — `Task.Run` per packet for the `PacketReceived` event

- **Location:** `F1Server.Service/Runtime/PacketProcessor.cs:169-172`
- **Problem:** For every packet with a header, a new thread-pool work item is queued just to
  invoke the `PacketReceived` event (plus one `PacketReceivedEventArgs` allocation per packet).
  At 300-500 packets/s this floods the thread pool and event ordering is lost.
- **Impact:** Scheduling overhead and allocations per packet; subscribers see packets out of
  order.
- **Recommendation:** Invoke the handler synchronously if subscribers are cheap (UI/console
  status), or push the header into a dedicated bounded channel consumed by one dispatcher task.
  Skip entirely when there are no subscribers (already guarded) — check what actually subscribes
  in `F1Server`; if it is only the console status line, throttling to a few events/s is enough.
- **Suggested issue:** *Stop spawning a Task per packet for PacketReceived*

#### PERF-16 (Medium) — Per-packet Activities across the whole pipeline (4-6 spans per packet, more per DB command)

- **Location:** `TelemetryClient.ReceiveCallback:320-325` ("PacketReceived" span per datagram),
  `TelemetryClient.ProcessCurrentPackets:484` ("ProcessCurrentPacket"),
  `PacketProcessor.AnalyzePacket:282`, every processor (`LapDataProcessor:62`,
  `CarTelemetryProcessor:61`, …), every `PacketToX` transformation
  (`PacketToCarTelemetry.cs:31`), and `F1Server.Db/Entity/CommandInterceptor.cs` (a span for
  *Executing and *Executed of every DB command — duplicating what the EF Core instrumentation
  already provides).
- **Problem:** With an OTLP listener attached, every datagram produces 4-6 spans plus 2 spans per
  SQL command; tags are stringified per span. Without a listener the cost is small but not zero
  (null checks, `using` frames).
- **Impact:** Under tracing, span volume scales with packet rate (millions of spans per session)
  — cost in the app *and* in the collector; trace data is mostly noise.
- **Recommendation:** Keep spans for rare/structural events (session start/end, game version,
  final classification, errors) and drop or sample the per-packet/per-parse spans (e.g., only
  record when processing time exceeds a threshold, or head-sample 1/N). Remove the
  `CommandInterceptor` spans in favor of the standard EF Core/ADO instrumentation, or make the
  interceptor registration opt-in via configuration.
- **Suggested issue:** *Reduce tracing volume in the per-packet hot path*

#### PERF-17 (Low) — ProcessorFactory: double dictionary lookup + `Activator.CreateInstance` per processor type

- **Location:** `F1Server.Service/Processors/ProcessorFactory.cs:113-375` (nine near-identical
  `GetXProcessor` methods: `ContainsKey` + `TryGetValue`, creation via `Activator`)
- **Problem:** Two hash lookups per packet and reflection-based construction (only on first use
  per session — cheap, but the pattern is 9× copy-paste and error-prone).
- **Impact:** Minor per-packet cost; mostly maintainability.
- **Recommendation:** One generic helper
  `GetOrCreate<T>(Func<T> factory) => (T)_processors.GetOrAdd(typeof(T), _ => factory())` with
  direct `new SessionProcessor(...)` lambdas. Removes ~250 lines and the double lookup.
- **Suggested issue:** *Simplify ProcessorFactory processor caching*

#### PERF-18 (Low) — `RepositoryFactory.GetRepository`/`GetQuery` use reflection per call

- **Location:** `F1Server.Db/Entity/RepositoryFactory.cs:92-102`
  (`Activator.CreateInstance(typeof(TRepository), _dbContext)`),
  `RepositoryBase{TQueryable,TEntity}.cs:65-68` (`Activator.CreateInstance` per `GetQuery()`)
- **Problem:** Repositories are cached per factory instance, but factories are short-lived (see
  PERF-01), so `Activator` runs on effectively every repository access; `GetQuery()` reflects on
  every single query.
- **Impact:** Small but constant reflection overhead multiplied by call frequency.
- **Recommendation:** Cache compiled factory delegates
  (`static readonly ConcurrentDictionary<Type, Func<F1ServerDbContext, RepositoryBase>>` built
  via expression trees) or require a `new(F1ServerDbContext)` pattern via generic constraints.
  Loses urgency once PERF-01 makes factories long-lived.
- **Suggested issue:** *Replace Activator-based construction in the repository layer*

### 2.4 Parsing (`F1Server.Core`)

#### PERF-19 (Medium) — Full managed object graph + transformation object allocated per packet

- **Location:** `F1Server.Core/PacketAnalyzer.cs` (a `new PacketToXxx(packetHeader)` per
  packet), `F1Server.Core/Packets/PacketToObject/PacketToCarTelemetry.cs:39-50` (per packet: a
  `CarTelemetry` wrapper, a `CarTelemetry20xx` payload object, and inside 22-24 per-car data
  objects incl. nested wheel-data arrays); same pattern for LapData, CarStatus, SessionHistory,
  Session
- **Problem:** For the high-frequency packets, each datagram is expanded into dozens of small
  heap objects even though most consumers read only a handful of fields (e.g., CarTelemetry
  processing uses the data only for human drivers / recordable laps; LapData maps into
  `IndependentLapData` immediately).
- **Impact:** At 60 Hz × several packet types this is tens of thousands of allocations per
  second — the dominant GC-pressure source after PERF-12. Gen0 collections pause the processing
  thread.
- **Recommendation:** Staged approach: (a) make the `PacketToXxx` transformations stateless and
  reusable (they only carry the header — pass it per call) so at least the transformer
  allocation disappears; (b) for CarTelemetry/LapData add lean struct-based accessors over the
  raw span (read fields on demand) or pool the per-car data objects per processor; (c) skip
  extraction entirely for cars that will not be used (non-recordable, non-human) where the
  processor allows it. Each step is independently shippable; measure between steps.
- **Suggested issue:** *Reduce per-packet allocations in packet-to-object transformation*

#### PERF-20 (Low) — `Enum.ToObject` in the header parser

- **Location:** `F1Server.Core/Data/ReceivedPacketData.cs:147`
- **Problem:** `(PacketTypes)Enum.ToObject(typeof(PacketTypes), packetType + 1)` boxes per
  packet; a direct cast `(PacketTypes)(packetType + 1)` is allocation-free and identical in
  behavior.
- **Impact:** One boxed allocation per datagram.
- **Recommendation:** Direct cast; optionally validate with `<= PacketTypes.LapPositions`-style
  range check instead.
- **Suggested issue:** can be bundled with PERF-12 or PERF-19

### 2.5 Runtime/live data structures

#### PERF-21 (Medium) — O(n) `List.Find` per car per packet on live driver data

- **Location:** `F1Server.Service/Processors/LapDataProcessor.cs:344`
  (`Drivers?.Find(p => p.DbId == participantDbId)` inside the per-car loop → per LapData packet
  22× a linear scan), `F1Server.Service/Processors/CarStatusProcessor.cs:136`,
  `F1Server.Service/Processors/SessionHistoryProcessor.cs:63`
- **Problem:** `LiveSessionData.Drivers` is a `List<>`; lookups by `DbId` are linear and run in
  per-car loops of high-frequency packets (~22 × 22 comparisons per LapData packet at up to
  60 Hz, plus a closure allocation per `Find` call).
- **Impact:** Hundreds of thousands of delegate invocations/s in a full field; cheap
  individually but entirely avoidable in the hottest loop.
- **Recommendation:** Maintain a `Dictionary<long, LiveDriverData>` (keyed by `ParticipantDbId`)
  next to the list — or better, cache the `LiveDriverData` reference directly on
  `ParticipantRuntimeData` when the participant is created (both live for exactly one session).
- **Suggested issue:** *O(1) live-driver lookup in per-car packet loops*

#### PERF-22 (Low) — `LapDataProcessor.UpdateSessionInformation` re-runs LINQ ordering per packet while the timetable is empty

- **Location:** `F1Server.Service/Processors/LapDataProcessor.cs:137-153`
- **Problem:** While `TimeTable.Count == 0` (whole first lap of a race), every LapData packet
  runs up to two `Where/OrderBy/Select/ToList` chains over all drivers.
- **Impact:** Bounded (n = 22) but repeated ~60×/s during the opening lap; allocation per call.
- **Recommendation:** Compute once when grid positions become known (Participants/first LapData
  packet) and only recompute on change; or mark dirty via a flag.
- **Suggested issue:** can be bundled with PERF-21

#### PERF-23 (Low) — `SessionRuntimeData` property setters perform hidden synchronous DB writes

- **Location:** `F1Server.Service/Runtime/SessionRuntimeData.cs:84-97` (`Players` setter →
  `UpdateSessionPlayers()` → context + UPDATE), `132-141` (`FlashbacksUsed` setter →
  `UpdateFlashbacksUsed()`)
- **Problem:** Assigning a property (done from `ParticipantsProcessor.Process` per Participants
  packet, and from the flashback event path) synchronously opens a context and issues an UPDATE.
  Hidden I/O in setters makes cost invisible at call sites and resists the background-writer
  refactor (PERF-02).
- **Impact:** Bounded frequency, but couples runtime state to DB latency and complicates PERF-02.
- **Recommendation:** Convert to explicit methods routed through the same background DB-writer
  queue as PERF-02.
- **Suggested issue:** *Move session-attribute updates out of property setters into the DB
  writer*

---

## 3. Suggested issue breakdown and ordering

The findings compose into the following work packages. Ordering reflects dependency and
expected payoff; sizes are rough (S ≤ ½ day, M ≈ 1-2 days, L > 2 days incl. tests).

| # | Issue title | Findings | Priority | Size |
|---|---|---|---|---|
| 1 | Introduce DbContext pooling behind RepositoryFactory | PERF-01 | Critical | M |
| 2 | Move lap/telemetry persistence to an async background DB writer | PERF-02, PERF-23 | Critical | L |
| 3 | Fast insert path for CarTelemetry batches | PERF-03 | High | M |
| 4 | SessionHistoryProcessor: lazy context + convergent lap-diff detection | PERF-04 | High | M |
| 5 | Remove/bypass EF AutoIncludes in hot lap and telemetry queries | PERF-05 | High | S |
| 6 | Eliminate redundant round-trips in AddLap | PERF-06 | High | S |
| 7 | Remove per-packet INFO logging from the ingest hot path | PERF-09 | High | S |
| 8 | Replace ConcurrentQueue.Count sampling with O(1) counters | PERF-10 | High | S |
| 9 | Stop spawning a Task per packet for PacketReceived | PERF-15 | High | S |
| 10 | Replace polling queue workers with System.Threading.Channels | PERF-13, PERF-14 | Medium | M |
| 11 | Reduce per-datagram allocations in the UDP receive path | PERF-12, PERF-20 | Medium | M |
| 12 | Cheap per-packet bookkeeping (timeout timestamp, atomic statistics) | PERF-11 | Medium | S |
| 13 | Set-based deletes for session cleanup paths | PERF-07 | Medium | S |
| 14 | SessionProcessor: lazy DbContext creation | PERF-08 | Medium | S |
| 15 | Reduce tracing volume in the per-packet hot path | PERF-16 | Medium | M |
| 16 | O(1) live-driver lookup in per-car packet loops | PERF-21, PERF-22 | Medium | S |
| 17 | Reduce per-packet allocations in packet-to-object transformation | PERF-19 | Medium | L |
| 18 | Simplify ProcessorFactory / repository construction | PERF-17, PERF-18 | Low | S |

Notes for the executing agent(s):

- Issues 1 and 2 are the foundation; several later issues get simpler (or partially obsolete)
  once they land. Do them first and in this order.
- Issue 2 changes threading semantics: everything currently guarded by the single processing
  thread (runtime data mutation) must stay on that thread — only the *DB write jobs* move to the
  writer. Define the job types (`InsertLap`, `CompleteLap`, `InsertTelemetryBatch`,
  `UpdateSessionAttribute`, `DeleteLap`) explicitly.
- Every issue must respect the repository conventions in `CLAUDE.md` (MSTest, Reihitsu analyzer
  zero-warning builds, multi-provider support for MariaDB/MsSql/PostgreSQL — no provider-specific
  SQL without a fallback, `reihitsu-format` before commit).
- Add/extend tests in `F1Server.Tests` per issue; the InMemory provider
  (`F1SERVER_DATABASE_TYPE=99`) covers repository-level behavior, and the existing sample
  packets under `F1Server.Tests/SampleData` cover the processor paths.
- Before/after numbers: the pipeline already records per-packet-type processing times
  (`PacketsPerSessionMetrics`, `AppMetrics.RecordProcessedPacket`) — use the session summary
  written by `TelemetryClient.WriteLastSessionStatistics` and the `F1ReplayClient` to replay a
  recorded session as the benchmark harness for validating each change.
