using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Runtime;

namespace F1Server.Tests.Runtime;

/// <summary>
/// Tests for the background database writer and its telemetry batch job
/// </summary>
[TestClass]
public class DatabaseWriterTests
{
    #region Constants

    /// <summary>
    /// Unique game session id used by the tests in this class
    /// </summary>
    private const ulong TestSessionUniqueId = 421421421421UL;

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Database id of the session created for the tests in this class
    /// </summary>
    private static long _sessionDbId;

    /// <summary>
    /// Database id of the participant created for the tests in this class
    /// </summary>
    private static long _participantDbId;

    #endregion // Fields

    #region Static methods

    /// <summary>
    /// Creates the session and participant graph used by all tests in this class
    /// </summary>
    /// <param name="context">Test context</param>
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var driverEntity = new DriverEntity
                               {
                                   DriverGameId = 421001,
                                   Name = "Writer Test Driver"
                               };

            Assert.IsTrue(dbFactory.GetRepository<DriverRepository>()?.Add(driverEntity), "Driver entity could not be added to the database!");

            var nationalityEntity = new NationalityEntity
                                    {
                                        NationalityGameId = 421,
                                        Name = "Writer Test Nationality"
                                    };

            Assert.IsTrue(dbFactory.GetRepository<NationalityRepository>()?.Add(nationalityEntity), "Nationality entity could not be added to the database!");

            var teamEntity = new TeamEntity
                             {
                                 TeamGameId = 421002,
                                 Name = "Writer Test Team"
                             };

            Assert.IsTrue(dbFactory.GetRepository<TeamRepository>()?.Add(teamEntity), "Team entity could not be added to the database!");

            var sessionEntity = new SessionEntity
                                {
                                    SessionId = TestSessionUniqueId,
                                    CreationTimestamp = DateTime.UtcNow,
                                    SessionType = SessionType.Race,
                                    TrackId = 1,
                                    GameVersionId = 1
                                };

            Assert.IsTrue(dbFactory.GetRepository<SessionRepository>()?.Add(sessionEntity), "Session entity could not be added to the database!");

            var participantEntity = new ParticipantEntity
                                    {
                                        SessionId = sessionEntity.Id,
                                        DriverId = driverEntity.Id,
                                        NationalityId = nationalityEntity.Id,
                                        TeamId = teamEntity.Id,
                                        DriverName = "Writer Test Driver",
                                        ArrayIndex = 1
                                    };

            Assert.IsTrue(dbFactory.GetRepository<ParticipantRepository>()?.Add(participantEntity), "Participant entity could not be added to the database!");

