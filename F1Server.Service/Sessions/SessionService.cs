using F1Server.Core.Enumerations;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache;
using F1Server.Service.Core;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Service.Sessions;

/// <summary>
/// Business logic of the sessions - reads the stored sessions with their sprint race detection, the fastest laps and
/// the time table of a session and removes a session with all its data
/// </summary>
public class SessionService
{
    #region Methods

    /// <summary>
    /// Loads the finished sessions with their track, game version and weather, the races that belong to a sprint
    /// weekend are reported as sprint race
    /// </summary>
    /// <returns>Finished sessions ordered by the newest session</returns>
    public async Task<List<SessionViewData>> GetSessionsAsync()
    {
        List<SessionViewData> sessions = [];

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var attrQuery = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery()?.Select(obj => obj);
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            if (attrQuery != null && sessionQuery != null)
            {
                sessions = await sessionQuery.Join(attrQuery,
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
                                             .ToListAsync()
                                             .ConfigureAwait(false);

                AdjustSessionTypes(sessions);
            }
        }

        return sessions;
    }

    /// <summary>
    /// Counts the finished sessions
    /// </summary>
    /// <returns>Number of all finished sessions</returns>
    public async Task<int> GetSessionsCountAsync()
    {
        var numSessions = 0;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            if (sessionQuery != null)
            {
                numSessions = await sessionQuery.CountAsync(s => s.DbIsFinished == 1)
                                                .ConfigureAwait(false);
            }
        }

