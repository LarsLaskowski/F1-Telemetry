using System.Diagnostics;

using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Data;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Championship controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ChampionshipController : ControllerBase
{
    #region Fields

    private readonly ILogger<TracksController> _logger;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    public ChampionshipController(ILogger<TracksController> logger)
    {
        _logger = logger;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Get championships
    /// </summary>
    /// <returns>Championships</returns>
    [HttpGet]
    public IEnumerable<ChampionshipViewData> Get()
    {
        List<ChampionshipViewData> championships = new List<ChampionshipViewData>();

        using var currentActivity = AppActivity.ApiSource.StartActivity("GetChampionships");

        _logger?.LogInformation("Championschips loading...");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbChampionships = dbFactory.GetRepository<ChampionshipRepository>()
                                           ?.GetQuery()
                                           ?.OrderBy(c => c.GameVersionId)
                                           .ToList();

            if (dbChampionships?.Count > 0)
            {
                foreach (var dbChampionship in dbChampionships)
                {
                    var championshipData = new ChampionshipViewData
                                           {
                                               ChampionshipId = dbChampionship.Id,
                                               GameVersionId = dbChampionship.GameVersionId,
                                               GameVersionName = dbChampionship.GameVersion.Name,
                                               GameVersionYear = dbChampionship.GameVersion.Version,
                                               Number = dbChampionship.Number
                                           };

                    LoadTracks(dbFactory, championshipData);

                    championships.Add(championshipData);
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.LogInformation("Championship loaded ({ChampionshipsLoaded}).", championships.Count);

        return championships;
    }

    /// <summary>
    /// Creates a new championship
    /// </summary>
    /// <param name="championshipCreateData">Data to create a new championship</param>
    /// <returns>Status</returns>
    [Route("CreateChampionship")]
    [HttpPost]
    public IActionResult CreateChampionship([FromBody] ChampionshipCreateData championshipCreateData)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(CreateChampionship));

        _logger?.LogInformation("Create championship...");

        if (championshipCreateData is null
            || championshipCreateData.Tracks is null
            || championshipCreateData.GameVersionId <= 0
            || championshipCreateData.Tracks.Count <= 5)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error);

            return BadRequest("Invalid championship data. GameVersionId must be greater than 0 and at least 6 tracks are required.");
        }

        IActionResult result;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            ushort seasonNumber = 1;
            var championships = dbFactory.GetRepository<ChampionshipRepository>()
                                         ?.GetQuery()
                                         ?.Count(c => c.Mode == (ChampionshipMode)championshipCreateData.Mode
                                                      && c.GameVersionId == championshipCreateData.GameVersionId);

            if (championships > 0)
            {
                seasonNumber = (ushort)(championships + 1);
            }

            dbFactory.GetRepository<ChampionshipRepository>()?.Add(new ChampionshipEntity
                                                                   {
                                                                       GameVersionId = championshipCreateData.GameVersionId,
                                                                       Mode = (ChampionshipMode)championshipCreateData.Mode,
                                                                       Number = seasonNumber
                                                                   });

            var championship = dbFactory.GetRepository<ChampionshipRepository>()
                                        ?.GetQuery()
                                        ?.FirstOrDefault(c => c.Mode == (ChampionshipMode)championshipCreateData.Mode
                                                              && c.GameVersionId == championshipCreateData.GameVersionId);

            if (championship != null)
            {
                AddTracksToChampionship(championshipCreateData.Tracks, dbFactory, championship);

                currentActivity?.SetStatus(ActivityStatusCode.Ok);

                result = Ok(true);
            }
            else
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error);

                result = StatusCode(500, false);
            }
        }

        return result;
    }

    /// <summary>
    /// Is a championship available for this session?
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Id of championship</returns>
    [Route("IsActiveChampionship/{sessionId}")]
    [HttpGet]
    public long IsActiveChampionship(long sessionId)
    {
        var activeChampionship = 0L;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(IsActiveChampionship));

        _logger?.LogInformation("Exists an active championship for this session {SessionId}...", sessionId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionData = dbFactory.GetRepository<SessionRepository>()
                                       ?.GetQuery()
                                       ?.FirstOrDefault(s => s.Id == sessionId);

            if (sessionData != null && IsSessionTypeValid(sessionData.SessionType) && sessionData.FormulaType == Formula.F1Modern)
            {
                var championship = dbFactory.GetRepository<ChampionshipRepository>()
                                            ?.GetQuery()
                                            ?.FirstOrDefault(c => c.GameVersionId == sessionData.GameVersionId
                                                                  && c.DbIsFinished == 0);

                if (championship != null)
                {
                    var track = dbFactory.GetRepository<ChampionshipTrackRepository>()
                                         ?.GetQuery()
                                         ?.FirstOrDefault(t => t.TrackId == sessionData.TrackId
                                                               && t.ChampionshipId == championship.Id);

                    if (track != null)
                    {
                        activeChampionship = championship.Id;
                    }
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.LogInformation("Active championship exists: {ActiveChampionship}...", activeChampionship);

        return activeChampionship;
    }

    /// <summary>
    /// Is a session active in a running championship?
    /// </summary>
    /// <param name="championshipId">Id of championship</param>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Status</returns>
    [Route("IsSessionInChampionshipActive/{championshipId}/{sessionId}")]
    [HttpGet]
    public bool IsSessionInChampionshipActive(long championshipId, long sessionId)
    {
        var sessionIsActive = false;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(IsSessionInChampionshipActive));

        _logger?.LogInformation("Checking session {SessionId} is active in championship {ChampionshipId}...", sessionId, championshipId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionData = dbFactory.GetRepository<SessionRepository>()
                                       ?.GetQuery()
                                       ?.Where(s => s.Id == sessionId)
                                       .Select(s => new SessionBaseData
                                                    {
                                                        SessionId = s.Id,
                                                        SessionType = s.SessionType,
                                                        GameVersionId = s.GameVersionId,
                                                        TrackId = s.TrackId,
                                                        FormulaType = s.FormulaType
                                                    })
                                       .FirstOrDefault();

            if (sessionData != null)
            {
                var track = dbFactory.GetRepository<ChampionshipTrackRepository>()
                                     ?.GetQuery()
                                     ?.Where(t => t.ChampionshipId == championshipId
                                                  && t.TrackId == sessionData.TrackId)
                                     .Select(t => new
                                                  {
                                                      t.QualifyingSessionId,
                                                      t.SprintQualifyingSessionId,
                                                      t.RaceSessionId,
                                                      t.SprintSessionId
                                                  })
                                     .FirstOrDefault();

                AdjustSessionType(sessionData, dbFactory);

                if (track != null)
                {
                    sessionIsActive = sessionData.SessionType switch
                                      {
                                          SessionType.OneShotQualifying or SessionType.ShortQualifying or SessionType.Qualifying3
                    => track.QualifyingSessionId == sessionId,
                                          SessionType.OneShotSprintShootout or SessionType.ShortSprintShootout or SessionType.SprintShootout3
                    => track.SprintQualifyingSessionId == sessionId,
                                          SessionType.Race or SessionType.Race2 => track.RaceSessionId == sessionId,
                                          SessionType.Sprint => track.SprintSessionId == sessionId,
                                          _ => false
                                      };
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.LogInformation("Active session in championship exists: {SessionIsActive}...", sessionIsActive);

        return sessionIsActive;
    }

    /// <summary>
    /// Adds a session to an active championship based on the provided session ID
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session to be added to the championship. Must be greater than zero</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the success of the operation. Returns <see langword="true"/> if the
    /// operation completes successfully
    /// </returns>
    [Route("AddToChampionship/{sessionId}")]
    [HttpPost]
    public IActionResult AddToChampionship(long sessionId)
    {
        IActionResult result = BadRequest("Invalid session");
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(AddToChampionship));

        currentActivity?.SetTag("SessionId", sessionId);

        _logger?.LogInformation("Adding session {SessionId} to championship...", sessionId);

        if (sessionId > 0)
        {
            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                var sessionData = dbFactory.GetRepository<SessionRepository>()
                                           ?.GetQuery()
                                           ?.Select(s => new SessionBaseData
                                                         {
                                                             SessionId = s.Id,
                                                             SessionType = s.SessionType,
                                                             TrackId = s.TrackId,
                                                             GameVersionId = s.GameVersionId,
                                                             FormulaType = s.FormulaType
                                                         })
                                           .FirstOrDefault(s => s.SessionId == sessionId);

                if (sessionData != null)
                {
                    var championship = dbFactory.GetRepository<ChampionshipRepository>()
                                                ?.GetQuery()
                                                ?.FirstOrDefault(c => c.GameVersionId == sessionData.GameVersionId
                                                                      && c.DbIsFinished == 0);

                    AdjustSessionType(sessionData, dbFactory);

                    result = SaveSessionToChampionship(dbFactory, sessionData, championship);
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return result;
    }

    /// <summary>
    /// Load tracks of championship
    /// </summary>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="championshipData">Championship data</param>
    private void LoadTracks(RepositoryFactory dbFactory, ChampionshipViewData championshipData)
    {
        var tracks = dbFactory.GetRepository<ChampionshipTrackRepository>()
                              ?.GetQuery()
                              ?.Where(c => c.ChampionshipId == championshipData.ChampionshipId)
                              .ToList() ?? [];

        if (tracks.Count > 0)
        {
            championshipData.Tracks = new List<ChampionshipTrackViewData>();

            foreach (var track in tracks)
            {
                var trackData = new ChampionshipTrackViewData
                                {
                                    ChampionshipTrackId = track.TrackId,
                                    QualifyingPosition = GetGridPosition(dbFactory, track.SprintQualifyingSessionId),
                                    SprintQualifyingPosition = GetGridPosition(dbFactory, track.QualifyingSessionId),
                                    SprintPosition = GetGridPosition(dbFactory, track.SprintSessionId),
                                    RacePosition = GetGridPosition(dbFactory, track.RaceSessionId)
                                };

                CalculatePoints(trackData);

                championshipData.Tracks.Add(trackData);
            }
        }
    }

    /// <summary>
    /// Add tracks to championship
    /// </summary>
    /// <param name="tracks">Tracks</param>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="championship">Championship</param>
    private void AddTracksToChampionship(List<long> tracks, RepositoryFactory dbFactory, ChampionshipEntity championship)
    {
        foreach (var track in tracks)
        {
            var dbTrack = dbFactory.GetRepository<TrackRepository>()
                                   ?.GetQuery()
                                   ?.FirstOrDefault(t => t.TrackNumber == track);

            if (dbTrack != null)
            {
                dbFactory.GetRepository<ChampionshipTrackRepository>()
                         ?.Add(new ChampionshipTrackEntity
                               {
                                   ChampionshipId = championship.Id,
                                   TrackId = dbTrack.Id
                               });
            }
            else
            {
                _logger?.LogWarning("[CreateChampionship] Unknown track number {track}!", track);
            }
        }
    }

    /// <summary>
    /// Is session type a valid for championship?
    /// </summary>
    /// <param name="sessionType">Session type</param>
    /// <returns>Valid or not</returns>
    private bool IsSessionTypeValid(SessionType sessionType)
    {
        var isValid = sessionType switch
                      {
                          SessionType.OneShotQualifying => true,
                          SessionType.ShortQualifying => true,
                          SessionType.Qualifying3 => true,
                          SessionType.OneShotSprintShootout => true,
                          SessionType.ShortSprintShootout => true,
                          SessionType.SprintShootout3 => true,
                          SessionType.Race => true,
                          SessionType.Race2 => true,
                          SessionType.Sprint => true,
                          _ => false
                      };

        return isValid;
    }

    /// <summary>
    /// Saves the session data to the specified championship, associating it with the appropriate track
    /// </summary>
    /// <param name="dbFactory">The factory used to retrieve repositories for database operations</param>
    /// <param name="sessionData">The session data to be saved, including track and session details</param>
    /// <param name="championship">
    /// The championship entity to which the session data will be added. If <see langword="null"/>, the operation will
    /// not proceed
    /// </param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the
    /// session is successfully added to the championship; otherwise, returns <see cref="NotFoundResult"/> with an
    /// appropriate error message
    /// </returns>
    private IActionResult SaveSessionToChampionship(RepositoryFactory dbFactory, SessionBaseData sessionData, ChampionshipEntity? championship)
    {
        IActionResult result = NotFound("Track is not part of the championship!");

        if (championship != null)
        {
            var championshipTrack = dbFactory.GetRepository<ChampionshipTrackRepository>()
                                             ?.GetQuery()
                                             ?.FirstOrDefault(t => t.TrackId == sessionData.TrackId
                                                                   && t.ChampionshipId == championship.Id);

            if (championshipTrack != null
                && SetChampionShipSession(sessionData, championshipTrack))
            {
                var isRefreshed = dbFactory.GetRepository<ChampionshipTrackRepository>()
                                           ?.Refresh(ct => ct.Id == championshipTrack.Id,
                                                     obj =>
                                                     {
                                                         obj.QualifyingSessionId = championshipTrack.QualifyingSessionId;
                                                         obj.SprintQualifyingSessionId = championshipTrack.SprintQualifyingSessionId;
                                                         obj.RaceSessionId = championshipTrack.RaceSessionId;
                                                         obj.SprintSessionId = championshipTrack.SprintSessionId;
                                                     }) ?? false;

                result = isRefreshed ? Ok() : NotFound("Session not added to championship!");
            }
        }

        return result;
    }

    /// <summary>
    /// Adjusts the session type of the specified session based on the presence of prior sprint qualifying sessions
    /// </summary>
    /// <param name="session">The session data to be adjusted. This parameter cannot be null</param>
    /// <param name="dbFactory">The repository factory used to query session data. This parameter cannot be null</param>
    private void AdjustSessionType(SessionBaseData session, RepositoryFactory dbFactory)
    {
        if (session.SessionType is SessionType.Race)
        {
            var hasSprintQualifyings = dbFactory.GetRepository<SessionRepository>()
                                                ?.GetQuery()
                                                ?.Any(s => s.Id < session.SessionId
                                                           && s.SessionType >= SessionType.SprintShootout1
                                                           && s.SessionType <= SessionType.OneShotSprintShootout
                                                           && s.GameVersionId == session.GameVersionId
                                                           && s.TrackId == session.TrackId) ?? false;

            if (hasSprintQualifyings)
            {
                session.SessionType = SessionType.Sprint;
            }
        }
    }

    /// <summary>
    /// Associates a session with the appropriate championship track session type
    /// </summary>
    /// <param name="session">The session to be set, containing details such as its type and identifier</param>
    /// <param name="championshipTrack">The championship track entity to update with the session information</param>
    /// <returns>
    /// <see langword="true"/> if the session was successfully associated with the championship track; otherwise, <see
    /// langword="false"/> if the session type is unsupported
    /// </returns>
    private bool SetChampionShipSession(SessionBaseData session, ChampionshipTrackEntity championshipTrack)
    {
        var isSessionSet = true;

        switch (session.SessionType)
        {
            case SessionType.OneShotQualifying:
            case SessionType.ShortQualifying:
            case SessionType.Qualifying3:
                {
                    championshipTrack.QualifyingSessionId = session.SessionId;
                }
                break;

            case SessionType.OneShotSprintShootout:
            case SessionType.ShortSprintShootout:
            case SessionType.SprintShootout3:
                {
                    championshipTrack.SprintQualifyingSessionId = session.SessionId;
                }
                break;

            case SessionType.Sprint:
                {
                    championshipTrack.SprintSessionId = session.SessionId;
                }
                break;

            case SessionType.Race:
            case SessionType.Race2:
                {
                    championshipTrack.RaceSessionId = session.SessionId;
                }
                break;

            default:
                {
                    isSessionSet = false;
                }
                break;
        }

        return isSessionSet;
    }

    /// <summary>
    /// Retrieves the grid position of a human-controlled participant in a specified session
    /// </summary>
    /// <param name="dbFactory">The <see cref="RepositoryFactory"/> instance used to access the data repositories</param>
    /// <param name="sessionId">
    /// The identifier of the session for which the grid position is being retrieved. Must be greater than 0 to perform
    /// the lookup; otherwise, the method returns 0
    /// </param>
    /// <returns>
    /// The grid position of the human-controlled participant in the specified session. Returns 0 if no human-controlled
    /// participant is found or if <paramref name="sessionId"/> is null or less than 1
    /// </returns>
    private int GetGridPosition(RepositoryFactory dbFactory, long? sessionId)
    {
        var gridPosition = 0;

        if (sessionId > 0)
        {
            var humanDriverId = dbFactory.GetRepository<ParticipantRepository>()
                                         ?.GetQuery()
                                         ?.Where(p => p.SessionId == sessionId
                                                      && p.DbIsHumanControlled == 1)
                                         .Select(p => p.Id)
                                         .FirstOrDefault() ?? 0;

            if (humanDriverId > 0)
            {
                gridPosition = dbFactory.GetRepository<FinalClassificationRepository>()
                                        ?.GetQuery()
                                        ?.Where(f => f.SessionId == sessionId
                                                     && f.ParticipantId == humanDriverId)
                                        .Select(f => f.FinishPosition)
                                        .FirstOrDefault() ?? 0;
            }
        }

        return gridPosition;
    }

    /// <summary>
    /// Calculates and assigns race and sprint points based on the positions achieved in a championship track
    /// </summary>
    /// <param name="trackData">
    /// The track data containing race and sprint positions. The <see cref="ChampionshipTrackViewData.RacePoints"/> and
    /// <see cref="ChampionshipTrackViewData.SprintPoints"/> properties will be updated based on the positions
    /// </param>
    private void CalculatePoints(ChampionshipTrackViewData trackData)
    {
        trackData.RacePoints = trackData.RacePosition switch
                               {
                                   1 => 25,
                                   2 => 18,
                                   3 => 15,
                                   4 => 12,
                                   5 => 10,
                                   6 => 8,
                                   7 => 6,
                                   8 => 4,
                                   9 => 2,
                                   10 => 1,
                                   _ => 0
                               };

        trackData.SprintPoints = trackData.SprintPosition switch
                                 {
                                     1 => 8,
                                     2 => 7,
                                     3 => 6,
                                     4 => 5,
                                     5 => 4,
                                     6 => 3,
                                     7 => 2,
                                     8 => 1,
                                     _ => 0
                                 };
    }

    #endregion // Methods
}