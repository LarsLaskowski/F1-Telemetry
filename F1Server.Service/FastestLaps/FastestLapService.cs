using System.Diagnostics;
using System.Linq.Expressions;

using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Service.FastestLaps;

/// <summary>
/// Business logic of the fastest laps - determines the fastest practice, qualifying and race lap of a track per
/// formula type and provides the cached fastest lap data of a session
/// </summary>
public class FastestLapService
{
    #region Constants

    /// <summary>
    /// Prefix of the activity started for the fastest lap of a single type of session
    /// </summary>
    private const string SessionTypeActivityPrefix = "FastestLapsOfSessions-";

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Session types of a practice
    /// </summary>
    private static readonly List<SessionType> _practiceSessions = [
                                                                      SessionType.Practice1,
                                                                      SessionType.Practice2,
                                                                      SessionType.Practice3,
                                                                      SessionType.ShortPractice
                                                                  ];

    /// <summary>
    /// Session types of a qualifying
    /// </summary>
    private static readonly List<SessionType> _qualifyingSessions = [
                                                                        SessionType.Qualifying1,
                                                                        SessionType.Qualifying2,
                                                                        SessionType.Qualifying3,
                                                                        SessionType.ShortQualifying,
                                                                        SessionType.OneShotQualifying
                                                                    ];

    /// <summary>
    /// Session types of a race
    /// </summary>
    private static readonly List<SessionType> _raceSessions = [
                                                                  SessionType.Race,
                                                                  SessionType.Race2,
                                                                  SessionType.Race3
                                                              ];

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Determines the fastest practice, qualifying and race lap of a track for the formula one and the formula two
    /// sessions driven on it
    /// </summary>
    /// <param name="trackId">Database id of the track</param>
    /// <returns>Fastest laps of the track</returns>
    public async Task<List<FastestLapOfTrackViewData>> GetFastestLapsOfTrackAsync(long trackId)
    {
        var fastestLaps = new List<FastestLapOfTrackViewData>();

        var fastestF1Laps = await GetFastestF1LapsAsync(trackId).ConfigureAwait(false);
        var fastestF2Laps = await GetFastestF2LapsAsync(trackId).ConfigureAwait(false);

        if (fastestF1Laps?.Count > 0)
        {
            fastestLaps.AddRange(fastestF1Laps);
        }

        if (fastestF2Laps?.Count > 0)
        {
            fastestLaps.AddRange(fastestF2Laps);
        }

        return fastestLaps;
    }

    /// <summary>
    /// Provides the cached fastest lap data of a session
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Fastest lap data of the session</returns>
    public async Task<FastestLapSessionViewData> GetFastestLapDataOfSessionAsync(long sessionId)
    {
        return await FastestLapPerSessionCache.GetFastestLapDataForSessionAsync(sessionId).ConfigureAwait(false);
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Determines the session types a type of fastest lap is searched in
    /// </summary>
    /// <param name="lapSessionType">Type of the fastest lap</param>
    /// <returns>Session types of the type of fastest lap</returns>
    private static List<SessionType> GetSessionTypes(FastestLapSessionType lapSessionType)
    {
        return lapSessionType switch
               {
                   FastestLapSessionType.Practice => _practiceSessions,
                   FastestLapSessionType.Qualifying => _qualifyingSessions,
                   _ => _raceSessions
               };
    }

    /// <summary>
    /// Get fastest laps in F1
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>List with fastest sessions laps</returns>
    private static async Task<List<FastestLapOfTrackViewData>?> GetFastestF1LapsAsync(long trackId)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(GetFastestF1LapsAsync));

        var fastestLaps = await GetFastestLapsOfFormulaAsync(trackId,
                                                             s => s.TrackId == trackId
                                                                  && (s.FormulaType == Formula.F1Modern
                                                                      || s.FormulaType == Formula.F12026),
                                                             Formula.F1Modern).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        return fastestLaps;
    }

    /// <summary>
    /// Get fastest laps in F2
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>List with fastest session laps</returns>
    private static async Task<List<FastestLapOfTrackViewData>?> GetFastestF2LapsAsync(long trackId)
    {
        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(GetFastestF2LapsAsync));

        var fastestLaps = await GetFastestLapsOfFormulaAsync(trackId,
                                                             s => s.TrackId == trackId
                                                                  && (s.FormulaType == Formula.F2
                                                                      || s.FormulaType == Formula.F2TwentyOne),
                                                             Formula.F2).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        return fastestLaps;
    }

    /// <summary>
    /// Get the fastest laps of the sessions of a track that belong to the given formula type
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <param name="sessionFilter">Filter selecting the sessions of the formula type</param>
    /// <param name="formulaType">Formula type reported for the fastest laps</param>
    /// <returns>List with fastest session laps</returns>
    private static async Task<List<FastestLapOfTrackViewData>?> GetFastestLapsOfFormulaAsync(long trackId,
                                                                                             Expression<Func<SessionEntity, bool>> sessionFilter,
                                                                                             Formula formulaType)
    {
        List<FastestLapOfTrackViewData>? fastestLaps = null;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var dbSessions = sessionQuery == null
                                 ? null
                                 : await sessionQuery.Where(sessionFilter)
                                                     .ToListAsync()
                                                     .ConfigureAwait(false);

            if (dbSessions?.Count > 0)
            {
                fastestLaps = await GetFastestLapsOfSessionsAsync(trackId, dbFactory, dbSessions, formulaType).ConfigureAwait(false);
            }
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
    private static async Task<List<FastestLapOfTrackViewData>?> GetFastestLapsOfSessionsAsync(long trackId, RepositoryFactory dbFactory, List<SessionEntity> dbSessions, Formula formulaType)
    {
        List<FastestLapOfTrackViewData>? fastestLaps = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(GetFastestLapsOfSessionsAsync));

        var trackQuery = dbFactory.GetRepository<TrackRepository>()?.GetQuery();

        var trackData = trackQuery == null
                            ? null
                            : await trackQuery.FirstOrDefaultAsync(t => t.Id == trackId)
                                              .ConfigureAwait(false);

        // The fastest laps are reported in the declaration order of the lap session types
        foreach (var lapSessionType in Enum.GetValues<FastestLapSessionType>())
        {
            using (AppActivity.SrvSource.StartActivity($"{SessionTypeActivityPrefix}{lapSessionType}"))
            {
                var fastestSessionLap = await FastestLapInSessionsAsync(dbFactory, dbSessions, GetSessionTypes(lapSessionType)).ConfigureAwait(false);

                var fastestLap = await GetFastestLapDataAsync(dbFactory, fastestSessionLap, lapSessionType).ConfigureAwait(false);

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
    private static async Task<FastestLapOfTrackViewData?> GetFastestLapDataAsync(RepositoryFactory dbFactory, LapEntity? fastestLap, FastestLapSessionType sessionType)
    {
        FastestLapOfTrackViewData? fastestLapData = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(GetFastestLapDataAsync));

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
    private static async Task<LapEntity?> FastestLapInSessionsAsync(RepositoryFactory dbFactory, List<SessionEntity> dbSessions, List<SessionType> sessionTypes)
    {
        LapEntity? fastestLap = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(FastestLapInSessionsAsync));

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
    private static async Task<(bool HasData, long GameVersionId, string GameVersionName)> GetGameDataOfSessionAsync(RepositoryFactory dbFactory, long sessionId)
    {
        var hasData = false;
        var gameVersionId = 0L;
        var gameVersionName = string.Empty;

        using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(GetGameDataOfSessionAsync));

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