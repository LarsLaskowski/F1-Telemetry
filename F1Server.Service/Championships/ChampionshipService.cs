using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace F1Server.Service.Championships;

/// <summary>
/// Business logic of the championships - reads the championships with their tracks and points, creates new
/// championships and assigns finished sessions to the track of a running championship
/// </summary>
public class ChampionshipService
{
    #region Constants

    /// <summary>
    /// Minimum number of tracks a new championship has to be created with
    /// </summary>
    private const int MinimumTrackCount = 6;

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Logging interface
    /// </summary>
    private readonly ILogger<ChampionshipService> _logger;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    public ChampionshipService(ILogger<ChampionshipService> logger)
    {
        _logger = logger;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Loads all championships with their tracks, the grid positions of the human controlled participant and the
    /// points awarded for those positions
    /// </summary>
    /// <returns>Championships ordered by their game version</returns>
    public async Task<List<ChampionshipViewData>> GetChampionshipsAsync()
    {
        List<ChampionshipViewData> championships = [];

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var championshipQuery = dbFactory.GetRepository<ChampionshipRepository>()?.GetQuery();

            var dbChampionships = championshipQuery == null
                                      ? null
                                      : await championshipQuery.OrderBy(c => c.GameVersionId)
                                                               .ToListAsync()
                                                               .ConfigureAwait(false);

            if (dbChampionships?.Count > 0)
            {
                var championshipTracks = await LoadChampionshipTracksAsync(dbFactory, dbChampionships).ConfigureAwait(false);
                var gridPositions = await LoadGridPositionsAsync(dbFactory, championshipTracks).ConfigureAwait(false);

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

                    LoadTracks(championshipData, championshipTracks, gridPositions);

                    championships.Add(championshipData);
                }
            }
        }

