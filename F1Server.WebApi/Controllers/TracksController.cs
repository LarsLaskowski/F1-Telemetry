using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    public TracksController(ILogger<TracksController> logger)
    {
        _logger = logger;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Get tracks
    /// </summary>
    /// <returns>Tracks</returns>
    [HttpGet]
    public async Task<IEnumerable<TrackViewData>?> Get()
    {
        List<TrackViewData>? tracks = [];

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(Get));

        _logger?.LoadingTracks();

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var uniqueSessionTracks = sessionQuery == null
                                          ? null
                                          : await sessionQuery.Select(s => s.TrackId)
                                                              .Distinct()
                                                              .ToListAsync()
                                                              .ConfigureAwait(false);

            var trackQuery = dbFactory.GetRepository<TrackRepository>()?.GetQuery();

            if (trackQuery != null)
            {
                tracks = await trackQuery.OrderBy(t => t.Id)
                                         .Select(obj => new TrackViewData
                                                        {
                                                            TrackId = obj.Id,
                                                            TrackNumber = obj.TrackNumber,
                                                            TrackName = obj.Name,
                                                            HasSession = uniqueSessionTracks != null && uniqueSessionTracks.Contains(obj.Id),
                                                            Sessions = sessionQuery != null ? sessionQuery.Count(s => s.TrackId == obj.Id) : 0,
                                                            ReferenceLapTime = TimeSpan.FromMilliseconds(obj.LapReferenceTime, 0).ToString(@"mm\:ss\.fff")
                                                        })
                                         .ToListAsync()
                                         .ConfigureAwait(false);
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

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
        TrackViewData? trackData = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetTrack));

        _logger?.LoadingTrack(trackId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var trackQuery = dbFactory.GetRepository<TrackRepository>()?.GetQuery();

            var track = trackQuery == null
                            ? null
                            : await trackQuery.FirstOrDefaultAsync(t => t.Id == trackId)
                                              .ConfigureAwait(false);

            if (track != null)
            {
                trackData = new TrackViewData
                            {
                                TrackId = track.Id,
                                TrackName = track.Name,
                                TrackNumber = track.TrackNumber
                            };
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.TrackLoaded(trackData != null);

        return Ok(trackData);
    }

    #endregion // Methods
}