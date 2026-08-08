using F1Server.Core.Enumerations;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Service.Sessions;
using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the asynchronous actions of the <see cref="SessionsController"/> read the
/// expected data from the database
/// </summary>
[TestClass]
public class SessionsControllerTests
{
    #region Properties

    /// <summary>
    /// Gets or sets the test context that provides the cancellation token for the running test
    /// </summary>
    public TestContext TestContext { get; set; }

    #endregion // Properties

    #region Static methods

    /// <summary>
    /// Creates the entity graph read by the tests in this class
    /// </summary>
    /// <param name="context">Test context</param>
    /// <returns>Task</returns>
    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        await ControllerTestData.EnsureCreatedAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a controller instance with an isolated cache, so a cached session list does not leak between tests
    /// </summary>
    /// <param name="cache">Cache used by the created controller</param>
    /// <returns>Controller</returns>
    private static SessionsController CreateController(out MemoryCache cache)
    {
        cache = new MemoryCache(new MemoryCacheOptions());

        return new SessionsController(NullLogger<SessionsController>.Instance, cache, new SessionService());
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that the session list contains the finished test session
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsReturnsFinishedSession()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            // The session list is ordered by the newest session, so the page has to be large enough to also contain
            // the shared test session behind the sessions created by the other tests
            var result = await controller.GetSessions(pageIndex: 0, pageSize: 100).ConfigureAwait(false);

            var pageResult = (result.Result as OkObjectResult)?.Value as PageResultData<SessionViewData>;

            Assert.IsNotNull(pageResult, "The action should return a page result!");
            Assert.IsGreaterThan(0, pageResult.TotalCount, "The finished test session should be counted!");

            var testSession = pageResult.Items.Find(s => s.SessionDbId == ControllerTestData.SessionId);

            Assert.IsNotNull(testSession, "The finished test session should be part of the first page!");
            Assert.AreEqual(ControllerTestData.TrackName, testSession.Track, "The track name of the test session should be returned!");
            Assert.AreEqual(ControllerTestData.GameVersionName, testSession.GameVersion, "The game version name of the test session should be returned!");
            Assert.AreEqual(ControllerTestData.ParticipantCount, testSession.Cars, "The number of active cars of the test session should be returned!");
        }
    }

    /// <summary>
    /// Verifies that the requested page size limits the number of returned sessions
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsRespectsPageSize()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSessions(pageIndex: 0, pageSize: 1).ConfigureAwait(false);

            var pageResult = (result.Result as OkObjectResult)?.Value as PageResultData<SessionViewData>;

            Assert.IsNotNull(pageResult, "The action should return a page result!");
            Assert.HasCount(1, pageResult.Items, "A page size of one should return a single session!");
        }
    }

    /// <summary>
    /// Verifies that the race preceding a second race at the same track is reported as sprint race
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsReportsRaceBeforeSecondRaceAsSprint()
    {
        // A dedicated game version keeps the sprint detection limited to the sessions of this test
        var gameVersionId = ControllerTestData.AddGameVersion(3771003);

        var sprintSessionId = ControllerTestData.AddSession(SessionType.Race, isFinished: true, gameVersionId);
        var raceSessionId = ControllerTestData.AddSession(SessionType.Race2, isFinished: true, gameVersionId);

        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSessions(pageIndex: 0, pageSize: 100).ConfigureAwait(false);

            var pageResult = (result.Result as OkObjectResult)?.Value as PageResultData<SessionViewData>;

            Assert.IsNotNull(pageResult, "The action should return a page result!");

            var sprintSession = pageResult.Items.Find(s => s.SessionDbId == sprintSessionId);
            var raceSession = pageResult.Items.Find(s => s.SessionDbId == raceSessionId);

            Assert.IsNotNull(sprintSession, "The race preceding the second race should be part of the session list!");
            Assert.IsNotNull(raceSession, "The second race should be part of the session list!");
            Assert.AreEqual(SessionType.Sprint, sprintSession.SessionType, "The race preceding a second race should be reported as sprint race!");
            Assert.AreEqual(SessionType.Race2, raceSession.SessionType, "The second race should keep its session type!");
        }
    }

    /// <summary>
    /// Verifies that a race preceded by a sprint shootout is reported as sprint race in the session list
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsReportsRaceAfterSprintShootoutAsSprint()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3771004);

        ControllerTestData.AddSession(SessionType.SprintShootout1, isFinished: true, gameVersionId);

        var raceSessionId = ControllerTestData.AddSession(SessionType.Race, isFinished: true, gameVersionId);

        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSessions(pageIndex: 0, pageSize: 100).ConfigureAwait(false);

            var pageResult = (result.Result as OkObjectResult)?.Value as PageResultData<SessionViewData>;

            Assert.IsNotNull(pageResult, "The action should return a page result!");

            var raceSession = pageResult.Items.Find(s => s.SessionDbId == raceSessionId);

            Assert.IsNotNull(raceSession, "The race session should be part of the session list!");
            Assert.AreEqual(SessionType.Sprint, raceSession.SessionType, "A race preceded by a sprint shootout should be reported as sprint race!");
        }
    }

    /// <summary>
    /// Verifies that a page size above the allowed maximum is rejected
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsWithTooLargePageSizeReturnsBadRequest()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSessions(pageIndex: 0, pageSize: 101).ConfigureAwait(false);

            Assert.IsInstanceOfType<BadRequestObjectResult>(result.Result, "A page size above the maximum should be rejected!");
        }
    }

    /// <summary>
    /// Verifies that a page behind the last session is returned empty while the total count stays complete
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsBehindLastPageReturnsEmptyPage()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSessions(pageIndex: 100000, pageSize: 15).ConfigureAwait(false);

            var pageResult = (result.Result as OkObjectResult)?.Value as PageResultData<SessionViewData>;

            Assert.IsNotNull(pageResult, "The action should return a page result!");
            Assert.IsEmpty(pageResult.Items, "A page behind the last session must not contain sessions!");
            Assert.IsGreaterThan(0, pageResult.TotalCount, "The total count should stay complete on a page behind the last session!");
        }
    }

    /// <summary>
    /// Verifies that invalid paging parameters are rejected before the database is queried
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsWithInvalidPagingReturnsBadRequest()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSessions(pageIndex: -1).ConfigureAwait(false);

            Assert.IsInstanceOfType<BadRequestObjectResult>(result.Result, "A negative page index should be rejected!");
        }
    }

    /// <summary>
    /// Verifies that the session count includes the finished test session
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsCountCountsFinishedSessions()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var sessionCount = await controller.GetSessionsCount().ConfigureAwait(false);

            Assert.IsGreaterThan(0, sessionCount, "The finished test session should be counted!");
        }
    }

    /// <summary>
    /// Verifies that the id of a finished session is returned as last finished session
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetLastFinishedSessionReturnsSessionId()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var lastFinishedSession = await controller.GetLastFinishedSession().ConfigureAwait(false);

            Assert.IsGreaterThanOrEqualTo(ControllerTestData.SessionId, lastFinishedSession, "The last finished session should not be older than the finished test session!");
        }
    }

    /// <summary>
    /// Verifies that a single session is returned with its joined track, game version and weather data
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionReturnsSessionData()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSession(ControllerTestData.SessionId).ConfigureAwait(false);

            var session = (result as OkObjectResult)?.Value as SessionViewData;

            Assert.IsNotNull(session, "The test session should be returned!");
            Assert.AreEqual(ControllerTestData.SessionId, session.SessionDbId, "The returned session should be the requested session!");
            Assert.AreEqual(ControllerTestData.TrackName, session.Track, "The track name of the test session should be returned!");
            Assert.AreEqual(WeatherCondition.Clear, session.Weather, "The weather of the session attributes should be returned!");
        }
    }

    /// <summary>
    /// Verifies that a race preceded by a sprint shootout is reported as sprint race
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionReportsRaceAfterSprintShootoutAsSprint()
    {
        // A dedicated game version keeps the sprint detection limited to the sessions of this test
        var gameVersionId = ControllerTestData.AddGameVersion(3771001);

        ControllerTestData.AddSession(SessionType.SprintShootout1, isFinished: true, gameVersionId);

        var raceSessionId = ControllerTestData.AddSession(SessionType.Race, isFinished: true, gameVersionId);

        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSession(raceSessionId).ConfigureAwait(false);

            var session = (result as OkObjectResult)?.Value as SessionViewData;

            Assert.IsNotNull(session, "The race session should be returned!");
            Assert.AreEqual(SessionType.Sprint, session.SessionType, "A race preceded by a sprint shootout should be reported as sprint race!");
        }
    }

    /// <summary>
    /// Verifies that a race without a preceding sprint shootout keeps its session type
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionKeepsRaceWithoutSprintShootout()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3771002);

        var raceSessionId = ControllerTestData.AddSession(SessionType.Race, isFinished: true, gameVersionId);

        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSession(raceSessionId).ConfigureAwait(false);

            var session = (result as OkObjectResult)?.Value as SessionViewData;

            Assert.IsNotNull(session, "The race session should be returned!");
            Assert.AreEqual(SessionType.Race, session.SessionType, "A race without a preceding sprint shootout should stay a race!");
        }
    }

    /// <summary>
    /// Verifies that a missing session id is answered with a not found result
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionWithoutIdReturnsNotFound()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSession(null).ConfigureAwait(false);

            Assert.IsInstanceOfType<NotFoundResult>(result, "A missing session id should be answered with not found!");
        }
    }

    /// <summary>
    /// Verifies that the sessions of a track are returned
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetSessionsOfTrackReturnsSessionsOfTrack()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetSessionsOfTrack(ControllerTestData.TrackId).ConfigureAwait(false);

            var sessions = (result as OkObjectResult)?.Value as List<SessionViewData>;

            Assert.IsNotNull(sessions, "The sessions of the test track should be returned!");
            Assert.Contains(ControllerTestData.SessionId,
                            sessions.Select(s => s.SessionDbId).ToList(),
                            "The test session should be part of the sessions of its track!");
        }
    }

    /// <summary>
    /// Verifies that the fastest lap of a session is returned with its driver
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetFastestLapOfSessionReturnsFastestLap()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetFastestLapOfSession(ControllerTestData.SessionId).ConfigureAwait(false);

            var fastestLap = (result as OkObjectResult)?.Value as FastestLapViewData;

            Assert.IsNotNull(fastestLap, "The fastest lap of the test session should be returned!");
            Assert.AreEqual(ControllerTestData.FastestLapTime, fastestLap.LapTime, "The faster of both laps should be returned!");
            Assert.AreEqual(ControllerTestData.HumanDriverName, fastestLap.DriverName, "The fastest lap should be assigned to the human test driver!");
        }
    }

    /// <summary>
    /// Verifies that all laps of a session are returned ordered by their lap time
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerGetFastestLapsOfSessionReturnsLapsOrderedByLapTime()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.GetFastestLapsOfSession(ControllerTestData.SessionId).ConfigureAwait(false);

            var fastestLaps = (result as OkObjectResult)?.Value as List<FastestLapViewData>;

            Assert.IsNotNull(fastestLaps, "The laps of the test session should be returned!");
            Assert.HasCount(2, fastestLaps, "Both laps of the test session should be returned!");
            Assert.AreEqual(ControllerTestData.FastestLapTime, fastestLaps[0].LapTime, "The fastest lap should be returned first!");
            Assert.AreEqual(ControllerTestData.SlowerLapTime, fastestLaps[1].LapTime, "The slower lap should be returned last!");
        }
    }

    /// <summary>
    /// Verifies that the time table of a session contains its drivers and their final classification
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerLoadSessionTimeTableReturnsDriversAndTimeTable()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.LoadSessionTimeTable(ControllerTestData.SessionId).ConfigureAwait(false);

            var timeTable = (result as OkObjectResult)?.Value as SessionTimeTableViewData;

            Assert.IsNotNull(timeTable, "The time table of the test session should be returned!");
            Assert.HasCount(ControllerTestData.ParticipantCount, timeTable.Drivers, "Every participant of the test session should be listed!");
            Assert.HasCount(ControllerTestData.ParticipantCount, timeTable.TimeTable, "Every final classification of the test session should be listed!");
            Assert.Contains(ControllerTestData.HumanDriverName,
                            timeTable.Drivers.Select(d => d.DriverName).ToList(),
                            "The human test driver should be listed!");
        }
    }

    /// <summary>
    /// Verifies that deleting a session removes the session and its participants, laps and telemetry
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerDeleteSessionRemovesSessionGraph()
    {
        var sessionId = ControllerTestData.AddDeletableSession(out var sessionCode);

        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.DeleteSession(sessionId, sessionCode).ConfigureAwait(false);

            Assert.IsTrue((result as OkObjectResult)?.Value as bool?, "Deleting the session should report success!");
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            Assert.IsNotNull(sessionQuery, "Session query should be resolvable!");

            var isSessionPresent = await sessionQuery.AnyAsync(s => s.Id == sessionId, TestContext.CancellationToken).ConfigureAwait(false);

            Assert.IsFalse(isSessionPresent, "The deleted session must no longer be stored!");

            var participantQuery = dbFactory.GetRepository<ParticipantRepository>()?.GetQuery();

            Assert.IsNotNull(participantQuery, "Participant query should be resolvable!");

            var isParticipantPresent = await participantQuery.AnyAsync(p => p.SessionId == sessionId, TestContext.CancellationToken).ConfigureAwait(false);

            Assert.IsFalse(isParticipantPresent, "The participants of the deleted session must no longer be stored!");
        }
    }

    /// <summary>
    /// Verifies that an unknown session code leaves the session untouched
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task SessionsControllerDeleteSessionWithWrongCodeKeepsSession()
    {
        var controller = CreateController(out var cache);

        using (cache)
        {
            var result = await controller.DeleteSession(ControllerTestData.SessionId, ControllerTestData.SessionCode + 1UL).ConfigureAwait(false);

            Assert.IsFalse((result as OkObjectResult)?.Value as bool?, "A session code that does not match must not delete the session!");
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            Assert.IsNotNull(sessionQuery, "Session query should be resolvable!");

            var isSessionPresent = await sessionQuery.AnyAsync(s => s.Id == ControllerTestData.SessionId, TestContext.CancellationToken).ConfigureAwait(false);

            Assert.IsTrue(isSessionPresent, "The test session should still be stored!");
        }
    }

    #endregion // Methods
}