using System.Reflection;

using F1Server.Core.Enumerations;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache;

namespace F1Server.Tests.Cache;

/// <summary>
/// Contains unit tests verifying the on demand calculation and the parallel access behaviour of the
/// <see cref="FastestLapPerSessionCache"/>
/// </summary>
[TestClass]
public class FastestLapPerSessionCacheTests
{
    #region Constants

    /// <summary>
    /// Number of sessions created for the tests in this class
    /// </summary>
    private const int TestSessionCount = 8;

    /// <summary>
    /// Number of requests issued in parallel for a single session
    /// </summary>
    private const int ParallelRequestCount = 8;

    /// <summary>
    /// Lap time in milliseconds of the fastest lap created for every test session
    /// </summary>
    private const uint FastestLapTime = 80000U;

    /// <summary>
    /// Lap time in milliseconds of a lap that is added after the fastest lap of a session was already cached
    /// </summary>
    private const uint FasterLapTime = 78500U;

    /// <summary>
    /// Reference lap time in milliseconds of the track created for the tests in this class
    /// </summary>
    private const uint ReferenceLapTime = 79000U;

    /// <summary>
    /// Format of a lap time
    /// </summary>
    private const string LapTimeLiteral = @"mm\:ss\.fff";

    /// <summary>
    /// Name of the driver created for the tests in this class
    /// </summary>
    private const string TestDriverName = "Fastest Lap Cache Driver";

    /// <summary>
    /// Name of the private field holding the warm up state of the cache
    /// </summary>
    private const string CacheInitializedFieldName = "_cacheInitialized";

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Database ids of the sessions created for the tests in this class
    /// </summary>
    private static readonly List<long> _testSessionIds = [];

    /// <summary>
    /// Database id of the track created for the tests in this class
    /// </summary>
    private static long _testTrackId;

    /// <summary>
    /// Database id of the driver created for the tests in this class
    /// </summary>
    private static long _testDriverId;

    /// <summary>
    /// Database id of the nationality created for the tests in this class
    /// </summary>
    private static long _testNationalityId;

    /// <summary>
    /// Database id of the team created for the tests in this class
    /// </summary>
    private static long _testTeamId;

    /// <summary>
    /// Game session code of the last session created for the tests in this class
    /// </summary>
    private static ulong _lastSessionCode = 198000000000UL;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Gets or sets the test context that provides the cancellation token for the running test
    /// </summary>
    public TestContext TestContext { get; set; }

    #endregion // Properties

    #region Static methods

    /// <summary>
    /// Warms up the cache and creates the sessions, participants and laps used by the tests in this class
    /// </summary>
    /// <param name="context">Test context</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        // Warm up before the test sessions exist, so every test session has to be calculated on demand
        await FastestLapPerSessionCache.InitializeCacheAsync(CancellationToken.None).ConfigureAwait(false);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var driverEntity = new DriverEntity
                               {
                                   DriverGameId = 198001,
                                   Name = TestDriverName
                               };

            Assert.IsTrue(dbFactory.GetRepository<DriverRepository>()?.Add(driverEntity), "Driver entity could not be added to the database!");

            var nationalityEntity = new NationalityEntity
                                    {
                                        NationalityGameId = 198,
                                        Name = "Fastest Lap Cache Nationality"
                                    };

            Assert.IsTrue(dbFactory.GetRepository<NationalityRepository>()?.Add(nationalityEntity), "Nationality entity could not be added to the database!");

            var teamEntity = new TeamEntity
                             {
                                 TeamGameId = 198002,
                                 Name = "Fastest Lap Cache Team"
                             };

            Assert.IsTrue(dbFactory.GetRepository<TeamRepository>()?.Add(teamEntity), "Team entity could not be added to the database!");

            // The session query includes the track, so the sessions need a track that exists in the test database
            var trackEntity = new TrackEntity
                              {
                                  TrackNumber = 198,
                                  Name = "Fastest Lap Cache Track",
                                  LapReferenceTime = ReferenceLapTime,
                                  Sector1ReferenceTime = 26000,
                                  Sector2ReferenceTime = 17000,
                                  Sector3ReferenceTime = 36000
                              };

            Assert.IsTrue(dbFactory.GetRepository<TrackRepository>()?.Add(trackEntity), "Track entity could not be added to the database!");

            _testTrackId = trackEntity.Id;
            _testDriverId = driverEntity.Id;
            _testNationalityId = nationalityEntity.Id;
            _testTeamId = teamEntity.Id;

