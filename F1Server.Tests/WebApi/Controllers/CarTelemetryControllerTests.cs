using F1Server.Data.ViewData;
using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the asynchronous actions of the <see cref="CarTelemetryController"/> read the
/// expected telemetry data
/// </summary>
[TestClass]
public class CarTelemetryControllerTests
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
    private static CarTelemetryController CreateController()
    {
        return new CarTelemetryController(NullLogger<CarTelemetryController>.Instance);
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that telemetry of a human driver is reported for the test session
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task CarTelemetryControllerHasUserTelemetryDataReturnsTrueForSessionWithHumanTelemetry()
    {
        var controller = CreateController();

        var hasUserTelemetryData = await controller.HasUserTelemetryData(ControllerTestData.SessionId).ConfigureAwait(false);

        Assert.IsTrue(hasUserTelemetryData, "The test session should report telemetry of its human driver!");
    }

    /// <summary>
    /// Verifies that an unknown session does not report telemetry of a human driver
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task CarTelemetryControllerHasUserTelemetryDataReturnsFalseForUnknownSession()
    {
        var controller = CreateController();

        var hasUserTelemetryData = await controller.HasUserTelemetryData(771999999L).ConfigureAwait(false);

        Assert.IsFalse(hasUserTelemetryData, "An unknown session must not report telemetry!");
    }

    /// <summary>
    /// Verifies that the telemetry rows of a lap are returned
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task CarTelemetryControllerTelemetryOfLapReturnsTelemetryOfLap()
    {
        var controller = CreateController();

        var result = await controller.TelemetryOfLap(ControllerTestData.FastestLapId).ConfigureAwait(false);

        var telemetryData = (result as OkObjectResult)?.Value as List<TelemetryViewData>;

        Assert.IsNotNull(telemetryData, "The telemetry of the test lap should be returned!");
        Assert.HasCount(1, telemetryData, "The single telemetry row of the test lap should be returned!");
        Assert.AreEqual(271, telemetryData[0].Speed, "The speed of the telemetry row should be returned!");
    }

    /// <summary>
    /// Verifies that an unknown lap returns an empty telemetry list
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task CarTelemetryControllerTelemetryOfUnknownLapReturnsEmptyList()
    {
        var controller = CreateController();

        var result = await controller.TelemetryOfLap(771999999L).ConfigureAwait(false);

        var telemetryData = (result as OkObjectResult)?.Value as List<TelemetryViewData>;

        Assert.IsNotNull(telemetryData, "An unknown lap should still return a list!");
        Assert.IsEmpty(telemetryData, "An unknown lap must not return telemetry rows!");
    }

    /// <summary>
    /// Verifies that the telemetry of the fastest lap of a participant is returned
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task CarTelemetryControllerTelemetryOfParticipantFastestLapReturnsTelemetry()
    {
        var controller = CreateController();

        var result = await controller.TelemetryOfParticipantFastestLap(ControllerTestData.HumanParticipantId).ConfigureAwait(false);

        var telemetryData = (result as OkObjectResult)?.Value as List<TelemetryViewData>;

        Assert.IsNotNull(telemetryData, "The telemetry of the fastest lap of the participant should be returned!");
        Assert.HasCount(1, telemetryData, "The single telemetry row of the fastest lap should be returned!");
        Assert.AreEqual(7, telemetryData[0].Gear, "The gear of the telemetry row should be returned!");
    }

    /// <summary>
    /// Verifies that a participant without laps returns an empty telemetry list
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task CarTelemetryControllerTelemetryOfUnknownParticipantReturnsEmptyList()
    {
        var controller = CreateController();

        var result = await controller.TelemetryOfParticipantFastestLap(771999999L).ConfigureAwait(false);

        var telemetryData = (result as OkObjectResult)?.Value as List<TelemetryViewData>;

        Assert.IsNotNull(telemetryData, "An unknown participant should still return a list!");
        Assert.IsEmpty(telemetryData, "An unknown participant must not return telemetry rows!");
    }

    #endregion // Methods
}