        return numSessions;
    }

    /// <summary>
    /// Determines the newest finished session
    /// </summary>
    /// <returns>Database id of the newest finished session, or zero when no session is finished</returns>
    public async Task<long> GetLastFinishedSessionAsync()
    {
        var lastFinishedSession = 0L;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var session = sessionQuery == null
                              ? null
                              : await sessionQuery.Where(s => s.DbIsFinished == 1 && s.FormulaType != Formula.SuperCars)
                                                  .OrderByDescending(s => s.Id)
                                                  .FirstOrDefaultAsync()
                                                  .ConfigureAwait(false);

            if (session != null)
            {
                lastFinishedSession = session.Id;
            }
        }

        return lastFinishedSession;
    }

    /// <summary>
    /// Loads a single session with its track, game version and weather
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Session data, or <see langword="null"/> when the session is unknown</returns>
    public async Task<SessionViewData?> GetSessionAsync(long sessionId)
    {
        SessionViewData? session = null;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var attrQuery = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery();
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            if (attrQuery != null && sessionQuery != null)
            {
                var dbSession = await sessionQuery.Include(s => s.GameVersion)
                                                  .Include(s => s.Track)
                                                  .Join(attrQuery,
                                                        obj => obj.Id,
                                                        obj => obj.SessionId,
                                                        (obj1, obj2) => new
                                                                        {
                                                                            Session = obj1,
                                                                            obj2.WeatherStart
                                                                        })
                                                  .FirstOrDefaultAsync(s => s.Session.Id == sessionId)
                                                  .ConfigureAwait(false);

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

                    await AdjustSessionTypeAsync(session, dbFactory).ConfigureAwait(false);
                }
            }
        }

        return session;
    }

    /// <summary>
    /// Loads the sessions driven on a track
    /// </summary>
    /// <param name="trackId">Database id of the track</param>
    /// <returns>Sessions of the track, or <see langword="null"/> when the track carries no sessions</returns>
    public async Task<List<SessionViewData>?> GetSessionsOfTrackAsync(long? trackId)
    {
        List<SessionViewData>? sessions = null;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var attrQuery = dbFactory.GetRepository<SessionAttributesRepository>()?.GetQuery();
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            if (attrQuery != null && sessionQuery != null)
            {
                var dbSessions = await sessionQuery.Include(s => s.GameVersion)
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
                                                   .ToListAsync()
                                                   .ConfigureAwait(false);

                if (dbSessions.Count > 0)
                {
                    sessions = [];

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
        }

        return sessions;
    }

    /// <summary>
    /// Determines the fastest valid lap of a session
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Data of the fastest lap, the data stays empty when the session carries no valid lap</returns>
    public async Task<FastestLapViewData> GetFastestLapOfSessionAsync(long? sessionId)
    {
        FastestLapViewData fastestLapData = new();

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var dbSession = sessionQuery == null
                                ? null
                                : await sessionQuery.FirstOrDefaultAsync(s => s.Id == sessionId)
                                                    .ConfigureAwait(false);

            var lapQuery = dbFactory.GetRepository<LapRepository>()?.GetQuery();

            if (dbSession != null && lapQuery != null)
            {
                var fastestLap = await GetValidLapsOfSessionQuery(lapQuery, sessionId).FirstOrDefaultAsync()
                                                                                      .ConfigureAwait(false);

                if (fastestLap != null)
                {
                    fastestLapData = CreateFastestLapData(fastestLap);
                }
            }
        }

        return fastestLapData;
    }

    /// <summary>
    /// Loads the valid laps of a session ordered by their lap time
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Laps of the session ordered by their lap time</returns>
    public async Task<List<FastestLapViewData>> GetFastestLapsOfSessionAsync(long? sessionId)
    {
        List<FastestLapViewData> fastestLapsList = [];

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var dbSession = sessionQuery == null
                                ? null
                                : await sessionQuery.Include(s => s.Participants)
                                                    .FirstOrDefaultAsync(s => s.Id == sessionId)
                                                    .ConfigureAwait(false);

            if (dbSession != null)
            {
                var lapQuery = dbFactory.GetRepository<LapRepository>()?.GetQuery();

                List<LapEntity> fastestLaps = [];

                if (lapQuery != null)
                {
                    fastestLaps = await GetValidLapsOfSessionQuery(lapQuery, sessionId).ToListAsync()
                                                                                       .ConfigureAwait(false);
                }

                foreach (var fastestLap in fastestLaps)
                {
                    fastestLapsList.Add(CreateFastestLapData(fastestLap));
                }
            }
        }

        return fastestLapsList;
    }

    /// <summary>
    /// Loads the drivers of a session together with their final classification
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Time table of the session</returns>
    public async Task<SessionTimeTableViewData> GetSessionTimeTableAsync(long? sessionId)
    {
        var sessionTimeTable = new SessionTimeTableViewData();

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var dbSession = sessionQuery == null
                                ? null
                                : await sessionQuery.Include(s => s.Participants)
                                                    .FirstOrDefaultAsync(s => s.Id == sessionId)
                                                    .ConfigureAwait(false);

            if (dbSession != null)
            {
                var finalClassifications = await LoadFinalClassificationsAsync(dbFactory, dbSession.Participants).ConfigureAwait(false);

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

                    if (finalClassifications.TryGetValue(attendee.Id, out var dbFinal))
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
                                                           TotalRaceTime = RaceTimeFormatter.FormatTotalRaceTime(dbFinal.TotalRaceTime),
                                                           FastestLapTime = TimeSpan.FromMilliseconds(dbFinal.FastestLapTime).ToString(@"mm\:ss\.fff")
                                                       });
                    }
                }
            }
        }

        return sessionTimeTable;
    }

    /// <summary>
    /// Removes a session including its participants, laps and telemetry
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <param name="sessionCode">Game session code of the session</param>
    /// <returns>Status whether the session was removed</returns>
    public async Task<bool> DeleteSessionAsync(long sessionId, ulong sessionCode)
    {
        var isDeleted = false;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionRepository = dbFactory.GetRepository<SessionRepository>();
            var sessionQuery = sessionRepository?.GetQuery();

            var session = sessionQuery == null
                              ? null
                              : await sessionQuery.FirstOrDefaultAsync(s => s.Id == sessionId && s.SessionId == sessionCode)
                                                  .ConfigureAwait(false);

            if (session != null)
            {
                var isSessionContentRemoved = await RemoveSessionContentAsync(dbFactory, session.Id).ConfigureAwait(false);

                var isSessionRemoved = sessionRepository != null
                                       && await sessionRepository.RemoveAsync(s => s.Id == session.Id)
                                                                 .ConfigureAwait(false);

                // No data of a removed session may stay in the cache
                FastestLapPerSessionCache.RemoveSession(session.Id);

                isDeleted = isSessionRemoved && isSessionContentRemoved;
            }
        }

        return isDeleted;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Builds the query of the completed laps of a session that carry a valid lap time, ordered by their lap time
    /// </summary>
    /// <param name="lapQuery">Query of the stored laps</param>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Query of the valid laps of the session</returns>
    private static IQueryable<LapEntity> GetValidLapsOfSessionQuery(IQueryable<LapEntity> lapQuery, long? sessionId)
    {
        return lapQuery.Include(l => l.Participant)
                       .Where(l => l.SessionId == sessionId && l.LapTime > 0 && l.DbIsCompleted == 1 && l.DbIsInvalidLapTime == 0)
                       .OrderBy(l => l.LapTime);
    }

    /// <summary>
    /// Creates the fastest lap data of a lap together with its driver
    /// </summary>
    /// <param name="lap">Lap the data is created for</param>
    /// <returns>Fastest lap data of the lap</returns>
    private static FastestLapViewData CreateFastestLapData(LapEntity lap)
    {
        return new FastestLapViewData
               {
                   DriverName = lap.Participant.Driver.Name,
                   LapTime = lap.LapTime,
                   LapTimeSector1 = lap.Sector1Time,
                   LapTimeSector2 = lap.Sector2Time,
                   LapTimeSector3 = lap.Sector3Time,
                   LapNumber = lap.LapNumber,
                   CarPosition = lap.CarPosition,
                   SessionId = lap.SessionId,
                   LapId = lap.Id,
                   ParticipantId = lap.ParticipantId,
                   DriverId = lap.Participant.DriverId
               };
    }

    /// <summary>
    /// Removes the participants, laps and telemetry data belonging to the given session
    /// </summary>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Status whether all content of the session was removed</returns>
    private static async Task<bool> RemoveSessionContentAsync(RepositoryFactory dbFactory, long sessionId)
    {
        // Get participants of session
        var participantRepository = dbFactory.GetRepository<ParticipantRepository>();
        var participantQuery = participantRepository?.GetQuery();

        List<long> participants = [];

        if (participantQuery != null)
        {
            participants = await participantQuery.Where(p => p.SessionId == sessionId)
                                                 .Select(p => p.Id)
                                                 .ToListAsync()
                                                 .ConfigureAwait(false);
        }

        if (participants.Count == 0)
        {
            return true;
        }

        var isLapDataRemoved = await RemoveLapDataAsync(dbFactory, participants).ConfigureAwait(false);

        var isParticipantsRemoved = participantRepository != null
                                    && await participantRepository.RemoveRangeAsync(p => participants.Contains(p.Id))
                                                                  .ConfigureAwait(false);

        return isParticipantsRemoved && isLapDataRemoved;
    }

    /// <summary>
    /// Removes the laps of the given participants including their telemetry data
    /// </summary>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="participants">Database ids of the participants</param>
    /// <returns>Status whether all laps and their telemetry data were removed</returns>
    private static async Task<bool> RemoveLapDataAsync(RepositoryFactory dbFactory, List<long> participants)
    {
        // Get laps of participants
        var lapRepository = dbFactory.GetRepository<LapRepository>();
        var lapQuery = lapRepository?.GetQuery();

        List<long> laps = [];

        if (lapQuery != null)
        {
            laps = await lapQuery.Where(l => participants.Contains(l.ParticipantId))
                                 .Select(l => l.Id)
                                 .ToListAsync()
                                 .ConfigureAwait(false);
        }

        if (laps.Count == 0)
        {
            return true;
        }

        // Get telemetry data
        var telemetryRepository = dbFactory.GetRepository<CarTelemetryRepository>();

        var isTelemetryRemoved = telemetryRepository != null
                                 && await telemetryRepository.RemoveRangeAsync(t => laps.Contains(t.LapNumberId))
                                                             .ConfigureAwait(false);

        var isLapsRemoved = lapRepository != null
                            && await lapRepository.RemoveRangeAsync(l => laps.Contains(l.Id))
                                                  .ConfigureAwait(false);

        return isLapsRemoved && isTelemetryRemoved;
    }

    /// <summary>
    /// Marks the race that precedes a second race at the same track as sprint race
    /// </summary>
    /// <param name="sessions">Sessions to be analyzed and adjusted</param>
    private static void MarkSprintRacesBeforeSecondRaces(List<SessionViewData> sessions)
    {
        // All races with session type 'Race2'
        var postSprintRaces = sessions.Where(s => s.SessionType == SessionType.Race2)
                                      .ToList();

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

    /// <summary>
    /// Marks all races that are preceded by a sprint qualifying at the same track as sprint race
    /// </summary>
    /// <param name="sessions">Sessions to be analyzed and adjusted</param>
    private static void MarkSprintRacesWithSprintQualifying(List<SessionViewData> sessions)
    {
        var races = sessions.Where(s => s.SessionType == SessionType.Race)
                            .ToList();

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

    /// <summary>
    /// Adjusts the session types for sprint races based on their relationship to subsequent races
    /// </summary>
    /// <param name="sessions">A list of <see cref="SessionViewData"/> objects representing the sessions to be analyzed and adjusted</param>
    private static void AdjustSessionTypes(List<SessionViewData>? sessions)
    {
        if (sessions == null)
        {
            return;
        }

        MarkSprintRacesBeforeSecondRaces(sessions);

        MarkSprintRacesWithSprintQualifying(sessions);
    }

    /// <summary>
    /// Adjusts the session type of the specified session based on the presence of prior sprint qualifying sessions
    /// </summary>
    /// <param name="session">The session data to be adjusted. This parameter cannot be null</param>
    /// <param name="dbFactory">The repository factory used to query session data. This parameter cannot be null</param>
    /// <returns>Task</returns>
    private static async Task AdjustSessionTypeAsync(SessionViewData session, RepositoryFactory dbFactory)
    {
        var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

        if (sessionQuery == null)
        {
            return;
        }

        var hasSprintQualifyings = await sessionQuery.AnyAsync(s => s.Id < session.SessionDbId
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

    /// <summary>
    /// Loads the final classifications of the given participants with a single query
    /// </summary>
    /// <param name="dbFactory">Database factory</param>
    /// <param name="participants">Participants of the session</param>
    /// <returns>Final classification per database id of the participant</returns>
    private static async Task<Dictionary<long, FinalClassificationEntity>> LoadFinalClassificationsAsync(RepositoryFactory dbFactory, ICollection<ParticipantEntity> participants)
    {
        var finalClassifications = new Dictionary<long, FinalClassificationEntity>();

        var finalQuery = dbFactory.GetRepository<FinalClassificationRepository>()?.GetQuery();

        if (finalQuery == null || participants.Count == 0)
        {
            return finalClassifications;
        }

        var participantIds = participants.Select(p => p.Id)
                                         .ToList();

        var dbFinals = await finalQuery.Where(f => participantIds.Contains(f.ParticipantId))
                                       .ToListAsync()
                                       .ConfigureAwait(false);

        foreach (var dbFinal in dbFinals)
        {
            finalClassifications.TryAdd(dbFinal.ParticipantId, dbFinal);
        }

        return finalClassifications;
    }

    #endregion // Private methods
}