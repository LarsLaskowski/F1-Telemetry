using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the asynchronous actions of the <see cref="ChampionshipController"/> read and
/// write the expected championship data
/// </summary>
[TestClass]
public class ChampionshipControllerTests
{
    #region Constants

    /// <summary>
    /// Number of tracks a new championship is created with
    /// </summary>
    private const int ChampionshipTrackCount = 6;

    #endregion // Constants

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
    /// Creates a controller instance
    /// </summary>
    /// <returns>Controller</returns>
    private static ChampionshipController CreateController()
    {
        return new ChampionshipController(NullLogger<TracksController>.Instance);
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that a created championship is returned with the tracks and the points of its human driver
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerGetReturnsChampionshipWithTracks()
    {
        var championshipId = ControllerTestData.AddChampionship(ControllerTestData.GameVersionId, ControllerTestData.SessionId);

        var controller = CreateController();

        var championships = await controller.Get().ConfigureAwait(false);

        var championship = championships.ToList()
                                        .Find(c => c.ChampionshipId == championshipId);

        Assert.IsNotNull(championship, "The created championship should be returned!");
        Assert.AreEqual(ControllerTestData.GameVersionName, championship.GameVersionName, "The game version name of the championship should be returned!");
        Assert.IsNotNull(championship.Tracks, "The tracks of the championship should be returned!");
        Assert.HasCount(1, championship.Tracks, "The single track of the championship should be returned!");

        // The human driver finished the qualifying session first, so the grid position is read from its final classification
        Assert.AreEqual(ControllerTestData.HumanFinishPosition,
                        championship.Tracks[0].SprintQualifyingPosition,
                        "The finish position of the human driver of the qualifying session should be returned!");
    }

    /// <summary>
    /// Verifies that a championship is created with the requested tracks
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerCreateChampionshipStoresChampionshipWithTracks()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3781);

        var trackNumbers = ControllerTestData.AddTracks(ChampionshipTrackCount);

        var controller = CreateController();

        var result = await controller.CreateChampionship(new ChampionshipCreateData
                                                         {
                                                             GameVersionId = gameVersionId,
                                                             Mode = (int)ChampionshipMode.Career,
                                                             Tracks = trackNumbers
                                                         })
                                     .ConfigureAwait(false);

