using F1Server.Data;
using F1Server.WebApi.Core;
using F1Server.WebApi.Hubs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Live session controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LiveSessionController : ControllerBase
{
    #region Fields

    private readonly F1ServerApplicationData _appData;
    private readonly TimerManager _timerManager;
    private readonly IHubContext<LiveSessionHub> _hub;
    private readonly ILogger<LiveSessionController> _logger;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appData">Application data</param>
    /// <param name="timerManager">Timer manager</param>
    /// <param name="hub">Live session SignalR hub</param>
    /// <param name="logger">Logging interface</param>
    public LiveSessionController(F1ServerApplicationData appData, TimerManager timerManager, IHubContext<LiveSessionHub> hub, ILogger<LiveSessionController> logger)
    {
        _appData = appData;
        _timerManager = timerManager;
        _hub = hub;
        _logger = logger;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Get action
    /// </summary>
    /// <returns>Message</returns>
    [HttpGet]
    public IActionResult Get()
    {
        _logger?.LogInformation("Live session controller...");

        if (_timerManager?.IsTimerStarted == false)
        {
            bool? lastIsLiveSession = null;
            long? lastLiveSessionId = null;

            _timerManager.PrepareTimer(() =>
                                       {
                                           var isLiveSession = _appData?.IsLiveSession == true;
                                           var liveSessionId = _appData?.LiveSessionId;

                                           if (isLiveSession != lastIsLiveSession || liveSessionId != lastLiveSessionId)
                                           {
                                               lastIsLiveSession = isLiveSession;
                                               lastLiveSessionId = liveSessionId;

                                               _hub.Clients.All.SendAsync("IsLiveSession", isLiveSession, liveSessionId);
                                           }
                                       });
        }

        return Ok();
    }

    #endregion // Methods
}