using System.Diagnostics;

using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.WebApi.Cache;

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

        _practiceSessions = new List<SessionType>()
                            {
                                SessionType.Practice1,
                                SessionType.Practice2,
                                SessionType.Practice3,
                                SessionType.ShortPractice
                            };
        _qualifyingSessions = new List<SessionType>()
                              {
                                  SessionType.Qualifying1,
                                  SessionType.Qualifying2,
                                  SessionType.Qualifying3,
                                  SessionType.ShortQualifying,
                                  SessionType.OneShotQualifying
                              };
        _raceSessions = new List<SessionType>()
                        {
                            SessionType.Race,
                            SessionType.Race2,
                            SessionType.Race3
                        };
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
    public IActionResult GetFastestLaps(long? trackId)
    {
        var fastestLaps = new List<FastestLapOfTrackViewData>();

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLaps));

        _logger?.LogInformation("Fastest laps of track {TrackId}...", trackId);

        if (trackId.HasValue)
        {
            // F1
            var fastestF1Laps = GetFastestF1Laps(trackId.Value);

            // F2
            var fastestF2Laps = GetFastestF2Laps(trackId.Value);

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

        _logger?.LogInformation("Fastest laps of track loaded ({FastestLaps})", fastestLaps.Count);

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
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapsOfSessions));

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
    private List<FastestLapOfTrackViewData>? GetFastestF1Laps(long trackId)
    {
        List<FastestLapOfTrackViewData>? fastestLaps = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestF1Laps));

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSessions = dbFactory.GetRepository<SessionRepository>()
                                      ?.GetQuery()
                                      ?.Where(s => s.TrackId == trackId
                                                   && (s.FormulaType == Formula.F1Modern
                                                       || s.FormulaType == Formula.F12026))
                                      .ToList();

            if (dbSessions?.Count > 0)
            {
                fastestLaps = GetFastestLapsOfSessions(trackId, dbFactory, dbSessions, Formula.F1Modern);
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
    private List<FastestLapOfTrackViewData>? GetFastestF2Laps(long trackId)
    {
        List<FastestLapOfTrackViewData>? fastestLaps = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestF2Laps));

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSessions = dbFactory.GetRepository<SessionRepository>()
                                      ?.GetQuery()
                                      ?.Where(s => s.TrackId == trackId
                                                   && (s.FormulaType == Formula.F2
                                                       || s.FormulaType == Formula.F2TwentyOne))
                                      .ToList();

            if (dbSessions?.Count > 0)
            {
                fastestLaps = GetFastestLapsOfSessions(trackId, dbFactory, dbSessions, Formula.F2);
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
    private List<FastestLapOfTrackViewData>? GetFastestLapsOfSessions(long trackId, RepositoryFactory dbFactory, List<SessionEntity> dbSessions, Formula formulaType)
    {
        List<FastestLapOfTrackViewData>? fastestLaps = null;
        FastestLapOfTrackViewData? fastestLap = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapsOfSessions));

        var trackData = dbFactory.GetRepository<TrackRepository>()?.GetQuery()?.FirstOrDefault(t => t.Id == trackId);

        // Practice
        using (var currentActivityPractice = AppActivity.ApiSource.StartActivity("FastestLapsOfSessions-Practice"))
        {
            var fastestPracticeLap = FastestLapInSessions(dbFactory, dbSessions, _practiceSessions);

            fastestLap = GetFastestLapData(dbFactory, fastestPracticeLap, FastestLapSessionType.Practice);

            if (fastestLap != null)
            {
                fastestLap.FormulaType = formulaType;
                fastestLap.TrackId = trackId;

                if (trackData != null)
                {
                    fastestLap.ReferenceTime = trackData.LapReferenceTime;
                    fastestLap.DiffReference = fastestLap.LapTime - trackData.LapReferenceTime;
                }

                fastestLaps = new List<FastestLapOfTrackViewData>
                              {
                                  fastestLap
                              };
            }
        }

        // Qualifying
        using (var currentActivityQualifying = AppActivity.ApiSource.StartActivity("FastestLapsOfSessions-Qualifying"))
        {
            var fastestQualifyingLap = FastestLapInSessions(dbFactory, dbSessions, _qualifyingSessions);

            fastestLap = GetFastestLapData(dbFactory, fastestQualifyingLap, FastestLapSessionType.Qualifying);

            if (fastestLap != null)
            {
                fastestLap.FormulaType = formulaType;
                fastestLap.TrackId = trackId;

                if (trackData != null)
                {
                    fastestLap.ReferenceTime = trackData.LapReferenceTime;
                    fastestLap.DiffReference = fastestLap.LapTime - trackData.LapReferenceTime;
                }

                fastestLaps ??= new List<FastestLapOfTrackViewData>();

                fastestLaps.Add(fastestLap);
            }
        }

        // Race
        using (var currentActivityRace = AppActivity.ApiSource.StartActivity("FastestLapsOfSessions-Race"))
        {
            var fastestRaceLap = FastestLapInSessions(dbFactory, dbSessions, _raceSessions);

            fastestLap = GetFastestLapData(dbFactory, fastestRaceLap, FastestLapSessionType.Race);

            if (fastestLap != null)
            {
                fastestLap.FormulaType = formulaType;
                fastestLap.TrackId = trackId;

                if (trackData != null)
                {
                    fastestLap.ReferenceTime = trackData.LapReferenceTime;
                    fastestLap.DiffReference = fastestLap.LapTime - trackData.LapReferenceTime;
                }

                fastestLaps ??= new List<FastestLapOfTrackViewData>();

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
    private FastestLapOfTrackViewData? GetFastestLapData(RepositoryFactory dbFactory, LapEntity? fastestLap, FastestLapSessionType sessionType)
    {
        FastestLapOfTrackViewData? fastestLapData = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapData));

        if (fastestLap != null)
        {
            var driver = dbFactory.GetRepository<ParticipantRepository>()
                                  ?.GetQuery()
                                  ?.Include(obj => obj.Driver)
                                  ?.FirstOrDefault(p => p.Id == fastestLap.ParticipantId);

            if (driver != null)
            {
                fastestLapData = new FastestLapOfTrackViewData()
                                 {
                                     LapTime = fastestLap.LapTime,
                                     LapSessionType = sessionType,
                                     DriverName = driver.Driver.Name,
                                     DriverId = driver.DriverId
                                 };

                if (GetGameDataOfSession(dbFactory, driver.SessionId, out var gameVersionId, out var gameVersionName))
                {
                    fastestLapData.GameVersionId = gameVersionId;
                    fastestLapData.GameVersionName = gameVersionName;
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
    private LapEntity? FastestLapInSessions(RepositoryFactory dbFactory, List<SessionEntity> dbSessions, List<SessionType> sessionTypes)
    {
        LapEntity? fastestLap = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(FastestLapInSessions));

        if (dbSessions.Count > 0)
        {
            var sessions = dbSessions.Where(s => sessionTypes.Contains(s.SessionType))
                                     .Select(s => s.Id)
                                     .ToList();

            if (sessions.Count > 0)
            {
                // Fastests valid and completed lap within session
                var fastestLaps = dbFactory.GetRepository<LapRepository>()
                                           ?.GetQuery()
                                           ?.Where(l => l.DbIsInvalid == 0
                                                        && l.DbIsCompleted == 1
                                                        && sessions.Contains(l.SessionId)
                                                        && l.LapTime > 0
                                                        && l.DbIsInvalidLapTime == 0)
                                           .ToList();

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
    /// <param name="gameVersionId">Game version id</param>
    /// <param name="gameVersionName">Game version name</param>
    /// <returns>Success?</returns>
    private bool GetGameDataOfSession(RepositoryFactory dbFactory, long sessionId, out long gameVersionId, out string gameVersionName)
    {
        var hasData = false;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetGameDataOfSession));

        gameVersionId = 0;
        gameVersionName = string.Empty;

        var sessionData = dbFactory.GetRepository<SessionRepository>()
                                   ?.GetQuery()
                                   ?.FirstOrDefault(s => s.Id == sessionId);

        if (sessionData != null)
        {
            var gameData = dbFactory.GetRepository<GameVersionRepository>()
                                    ?.GetQuery()
                                    ?.FirstOrDefault(g => g.Id == sessionData.GameVersionId);

            if (gameData != null)
            {
                gameVersionName = gameData.Name;

                hasData = true;
            }

            gameVersionId = sessionData.GameVersionId;

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return hasData;
    }

    #endregion // Private methods
}