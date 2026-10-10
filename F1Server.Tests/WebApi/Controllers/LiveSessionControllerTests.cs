using F1Server.Data;
using F1Server.WebApi.Controllers;
using F1Server.WebApi.Core;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the <see cref="LiveSessionController"/> starts the live session broadcast
/// </summary>
[TestClass]
public class LiveSessionControllerTests
{
    #region Constants

    /// <summary>
    /// Maximum time in milliseconds a test waits for a broadcast of the timer
    /// </summary>
    private const int BroadcastTimeout = 10000;

    #endregion // Constants

    #region Methods

    /// <summary>
    /// Verifies that the first request starts the timer, which broadcasts the live session state and data
    /// </summary>
    [TestMethod]
    public void LiveSessionControllerGetBroadcastsLiveSession()
    {
        var appData = new F1ServerApplicationData
                      {
                          IsLiveSession = true,
                          LiveSessionId = 460200,
                          LiveSessionData = new LiveSessionData
                                            {
                                                DbId = 460200
                                            }
                      };

        using (var hub = new RecordingHubContext())
        {
            using (var timerManager = new TimerManager())
            {
                var controller = new LiveSessionController(appData, timerManager, hub, NullLogger<LiveSessionController>.Instance);

                var result = controller.Get();

                Assert.IsInstanceOfType<OkResult>(result, "The live session request should be answered with Ok!");
                Assert.IsTrue(timerManager.IsTimerStarted, "The first request should start the broadcast timer!");

                var liveSessionState = hub.WaitForMessage("IsLiveSession", BroadcastTimeout);

                Assert.IsNotNull(liveSessionState, "The live session state should be broadcast!");
                Assert.IsTrue((bool?)liveSessionState[0], "The broadcast should report an active live session!");
                Assert.AreEqual(460200L, liveSessionState[1], "The broadcast should contain the live session id!");
                Assert.IsNotNull(hub.WaitForMessage("LiveSessionDataUpdated", BroadcastTimeout), "The live session data should be broadcast!");
            }
        }
    }

    /// <summary>
    /// Verifies that a request with an already started timer keeps the running broadcast
    /// </summary>
    [TestMethod]
    public void LiveSessionControllerGetWithStartedTimerKeepsTimer()
    {
        using (var hub = new RecordingHubContext())
        {
            using (var timerManager = new TimerManager())
            {
                timerManager.PrepareTimer(() =>
                                          {
                                          });

                var timerStarted = timerManager.TimerStarted;
                var controller = new LiveSessionController(new F1ServerApplicationData(), timerManager, hub, NullLogger<LiveSessionController>.Instance);

                var result = controller.Get();

                Assert.IsInstanceOfType<OkResult>(result, "The live session request should be answered with Ok!");
                Assert.AreEqual(timerStarted, timerManager.TimerStarted, "An already started timer must not be restarted!");
            }
        }
    }

    #endregion // Methods
}