using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data;
using F1Server.Tests.Processors;
using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying the server feature checks of the <see cref="ServerHealthController"/>
/// </summary>
[TestClass]
public class ServerHealthControllerTests
{
    #region Static methods

    /// <summary>
    /// Creates application data of a receiving server with a ready telemetry writer
    /// </summary>
    /// <returns>Application data</returns>
    private static F1ServerApplicationData CreateReceivingAppData()
    {
        return new F1ServerApplicationData
               {
                   IsReceiving = true,
                   TelemetryWriter = new RecordingTelemetryWriter()
               };
    }

    /// <summary>
    /// Creates a server health controller
    /// </summary>
    /// <param name="appData">Application data</param>
    /// <returns>Server health controller</returns>
    private static ServerHealthController CreateController(F1ServerApplicationData appData)
    {
        return new ServerHealthController(appData, NullLogger<ServerHealthController>.Instance);
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that a receiving server without configured observability features reports a healthy server
    /// </summary>
    [TestMethod]
    public void ServerHealthControllerGetReceivingServerReturnsOk()
    {
        var result = CreateController(CreateReceivingAppData()).Get();

        Assert.IsInstanceOfType<OkResult>(result, "A receiving server with a ready telemetry writer should report a healthy server!");
    }

    /// <summary>
    /// Verifies that a server that is not receiving data reports an unhealthy server
    /// </summary>
    [TestMethod]
    public void ServerHealthControllerGetNotReceivingServerReturnsBadRequest()
    {
        var appData = CreateReceivingAppData();

        appData.IsReceiving = false;

        var result = CreateController(appData).Get();

        Assert.IsInstanceOfType<BadRequestResult>(result, "A server that is not receiving data should report an unhealthy server!");
    }

    /// <summary>
    /// Verifies that a missing telemetry writer reports an unhealthy server
    /// </summary>
    [TestMethod]
    public void ServerHealthControllerGetWithoutTelemetryWriterReturnsBadRequest()
    {
        var appData = CreateReceivingAppData();

        appData.TelemetryWriter = null;

        var result = CreateController(appData).Get();

        Assert.IsInstanceOfType<BadRequestResult>(result, "A missing telemetry writer should report an unhealthy server!");
    }

    /// <summary>
    /// Verifies that configured but uninitialized metrics report an unhealthy server
    /// </summary>
    [TestMethod]
    public void ServerHealthControllerGetConfiguredMetricsWithoutInstanceReturnsBadRequest()
    {
        var appData = CreateReceivingAppData();

        appData.IsMetricsConfigured = true;

        var result = CreateController(appData).Get();

        Assert.IsInstanceOfType<BadRequestResult>(result, "Configured but uninitialized metrics should report an unhealthy server!");
    }

    /// <summary>
    /// Verifies that configured logging reports a healthy server only with a logger factory
    /// </summary>
    /// <param name="hasLoggerFactory">Is a logger factory available?</param>
    /// <param name="expectedHealthy">Expected health state</param>
    [TestMethod]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void ServerHealthControllerGetConfiguredLoggingRequiresLoggerFactory(bool hasLoggerFactory, bool expectedHealthy)
    {
        var appData = CreateReceivingAppData();

        appData.IsLoggingConfigured = true;
        appData.LoggerFactory = hasLoggerFactory ? NullLoggerFactory.Instance : null;

        var result = CreateController(appData).Get();

        Assert.AreEqual(expectedHealthy, result is OkResult, "The health state with configured logging is not as expected!");
    }

    /// <summary>
    /// Verifies that configured tracing with an attached activity listener reports a healthy server
    /// </summary>
    [TestMethod]
    public void ServerHealthControllerGetConfiguredTracingWithListenerReturnsOk()
    {
        var appData = CreateReceivingAppData();

        appData.IsTracingConfigured = true;

        using var listener = new ActivityListener
                             {
                                 ShouldListenTo = source => source.Name == AppActivity.SrvSource.Name,
                                 Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData
                             };

        ActivitySource.AddActivityListener(listener);

        var result = CreateController(appData).Get();

        Assert.IsInstanceOfType<OkResult>(result, "Configured tracing with an attached listener should report a healthy server!");
    }

    #endregion // Methods
}