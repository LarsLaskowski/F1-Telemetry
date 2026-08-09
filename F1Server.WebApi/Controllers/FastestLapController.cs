using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Service.FastestLaps;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Controller receiving fastest laps
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class FastestLapController : ControllerBase
{
    #region Fields

    private readonly ILogger<FastestLapController> _logger;
    private readonly FastestLapService _fastestLapService;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="fastestLapService">Fastest lap business logic</param>
    public FastestLapController(ILogger<FastestLapController> logger, FastestLapService fastestLapService)
    {
        _logger = logger;
        _fastestLapService = fastestLapService;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Reading fastest laps of track
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>Fastest laps</returns>
    [Route("FastestLap/{trackId?}")]
    [HttpGet]
    public async Task<IActionResult> GetFastestLaps(long? trackId)
    {
        var fastestLaps = new List<FastestLapOfTrackViewData>();

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLaps));

        _logger?.LoadingFastestLapsOfTrack(trackId);

        if (trackId.HasValue)
        {
            fastestLaps = await _fastestLapService.GetFastestLapsOfTrackAsync(trackId.Value).ConfigureAwait(false);

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.FastestLapsOfTrackLoaded(fastestLaps.Count);

        return Ok(fastestLaps);
    }

    /// <summary>
    /// Getting fastest lap data of session
    /// </summary>
    /// <param name="sessionId">Session id</param>
    /// <returns>Fastest lap data</returns>
    [Route("FastestLapDataOfSession/{sessionId}")]
    [HttpGet]
    public async Task<IActionResult> GetFastestLapDataOfSession(long sessionId)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapDataOfSession));

        var fastestLapData = await _fastestLapService.GetFastestLapDataOfSessionAsync(sessionId).ConfigureAwait(false);

        return Ok(fastestLapData);
    }

    #endregion // Controller methods
}