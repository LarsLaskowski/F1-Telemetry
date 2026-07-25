using F1Server.Core.Enumerations;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.WebApi.Cache;

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
    /// Reference lap time in milliseconds of the track created for the tests in this class
    /// </summary>
    private const uint ReferenceLapTime = 79000U;

    /// <summary>
    /// Name of the driver created for the tests in this class
    /// </summary>
    private const string TestDriverName = "Fastest Lap Cache Driver";

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Database ids of the sessions created for the tests in this class
    /// </summary>
    private static readonly List<long> _testSessionIds = [];

    #endregion // Fields

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

            for (var sessionIndex = 0; sessionIndex < TestSessionCount; sessionIndex++)
            {
                var sessionEntity = new SessionEntity
                                    {
                                        SessionId = 198000000000UL + (ulong)sessionIndex,
                                        CreationTimestamp = DateTime.UtcNow,
                                        SessionType = SessionType.Qualifying1,
                                        TrackId = trackEntity.Id,
                                        GameVersionId = 1
                                    };

                Assert.IsTrue(dbFactory.GetRepository<SessionRepository>()?.Add(sessionEntity), "Session entity could not be added to the database!");

                var participantEntity = new ParticipantEntity
                                        {
                                            SessionId = sessionEntity.Id,
                                            DriverId = driverEntity.Id,
                                            NationalityId = nationalityEntity.Id,
                                            TeamId = teamEntity.Id,
                                            DriverName = TestDriverName,
                                            ArrayIndex = 1,
                                            IsHumanControlled = true
                                        };

                Assert.IsTrue(dbFactory.GetRepository<ParticipantRepository>()?.Add(participantEntity), "Participant entity could not be added to the database!");

                AddLap(dbFactory, sessionEntity.Id, participantEntity.Id, 1, FastestLapTime);
                AddLap(dbFactory, sessionEntity.Id, participantEntity.Id, 2, FastestLapTime + 1500U);

                _testSessionIds.Add(sessionEntity.Id);
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
                                .WaitAsync(TimeSpan.FromSeconds(30))
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
                                .WaitAsync(TimeSpan.FromSeconds(30))
                                .ConfigureAwait(false);

        var returnedSessionIds = results.Select(fastestLapData => fastestLapData.SessionId)
                                        .ToList();

        CollectionAssert.AreEquivalent(_testSessionIds, returnedSessionIds, "Every parallel request should return the data of the session it asked for!");

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

    #endregion // Methods

    #region Private methods

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