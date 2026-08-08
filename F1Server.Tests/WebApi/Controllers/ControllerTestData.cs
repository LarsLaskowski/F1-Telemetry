using F1Server.Core.Enumerations;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Creates and provides the entity graph the controller tests read through the Web API controllers
/// </summary>
internal static class ControllerTestData
{
    #region Constants

    /// <summary>
    /// Lap time in milliseconds of the fastest lap of the test session
    /// </summary>
    public const uint FastestLapTime = 81000U;

    /// <summary>
    /// Lap time in milliseconds of the slower lap of the test session
    /// </summary>
    public const uint SlowerLapTime = 83500U;

    /// <summary>
    /// Reference lap time in milliseconds of the track of the test session
    /// </summary>
    public const uint ReferenceLapTime = 80000U;

    /// <summary>
    /// Name of the driver steering the human controlled participant of the test session
    /// </summary>
    public const string HumanDriverName = "Controller Test Human Driver";

    /// <summary>
    /// Name of the driver steering the computer controlled participant of the test session
    /// </summary>
    public const string AiDriverName = "Controller Test AI Driver";

    /// <summary>
    /// Name of the team of the participants of the test session
    /// </summary>
    public const string TeamName = "Controller Test Team";

    /// <summary>
    /// Name of the nationality of the participants of the test session
    /// </summary>
    public const string NationalityName = "Controller Test Nationality";

    /// <summary>
    /// Name of the track of the test session
    /// </summary>
    public const string TrackName = "Controller Test Track";

    /// <summary>
    /// Name of the game version of the test session
    /// </summary>
    public const string GameVersionName = "Controller Test Game";

    /// <summary>
    /// Track number of the track of the test session
    /// </summary>
    public const int TrackNumber = 771;

    /// <summary>
    /// Finish position of the human controlled participant in the final classification of the test session
    /// </summary>
    public const int HumanFinishPosition = 1;

    /// <summary>
    /// Number of participants of the test session
    /// </summary>
    public const int ParticipantCount = 2;

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Gates the creation of the entity graph, so parallel test classes only create it once
    /// </summary>
    private static readonly SemaphoreSlim _creationLock = new(1, 1);

    /// <summary>
    /// Game session code of the last session created by this class
    /// </summary>
    private static long _lastSessionCode = 771000000000L;

    /// <summary>
    /// Game id of the last driver created by this class
    /// </summary>
    private static int _lastDriverGameId = 771000;

    /// <summary>
    /// Track number of the last track created by this class
    /// </summary>
    private static int _lastTrackNumber = 772000;

    /// <summary>
    /// Is the entity graph already created?
    /// </summary>
    private static bool _isCreated;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Database id of the finished race session all controller tests read
    /// </summary>
    public static long SessionId { get; private set; }

    /// <summary>
    /// Game session code of the finished race session all controller tests read
    /// </summary>
    public static ulong SessionCode { get; private set; }

    /// <summary>
    /// Database id of the track of the test session
    /// </summary>
    public static long TrackId { get; private set; }

    /// <summary>
    /// Database id of the game version of the test session
    /// </summary>
    public static long GameVersionId { get; private set; }

    /// <summary>
    /// Database id of the human controlled participant of the test session
    /// </summary>
    public static long HumanParticipantId { get; private set; }

    /// <summary>
    /// Database id of the driver of the human controlled participant of the test session
    /// </summary>
    public static long HumanDriverId { get; private set; }

    /// <summary>
    /// Database id of the fastest lap of the test session
    /// </summary>
    public static long FastestLapId { get; private set; }

    /// <summary>
    /// Database id of the nationality of the participants of the test session
    /// </summary>
    public static long NationalityId { get; private set; }