            for (var sessionIndex = 0; sessionIndex < TestSessionCount; sessionIndex++)
            {
                var sessionId = AddSession(dbFactory, out var participantId);

                AddLap(dbFactory, sessionId, participantId, 1, FastestLapTime);
                AddLap(dbFactory, sessionId, participantId, 2, FastestLapTime + 1500U);

                _testSessionIds.Add(sessionId);
            }
        }
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that a session added after the warm up is calculated on demand and returns its fastest lap
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [TestMethod]
    public async Task FastestLapPerSessionCacheGetFastestLapDataForSessionReturnsCalculatedData()
    {
        var sessionId = _testSessionIds[0];

        var fastestLapData = await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId).ConfigureAwait(false);

        Assert.AreEqual(sessionId, fastestLapData.SessionId, "The returned data should belong to the requested session!");
        Assert.AreEqual(TimeSpan.FromMilliseconds(FastestLapTime).ToString(@"mm\:ss\.fff"), fastestLapData.FastestLap, "The faster of both created laps should be reported as fastest lap!");
        Assert.AreEqual(TestDriverName, fastestLapData.FastestLapDriver, "The fastest lap should be assigned to the created test driver!");
        Assert.AreEqual(TimeSpan.FromMilliseconds(ReferenceLapTime).ToString(@"mm\:ss\.fff"), fastestLapData.ReferenceLapTime, "The reference lap time of the track of the session should be returned!");
    }

    /// <summary>
    /// Verifies that parallel requests for the same uncached session calculate the data only once and share the
    /// cached instance
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [TestMethod]
    public async Task FastestLapPerSessionCacheParallelRequestsForSameSessionReturnCachedInstance()
    {
        var sessionId = _testSessionIds[1];

        var requests = Enumerable.Range(0, ParallelRequestCount)
                                 .Select(_ => Task.Run(() => FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId)))
                                 .ToArray();

        var results = await Task.WhenAll(requests)
                                .WaitAsync(TimeSpan.FromSeconds(30), TestContext.CancellationToken)
                                .ConfigureAwait(false);

        foreach (var result in results)
        {
            Assert.AreSame(results[0], result, "All parallel requests for the same session should return the same cached instance!");
        }
    }

    /// <summary>
    /// Verifies that parallel requests for different uncached sessions complete and each one returns the data of its
    /// own session
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [TestMethod]
    public async Task FastestLapPerSessionCacheParallelRequestsForDifferentSessionsReturnMatchingData()
    {
        var requests = _testSessionIds.Select(sessionId => Task.Run(() => FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId)))
                                      .ToArray();

        var results = await Task.WhenAll(requests)
                                .WaitAsync(TimeSpan.FromSeconds(30), TestContext.CancellationToken)
                                .ConfigureAwait(false);

        // Task.WhenAll keeps the order of the requests, so the results are in the order of the session ids
        var returnedSessionIds = results.Select(fastestLapData => fastestLapData.SessionId)
                                        .ToList();

        Assert.AreSequenceEqual(_testSessionIds, returnedSessionIds, "Every parallel request should return the data of the session it asked for!");

        var expectedFastestLap = TimeSpan.FromMilliseconds(FastestLapTime).ToString(@"mm\:ss\.fff");

        foreach (var result in results)
        {
            Assert.AreEqual(expectedFastestLap, result.FastestLap, "Every parallel request should return the calculated fastest lap of its session!");
        }
    }

    /// <summary>
    /// Verifies that an unknown session does not throw and returns empty data for the requested session
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [TestMethod]
    public async Task FastestLapPerSessionCacheGetFastestLapDataForUnknownSessionReturnsEmptyData()
    {
        const long unknownSessionId = 198999999L;

        var fastestLapData = await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(unknownSessionId).ConfigureAwait(false);

        Assert.AreEqual(unknownSessionId, fastestLapData.SessionId, "The returned data should belong to the requested session!");
        Assert.IsNull(fastestLapData.FastestLap, "An unknown session should not report a fastest lap!");
    }

    /// <summary>
    /// Verifies that a warm up aborted by a cancellation request leaves the cache uninitialized, so the incomplete
    /// cache is not treated as complete for the rest of the process lifetime
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [TestMethod]
    public async Task FastestLapPerSessionCacheCancelledWarmUpDoesNotMarkCacheAsInitialized()
    {
        var initializedField = typeof(FastestLapPerSessionCache).GetField(CacheInitializedFieldName, BindingFlags.NonPublic | BindingFlags.Static);

        Assert.IsNotNull(initializedField, "The warm up state of the cache could not be resolved!");

        var previousState = initializedField.GetValue(null);

        // Force a new warm up, otherwise the already warmed up cache returns before the cancellation is evaluated
        initializedField.SetValue(null, false);

        try
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.Cancel();

                try
                {
                    await FastestLapPerSessionCache.InitializeCacheAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected - the warm up honours the cancellation request
                }
            }

            var stateAfterCancellation = initializedField.GetValue(null) as bool?;

            Assert.IsNotNull(stateAfterCancellation, "The warm up state of the cache could not be read!");
            Assert.IsFalse(stateAfterCancellation.Value, "A warm up aborted by a cancellation request must not mark the cache as initialized!");
        }
        finally
        {
            initializedField.SetValue(null, previousState);
        }
    }

    /// <summary>
    /// Verifies that a lap added to an already cached session is reported after the session was invalidated, so a
    /// running session does not keep the fastest lap of the moment its entry was calculated
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [TestMethod]
    public async Task FastestLapPerSessionCacheInvalidateSessionReturnsNewFastestLap()
    {
        long sessionId;
        long participantId;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            sessionId = AddSession(dbFactory, out participantId);

            AddLap(dbFactory, sessionId, participantId, 1, FastestLapTime);
        }

        var cachedLapData = await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId).ConfigureAwait(false);

        Assert.AreEqual(TimeSpan.FromMilliseconds(FastestLapTime).ToString(LapTimeLiteral), cachedLapData.FastestLap, "The only lap of the session should be reported as fastest lap!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            AddLap(dbFactory, sessionId, participantId, 2, FasterLapTime);
        }

        var staleLapData = await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId).ConfigureAwait(false);

        Assert.AreSame(cachedLapData, staleLapData, "Without an invalidation the already cached data of the session should be returned!");

        FastestLapPerSessionCache.InvalidateSession(sessionId);

        var updatedLapData = await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId).ConfigureAwait(false);

        Assert.AreEqual(TimeSpan.FromMilliseconds(FasterLapTime).ToString(LapTimeLiteral), updatedLapData.FastestLap, "After an invalidation the faster lap added to the session should be reported as fastest lap!");
    }

    /// <summary>
    /// Verifies that a removed session is calculated again instead of returning the data of the deleted session
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [TestMethod]
    public async Task FastestLapPerSessionCacheRemoveSessionDropsCachedData()
    {
        long sessionId;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            sessionId = AddSession(dbFactory, out var participantId);

            AddLap(dbFactory, sessionId, participantId, 1, FastestLapTime);
        }

        var cachedLapData = await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId).ConfigureAwait(false);

        FastestLapPerSessionCache.RemoveSession(sessionId);

        var reloadedLapData = await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId).ConfigureAwait(false);

        Assert.AreNotSame(cachedLapData, reloadedLapData, "A removed session must not be served from the cache anymore!");
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Adds a session with one participant to the test database
    /// </summary>
    /// <param name="dbFactory">Repository factory used to store the session</param>
    /// <param name="participantId">Database id of the participant created for the session</param>
    /// <returns>Database id of the created session</returns>
    private static long AddSession(RepositoryFactory dbFactory, out long participantId)
    {
        _lastSessionCode += 1UL;

        var sessionEntity = new SessionEntity
                            {
                                SessionId = _lastSessionCode,
                                CreationTimestamp = DateTime.UtcNow,
                                SessionType = SessionType.Qualifying1,
                                TrackId = _testTrackId,
                                GameVersionId = 1
                            };

        Assert.IsTrue(dbFactory.GetRepository<SessionRepository>()?.Add(sessionEntity), "Session entity could not be added to the database!");

        var participantEntity = new ParticipantEntity
                                {
                                    SessionId = sessionEntity.Id,
                                    DriverId = _testDriverId,
                                    NationalityId = _testNationalityId,
                                    TeamId = _testTeamId,
                                    DriverName = TestDriverName,
                                    ArrayIndex = 1,
                                    IsHumanControlled = true
                                };

        Assert.IsTrue(dbFactory.GetRepository<ParticipantRepository>()?.Add(participantEntity), "Participant entity could not be added to the database!");

        participantId = participantEntity.Id;

        return sessionEntity.Id;
    }

    /// <summary>
    /// Adds a valid completed lap to the test database
    /// </summary>
    /// <param name="dbFactory">Repository factory used to store the lap</param>
    /// <param name="sessionId">Database id of the session the lap belongs to</param>
    /// <param name="participantId">Database id of the participant the lap belongs to</param>
    /// <param name="lapNumber">Number of the lap</param>
    /// <param name="lapTime">Lap time in milliseconds</param>
    private static void AddLap(RepositoryFactory dbFactory, long sessionId, long participantId, ushort lapNumber, uint lapTime)
    {
        var lapEntity = new LapEntity
                        {
                            LapNumber = lapNumber,
                            ParticipantId = participantId,
                            SessionId = sessionId,
                            LapTime = lapTime,
                            Sector1Time = lapTime / 3U,
                            Sector2Time = lapTime / 3U,
                            Sector3Time = lapTime - (2U * (lapTime / 3U)),
                            IsCompleted = true,
                            IsInvalidLapTime = false,
                            DriverStatus = DriverStatus.FlyingLap,
                            PitStatus = PitStatus.None,
                            ResultStatus = ResultStatus.Active
                        };

        Assert.IsTrue(dbFactory.GetRepository<LapRepository>()?.Add(lapEntity), "Lap entity could not be added to the database!");
    }

    #endregion // Private methods
}