using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Service.Sessions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Sessions controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    #region Constants

    private const string CacheKeySessions = "Sessions";

    private const int MaxPageSize = 100;

    #endregion // Constants

    #region Fields

    private readonly ILogger<SessionsController> _logger;
    private readonly IMemoryCache _cache;
    private readonly SessionService _sessionService;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="cache">Cache</param>
    /// <param name="sessionService">Session business logic</param>
    public SessionsController(ILogger<SessionsController> logger, IMemoryCache cache, SessionService sessionService)
    {
        _logger = logger;
        _cache = cache;
        _sessionService = sessionService;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Get sessions
    /// </summary>
    /// <param name="pageIndex">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Sessions</returns>
    [HttpGet]
    public async Task<ActionResult<PageResultData<SessionViewData>>> GetSessions([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 15)
    {
        if (pageIndex < 0 || pageSize <= 0 || pageSize > MaxPageSize)
        {
            return BadRequest($"Invalid paging parameters. pageIndex must be >= 0 and pageSize must be between 1 and {MaxPageSize}.");
        }

        if (_cache.TryGetValue(CacheKeySessions, out List<SessionViewData>? sessions) == false)
        {
            sessions = [];

            using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetSessions));

            _logger?.LoadingSessions();

            try
            {
                sessions = await _sessionService.GetSessionsAsync().ConfigureAwait(false);

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                _logger?.ErrorLoadingSessions(ex);

                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);
            }

            _cache.Set(CacheKeySessions, sessions, TimeSpan.FromMinutes(5));
        }

        var totalCount = sessions?.Count ?? 0;
        var skip = (int)Math.Min((long)pageIndex * pageSize, totalCount);

        var pagedSessions = sessions?.Skip(skip)
                                    .Take(pageSize)
                                    .ToList();

        var pageResult = new PageResultData<SessionViewData>
                         {
                             Items = pagedSessions ?? [],
                             TotalCount = totalCount
                         };

        _logger?.SessionsLoaded(pageIndex, pageSize, sessions?.Count);

        return Ok(pageResult);
    }

    /// <summary>
    /// Get sessions count
    /// </summary>
    /// <returns>Number of all known sessions</returns>
    [Route("SessionsCount")]
    [HttpGet]
    public async Task<int> GetSessionsCount()
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetSessionsCount));

        _logger?.CountingSessions();

        var numSessions = await _sessionService.GetSessionsCountAsync().ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.SessionsCounted(numSessions);

        return numSessions;
    }

    /// <summary>
    /// Get last finished session
    /// </summary>
    /// <returns>Database id of session</returns>
    [Route("LastFinishedSession")]
    [HttpGet]
    public async Task<long> GetLastFinishedSession()
    {
        var lastFinishedSession = 0L;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetLastFinishedSession));

        _logger?.LoadingLastFinishedSession();

        try
        {
            lastFinishedSession = await _sessionService.GetLastFinishedSessionAsync().ConfigureAwait(false);

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            _logger?.ErrorLoadingLastFinishedSession(ex);

            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
            currentActivity?.AddException(ex);
        }

        _logger?.LastFinishedSessionLoaded(lastFinishedSession);

        return lastFinishedSession;
    }

    /// <summary>
    /// Get data for specific session
    /// </summary>
    /// <param name="id">Session id</param>
    /// <returns>Session view data</returns>
    [Route("Session/{id?}")]
    [HttpGet]
    public async Task<IActionResult> GetSession(long? id)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetSession));

        _logger?.LoadingSession(id ?? -1);

        if (id == null || id == 0)
        {
            _logger?.SessionIdNullOrZero();

            return NotFound();
        }

        var session = await _sessionService.GetSessionAsync(id.Value).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.SessionLoaded(session != null);

        return Ok(session);
    }

    /// <summary>
    /// Get session for specific track
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>List of sessions view data</returns>
    [Route("SessionsOfTrack/{trackId?}")]
    [HttpGet]
    public async Task<IActionResult> GetSessionsOfTrack(long? trackId)
    {
        List<SessionViewData>? sessions;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetSessionsOfTrack));

        _logger?.LoadingSessionsOfTrack(trackId);

        try
        {
            sessions = await _sessionService.GetSessionsOfTrackAsync(trackId).ConfigureAwait(false);

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            _logger?.ErrorLoadingSessionsOfTrack(ex, trackId);

            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
            currentActivity?.AddException(ex);

            sessions = null;
        }

        return Ok(sessions);
    }

    /// <summary>
    /// Get fastest lap from a specific session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Data of fastest lap</returns>
    [Route("FastestLapOfSession/{sessionId?}")]
    [HttpGet]
    public async Task<IActionResult> GetFastestLapOfSession(long? sessionId)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapOfSession));

        _logger?.LoadingFastestLapOfSession(sessionId);

        var fastestLapData = await _sessionService.GetFastestLapOfSessionAsync(sessionId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        return Ok(fastestLapData);
    }

    /// <summary>
    /// Get fastest lap from a specific session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Data of fastest lap</returns>
    [Route("FastestLapsOfSession/{sessionId?}")]
    [HttpGet]
    public async Task<IActionResult> GetFastestLapsOfSession(long? sessionId)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapsOfSession));

        _logger?.LoadingFastestLapOfSession(sessionId);

        var fastestLapsList = await _sessionService.GetFastestLapsOfSessionAsync(sessionId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        return Ok(fastestLapsList);
    }

    /// <summary>
    /// Load time table of session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Time table</returns>
    [Route("LoadSessionTimeTable/{sessionId?}")]
    [HttpGet]
    public async Task<IActionResult> LoadSessionTimeTable(long? sessionId)
    {
        var sessionTimeTable = new SessionTimeTableViewData();

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(LoadSessionTimeTable));

        _logger?.LoadingSessionTimeTable(sessionId);

        try
        {
            sessionTimeTable = await _sessionService.GetSessionTimeTableAsync(sessionId).ConfigureAwait(false);

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            _logger?.ErrorLoadingSessionTimeTable(ex);

            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
            currentActivity?.AddException(ex);
        }

        return Ok(sessionTimeTable);
    }

    /// <summary>
    /// Delete a specific session including its participants, laps and telemetry
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <param name="sessionCode">Session code</param>
    /// <returns>Status whether the session was deleted</returns>
    [Route("DeleteSession/{sessionId}/{sessionCode}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteSession(long sessionId, ulong sessionCode)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(DeleteSession));

        _logger?.DeletingSession(sessionId);

        var isDeleted = await _sessionService.DeleteSessionAsync(sessionId, sessionCode).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.SessionDeleted(isDeleted);

        return Ok(isDeleted);
    }

    #endregion // Controller methods
}