    /// <summary>
    /// Database id of the team of the participants of the test session
    /// </summary>
    public static long TeamId { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Creates the entity graph of the controller tests once
    /// </summary>
    /// <returns>Task</returns>
    public static async Task EnsureCreatedAsync()
    {
        if (_isCreated)
        {
            return;
        }

        await _creationLock.WaitAsync().ConfigureAwait(false);

        try
        {
            if (_isCreated)
            {
                return;
            }

            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                CreateBaseData(dbFactory);

                SessionId = AddSession(dbFactory, SessionType.Race, isFinished: true, GameVersionId, TrackId, Formula.F1Modern, out var sessionCode);

                SessionCode = sessionCode;

                HumanParticipantId = AddParticipant(dbFactory, SessionId, HumanDriverName, isHumanControlled: true, out var humanDriverId);

                HumanDriverId = humanDriverId;

                var aiParticipantId = AddParticipant(dbFactory, SessionId, AiDriverName, isHumanControlled: false, out _);

                FastestLapId = AddLap(dbFactory, SessionId, HumanParticipantId, 1, FastestLapTime);

                AddTelemetry(dbFactory, FastestLapId);

                AddLap(dbFactory, SessionId, aiParticipantId, 1, SlowerLapTime);

                AddFinalClassification(dbFactory, SessionId, HumanParticipantId, HumanFinishPosition, FastestLapTime);
                AddFinalClassification(dbFactory, SessionId, aiParticipantId, HumanFinishPosition + 1, SlowerLapTime);
            }

            _isCreated = true;
        }
        finally
        {
            _creationLock.Release();
        }
    }

    /// <summary>
    /// Adds a session with a participant, a lap and a telemetry row, so a test can delete the whole graph without
    /// touching the shared test session
    /// </summary>
    /// <param name="sessionCode">Game session code of the created session</param>
    /// <returns>Database id of the created session</returns>
    public static long AddDeletableSession(out ulong sessionCode)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionId = AddSession(dbFactory, SessionType.Race, isFinished: true, GameVersionId, TrackId, Formula.F1Modern, out sessionCode);

            var participantId = AddParticipant(dbFactory, sessionId, "Controller Test Delete Driver", isHumanControlled: true, out _);

            var lapId = AddLap(dbFactory, sessionId, participantId, 1, FastestLapTime);

            AddTelemetry(dbFactory, lapId);

