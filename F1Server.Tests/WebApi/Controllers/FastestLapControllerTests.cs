using F1Server.Core.Enumerations;
using F1Server.Data.ViewData;
using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the asynchronous actions of the <see cref="FastestLapController"/> read the
/// expected fastest lap data
/// </summary>
[TestClass]
public class FastestLapControllerTests
{
    #region Constants

    /// <summary>
    /// Lap time in milliseconds of the practice session of the tests in this class
    /// </summary>
    private const uint PracticeLapTime = 82500U;

    /// <summary>
    /// Lap time in milliseconds of the qualifying session of the tests in this class
    /// </summary>
    private const uint QualifyingLapTime = 80750U;

    #endregion // Constants

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
    private static FastestLapController CreateController()
    {
        return new FastestLapController(NullLogger<FastestLapController>.Instance);
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that the fastest race lap of the test track is returned with its driver and game version
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FastestLapControllerGetFastestLapsReturnsFastestRaceLapOfTrack()
    {
        var controller = CreateController();

        var result = await controller.GetFastestLaps(ControllerTestData.TrackId).ConfigureAwait(false);

        var fastestLaps = (result as OkObjectResult)?.Value as List<FastestLapOfTrackViewData>;

        Assert.IsNotNull(fastestLaps, "The fastest laps of the test track should be returned!");

        var raceLap = fastestLaps.Find(l => l.LapSessionType == FastestLapSessionType.Race);

        Assert.IsNotNull(raceLap, "The fastest race lap of the test track should be returned!");
        Assert.AreEqual(ControllerTestData.FastestLapTime, raceLap.LapTime, "The faster of both race laps should be returned!");
        Assert.AreEqual(ControllerTestData.HumanDriverName, raceLap.DriverName, "The fastest race lap should be assigned to the human test driver!");
        Assert.AreEqual(ControllerTestData.GameVersionName, raceLap.GameVersionName, "The game version of the session of the fastest lap should be returned!");
        Assert.AreEqual(Formula.F1Modern, raceLap.FormulaType, "The formula type of the sessions of the test track should be returned!");
        Assert.AreEqual(ControllerTestData.ReferenceLapTime, raceLap.ReferenceTime, "The reference lap time of the test track should be returned!");
    }

    /// <summary>
    /// Verifies that the fastest lap of a formula two session is reported for its formula type
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FastestLapControllerGetFastestLapsReturnsFormulaTwoLapOfTrack()
    {
        var trackId = ControllerTestData.AddTrack();

        ControllerTestData.AddSessionWithLap(SessionType.Race, Formula.F2, trackId, ControllerTestData.FastestLapTime);

        var controller = CreateController();

        var result = await controller.GetFastestLaps(trackId).ConfigureAwait(false);

        var fastestLaps = (result as OkObjectResult)?.Value as List<FastestLapOfTrackViewData>;

        Assert.IsNotNull(fastestLaps, "The fastest laps of the track should be returned!");

        var raceLap = fastestLaps.Find(l => l.LapSessionType == FastestLapSessionType.Race);

        Assert.IsNotNull(raceLap, "The fastest race lap of the formula two session should be returned!");
        Assert.AreEqual(Formula.F2, raceLap.FormulaType, "The lap should be reported for formula two!");
        Assert.AreEqual(ControllerTestData.FastestLapTime, raceLap.LapTime, "The lap time of the formula two lap should be returned!");
    }

    /// <summary>
    /// Verifies that the fastest practice and qualifying laps of a track are reported for their session type
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FastestLapControllerGetFastestLapsReturnsPracticeAndQualifyingLapsOfTrack()
    {
        var trackId = ControllerTestData.AddTrack();

        ControllerTestData.AddSessionWithLap(SessionType.Practice2, Formula.F1Modern, trackId, PracticeLapTime);
        ControllerTestData.AddSessionWithLap(SessionType.Qualifying3, Formula.F1Modern, trackId, QualifyingLapTime);

        var controller = CreateController();

        var result = await controller.GetFastestLaps(trackId).ConfigureAwait(false);

        var fastestLaps = (result as OkObjectResult)?.Value as List<FastestLapOfTrackViewData>;

        Assert.IsNotNull(fastestLaps, "The fastest laps of the track should be returned!");

        var practiceLap = fastestLaps.Find(l => l.LapSessionType == FastestLapSessionType.Practice);
        var qualifyingLap = fastestLaps.Find(l => l.LapSessionType == FastestLapSessionType.Qualifying);

        Assert.IsNotNull(practiceLap, "The fastest practice lap of the track should be returned!");
        Assert.IsNotNull(qualifyingLap, "The fastest qualifying lap of the track should be returned!");
        Assert.AreEqual(PracticeLapTime, practiceLap.LapTime, "The lap time of the practice session should be returned!");
        Assert.AreEqual(QualifyingLapTime, qualifyingLap.LapTime, "The lap time of the qualifying session should be returned!");
        Assert.IsNull(fastestLaps.Find(l => l.LapSessionType == FastestLapSessionType.Race), "A track without race sessions must not report a fastest race lap!");
    }

    /// <summary>
    /// Verifies that the difference of a fastest lap to the reference lap time of its track is returned
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FastestLapControllerGetFastestLapsReturnsDifferenceToReferenceTime()
    {
        var trackId = ControllerTestData.AddTrack();

        ControllerTestData.AddSessionWithLap(SessionType.Race, Formula.F1Modern, trackId, QualifyingLapTime);

        var controller = CreateController();

        var result = await controller.GetFastestLaps(trackId).ConfigureAwait(false);

        var fastestLaps = (result as OkObjectResult)?.Value as List<FastestLapOfTrackViewData>;

        Assert.IsNotNull(fastestLaps, "The fastest laps of the track should be returned!");

        var raceLap = fastestLaps.Find(l => l.LapSessionType == FastestLapSessionType.Race);

        Assert.IsNotNull(raceLap, "The fastest race lap of the track should be returned!");
        Assert.AreEqual(ControllerTestData.ReferenceLapTime, raceLap.ReferenceTime, "The reference lap time of the track should be returned!");
        Assert.AreEqual(QualifyingLapTime - ControllerTestData.ReferenceLapTime, raceLap.DiffReference, "The difference of the fastest lap to the reference lap time should be returned!");
    }

    /// <summary>
    /// Verifies that a track without sessions does not report fastest laps
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FastestLapControllerGetFastestLapsForUnknownTrackReturnsEmptyList()
    {
        var controller = CreateController();

        var result = await controller.GetFastestLaps(771999999L).ConfigureAwait(false);

        var fastestLaps = (result as OkObjectResult)?.Value as List<FastestLapOfTrackViewData>;

        Assert.IsNotNull(fastestLaps, "An unknown track should still return a list!");
        Assert.IsEmpty(fastestLaps, "An unknown track must not report fastest laps!");
    }

    /// <summary>
    /// Verifies that a missing track id does not report fastest laps
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FastestLapControllerGetFastestLapsWithoutTrackIdReturnsEmptyList()
    {
        var controller = CreateController();

        var result = await controller.GetFastestLaps(null).ConfigureAwait(false);

        var fastestLaps = (result as OkObjectResult)?.Value as List<FastestLapOfTrackViewData>;

        Assert.IsNotNull(fastestLaps, "A missing track id should still return a list!");
        Assert.IsEmpty(fastestLaps, "A missing track id must not report fastest laps!");
    }

    /// <summary>
    /// Verifies that the cached fastest lap data of a session is returned
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FastestLapControllerGetFastestLapDataOfSessionReturnsSessionData()
    {
        var controller = CreateController();

        var result = await controller.GetFastestLapDataOfSession(ControllerTestData.SessionId).ConfigureAwait(false);

        var fastestLapData = (result as OkObjectResult)?.Value as FastestLapSessionViewData;

        Assert.IsNotNull(fastestLapData, "The fastest lap data of the test session should be returned!");
        Assert.AreEqual(ControllerTestData.SessionId, fastestLapData.SessionId, "The returned data should belong to the requested session!");
        Assert.AreEqual(ControllerTestData.HumanDriverName, fastestLapData.FastestLapDriver, "The fastest lap of the session should be assigned to the human test driver!");
    }

    #endregion // Methods
}