            _sessionDbId = sessionEntity.Id;
            _participantDbId = participantEntity.Id;
        }
    }

    /// <summary>
    /// Creates and stores a lap of the test participant
    /// </summary>
    /// <param name="lapNumber">Number of the lap</param>
    /// <returns>The stored lap entity with its generated database id</returns>
    private static LapEntity CreateLap(ushort lapNumber)
    {
        var lapEntity = new LapEntity
                        {
                            LapNumber = lapNumber,
                            ParticipantId = _participantDbId,
                            SessionId = _sessionDbId,
                            DriverStatus = DriverStatus.FlyingLap,
                            PitStatus = PitStatus.None,
                            ResultStatus = ResultStatus.Active
                        };

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var isAdded = dbFactory.GetRepository<LapRepository>()?.Add(lapEntity) == true;

            Assert.IsTrue(isAdded, "Adding the test lap should succeed!");
        }

        return lapEntity;
    }

    /// <summary>
    /// Creates a list of telemetry rows for a batch
    /// </summary>
    /// <param name="rowCount">Number of rows to create</param>
    /// <returns>List of telemetry rows</returns>
    private static List<CarTelemetryEntity> CreateTelemetryRows(int rowCount)
    {
        var telemetryRows = new List<CarTelemetryEntity>(rowCount);

        for (var index = 0; index < rowCount; ++index)
        {
            telemetryRows.Add(new CarTelemetryEntity
                              {
                                  PacketNumber = index + 1,
                                  Speed = 250 + index,
                                  Gear = 7,
                                  Throttle = 1.0f
                              });
        }

        return telemetryRows;
    }

    /// <summary>
    /// Loads all stored telemetry rows of the given lap
    /// </summary>
    /// <param name="lapDbId">Database id of the lap</param>
    /// <returns>List of stored telemetry rows</returns>
    private static List<CarTelemetryEntity> GetStoredTelemetryRows(long lapDbId)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            return dbFactory.GetRepository<CarTelemetryRepository>()
                            ?.GetQuery()
                            ?.Where(t => t.LapNumberId == lapDbId)
                            .ToList() ?? [];
        }
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that an enqueued telemetry batch with a known lap id is written after a flush
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task DatabaseWriterInsertTelemetryBatchJobWritesRowsWithLapNumberId()
    {
        var lapEntity = CreateLap(1);

        DatabaseWriter.Enqueue(new InsertTelemetryBatchJob
                               {
                                   LapDbId = lapEntity.Id,
                                   LapNumber = lapEntity.LapNumber,
                                   ParticipantDbId = lapEntity.ParticipantId,
                                   Rows = CreateTelemetryRows(3)
                               });

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        var storedRows = GetStoredTelemetryRows(lapEntity.Id);

        Assert.AreEqual(3, storedRows.Count, "All rows of the batch should be stored with the lap id of the batch!");
    }

    /// <summary>
    /// Verifies that the writer resolves the lap id by participant and lap number when the batch has no lap id
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task DatabaseWriterInsertTelemetryBatchJobResolvesLapIdFromDatabase()
    {
        var lapEntity = CreateLap(2);

        DatabaseWriter.Enqueue(new InsertTelemetryBatchJob
                               {
                                   LapDbId = 0,
                                   LapNumber = lapEntity.LapNumber,
                                   ParticipantDbId = lapEntity.ParticipantId,
                                   Rows = CreateTelemetryRows(2)
                               });

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        var storedRows = GetStoredTelemetryRows(lapEntity.Id);

        Assert.AreEqual(2, storedRows.Count, "The writer should resolve the lap id from the database and store all rows of the batch!");
    }

    /// <summary>
    /// Verifies that a shutdown executes all pending jobs and stops the consumer task
    /// </summary>
    [TestMethod]
    public void DatabaseWriterShutdownDrainsPendingJobs()
    {
        var lapEntity = CreateLap(3);

        DatabaseWriter.Enqueue(new InsertTelemetryBatchJob
                               {
                                   LapDbId = lapEntity.Id,
                                   LapNumber = lapEntity.LapNumber,
                                   ParticipantDbId = lapEntity.ParticipantId,
                                   Rows = CreateTelemetryRows(4)
                               });

        DatabaseWriter.Shutdown();

        var storedRows = GetStoredTelemetryRows(lapEntity.Id);

        Assert.AreEqual(4, storedRows.Count, "All pending batches should be written before the shutdown completes!");
        Assert.IsFalse(DatabaseWriter.IsRunning, "The consumer task should be stopped after the shutdown!");
    }

    /// <summary>
    /// Verifies that completing telemetry data enqueues the buffered rows and the writer stores them
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ParticipantRuntimeDataCompleteTelemetryDataWritesBufferedRows()
    {
        var lapEntity = CreateLap(4);

        var sessionRuntimeData = new SessionRuntimeData(2025, TestSessionUniqueId, SessionType.Race)
                                 {
                                     CurrentSession = new LiveSessionData
                                                      {
                                                          DbId = _sessionDbId,
                                                          SessionGameId = TestSessionUniqueId,
                                                          SessionType = SessionType.Race
                                                      }
                                 };

        using (var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData))
        {
            participantRuntimeData.IsValidObject = true;
            participantRuntimeData.ParticipantDbId = lapEntity.ParticipantId;
            participantRuntimeData.IsNewTelemetry = true;

            var telemetryEntity = new CarTelemetryEntity
                                  {
                                      PacketNumber = 1,
                                      Speed = 280
                                  };

            participantRuntimeData.AddTelemetryData(lapEntity.LapNumber, telemetryEntity);

            participantRuntimeData.CompleteTelemetryData(lapEntity.LapNumber);

            Assert.IsFalse(participantRuntimeData.IsNewTelemetry, "Completing telemetry data should reset the new telemetry flag!");
        }

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        var storedRows = GetStoredTelemetryRows(lapEntity.Id);

        Assert.AreEqual(1, storedRows.Count, "The buffered telemetry row should be stored for the completed lap!");
    }

    #endregion // Methods
}