            return sessionId;
        }
    }

    /// <summary>
    /// Adds an unfinished championship together with a championship track for the track of the test session
    /// </summary>
    /// <param name="gameVersionId">Database id of the game version of the championship</param>
    /// <param name="qualifyingSessionId">Database id of the session already assigned as qualifying session</param>
    /// <returns>Database id of the created championship</returns>
    public static long AddChampionship(long gameVersionId, long? qualifyingSessionId = null)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var championshipEntity = new ChampionshipEntity
                                     {
                                         GameVersionId = gameVersionId,
                                         Mode = ChampionshipMode.Career,
                                         Number = 1
                                     };

            Assert.IsTrue(dbFactory.GetRepository<ChampionshipRepository>()?.Add(championshipEntity), "Championship entity could not be added to the database!");

            var championshipTrackEntity = new ChampionshipTrackEntity
                                          {
                                              ChampionshipId = championshipEntity.Id,
                                              TrackId = TrackId,
                                              QualifyingSessionId = qualifyingSessionId
                                          };

            Assert.IsTrue(dbFactory.GetRepository<ChampionshipTrackRepository>()?.Add(championshipTrackEntity), "Championship track entity could not be added to the database!");

            return championshipEntity.Id;
        }
    }

    /// <summary>
    /// Adds a game version, so a test can work on a game version that carries no other data
    /// </summary>
    /// <param name="version">Version of the game</param>
    /// <returns>Database id of the created game version</returns>
    public static long AddGameVersion(int version)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var gameVersionEntity = new GameVersionEntity
                                    {
                                        Version = version,
                                        Name = $"Controller Test Game {version}",
                                        MajorVersion = 1,
                                        MinorVersion = 0
                                    };

            Assert.IsTrue(dbFactory.GetRepository<GameVersionRepository>()?.Add(gameVersionEntity), "Game version entity could not be added to the database!");

            return gameVersionEntity.Id;
        }
    }

    /// <summary>
    /// Adds a session of the given type for the track of the test data
    /// </summary>
    /// <param name="sessionType">Type of the session</param>
    /// <param name="isFinished">Is the session finished?</param>
    /// <param name="gameVersionId">Database id of the game version of the session, or <see langword="null"/> to use the game version of the test data</param>
    /// <returns>Database id of the created session</returns>
    public static long AddSession(SessionType sessionType, bool isFinished, long? gameVersionId = null)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            return AddSession(dbFactory, sessionType, isFinished, gameVersionId ?? GameVersionId, TrackId, Formula.F1Modern, out _);
        }
    }

    /// <summary>
    /// Adds a session of the given formula type together with a participant and one completed lap
    /// </summary>
    /// <param name="sessionType">Type of the session</param>
    /// <param name="formulaType">Formula type of the session</param>
    /// <param name="trackId">Database id of the track of the session</param>
    /// <param name="lapTime">Lap time in milliseconds of the lap of the session</param>
    /// <returns>Database id of the created session</returns>
    public static long AddSessionWithLap(SessionType sessionType, Formula formulaType, long trackId, uint lapTime)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionId = AddSession(dbFactory, sessionType, isFinished: true, GameVersionId, trackId, formulaType, out _);

            var participantId = AddParticipant(dbFactory, sessionId, HumanDriverName, isHumanControlled: true, out _);

            AddLap(dbFactory, sessionId, participantId, 1, lapTime);

            return sessionId;
        }
    }

    /// <summary>
    /// Adds a finished qualifying session together with a human controlled participant and its final classification
    /// </summary>
    /// <param name="finishPosition">Finish position of the human controlled participant</param>
    /// <returns>Database id of the created session</returns>
    public static long AddSessionWithFinalClassification(int finishPosition)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionId = AddSession(dbFactory, SessionType.Qualifying3, isFinished: true, GameVersionId, TrackId, Formula.F1Modern, out _);

            var participantId = AddParticipant(dbFactory, sessionId, HumanDriverName, isHumanControlled: true, out _);

            AddFinalClassification(dbFactory, sessionId, participantId, finishPosition, FastestLapTime);

            return sessionId;
        }
    }

    /// <summary>
    /// Adds a track and returns its database id
    /// </summary>
    /// <returns>Database id of the created track</returns>
    public static long AddTrack()
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var trackNumber = Interlocked.Increment(ref _lastTrackNumber);

            var trackEntity = new TrackEntity
                              {
                                  TrackNumber = trackNumber,
                                  Name = $"Controller Test Track {trackNumber}",
                                  LapReferenceTime = ReferenceLapTime
                              };

            Assert.IsTrue(dbFactory.GetRepository<TrackRepository>()?.Add(trackEntity), "Track entity could not be added to the database!");

            return trackEntity.Id;
        }
    }

    /// <summary>
    /// Adds tracks to the test database, so a test does not depend on the tracks created by the sample sessions
    /// </summary>
    /// <param name="count">Number of tracks to add</param>
    /// <returns>Track numbers of the created tracks</returns>
    public static List<long> AddTracks(int count)
    {
        var trackNumbers = new List<long>();

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var trackRepository = dbFactory.GetRepository<TrackRepository>();

            Assert.IsNotNull(trackRepository, "Track repository should be resolvable!");

            for (var trackIndex = 0; trackIndex < count; trackIndex++)
            {
                var trackNumber = Interlocked.Increment(ref _lastTrackNumber);

                var trackEntity = new TrackEntity
                                  {
                                      TrackNumber = trackNumber,
                                      Name = $"Controller Test Track {trackNumber}",
                                      LapReferenceTime = ReferenceLapTime
                                  };

                Assert.IsTrue(trackRepository.Add(trackEntity), "Track entity could not be added to the database!");

                trackNumbers.Add(trackNumber);
            }
        }

        return trackNumbers;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Creates the track, game version, nationality and team the test session refers to
    /// </summary>
    /// <param name="dbFactory">Repository factory used to store the entities</param>
    private static void CreateBaseData(RepositoryFactory dbFactory)
    {
        var trackEntity = new TrackEntity
                          {
                              TrackNumber = TrackNumber,
                              Name = TrackName,
                              LapReferenceTime = ReferenceLapTime,
                              Sector1ReferenceTime = 26000,
                              Sector2ReferenceTime = 18000,
                              Sector3ReferenceTime = 36000
                          };

        Assert.IsTrue(dbFactory.GetRepository<TrackRepository>()?.Add(trackEntity), "Track entity could not be added to the database!");

        var gameVersionEntity = new GameVersionEntity
                                {
                                    Version = 3771,
                                    Name = GameVersionName,
                                    MajorVersion = 1,
                                    MinorVersion = 7
                                };

        Assert.IsTrue(dbFactory.GetRepository<GameVersionRepository>()?.Add(gameVersionEntity), "Game version entity could not be added to the database!");

        var nationalityEntity = new NationalityEntity
                                {
                                    NationalityGameId = 771,
                                    Name = NationalityName
                                };

        Assert.IsTrue(dbFactory.GetRepository<NationalityRepository>()?.Add(nationalityEntity), "Nationality entity could not be added to the database!");

        var teamEntity = new TeamEntity
                         {
                             TeamGameId = 771002,
                             Name = TeamName
                         };

        Assert.IsTrue(dbFactory.GetRepository<TeamRepository>()?.Add(teamEntity), "Team entity could not be added to the database!");

        TrackId = trackEntity.Id;
        GameVersionId = gameVersionEntity.Id;
        NationalityId = nationalityEntity.Id;
        TeamId = teamEntity.Id;
    }

    /// <summary>
    /// Adds a session together with its session attributes
    /// </summary>
    /// <param name="dbFactory">Repository factory used to store the entities</param>
    /// <param name="sessionType">Type of the session</param>
    /// <param name="isFinished">Is the session finished?</param>
    /// <param name="gameVersionId">Database id of the game version of the session</param>
    /// <param name="trackId">Database id of the track of the session</param>
    /// <param name="formulaType">Formula type of the session</param>
    /// <param name="sessionCode">Game session code of the created session</param>
    /// <returns>Database id of the created session</returns>
    private static long AddSession(RepositoryFactory dbFactory,
                                   SessionType sessionType,
                                   bool isFinished,
                                   long gameVersionId,
                                   long trackId,
                                   Formula formulaType,
                                   out ulong sessionCode)
    {
        sessionCode = (ulong)Interlocked.Increment(ref _lastSessionCode);

        var sessionEntity = new SessionEntity
                            {
                                SessionId = sessionCode,
                                CreationTimestamp = DateTime.UtcNow,
                                SessionType = sessionType,
                                FormulaType = formulaType,
                                TrackId = trackId,
                                GameVersionId = gameVersionId,
                                ActiveCars = ParticipantCount,
                                AiDifficulty = 95,
                                DbIsFinished = isFinished
                                                   ? 1
                                                   : 0
                            };

        Assert.IsTrue(dbFactory.GetRepository<SessionRepository>()?.Add(sessionEntity), "Session entity could not be added to the database!");

        // The session list joins the attributes, so every session needs an attribute row to show up
        var sessionAttributesEntity = new SessionAttributesEntity
                                      {
                                          SessionId = sessionEntity.Id,
                                          WeatherStart = WeatherCondition.Clear,
                                          WeatherEnd = WeatherCondition.Clear
                                      };

        Assert.IsTrue(dbFactory.GetRepository<SessionAttributesRepository>()?.Add(sessionAttributesEntity), "Session attributes entity could not be added to the database!");

        return sessionEntity.Id;
    }

    /// <summary>
    /// Adds a participant together with its driver
    /// </summary>
    /// <param name="dbFactory">Repository factory used to store the entities</param>
    /// <param name="sessionId">Database id of the session of the participant</param>
    /// <param name="driverName">Name of the driver of the participant</param>
    /// <param name="isHumanControlled">Is the participant human controlled?</param>
    /// <param name="driverId">Database id of the created driver</param>
    /// <returns>Database id of the created participant</returns>
    private static long AddParticipant(RepositoryFactory dbFactory, long sessionId, string driverName, bool isHumanControlled, out long driverId)
    {
        var driverEntity = new DriverEntity
                           {
                               DriverGameId = Interlocked.Increment(ref _lastDriverGameId),
                               Name = driverName
                           };

        Assert.IsTrue(dbFactory.GetRepository<DriverRepository>()?.Add(driverEntity), "Driver entity could not be added to the database!");

        var participantEntity = new ParticipantEntity
                                {
                                    SessionId = sessionId,
                                    DriverId = driverEntity.Id,
                                    NationalityId = NationalityId,
                                    TeamId = TeamId,
                                    DriverName = driverName,
                                    CarRaceNumber = 7,
                                    ArrayIndex = 1,
                                    DbIsHumanControlled = isHumanControlled
                                                              ? 1
                                                              : 0
                                };

        Assert.IsTrue(dbFactory.GetRepository<ParticipantRepository>()?.Add(participantEntity), "Participant entity could not be added to the database!");

        driverId = driverEntity.Id;

        return participantEntity.Id;
    }

    /// <summary>
    /// Adds a completed and valid lap
    /// </summary>
    /// <param name="dbFactory">Repository factory used to store the entity</param>
    /// <param name="sessionId">Database id of the session of the lap</param>
    /// <param name="participantId">Database id of the participant of the lap</param>
    /// <param name="lapNumber">Number of the lap</param>
    /// <param name="lapTime">Lap time in milliseconds</param>
    /// <returns>Database id of the created lap</returns>
    private static long AddLap(RepositoryFactory dbFactory, long sessionId, long participantId, ushort lapNumber, uint lapTime)
    {
        var lapEntity = new LapEntity
                        {
                            SessionId = sessionId,
                            ParticipantId = participantId,
                            LapNumber = lapNumber,
                            LapTime = lapTime,
                            Sector1Time = lapTime / 3U,
                            Sector2Time = lapTime / 3U,
                            Sector3Time = lapTime - (2U * (lapTime / 3U)),
                            CarPosition = lapNumber,
                            DbIsCompleted = 1,
                            DbIsInvalid = 0,
                            DbIsInvalidLapTime = 0,
                            DriverStatus = DriverStatus.FlyingLap,
                            PitStatus = PitStatus.None,
                            ResultStatus = ResultStatus.Active
                        };

        Assert.IsTrue(dbFactory.GetRepository<LapRepository>()?.Add(lapEntity), "Lap entity could not be added to the database!");

        return lapEntity.Id;
    }

    /// <summary>
    /// Adds a telemetry row for a lap
    /// </summary>
    /// <param name="dbFactory">Repository factory used to store the entity</param>
    /// <param name="lapId">Database id of the lap of the telemetry row</param>
    private static void AddTelemetry(RepositoryFactory dbFactory, long lapId)
    {
        var telemetryEntity = new CarTelemetryEntity
                              {
                                  LapNumberId = lapId,
                                  PacketNumber = 1,
                                  Speed = 271,
                                  Gear = 7,
                                  Throttle = 1,
                                  Brake = 0,
                                  EngineRPM = 11500,
                                  LapDistance = 1200
                              };

        Assert.IsTrue(dbFactory.GetRepository<CarTelemetryRepository>()?.Add(telemetryEntity), "Telemetry entity could not be added to the database!");
    }

    /// <summary>
    /// Adds a final classification row for a participant
    /// </summary>
    /// <param name="dbFactory">Repository factory used to store the entity</param>
    /// <param name="sessionId">Database id of the session of the final classification</param>
    /// <param name="participantId">Database id of the participant of the final classification</param>
    /// <param name="finishPosition">Finish position of the participant</param>
    /// <param name="fastestLapTime">Fastest lap time of the participant in milliseconds</param>
    private static void AddFinalClassification(RepositoryFactory dbFactory, long sessionId, long participantId, int finishPosition, uint fastestLapTime)
    {
        var finalClassificationEntity = new FinalClassificationEntity
                                        {
                                            SessionId = sessionId,
                                            ParticipantId = participantId,
                                            GridPosition = finishPosition,
                                            FinishPosition = finishPosition,
                                            LapsDriven = 1,
                                            PitStops = 1,
                                            ResultStatus = ResultStatus.Finished,
                                            FastestLapTime = fastestLapTime,
                                            TotalRaceTime = fastestLapTime / 1000.0,
                                            PenaltiesTime = 0,
                                            NumberOfPenalties = 0
                                        };

        Assert.IsTrue(dbFactory.GetRepository<FinalClassificationRepository>()?.Add(finalClassificationEntity), "Final classification entity could not be added to the database!");
    }

    #endregion // Private methods
}