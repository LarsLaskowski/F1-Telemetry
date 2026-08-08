using System.Diagnostics;

using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    private readonly List<SessionType> _practiceSessions;
    private readonly List<SessionType> _qualifyingSessions;
    private readonly List<SessionType> _raceSessions;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    public FastestLapController(ILogger<FastestLapController> logger)
    {
        _logger = logger;

        _practiceSessions = [
                                SessionType.Practice1,
                                SessionType.Practice2,
                                SessionType.Practice3,
                                SessionType.ShortPractice
                            ];
        _qualifyingSessions = [
                                  SessionType.Qualifying1,
                                  SessionType.Qualifying2,
                                  SessionType.Qualifying3,
                                  SessionType.ShortQualifying,
                                  SessionType.OneShotQualifying
                              ];
        _raceSessions = [
                            SessionType.Race,
                            SessionType.Race2,
                            SessionType.Race3
                        ];
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
            // F1
            var fastestF1Laps = await GetFastestF1LapsAsync(trackId.Value).ConfigureAwait(false);

            // F2
            var fastestF2Laps = await GetFastestF2LapsAsync(trackId.Value).ConfigureAwait(false);

            if (fastestF1Laps?.Count > 0)
            {
                fastestLaps.AddRange(fastestF1Laps);
            }

            if (fastestF2Laps?.Count > 0)
            {
                fastestLaps.AddRange(fastestF2Laps);
            }

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

        var fastestLapData = await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId).ConfigureAwait(false);

        return Ok(fastestLapData);
    }

    #endregion // Controller methods

    #region Private methods

    /// <summary>
    /// Get fastest laps in F1
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>List with fastest sessions laps</returns>
    private async Task<List<FastestLapOfTrackViewData>?> GetFastestF1LapsAsync(long trackId)
    {
        List<FastestLapOfTrackViewData>? fastestLaps = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestF1LapsAsync));

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var dbSessions = sessionQuery == null
                                 ? null
                                 : await sessionQuery.Where(s => s.TrackId == trackId
                                                                 && (s.FormulaType == Formula.F1Modern
                                                                     || s.FormulaType == Formula.F12026))
                                                     .ToListAsync()
                                                     .ConfigureAwait(false);

            if (dbSessions?.Count > 0)
            {
                fastestLaps = await GetFastestLapsOfSessionsAsync(trackId, dbFactory, dbSessions, Formula.F1Modern).ConfigureAwait(false);
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return fastestLaps;
    }

    /// <summary>
    /// Get fastest laps in F2
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>List with fastest session laps</returns>
    private async Task<List<FastestLapOfTrackViewData>?> GetFastestF2LapsAsync(long trackId)
    {
        List<FastestLapOfTrackViewData>? fastestLaps = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestF2LapsAsync));

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var dbSessions = sessionQuery == null
                                 ? null
                                 : await sessionQuery.Where(s => s.TrackId == trackId
                                                                 && (s.FormulaType == Formula.F2
                                                                     || s.FormulaType == Formula.F2TwentyOne))
                                                     .ToListAsync()
                                                     .ConfigureAwait(false);

            if (dbSessions?.Count > 0)
            {
                fastestLaps = await GetFastestLapsOfSessionsAsync(trackId, dbFactory, dbSessions, Formula.F2).ConfigureAwait(false);
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return fastestLaps;
    }

    /// <summary>
    /// Get fastest laps of session for specific formula type
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <param name="dbFactory">Database factory object</param>
    /// <param name="dbSessions">Sessions</param>
    /// <param name="formulaType">Type of formula</param>
    /// <returns>List with fastest laps</returns>
    private async Task<List<FastestLapOfTrackViewData>?> GetFastestLapsOfSessionsAsync(long trackId, RepositoryFactory dbFactory, List<SessionEntity> dbSessions, Formula formulaType)
    {
        List<FastestLapOfTrackViewData>? fastestLaps = null;
        FastestLapOfTrackViewData? fastestLap = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapsOfSessionsAsync));

        var trackQuery = dbFactory.GetRepository<TrackRepository>()?.GetQuery();

        var trackData = trackQuery == null
                            ? null
                            : await trackQuery.FirstOrDefaultAsync(t => t.Id == trackId)
                                              .ConfigureAwait(false);

        // Practice
        using (var currentActivityPractice = AppActivity.ApiSource.StartActivity("FastestLapsOfSessions-Practice"))
        {
            var fastestPracticeLap = await FastestLapInSessionsAsync(dbFactory, dbSessions, _practiceSessions).ConfigureAwait(false);

            fastestLap = await GetFastestLapDataAsync(dbFactory, fastestPracticeLap, FastestLapSessionType.Practice).ConfigureAwait(false);

            if (fastestLap != null)
            {
                fastestLap.FormulaType = formulaType;
                fastestLap.TrackId = trackId;

                if (trackData != null)
                {
                    fastestLap.ReferenceTime = trackData.LapReferenceTime;
                    fastestLap.DiffReference = fastestLap.LapTime - trackData.LapReferenceTime;
                }

                fastestLaps = [fastestLap];
            }
        }

        // Qualifying
        using (var currentActivityQualifying = AppActivity.ApiSource.StartActivity("FastestLapsOfSessions-Qualifying"))
        {
            var fastestQualifyingLap = await FastestLapInSessionsAsync(dbFactory, dbSessions, _qualifyingSessions).ConfigureAwait(false);

            fastestLap = await GetFastestLapDataAsync(dbFactory, fastestQualifyingLap, FastestLapSessionType.Qualifying).ConfigureAwait(false);

            if (fastestLap != null)
            {
                fastestLap.FormulaType = formulaType;
                fastestLap.TrackId = trackId;

                if (trackData != null)
                {
                    fastestLap.ReferenceTime = trackData.LapReferenceTime;
                    fastestLap.DiffReference = fastestLap.LapTime - trackData.LapReferenceTime;
                }

                fastestLaps ??= [];

                fastestLaps.Add(fastestLap);
            }
        }

        // Race
        using (var currentActivityRace = AppActivity.ApiSource.StartActivity("FastestLapsOfSessions-Race"))
        {
            var fastestRaceLap = await FastestLapInSessionsAsync(dbFactory, dbSessions, _raceSessions).ConfigureAwait(false);

            fastestLap = await GetFastestLapDataAsync(dbFactory, fastestRaceLap, FastestLapSessionType.Race).ConfigureAwait(false);

            if (fastestLap != null)
            {
                fastestLap.FormulaType = formulaType;
                fastestLap.TrackId = trackId;

                if (trackData != null)
                {
                    fastestLap.ReferenceTime = trackData.LapReferenceTime;
                    fastestLap.DiffReference = fastestLap.LapTime - trackData.LapReferenceTime;
                }

                fastestLaps ??= [];

                fastestLaps.Add(fastestLap);
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        return fastestLaps;
    }

    /// <summary>
    /// Get data of fastest lap
    /// </summary>
    /// <param name="dbFactory">Database factory object</param>
    /// <param name="fastestLap">Fastest lap entity</param>
    /// <param name="sessionType">Type of session</param>
    /// <returns>Data of this fastest lap</returns>
    private async Task<FastestLapOfTrackViewData?> GetFastestLapDataAsync(RepositoryFactory dbFactory, LapEntity? fastestLap, FastestLapSessionType sessionType)
    {
        FastestLapOfTrackViewData? fastestLapData = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapDataAsync));

        if (fastestLap != null)
        {
            var participantQuery = dbFactory.GetRepository<ParticipantRepository>()?.GetQuery();

            var driver = participantQuery == null
                             ? null
                             : await participantQuery.Include(obj => obj.Driver)
                                                     .FirstOrDefaultAsync(p => p.Id == fastestLap.ParticipantId)
                                                     .ConfigureAwait(false);

            if (driver != null)
            {
                fastestLapData = new FastestLapOfTrackViewData()
                                 {
                                     LapTime = fastestLap.LapTime,
                                     LapSessionType = sessionType,
                                     DriverName = driver.Driver.Name,
                                     DriverId = driver.DriverId
                                 };

                var gameData = await GetGameDataOfSessionAsync(dbFactory, driver.SessionId).ConfigureAwait(false);

                if (gameData.HasData)
                {
                    fastestLapData.GameVersionId = gameData.GameVersionId;
                    fastestLapData.GameVersionName = gameData.GameVersionName;
                }
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        return fastestLapData;
    }

    /// <summary>
    /// Determine fastest lap in practice sessions
    /// </summary>
    /// <param name="dbFactory">DB-Factory object</param>
    /// <param name="dbSessions">Practice sessions</param>
    /// <param name="sessionTypes">Type of sessions</param>
    /// <returns>Lap entity</returns>
    private async Task<LapEntity?> FastestLapInSessionsAsync(RepositoryFactory dbFactory, List<SessionEntity> dbSessions, List<SessionType> sessionTypes)
    {
        LapEntity? fastestLap = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(FastestLapInSessionsAsync));

        if (dbSessions.Count > 0)
        {
            var sessions = dbSessions.Where(s => sessionTypes.Contains(s.SessionType))
                                     .Select(s => s.Id)
                                     .ToList();

            if (sessions.Count > 0)
            {
                var lapQuery = dbFactory.GetRepository<LapRepository>()?.GetQuery();

                // Fastests valid and completed lap within session
                var fastestLaps = lapQuery == null
                                      ? null
                                      : await lapQuery.Where(l => l.DbIsInvalid == 0
                                                                  && l.DbIsCompleted == 1
                                                                  && sessions.Contains(l.SessionId)
                                                                  && l.LapTime > 0
                                                                  && l.DbIsInvalidLapTime == 0)
                                                      .ToListAsync()
                                                      .ConfigureAwait(false);

                if (fastestLaps?.Count > 0)
                {
                    fastestLap = fastestLaps.MinBy(l => l.LapTime);
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return fastestLap;
    }

    /// <summary>
    /// Reading game information for specific session
    /// </summary>
    /// <param name="dbFactory">Database factory object</param>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Game version id and name of the session together with the information whether the game version was found</returns>
    private async Task<(bool HasData, long GameVersionId, string GameVersionName)> GetGameDataOfSessionAsync(RepositoryFactory dbFactory, long sessionId)
    {
        var hasData = false;
        var gameVersionId = 0L;
        var gameVersionName = string.Empty;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetGameDataOfSessionAsync));

        var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

        var sessionData = sessionQuery == null
                              ? null
                              : await sessionQuery.FirstOrDefaultAsync(s => s.Id == sessionId)
                                                  .ConfigureAwait(false);

        if (sessionData != null)
        {
            var gameQuery = dbFactory.GetRepository<GameVersionRepository>()?.GetQuery();

            var gameData = gameQuery == null
                               ? null
                               : await gameQuery.FirstOrDefaultAsync(g => g.Id == sessionData.GameVersionId)
                                                .ConfigureAwait(false);

            if (gameData != null)
            {
                gameVersionName = gameData.Name;

                hasData = true;
            }

            gameVersionId = sessionData.GameVersionId;

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return (hasData, gameVersionId, gameVersionName);
    }

    #endregion // Private methods
}