using System.Diagnostics;

using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    #endregion // Constants

    #region Fields

    private readonly ILogger<SessionsController> _logger;
    private readonly IMemoryCache _cache;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="cache">Cache</param>
    public SessionsController(ILogger<SessionsController> logger, IMemoryCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Get sessions
    /// </summary>
    /// <param name="pageIndex">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Sessions</returns>
    [HttpGet]
    public ActionResult<PageResultData<SessionViewData>> GetSessions([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 15)
    {
        if (_cache.TryGetValue(CacheKeySessions, out List<SessionViewData>? sessions) == false)
        {
            sessions = [];

            using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetSessions));

            _logger?.LogInformation("Sessions loading...");

            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                try
                {
                    var attrQuery = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.Select(obj => obj);

                    if (attrQuery != null)
                    {
                        sessions = dbFactory.GetRepository<SessionRepository>()
                                            ?.GetQuery()
                                            ?.Join(attrQuery,
                                                   obj => obj.Id,
                                                   obj => obj.SessionId,
                                                   (obj1, obj2) => new
                                                                   {
                                                                       Session = obj1,
                                                                       obj2.WeatherStart
                                                                   })
                                            .Where(s => s.Session.DbIsFinished == 1)
                                            .OrderByDescending(s => s.Session.Id)
                                            .Select(obj => new SessionViewData
                                                           {
                                                               SessionDbId = obj.Session.Id,
                                                               GameVersionId = obj.Session.GameVersionId,
                                                               GameVersion = obj.Session.GameVersion.Name,
                                                               Track = obj.Session.Track.Name,
                                                               TrackId = obj.Session.TrackId,
                                                               Cars = obj.Session.ActiveCars,
                                                               FormulaType = obj.Session.FormulaType,
                                                               SessionType = obj.Session.SessionType,
                                                               AiDifficulty = obj.Session.AiDifficulty,
                                                               Weather = obj.WeatherStart
                                                           })
                                            .ToList();

                        AdjustSessionTypes(sessions);
                    }

                    currentActivity?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Exception while loading sessions => {Exception}", ex.ToString());

                    currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                    currentActivity?.AddException(ex);
                }
            }

            _cache.Set(CacheKeySessions, sessions, TimeSpan.FromMinutes(5));
        }

        var pagedSessions = sessions?.Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .ToList();

        var pageResult = new PageResultData<SessionViewData>
                         {
                             Items = pagedSessions ?? [],
                             TotalCount = sessions?.Count ?? 0
                         };

        _logger?.LogInformation("Sessions loaded for page {PageIndex} - page size: {PageSize} - total sessions: {Sessions}", pageIndex, pageSize, sessions?.Count);

        return Ok(pageResult);
    }

    /// <summary>
    /// Get sessions count
    /// </summary>
    /// <returns>Number of all known sessions</returns>
    [Route("SessionsCount")]
    [HttpGet]
    public int GetSessionsCount()
    {
        var numSessions = 0;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetSessionsCount));

        _logger?.LogInformation("Sessions count...");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessions = dbFactory.GetRepository<SessionRepository>()
                                    ?.GetQuery()
                                    ?.Count(s => s.DbIsFinished == 1) ?? 0;

            numSessions = sessions;

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.LogInformation("Sessions found ({Sessions}).", numSessions);

        return numSessions;
    }

    /// <summary>
    /// Get last finished session
    /// </summary>
    /// <returns>Database id of session</returns>
    [Route("LastFinishedSession")]
    [HttpGet]
    public long GetLastFinishedSession()
    {
        var lastFinishedSession = 0L;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetLastFinishedSession));

        _logger?.LogInformation("Get last finished session...");

        try
        {
            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                var session = dbFactory.GetRepository<SessionRepository>()
                                       ?.GetQuery()
                                       ?.Where(s => s.DbIsFinished == 1 && s.FormulaType != Formula.SuperCars)
                                       .OrderByDescending(s => s.Id)
                                       .FirstOrDefault();

                if (session != null)
                {
                    lastFinishedSession = session.Id;
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Exception while reading last finished session => {Exception}", ex.ToString());

            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
            currentActivity?.AddException(ex);
        }

        _logger?.LogInformation("Last finished session: {LastFinishedSession}.", lastFinishedSession);

        return lastFinishedSession;
    }

    /// <summary>
    /// Get data for specific session
    /// </summary>
    /// <param name="id">Session id</param>
    /// <returns>Session view data</returns>
    [Route("Session/{id?}")]
    [HttpGet]
    public IActionResult GetSession(long? id)
    {
        SessionViewData? session = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetSession));

        _logger?.LogInformation("Session loading ({SessionId})...", id ?? -1);

        if (id == null || id == 0)
        {
            _logger?.LogWarning("Session id is null or zero!");

            return NotFound();
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var attrQuery = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery();
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            if (attrQuery != null && sessionQuery != null)
            {
                var dbSession = sessionQuery.Include(s => s.GameVersion)
                                            .Include(s => s.Track)
                                            .Join(attrQuery,
                                                  obj => obj.Id,
                                                  obj => obj.SessionId,
                                                  (obj1, obj2) => new
                                                                  {
                                                                      Session = obj1,
                                                                      obj2.WeatherStart
                                                                  })
                                            .FirstOrDefault(s => s.Session.Id == id);

                if (dbSession != null)
                {
                    session = new SessionViewData
                              {
                                  SessionDbId = dbSession.Session.Id,
                                  GameVersionId = dbSession.Session.GameVersionId,
                                  GameVersion = dbSession.Session.GameVersion.Name,
                                  TrackId = dbSession.Session.TrackId,
                                  Track = dbSession.Session.Track.Name,
                                  Cars = dbSession.Session.ActiveCars,
                                  FormulaType = dbSession.Session.FormulaType,
                                  SessionType = dbSession.Session.SessionType,
                                  AiDifficulty = dbSession.Session.AiDifficulty,
                                  Weather = dbSession.WeatherStart
                              };

                    AdjustSessionType(session, dbFactory);
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.LogInformation("Session loaded ({SessionLoaded}).", session != null);

        return Ok(session);
    }

    /// <summary>
    /// Get session for specific track
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>List of sessions view data</returns>
    [Route("SessionsOfTrack/{trackId?}")]
    [HttpGet]
    public IActionResult GetSessionsOfTrack(long? trackId)
    {
        List<SessionViewData>? sessions = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetSessionsOfTrack));

        _logger?.LogInformation("Load session for track {TrackId}...", trackId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            try
            {
                var attrQuery = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery();
                var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

                if (attrQuery != null && sessionQuery != null)
                {
                    var dbSessions = sessionQuery.Include(s => s.GameVersion)
                                                 .Include(s => s.Track)
                                                 .Where(s => s.TrackId == trackId)
                                                 .Join(attrQuery,
                                                       obj => obj.Id,
                                                       obj => obj.SessionId,
                                                       (obj1, obj2) => new
                                                                       {
                                                                           Session = obj1,
                                                                           obj2.WeatherStart
                                                                       })
                                                 .OrderByDescending(s => s.Session.CreationTimestamp)
                                                 .ToList();

                    if (dbSessions.Count > 0)
                    {
                        sessions = new List<SessionViewData>();

                        foreach (var dbSession in dbSessions)
                        {
                            var session = new SessionViewData
                                          {
                                              SessionDbId = dbSession.Session.Id,
                                              GameVersion = dbSession.Session.GameVersion.Name,
                                              Track = dbSession.Session.Track.Name,
                                              Cars = dbSession.Session.ActiveCars,
                                              FormulaType = dbSession.Session.FormulaType,
                                              SessionType = dbSession.Session.SessionType,
                                              AiDifficulty = dbSession.Session.AiDifficulty,
                                              Weather = dbSession.WeatherStart
                                          };

                            sessions.Add(session);
                        }
                    }
                }

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Exception while loading sessions for track {TrackId} => {Exception}", trackId, ex.ToString());

                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                sessions = null;
            }
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
    public IActionResult GetFastestLapOfSession(long? sessionId)
    {
        FastestLapViewData fastestLapData = new();

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapOfSession));

        _logger?.LogInformation("Load fastest lap of session {SessionId}...", sessionId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSession = dbFactory.GetRepository<SessionRepository>()
                                     ?.GetQuery()
                                     ?.FirstOrDefault(s => s.Id == sessionId);

            var lapQuery = dbFactory.GetRepository<LapRepository>()?.GetQuery();

            if (dbSession != null && lapQuery != null)
            {
                var fastestLap = lapQuery.Include(l => l.Participant)
                                         .Where(l => l.SessionId == sessionId && l.LapTime > 0 && l.DbIsCompleted == 1 && l.DbIsInvalidLapTime == 0)
                                         .OrderBy(l => l.LapTime)
                                         .First();

                fastestLapData.DriverName = fastestLap.Participant.Driver.Name;
                fastestLapData.LapTime = fastestLap.LapTime;
                fastestLapData.LapTimeSector1 = fastestLap.Sector1Time;
                fastestLapData.LapTimeSector2 = fastestLap.Sector2Time;
                fastestLapData.LapTimeSector3 = fastestLap.Sector3Time;
                fastestLapData.LapNumber = fastestLap.LapNumber;
                fastestLapData.CarPosition = fastestLap.CarPosition;
                fastestLapData.SessionId = fastestLap.SessionId;
                fastestLapData.LapId = fastestLap.Id;
                fastestLapData.ParticipantId = fastestLap.ParticipantId;
                fastestLapData.DriverId = fastestLap.Participant.DriverId;
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return Ok(fastestLapData);
    }

    /// <summary>
    /// Get fastest lap from a specific session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Data of fastest lap</returns>
    [Route("FastestLapsOfSession/{sessionId?}")]
    [HttpGet]
    public IActionResult GetFastestLapsOfSession(long? sessionId)
    {
        List<FastestLapViewData> fastestLapsList = [];

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestLapsOfSession));

        _logger?.LogInformation("Load fastest lap of session {SessionId}...", sessionId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbSession = dbFactory.GetRepository<SessionRepository>()
                                     ?.GetQuery()
                                     ?.Include(s => s.Participants)
                                     .FirstOrDefault(s => s.Id == sessionId);

            if (dbSession != null)
            {
                var fastestLaps = dbFactory.GetRepository<LapRepository>()
                                           ?.GetQuery()
                                           ?.Include(l => l.Participant)
                                           .Where(l => l.SessionId == sessionId && l.LapTime > 0 && l.DbIsCompleted == 1 && l.DbIsInvalidLapTime == 0)
                                           .OrderBy(l => l.LapTime)
                                           .ToList() ?? [];

                foreach (var fastestLap in fastestLaps)
                {
                    fastestLapsList.Add(new FastestLapViewData
                                        {
                                            DriverName = fastestLap.Participant.Driver.Name,
                                            LapTime = fastestLap.LapTime,
                                            LapTimeSector1 = fastestLap.Sector1Time,
                                            LapTimeSector2 = fastestLap.Sector2Time,
                                            LapTimeSector3 = fastestLap.Sector3Time,
                                            LapNumber = fastestLap.LapNumber,
                                            CarPosition = fastestLap.CarPosition,
                                            SessionId = fastestLap.SessionId,
                                            LapId = fastestLap.Id,
                                            ParticipantId = fastestLap.ParticipantId,
                                            DriverId = fastestLap.Participant.DriverId
                                        });
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return Ok(fastestLapsList);
    }

    /// <summary>
    /// Load time table of session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Time table</returns>
    [Route("LoadSessionTimeTable/{sessionId?}")]
    [HttpGet]
    public IActionResult LoadSessionTimeTable(long? sessionId)
    {
        var sessionTimeTable = new SessionTimeTableViewData();

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(LoadSessionTimeTable));

        _logger?.LogInformation("Loading session time table ({SessionId})...", sessionId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            try
            {
                var dbSession = dbFactory.GetRepository<SessionRepository>()
                                         ?.GetQuery()
                                         ?.Include(s => s.Participants)
                                         .FirstOrDefault(s => s.Id == sessionId);

                if (dbSession != null)
                {
                    foreach (var attendee in dbSession.Participants)
                    {
                        sessionTimeTable.Drivers.Add(new DriverViewData
                                                     {
                                                         ArrayIndex = attendee.ArrayIndex,
                                                         CarNumber = attendee.CarRaceNumber,
                                                         ParticipantId = attendee.Id,
                                                         DriverName = attendee.Driver.Name,
                                                         Nationality = attendee.Nationality.Name,
                                                         TeamName = attendee.Team.Name
                                                     });

                        var dbFinal = dbFactory.GetRepository<FinalClassificationRepository>()
                                               ?.GetQuery()
                                               ?.FirstOrDefault(f => f.ParticipantId == attendee.Id);

                        if (dbFinal != null)
                        {
                            sessionTimeTable.TimeTable.Add(new FinalClassificationViewData
                                                           {
                                                               ArrayIndex = attendee.ArrayIndex,
                                                               DbId = dbFinal.Id,
                                                               ParticipantDbId = attendee.Id,
                                                               DriverName = attendee.Driver.Name,
                                                               CarNumber = attendee.CarRaceNumber,
                                                               TeamName = attendee.Team.Name,
                                                               Nationality = attendee.Nationality.Name,
                                                               StartingPosition = dbFinal.GridPosition,
                                                               FinishPosition = dbFinal.FinishPosition,
                                                               LapsDriven = dbFinal.LapsDriven,
                                                               NumberOfPenalties = dbFinal.NumberOfPenalties,
                                                               PitStops = dbFinal.PitStops,
                                                               PenaltiesTime = dbFinal.PenaltiesTime,
                                                               TotalRaceTime = TimeSpan.FromSeconds(dbFinal.TotalRaceTime).ToString(@"mm\:ss.fff"),
                                                               FastestLapTime = TimeSpan.FromMilliseconds(dbFinal.FastestLapTime).ToString(@"mm\:ss.fff")
                                                           });
                        }
                    }
                }

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Exception while loading session time table => {Exception}", ex.ToString());

                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);
            }
        }

        return Ok(sessionTimeTable);
    }

    /// <summary>
    /// Get fastest lap from a specific session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <param name="sessionCode">Session code</param>
    /// <returns>Data of fastest lap</returns>
    [Route("DeleteSession/{sessionId}/{sessionCode}")]
    [HttpGet]
    public IActionResult DeleteSession(long sessionId, ulong sessionCode)
    {
        var isDeleted = false;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(DeleteSession));

        _logger?.LogInformation("Delete session {SessionId}...", sessionId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var isTelemetryRemoved = true;
            var isLapsRemoved = true;
            var isParticipantsRemoved = true;
            var isSessionRemoved = false;

            var session = dbFactory.GetRepository<SessionRepository>()
                                   ?.GetQuery()
                                   ?.FirstOrDefault(s => s.Id == sessionId && s.SessionId == sessionCode);

            if (session != null)
            {
                // Get participants of session
                var participants = dbFactory.GetRepository<ParticipantRepository>()
                                            ?.GetQuery()
                                            ?.Where(p => p.SessionId == session.Id)
                                            .Select(p => p.Id)
                                            .ToList() ?? [];

                if (participants.Count > 0)
                {
                    // Get laps of participants
                    var laps = dbFactory.GetRepository<LapRepository>()
                                        ?.GetQuery()
                                        ?.Where(l => participants.Contains(l.ParticipantId))
                                        .Select(l => l.Id)
                                        .ToList() ?? [];

                    if (laps.Count > 0)
                    {
                        // Get telemetry data
                        isTelemetryRemoved = dbFactory.GetRepository<CarTelemetryRepository>()
                                                      ?.RemoveRange(t => laps.Contains(t.LapNumberId)) ?? false;

                        isLapsRemoved = dbFactory.GetRepository<LapRepository>()
                                                 ?.RemoveRange(l => laps.Contains(l.Id)) ?? false;
                    }

                    isParticipantsRemoved = dbFactory.GetRepository<ParticipantRepository>()
                                                     ?.RemoveRange(p => participants.Contains(p.Id)) ?? false;
                }

                isSessionRemoved = dbFactory.GetRepository<SessionRepository>()
                                            ?.Remove(s => s.Id == session.Id) ?? false;
            }

            isDeleted = isSessionRemoved && isParticipantsRemoved && isLapsRemoved && isTelemetryRemoved;

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.LogInformation("Session deleted: {Deleted}", isDeleted);

        return Ok(isDeleted);
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Adjusts the session types for sprint races based on their relationship to subsequent races
    /// </summary>
    /// <param name="sessions">A list of <see cref="SessionViewData"/> objects representing the sessions to be analyzed and adjusted</param>
    private void AdjustSessionTypes(List<SessionViewData>? sessions)
    {
        if (sessions == null)
        {
            return;
        }

        var postSprintRaces = sessions.Where(s => s.SessionType == SessionType.Race2)
                                      .ToList();

        // All race with session type 'Race2'
        if (postSprintRaces.Count > 0)
        {
            foreach (var postSprintRace in postSprintRaces)
            {
                var sprintRace = sessions.Where(s => s.SessionDbId < postSprintRace.SessionDbId
                                                     && s.SessionType == SessionType.Race
                                                     && s.TrackId == postSprintRace.TrackId
                                                     && s.GameVersionId == postSprintRace.GameVersionId)
                                         .OrderByDescending(s => s.SessionDbId)
                                         .Take(1)
                                         .ToList();

                // Only one race found?
                if (sprintRace.Count == 1)
                {
                    sprintRace[0].SessionType = SessionType.Sprint;
                }
            }
        }

        var races = sessions.Where(s => s.SessionType == SessionType.Race)
                            .ToList();

        if (races.Count > 0)
        {
            foreach (var race in races)
            {
                var hasSprintQualifyings = sessions.Any(s => s.SessionDbId < race.SessionDbId
                                                             && s.SessionType is >= SessionType.SprintShootout1 and <= SessionType.OneShotSprintShootout
                                                             && s.GameVersionId == race.GameVersionId
                                                             && s.TrackId == race.TrackId);

                if (hasSprintQualifyings)
                {
                    race.SessionType = SessionType.Sprint;
                }
            }
        }
    }

    /// <summary>
    /// Adjusts the session type of the specified session based on the presence of prior sprint qualifying sessions
    /// </summary>
    /// <param name="session">The session data to be adjusted. This parameter cannot be null</param>
    /// <param name="dbFactory">The repository factory used to query session data. This parameter cannot be null</param>
    private void AdjustSessionType(SessionViewData session, RepositoryFactory dbFactory)
    {
        var hasSprintQualifyings = dbFactory.GetRepository<SessionRepository>()
                                            ?.GetQuery()
                                            ?.Any(s => s.Id < session.SessionDbId
                                                       && s.SessionType >= SessionType.SprintShootout1
                                                       && s.SessionType <= SessionType.OneShotSprintShootout
                                                       && s.GameVersionId == session.GameVersionId
                                                       && s.TrackId == session.TrackId);

        if (hasSprintQualifyings == true)
        {
            session.SessionType = SessionType.Sprint;
        }
    }

    #endregion // Private methods
}