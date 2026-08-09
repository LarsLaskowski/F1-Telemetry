using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Service.Tracks;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Tracks controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TracksController : ControllerBase
{
    #region Fields

    private readonly ILogger<TracksController> _logger;
    private readonly TrackService _trackService;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="trackService">Track business logic</param>
    public TracksController(ILogger<TracksController> logger, TrackService trackService)
    {
        _logger = logger;
        _trackService = trackService;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Get tracks
    /// </summary>
    /// <returns>Tracks</returns>
    [HttpGet]
    public async Task<IEnumerable<TrackViewData>?> Get()
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(Get));

        _logger?.LoadingTracks();

        var tracks = await _trackService.GetTracksAsync().ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.TracksLoaded(tracks?.Count);

        return tracks;
    }

    /// <summary>
    /// Load track data
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>Track data</returns>
    [Route("Track/{trackId}")]
    [HttpGet]
    public async Task<IActionResult> GetTrack(long trackId)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetTrack));

        _logger?.LoadingTrack(trackId);

        var trackData = await _trackService.GetTrackAsync(trackId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.TrackLoaded(trackData != null);

        return Ok(trackData);
    }

    #endregion // Controller methods
}