using System.Diagnostics;

using F1Server.Core.Interfaces;
using F1Server.Core.Observability;
using F1Server.Data;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Returns runtime session data from a running live session
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LiveSessionDataController : ControllerBase
{
    #region Fields

    private readonly ILogger<LiveSessionDataController> _logger;
    private readonly F1ServerApplicationData _appData;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="appData">Application data</param>
    public LiveSessionDataController(ILogger<LiveSessionDataController> logger, F1ServerApplicationData appData)
    {
        _logger = logger;
        _appData = appData;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Returns live session data
    /// </summary>
    /// <returns>Live session data</returns>
    [HttpGet]
    public ILiveSessionData Get()
    {
        ILiveSessionData liveSessionData = new LiveSessionData();

        using var currentActivity = AppActivity.ApiSource.StartActivity("GetLiveSessionData");

        _logger?.LogInformation("Requesing live session data...");

        if (_appData?.LiveSessionData != null)
        {
            liveSessionData = _appData.LiveSessionData;

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error);

            _logger?.LogWarning("Live session data is not available.");
        }

        return liveSessionData;
    }

    #endregion // Methods
}