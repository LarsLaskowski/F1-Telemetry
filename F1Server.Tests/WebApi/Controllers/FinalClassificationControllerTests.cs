using F1Server.Data.ViewData;
using F1Server.Service.FinalClassifications;
using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the asynchronous action of the <see cref="FinalClassificationController"/>
/// returns the final classification of a session
/// </summary>
[TestClass]
public class FinalClassificationControllerTests
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

    /// <summary>
    /// Creates a controller instance
    /// </summary>
    /// <returns>Controller</returns>
    private static FinalClassificationController CreateController()
    {
        return new FinalClassificationController(NullLogger<FinalClassificationController>.Instance, new FinalClassificationService());
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that the final classification of a session is returned ordered by the finish position
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FinalClassificationControllerGetFromSessionReturnsClassificationOrderedByFinishPosition()
    {
        var controller = CreateController();

        var result = await controller.GetFromSession(ControllerTestData.SessionId).ConfigureAwait(false);

        var finalClassifications = (result as OkObjectResult)?.Value as List<FinalClassificationViewData>;

        Assert.IsNotNull(finalClassifications, "The final classification of the test session should be returned!");
        Assert.HasCount(ControllerTestData.ParticipantCount, finalClassifications, "Every participant of the test session should be classified!");
        Assert.AreEqual(ControllerTestData.HumanFinishPosition, finalClassifications[0].FinishPosition, "The winner should be returned first!");
        Assert.AreEqual(ControllerTestData.HumanDriverName, finalClassifications[0].DriverName, "The winner should be the human test driver!");
        Assert.AreEqual(ControllerTestData.TeamName, finalClassifications[0].TeamName, "The team of the winner should be returned!");
    }

    /// <summary>
    /// Verifies that the fastest lap of the session is marked in the final classification
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FinalClassificationControllerGetFromSessionMarksFastestSessionLap()
    {
        var controller = CreateController();

        var result = await controller.GetFromSession(ControllerTestData.SessionId).ConfigureAwait(false);

        var finalClassifications = (result as OkObjectResult)?.Value as List<FinalClassificationViewData>;

        Assert.IsNotNull(finalClassifications, "The final classification of the test session should be returned!");

        var fastestLapDriver = finalClassifications.Find(f => f.IsFastestSessionLapTime);

        Assert.IsNotNull(fastestLapDriver, "The driver of the fastest lap of the session should be marked!");
        Assert.AreEqual(ControllerTestData.FastestLapTime, fastestLapDriver.FastestLapTimeRaw, "The fastest lap of the session should be the faster of both laps!");
        Assert.AreEqual(ControllerTestData.HumanDriverName, fastestLapDriver.DriverName, "The fastest lap of the session should belong to the human test driver!");
    }

    /// <summary>
    /// Verifies that the race time difference to the leader is returned for a driver on the same lap
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FinalClassificationControllerGetFromSessionReturnsRaceTimeDifferenceToLeader()
    {
        var sessionId = ControllerTestData.AddRaceSession();

        ControllerTestData.AddClassifiedParticipant(sessionId, "Time Difference Leader", 1, 10, 90.0, ControllerTestData.FastestLapTime);
        ControllerTestData.AddClassifiedParticipant(sessionId, "Time Difference Follower", 2, 10, 91.5, ControllerTestData.SlowerLapTime);

        var controller = CreateController();

        var result = await controller.GetFromSession(sessionId).ConfigureAwait(false);

        var finalClassifications = (result as OkObjectResult)?.Value as List<FinalClassificationViewData>;

        Assert.IsNotNull(finalClassifications, "The final classification of the session should be returned!");
        Assert.HasCount(2, finalClassifications, "Both classified participants should be returned!");
        Assert.AreEqual("0.000", finalClassifications[0].RaceTimeDifference, "The leader should not be reported with a race time difference!");
        Assert.AreEqual("+1.500", finalClassifications[1].RaceTimeDifference, "The race time difference of the follower to the leader should be returned!");
    }

    /// <summary>
    /// Verifies that a driver with fewer driven laps than the leader is reported with the lap difference
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FinalClassificationControllerGetFromSessionReportsLappedDriverWithLapDifference()
    {
        var sessionId = ControllerTestData.AddRaceSession();

        ControllerTestData.AddClassifiedParticipant(sessionId, "Lapped Leader", 1, 10, 90.0, ControllerTestData.FastestLapTime);
        ControllerTestData.AddClassifiedParticipant(sessionId, "Lapped Follower", 2, 8, 95.0, ControllerTestData.SlowerLapTime);

        var controller = CreateController();

        var result = await controller.GetFromSession(sessionId).ConfigureAwait(false);

        var finalClassifications = (result as OkObjectResult)?.Value as List<FinalClassificationViewData>;

        Assert.IsNotNull(finalClassifications, "The final classification of the session should be returned!");
        Assert.HasCount(2, finalClassifications, "Both classified participants should be returned!");
        Assert.AreEqual("+ 2 lap(s)", finalClassifications[1].RaceTimeDifference, "A driver with fewer driven laps should be reported with the lap difference!");
    }

    /// <summary>
    /// Verifies that the difference to the fastest lap of the session is returned for the slower driver
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FinalClassificationControllerGetFromSessionReturnsFastestLapTimeDifference()
    {
        var sessionId = ControllerTestData.AddRaceSession();

        ControllerTestData.AddClassifiedParticipant(sessionId, "Fastest Lap Leader", 1, 10, 90.0, ControllerTestData.FastestLapTime);
        ControllerTestData.AddClassifiedParticipant(sessionId, "Fastest Lap Follower", 2, 10, 91.5, ControllerTestData.SlowerLapTime);

        var controller = CreateController();

        var result = await controller.GetFromSession(sessionId).ConfigureAwait(false);

        var finalClassifications = (result as OkObjectResult)?.Value as List<FinalClassificationViewData>;

        Assert.IsNotNull(finalClassifications, "The final classification of the session should be returned!");
        Assert.HasCount(2, finalClassifications, "Both classified participants should be returned!");
        Assert.IsTrue(finalClassifications[0].IsFastestSessionLapTime, "The driver of the fastest lap of the session should be marked!");
        Assert.AreEqual(string.Empty, finalClassifications[0].FastestLapTimeDifference, "The driver of the fastest lap of the session should not be reported with a difference!");
        Assert.AreEqual("+2.500", finalClassifications[1].FastestLapTimeDifference, "The difference of the slower driver to the fastest lap of the session should be returned!");
    }

    /// <summary>
    /// Verifies that an unknown session returns an empty final classification
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task FinalClassificationControllerGetFromUnknownSessionReturnsEmptyList()
    {
        var controller = CreateController();

        var result = await controller.GetFromSession(771999999L).ConfigureAwait(false);

        var finalClassifications = (result as OkObjectResult)?.Value as List<FinalClassificationViewData>;

        Assert.IsNotNull(finalClassifications, "An unknown session should still return a list!");
        Assert.IsEmpty(finalClassifications, "An unknown session must not return a final classification!");
    }

    #endregion // Methods
}