        return championships;
    }

    /// <summary>
    /// Creates a new championship with the given tracks. The season number is derived from the championships already
    /// stored for the same mode and game version
    /// </summary>
    /// <param name="championshipCreateData">Data to create a new championship</param>
    /// <returns>Result of the creation</returns>
    public async Task<ChampionshipCreateResult> CreateChampionshipAsync(ChampionshipCreateData? championshipCreateData)
    {
        if (championshipCreateData?.Tracks is null
            || championshipCreateData.GameVersionId <= 0
            || championshipCreateData.Tracks.Count < MinimumTrackCount)
        {
            return ChampionshipCreateResult.InvalidData;
        }

        ChampionshipCreateResult result;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            ushort seasonNumber = 1;

            var championshipRepository = dbFactory.GetRepository<ChampionshipRepository>();
            var championshipQuery = championshipRepository?.GetQuery();

            var championships = championshipQuery == null
                                    ? 0
                                    : await championshipQuery.CountAsync(c => c.Mode == (ChampionshipMode)championshipCreateData.Mode
                                                                              && c.GameVersionId == championshipCreateData.GameVersionId)
                                                             .ConfigureAwait(false);

            if (championships > 0)
            {
                seasonNumber = (ushort)(championships + 1);
            }

            var championship = new ChampionshipEntity
                               {
                                   GameVersionId = championshipCreateData.GameVersionId,
                                   Mode = (ChampionshipMode)championshipCreateData.Mode,
                                   Number = seasonNumber
                               };

            // The insert fills the generated id, so the new season is used directly instead of re-reading it - a
            // lookup by mode and game version would return the first season as soon as a second one is created
            var isCreated = championshipRepository != null
                            && await championshipRepository.AddAsync(championship).ConfigureAwait(false);

            if (isCreated)
            {
                await AddTracksToChampionshipAsync(championshipCreateData.Tracks, dbFactory, championship).ConfigureAwait(false);

                result = ChampionshipCreateResult.Created;
            }
            else
            {
                result = ChampionshipCreateResult.NotCreated;
            }
        }

        return result;
    }

    /// <summary>
    /// Determines the running championship the given session belongs to
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Id of the running championship of the session, or 0 if the session is not part of one</returns>
    public async Task<long> GetActiveChampionshipIdAsync(long sessionId)
    {
        var activeChampionship = 0L;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var sessionData = sessionQuery == null
                                  ? null
                                  : await sessionQuery.FirstOrDefaultAsync(s => s.Id == sessionId)
                                                      .ConfigureAwait(false);

            if (sessionData != null && IsSessionTypeValid(sessionData.SessionType) && sessionData.FormulaType == Formula.F1Modern)
            {
                var championshipQuery = dbFactory.GetRepository<ChampionshipRepository>()?.GetQuery();

                var championship = championshipQuery == null
                                       ? null
                                       : await championshipQuery.FirstOrDefaultAsync(c => c.GameVersionId == sessionData.GameVersionId
                                                                                          && c.DbIsFinished == 0)
                                                                .ConfigureAwait(false);

                if (championship != null)
                {
                    var trackQuery = dbFactory.GetRepository<ChampionshipTrackRepository>()?.GetQuery();

                    var track = trackQuery == null
                                    ? null
                                    : await trackQuery.FirstOrDefaultAsync(t => t.TrackId == sessionData.TrackId
                                                                                && t.ChampionshipId == championship.Id)
                                                      .ConfigureAwait(false);

                    if (track != null)
                    {
                        activeChampionship = championship.Id;
                    }
                }
            }
        }

        return activeChampionship;
    }

    /// <summary>
    /// Is the given session assigned to the track of the given championship?
    /// </summary>
    /// <param name="championshipId">Id of championship</param>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Session assigned to the championship track or not</returns>
    public async Task<bool> IsSessionInChampionshipActiveAsync(long championshipId, long sessionId)
    {
        var sessionIsActive = false;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var sessionData = sessionQuery == null
                                  ? null
                                  : await sessionQuery.Where(s => s.Id == sessionId)
                                                      .Select(s => new SessionBaseData
                                                                   {
                                                                       SessionId = s.Id,
                                                                       SessionType = s.SessionType,
                                                                       GameVersionId = s.GameVersionId,
                                                                       TrackId = s.TrackId,
                                                                       FormulaType = s.FormulaType
                                                                   })
                                                      .FirstOrDefaultAsync()
                                                      .ConfigureAwait(false);

            if (sessionData != null)
            {
                var championshipTrackQuery = dbFactory.GetRepository<ChampionshipTrackRepository>()?.GetQuery();

                var track = championshipTrackQuery == null
                                ? null
                                : await championshipTrackQuery.Where(t => t.ChampionshipId == championshipId
                                                                          && t.TrackId == sessionData.TrackId)
                                                              .Select(t => new
                                                                           {
                                                                               t.QualifyingSessionId,
                                                                               t.SprintQualifyingSessionId,
                                                                               t.RaceSessionId,
                                                                               t.SprintSessionId
                                                                           })
                                                              .FirstOrDefaultAsync()
                                                              .ConfigureAwait(false);

                await AdjustSessionTypeAsync(sessionData, dbFactory).ConfigureAwait(false);

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
        }

        return sessionIsActive;
    }

    /// <summary>
    /// Adds a finished session to the track of the running championship of its game version
    /// </summary>
    /// <param name="sessionId">Id of the session that is added to the championship</param>
    /// <returns>Result of the assignment</returns>
    public async Task<ChampionshipSessionResult> AddSessionToChampionshipAsync(long sessionId)
    {
        if (sessionId <= 0)
        {
            return ChampionshipSessionResult.InvalidSession;
        }

        var result = ChampionshipSessionResult.SessionNotFound;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var sessionData = sessionQuery == null
                                  ? null
                                  : await sessionQuery.Select(s => new SessionBaseData
                                                                   {
                                                                       SessionId = s.Id,
                                                                       SessionType = s.SessionType,
                                                                       TrackId = s.TrackId,
                                                                       GameVersionId = s.GameVersionId,
                                                                       FormulaType = s.FormulaType
                                                                   })
                                                      .FirstOrDefaultAsync(s => s.SessionId == sessionId)
                                                      .ConfigureAwait(false);

            if (sessionData != null)
            {
                var championshipQuery = dbFactory.GetRepository<ChampionshipRepository>()?.GetQuery();

                var championship = championshipQuery == null
                                       ? null
                                       : await championshipQuery.FirstOrDefaultAsync(c => c.GameVersionId == sessionData.GameVersionId
                                                                                          && c.DbIsFinished == 0)
                                                                .ConfigureAwait(false);

                await AdjustSessionTypeAsync(sessionData, dbFactory).ConfigureAwait(false);

                result = await SaveSessionToChampionshipAsync(dbFactory, sessionData, championship).ConfigureAwait(false);
            }
        }

        return result;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Loads the tracks of all given championships with a single query
    /// </summary>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="dbChampionships">Championships whose tracks are loaded</param>
    /// <returns>Tracks per database id of the championship</returns>
    private async Task<Dictionary<long, List<ChampionshipTrackEntity>>> LoadChampionshipTracksAsync(RepositoryFactory dbFactory, List<ChampionshipEntity> dbChampionships)
    {
        var championshipTracks = new Dictionary<long, List<ChampionshipTrackEntity>>();

        // Only the track and the session ids are read, so the auto-included sessions are not needed
        var championshipTrackQuery = dbFactory.GetRepository<ChampionshipTrackRepository>()?.GetQuery(ignoreAutoIncludes: true);

        if (championshipTrackQuery == null)
        {
            return championshipTracks;
        }

        var championshipIds = dbChampionships.Select(c => c.Id)
                                             .ToList();

        var tracks = await championshipTrackQuery.Where(t => championshipIds.Contains(t.ChampionshipId))
                                                 .OrderBy(t => t.Id)
                                                 .ToListAsync()
                                                 .ConfigureAwait(false);

        foreach (var track in tracks)
        {
            if (championshipTracks.TryGetValue(track.ChampionshipId, out var tracksOfChampionship) == false)
            {
                tracksOfChampionship = [];

                championshipTracks[track.ChampionshipId] = tracksOfChampionship;
            }

            tracksOfChampionship.Add(track);
        }

        return championshipTracks;
    }

    /// <summary>
    /// Loads the finish position of the human controlled participant of every session referenced by the given tracks
    /// </summary>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="championshipTracks">Tracks per database id of the championship</param>
    /// <returns>Finish position per database id of the session</returns>
    private async Task<Dictionary<long, int>> LoadGridPositionsAsync(RepositoryFactory dbFactory, Dictionary<long, List<ChampionshipTrackEntity>> championshipTracks)
    {
        var gridPositions = new Dictionary<long, int>();

        var sessionIds = CollectSessionIds(championshipTracks);

        if (sessionIds.Count == 0)
        {
            return gridPositions;
        }

        var participantQuery = dbFactory.GetRepository<ParticipantRepository>()?.GetQuery(ignoreAutoIncludes: true);
        var finalQuery = dbFactory.GetRepository<FinalClassificationRepository>()?.GetQuery();

        if (participantQuery == null || finalQuery == null)
        {
            return gridPositions;
        }

        var humanParticipants = await participantQuery.Where(p => sessionIds.Contains(p.SessionId)
                                                                  && p.DbIsHumanControlled == 1)
                                                      .OrderBy(p => p.Id)
                                                      .Select(p => new
                                                                   {
                                                                       p.SessionId,
                                                                       p.Id
                                                                   })
                                                      .ToListAsync()
                                                      .ConfigureAwait(false);

        var humanParticipantPerSession = new Dictionary<long, long>();

        foreach (var humanParticipant in humanParticipants)
        {
            humanParticipantPerSession.TryAdd(humanParticipant.SessionId, humanParticipant.Id);
        }

        if (humanParticipantPerSession.Count == 0)
        {
            return gridPositions;
        }

        var humanParticipantIds = humanParticipantPerSession.Values.ToList();

        var finishPositions = await finalQuery.Where(f => sessionIds.Contains(f.SessionId)
                                                          && humanParticipantIds.Contains(f.ParticipantId))
                                              .Select(f => new
                                                           {
                                                               f.SessionId,
                                                               f.ParticipantId,
                                                               f.FinishPosition
                                                           })
                                              .ToListAsync()
                                              .ConfigureAwait(false);

        foreach (var finishPosition in finishPositions)
        {
            if (humanParticipantPerSession.TryGetValue(finishPosition.SessionId, out var participantId)
                && participantId == finishPosition.ParticipantId)
            {
                gridPositions.TryAdd(finishPosition.SessionId, finishPosition.FinishPosition);
            }
        }

        return gridPositions;
    }

    /// <summary>
    /// Collects the database ids of all sessions referenced by the given championship tracks
    /// </summary>
    /// <param name="championshipTracks">Tracks per database id of the championship</param>
    /// <returns>Database ids of the referenced sessions</returns>
    private List<long> CollectSessionIds(Dictionary<long, List<ChampionshipTrackEntity>> championshipTracks)
    {
        var sessionIds = new HashSet<long>();

        foreach (var track in championshipTracks.Values.SelectMany(tracks => tracks))
        {
            AddSessionId(sessionIds, track.QualifyingSessionId);
            AddSessionId(sessionIds, track.SprintQualifyingSessionId);
            AddSessionId(sessionIds, track.SprintSessionId);
            AddSessionId(sessionIds, track.RaceSessionId);
        }

        return sessionIds.ToList();
    }

    /// <summary>
    /// Adds the database id of a session to the given set when it references an existing session
    /// </summary>
    /// <param name="sessionIds">Set the database id is added to</param>
    /// <param name="sessionId">Database id of the session</param>
    private void AddSessionId(HashSet<long> sessionIds, long? sessionId)
    {
        if (sessionId > 0)
        {
            sessionIds.Add(sessionId.Value);
        }
    }

    /// <summary>
    /// Fills the tracks of a championship from the already loaded tracks and grid positions
    /// </summary>
    /// <param name="championshipData">Championship data</param>
    /// <param name="championshipTracks">Tracks per database id of the championship</param>
    /// <param name="gridPositions">Finish position per database id of the session</param>
    private void LoadTracks(ChampionshipViewData championshipData,
                            Dictionary<long, List<ChampionshipTrackEntity>> championshipTracks,
                            Dictionary<long, int> gridPositions)
    {
        if (championshipTracks.TryGetValue(championshipData.ChampionshipId, out var tracks) == false
            || tracks.Count == 0)
        {
            return;
        }

        championshipData.Tracks = [];

        foreach (var track in tracks)
        {
            var trackData = new ChampionshipTrackViewData
                            {
                                ChampionshipTrackId = track.TrackId,
                                QualifyingPosition = GetGridPosition(gridPositions, track.QualifyingSessionId),
                                SprintQualifyingPosition = GetGridPosition(gridPositions, track.SprintQualifyingSessionId),
                                SprintPosition = GetGridPosition(gridPositions, track.SprintSessionId),
                                RacePosition = GetGridPosition(gridPositions, track.RaceSessionId)
                            };

            CalculatePoints(trackData);

            championshipData.Tracks.Add(trackData);
        }
    }

    /// <summary>
    /// Add tracks to championship
    /// </summary>
    /// <param name="tracks">Tracks</param>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="championship">Championship</param>
    /// <returns>Task</returns>
    private async Task AddTracksToChampionshipAsync(List<long> tracks, RepositoryFactory dbFactory, ChampionshipEntity championship)
    {
        var trackQuery = dbFactory.GetRepository<TrackRepository>()?.GetQuery();
        var championshipTrackRepository = dbFactory.GetRepository<ChampionshipTrackRepository>();

        foreach (var track in tracks)
        {
            var dbTrack = trackQuery == null
                              ? null
                              : await trackQuery.FirstOrDefaultAsync(t => t.TrackNumber == track)
                                                .ConfigureAwait(false);

            if (dbTrack != null)
            {
                if (championshipTrackRepository != null)
                {
                    await championshipTrackRepository.AddAsync(new ChampionshipTrackEntity
                                                               {
                                                                   ChampionshipId = championship.Id,
                                                                   TrackId = dbTrack.Id
                                                               })
                                                     .ConfigureAwait(false);
                }
            }
            else
            {
                _logger?.UnknownTrackNumber(track);
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
    /// <returns>Result of the assignment</returns>
    private async Task<ChampionshipSessionResult> SaveSessionToChampionshipAsync(RepositoryFactory dbFactory, SessionBaseData sessionData, ChampionshipEntity? championship)
    {
        var result = ChampionshipSessionResult.TrackNotInChampionship;

        if (championship != null)
        {
            var championshipTrackRepository = dbFactory.GetRepository<ChampionshipTrackRepository>();
            var championshipTrackQuery = championshipTrackRepository?.GetQuery();

            var championshipTrack = championshipTrackQuery == null
                                        ? null
                                        : await championshipTrackQuery.FirstOrDefaultAsync(t => t.TrackId == sessionData.TrackId
                                                                                                && t.ChampionshipId == championship.Id)
                                                                      .ConfigureAwait(false);

            if (championshipTrack != null
                && SetChampionShipSession(sessionData, championshipTrack))
            {
                var isRefreshed = championshipTrackRepository != null
                                  && await championshipTrackRepository.RefreshAsync(ct => ct.Id == championshipTrack.Id,
                                                                                    obj =>
                                                                                    {
                                                                                        obj.QualifyingSessionId = championshipTrack.QualifyingSessionId;
                                                                                        obj.SprintQualifyingSessionId = championshipTrack.SprintQualifyingSessionId;
                                                                                        obj.RaceSessionId = championshipTrack.RaceSessionId;
                                                                                        obj.SprintSessionId = championshipTrack.SprintSessionId;
                                                                                    })
                                                                      .ConfigureAwait(false);

                result = isRefreshed ? ChampionshipSessionResult.Added : ChampionshipSessionResult.NotAdded;
            }
        }

        return result;
    }

    /// <summary>
    /// Adjusts the session type of the specified session based on the presence of prior sprint qualifying sessions
    /// </summary>
    /// <param name="session">The session data to be adjusted. This parameter cannot be null</param>
    /// <param name="dbFactory">The repository factory used to query session data. This parameter cannot be null</param>
    /// <returns>Task</returns>
    private async Task AdjustSessionTypeAsync(SessionBaseData session, RepositoryFactory dbFactory)
    {
        if (session.SessionType is SessionType.Race)
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var hasSprintQualifyings = sessionQuery != null
                                       && await sessionQuery.AnyAsync(s => s.Id < session.SessionId
                                                                           && s.SessionType >= SessionType.SprintShootout1
                                                                           && s.SessionType <= SessionType.OneShotSprintShootout
                                                                           && s.GameVersionId == session.GameVersionId
                                                                           && s.TrackId == session.TrackId)
                                                            .ConfigureAwait(false);

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
    /// <param name="gridPositions">Finish position per database id of the session</param>
    /// <param name="sessionId">
    /// The identifier of the session for which the grid position is being retrieved. Must be greater than 0 to perform
    /// the lookup; otherwise, the method returns 0
    /// </param>
    /// <returns>
    /// The grid position of the human-controlled participant in the specified session. Returns 0 if no human-controlled
    /// participant is found or if <paramref name="sessionId"/> is null or less than 1
    /// </returns>
    private int GetGridPosition(Dictionary<long, int> gridPositions, long? sessionId)
    {
        return sessionId > 0 && gridPositions.TryGetValue(sessionId.Value, out var gridPosition)
                   ? gridPosition
                   : 0;
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

    #endregion // Private methods
}