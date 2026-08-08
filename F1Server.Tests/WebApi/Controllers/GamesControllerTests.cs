using F1Server.WebApi.Controllers;

using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the asynchronous action of the <see cref="GamesController"/> returns the game
/// versions together with the number of their finished sessions
/// </summary>
[TestClass]
public class GamesControllerTests
{
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

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that the game version of the test session is returned with its formatted version code
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task GamesControllerGetReturnsGameVersionWithVersionCode()
    {
        var controller = new GamesController(NullLogger<GamesController>.Instance);

        var games = await controller.Get().ConfigureAwait(false);

        Assert.IsNotNull(games, "The game versions should be returned!");

        var testGame = games.ToList()
                            .Find(g => g.Id == ControllerTestData.GameVersionId);

        Assert.IsNotNull(testGame, "The game version of the test session should be returned!");
        Assert.AreEqual(ControllerTestData.GameVersionName, testGame.GameVersion, "The name of the game version should be returned!");
        Assert.AreEqual("1.7", testGame.GameVersionCode, "The version code should be built from the major and minor version!");
        Assert.AreEqual("-", testGame.LastUsed, "A game version that was never used should report a dash!");
    }

    /// <summary>
    /// Verifies that the number of finished sessions is counted per game version
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task GamesControllerGetCountsFinishedSessionsPerGameVersion()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3791);

        ControllerTestData.AddSession(Core.Enumerations.SessionType.Race, isFinished: true, gameVersionId);
        ControllerTestData.AddSession(Core.Enumerations.SessionType.Race, isFinished: false, gameVersionId);

        var controller = new GamesController(NullLogger<GamesController>.Instance);

        var games = await controller.Get().ConfigureAwait(false);

        Assert.IsNotNull(games, "The game versions should be returned!");

        var testGame = games.ToList()
                            .Find(g => g.Id == gameVersionId);

        Assert.IsNotNull(testGame, "The created game version should be returned!");
        Assert.AreEqual(1, testGame.Sessions, "Only the finished session of the game version should be counted!");
    }

    /// <summary>
    /// Verifies that a game version without finished sessions is returned with a session count of zero
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task GamesControllerGetReportsZeroSessionsForUnusedGameVersion()
    {
        var gameVersionId = ControllerTestData.AddGameVersion(3792);

        var controller = new GamesController(NullLogger<GamesController>.Instance);

        var games = await controller.Get().ConfigureAwait(false);

        Assert.IsNotNull(games, "The game versions should be returned!");

        var testGame = games.ToList()
                            .Find(g => g.Id == gameVersionId);

        Assert.IsNotNull(testGame, "The created game version should be returned!");
        Assert.AreEqual(0, testGame.Sessions, "A game version without finished sessions should report no sessions!");
    }

    #endregion // Methods
}