        Assert.IsTrue((result as OkObjectResult)?.Value as bool?, "Creating the championship should report success!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var championshipQuery = dbFactory.GetRepository<ChampionshipRepository>()?.GetQuery();

            Assert.IsNotNull(championshipQuery, "Championship query should be resolvable!");

            var championship = await championshipQuery.FirstOrDefaultAsync(c => c.GameVersionId == gameVersionId, TestContext.CancellationToken).ConfigureAwait(false);

            Assert.IsNotNull(championship, "The created championship should be stored!");
            Assert.AreEqual(1, championship.Number, "The first championship of a game version should be season one!");

            var championshipTrackQuery = dbFactory.GetRepository<ChampionshipTrackRepository>()?.GetQuery();

            Assert.IsNotNull(championshipTrackQuery, "Championship track query should be resolvable!");

            var trackCount = await championshipTrackQuery.CountAsync(t => t.ChampionshipId == championship.Id, TestContext.CancellationToken).ConfigureAwait(false);

            Assert.AreEqual(ChampionshipTrackCount, trackCount, "Every requested track should be added to the championship!");
        }
    }

    /// <summary>
    /// Verifies that a second season of the same mode and game version gets its own tracks instead of extending the
    /// tracks of the first season
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerCreateChampionshipStoresTracksOfSecondSeason()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3788);

        var firstSeasonTracks = ControllerTestData.AddTracks(ChampionshipTrackCount);
        var secondSeasonTracks = ControllerTestData.AddTracks(ChampionshipTrackCount);

        var controller = CreateController();

        var firstResult = await controller.CreateChampionship(new ChampionshipCreateData
                                                              {
                                                                  GameVersionId = gameVersionId,
                                                                  Mode = (int)ChampionshipMode.Career,
                                                                  Tracks = firstSeasonTracks
                                                              })
                                          .ConfigureAwait(false);

        Assert.IsTrue((firstResult as OkObjectResult)?.Value as bool?, "Creating the first season should report success!");

        var secondResult = await controller.CreateChampionship(new ChampionshipCreateData
                                                               {
                                                                   GameVersionId = gameVersionId,
                                                                   Mode = (int)ChampionshipMode.Career,
                                                                   Tracks = secondSeasonTracks
                                                               })
                                           .ConfigureAwait(false);

        Assert.IsTrue((secondResult as OkObjectResult)?.Value as bool?, "Creating the second season should report success!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var championshipQuery = dbFactory.GetRepository<ChampionshipRepository>()?.GetQuery();

            Assert.IsNotNull(championshipQuery, "Championship query should be resolvable!");

            var championships = await championshipQuery.Where(c => c.GameVersionId == gameVersionId)
                                                       .OrderBy(c => c.Number)
                                                       .ToListAsync(TestContext.CancellationToken)
                                                       .ConfigureAwait(false);

            Assert.HasCount(2, championships, "Both seasons of the game version should be stored!");
            Assert.AreEqual(2, championships[1].Number, "The second championship of a game version should be season two!");

            var trackQuery = dbFactory.GetRepository<TrackRepository>()?.GetQuery();

            Assert.IsNotNull(trackQuery, "Track query should be resolvable!");

            // Both sides are ordered by the track id, so the sequences stay comparable regardless of the order the
            // database returns the rows in
            var expectedFirstSeasonTrackIds = await trackQuery.Where(t => firstSeasonTracks.Contains(t.TrackNumber))
                                                              .OrderBy(t => t.Id)
                                                              .Select(t => t.Id)
                                                              .ToListAsync(TestContext.CancellationToken)
                                                              .ConfigureAwait(false);

            var expectedSecondSeasonTrackIds = await trackQuery.Where(t => secondSeasonTracks.Contains(t.TrackNumber))
                                                               .OrderBy(t => t.Id)
                                                               .Select(t => t.Id)
                                                               .ToListAsync(TestContext.CancellationToken)
                                                               .ConfigureAwait(false);

            Assert.HasCount(ChampionshipTrackCount, expectedFirstSeasonTrackIds, "Every requested track of the first season should be stored!");
            Assert.HasCount(ChampionshipTrackCount, expectedSecondSeasonTrackIds, "Every requested track of the second season should be stored!");

            var championshipTrackQuery = dbFactory.GetRepository<ChampionshipTrackRepository>()?.GetQuery();

            Assert.IsNotNull(championshipTrackQuery, "Championship track query should be resolvable!");

            var firstSeasonTrackIds = await championshipTrackQuery.Where(t => t.ChampionshipId == championships[0].Id)
                                                                  .OrderBy(t => t.TrackId)
                                                                  .Select(t => t.TrackId)
                                                                  .ToListAsync(TestContext.CancellationToken)
                                                                  .ConfigureAwait(false);

            var secondSeasonTrackIds = await championshipTrackQuery.Where(t => t.ChampionshipId == championships[1].Id)
                                                                   .OrderBy(t => t.TrackId)
                                                                   .Select(t => t.TrackId)
                                                                   .ToListAsync(TestContext.CancellationToken)
                                                                   .ConfigureAwait(false);

            Assert.AreSequenceEqual(expectedFirstSeasonTrackIds,
                                    firstSeasonTrackIds,
                                    "The first season should keep exactly the tracks it was created with!");

            Assert.AreSequenceEqual(expectedSecondSeasonTrackIds,
                                    secondSeasonTrackIds,
                                    "The second season should carry exactly the tracks it was created with!");
        }
    }

    /// <summary>
    /// Verifies that championship data without enough tracks is rejected
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerCreateChampionshipWithTooFewTracksReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.CreateChampionship(new ChampionshipCreateData
                                                         {
                                                             GameVersionId = ControllerTestData.GameVersionId,
                                                             Mode = (int)ChampionshipMode.Career,
                                                             Tracks = [1, 2, 3]
                                                         })
                                     .ConfigureAwait(false);

        Assert.IsInstanceOfType<BadRequestObjectResult>(result, "A championship with less than six tracks should be rejected!");
    }

    /// <summary>
    /// Verifies that the id of the running championship of the session is returned
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerIsActiveChampionshipReturnsChampionshipId()
    {
        // A dedicated game version keeps the championship of this test the only one of its game version
        var gameVersionId = ControllerTestData.AddGameVersion(3782);

        var sessionId = ControllerTestData.AddSession(SessionType.Qualifying3, isFinished: true, gameVersionId);

        var championshipId = ControllerTestData.AddChampionship(gameVersionId);

        var controller = CreateController();

        var activeChampionship = await controller.IsActiveChampionship(sessionId).ConfigureAwait(false);

        Assert.AreEqual(championshipId, activeChampionship, "The running championship of the game version of the session should be reported!");
    }

    /// <summary>
    /// Verifies that a session type that cannot be part of a championship is not reported as active
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerIsActiveChampionshipForUnsupportedSessionTypeReturnsZero()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3783);

        var sessionId = ControllerTestData.AddSession(SessionType.Practice1, isFinished: true, gameVersionId);

        ControllerTestData.AddChampionship(gameVersionId);

        var controller = CreateController();

        var activeChampionship = await controller.IsActiveChampionship(sessionId).ConfigureAwait(false);

        Assert.AreEqual(0L, activeChampionship, "A practice session must not be reported as part of a championship!");
    }

    /// <summary>
    /// Verifies that a session already assigned to a championship track is reported as active
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerIsSessionInChampionshipActiveReturnsTrueForAssignedSession()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3784);

        var sessionId = ControllerTestData.AddSession(SessionType.Qualifying3, isFinished: true, gameVersionId);

        var championshipId = ControllerTestData.AddChampionship(gameVersionId, sessionId);

        var controller = CreateController();

        var isActive = await controller.IsSessionInChampionshipActive(championshipId, sessionId).ConfigureAwait(false);

        Assert.IsTrue(isActive, "A session assigned as qualifying session should be reported as active!");
    }

    /// <summary>
    /// Verifies that a session that is not assigned to the championship track is not reported as active
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerIsSessionInChampionshipActiveReturnsFalseForUnassignedSession()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3785);

        var sessionId = ControllerTestData.AddSession(SessionType.Qualifying3, isFinished: true, gameVersionId);

        var championshipId = ControllerTestData.AddChampionship(gameVersionId);

        var controller = CreateController();

        var isActive = await controller.IsSessionInChampionshipActive(championshipId, sessionId).ConfigureAwait(false);

        Assert.IsFalse(isActive, "A session that is not assigned to the championship track must not be reported as active!");
    }

    /// <summary>
    /// Verifies that a qualifying session is stored as qualifying session of the championship track
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerAddToChampionshipAssignsQualifyingSession()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3786);

        var sessionId = ControllerTestData.AddSession(SessionType.Qualifying3, isFinished: true, gameVersionId);

        var championshipId = ControllerTestData.AddChampionship(gameVersionId);

        var controller = CreateController();

        var result = await controller.AddToChampionship(sessionId).ConfigureAwait(false);

        Assert.IsInstanceOfType<OkResult>(result, "Adding the session to the championship should succeed!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var championshipTrackQuery = dbFactory.GetRepository<ChampionshipTrackRepository>()?.GetQuery();

            Assert.IsNotNull(championshipTrackQuery, "Championship track query should be resolvable!");

            var isAssigned = await championshipTrackQuery.AnyAsync(t => t.ChampionshipId == championshipId
                                                                        && t.QualifyingSessionId == sessionId,
                                                                   TestContext.CancellationToken)
                                                         .ConfigureAwait(false);

            Assert.IsTrue(isAssigned, "The session should be stored as qualifying session of the championship track!");
        }
    }

    /// <summary>
    /// Verifies that a race preceded by a sprint shootout is stored as sprint session of the championship track
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerAddToChampionshipAssignsSprintSession()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3787);

        ControllerTestData.AddSession(SessionType.SprintShootout1, isFinished: true, gameVersionId);

        var raceSessionId = ControllerTestData.AddSession(SessionType.Race, isFinished: true, gameVersionId);

        var championshipId = ControllerTestData.AddChampionship(gameVersionId);

        var controller = CreateController();

        var result = await controller.AddToChampionship(raceSessionId).ConfigureAwait(false);

        Assert.IsInstanceOfType<OkResult>(result, "Adding the sprint race to the championship should succeed!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var championshipTrackQuery = dbFactory.GetRepository<ChampionshipTrackRepository>()?.GetQuery();

            Assert.IsNotNull(championshipTrackQuery, "Championship track query should be resolvable!");

            var championshipTrack = await championshipTrackQuery.FirstOrDefaultAsync(t => t.ChampionshipId == championshipId, TestContext.CancellationToken).ConfigureAwait(false);

            Assert.IsNotNull(championshipTrack, "The championship track should be stored!");
            Assert.AreEqual(raceSessionId, championshipTrack.SprintSessionId, "A race preceded by a sprint shootout should be stored as sprint session!");
            Assert.IsNull(championshipTrack.RaceSessionId, "A sprint race must not be stored as race session!");
        }
    }

    /// <summary>
    /// Verifies that an invalid session id is rejected before the database is queried
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerAddToChampionshipWithInvalidSessionReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.AddToChampionship(0).ConfigureAwait(false);

        Assert.IsInstanceOfType<BadRequestObjectResult>(result, "A session id of zero should be rejected!");
    }

    /// <summary>
    /// Verifies that the championship points are calculated from the finish positions
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ChampionshipControllerGetCalculatesRacePointsOfFinishPosition()
    {
        var championshipId = ControllerTestData.AddChampionship(ControllerTestData.GameVersionId, ControllerTestData.SessionId);

        var controller = CreateController();

        var championships = await controller.Get().ConfigureAwait(false);

        var championship = championships.ToList()
                                        .Find(c => c.ChampionshipId == championshipId);

        Assert.IsNotNull(championship, "The created championship should be returned!");
        Assert.IsNotNull(championship.Tracks, "The tracks of the championship should be returned!");

        var trackData = championship.Tracks[0];

        Assert.AreEqual(0, trackData.RacePoints, "Without a race session the track should not award race points!");
        Assert.AreEqual(0, trackData.SprintPoints, "Without a sprint session the track should not award sprint points!");
    }

    #endregion // Methods
}