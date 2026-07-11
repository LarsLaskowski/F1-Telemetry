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

        Assert.HasCount(3, storedRows, "All rows of the batch should be stored with the lap id of the batch!");
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

        Assert.HasCount(2, storedRows, "The writer should resolve the lap id from the database and store all rows of the batch!");
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

        Assert.HasCount(4, storedRows, "All pending batches should be written before the shutdown completes!");
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

        Assert.HasCount(1, storedRows, "The buffered telemetry row should be stored for the completed lap!");
    }

    /// <summary>
    /// Verifies that the consumer keeps processing jobs after a failing job
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task DatabaseWriterConsumerContinuesAfterFailingJob()
    {
        var lapEntity = CreateLap(5);

        DatabaseWriter.Enqueue(new ThrowingJob());
        DatabaseWriter.Enqueue(new InsertTelemetryBatchJob
                               {
                                   LapDbId = lapEntity.Id,
                                   LapNumber = lapEntity.LapNumber,
                                   ParticipantDbId = lapEntity.ParticipantId,
                                   Rows = CreateTelemetryRows(1)
                               });

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        var storedRows = GetStoredTelemetryRows(lapEntity.Id);

        Assert.HasCount(1, storedRows, "Jobs enqueued after a failing job should still be executed!");
    }

    /// <summary>
    /// Verifies that a batch of an unknown lap is discarded without storing rows
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task DatabaseWriterInsertTelemetryBatchJobDiscardsBatchOfUnknownLap()
    {
        var telemetryEntity = new CarTelemetryEntity
                              {
                                  PacketNumber = 421421,
                                  Speed = 200
                              };

        DatabaseWriter.Enqueue(new InsertTelemetryBatchJob
                               {
                                   LapDbId = 0,
                                   LapNumber = 99,
                                   ParticipantDbId = 421999,
                                   Rows = [telemetryEntity]
                               });

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var storedRows = dbFactory.GetRepository<CarTelemetryRepository>()
                                      ?.GetQuery()
                                      ?.Count(t => t.PacketNumber == 421421) ?? -1;

            Assert.AreEqual(0, storedRows, "A batch of an unknown lap should be discarded!");
        }
    }

    /// <summary>
    /// Verifies that an empty batch is ignored without any database work
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task DatabaseWriterInsertTelemetryBatchJobIgnoresEmptyBatch()
    {
        DatabaseWriter.Enqueue(new InsertTelemetryBatchJob
                               {
                                   LapDbId = 0,
                                   LapNumber = 98,
                                   ParticipantDbId = 421998,
                                   Rows = []
                               });

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        Assert.IsTrue(DatabaseWriter.IsRunning, "The consumer task should still be running after an empty batch!");
    }

    /// <summary>
    /// Verifies that a null job neither starts the consumer nor blocks a following flush
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task DatabaseWriterEnqueueIgnoresNullJob()
    {
        DatabaseWriter.Shutdown();

        DatabaseWriter.Enqueue(null!);

        Assert.IsFalse(DatabaseWriter.IsRunning, "A null job should not start the consumer task!");

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        Assert.IsFalse(DatabaseWriter.IsRunning, "Flushing without pending jobs should not start the consumer task!");
    }

    /// <summary>
    /// Verifies that telemetry data of an invalid participant stays buffered until the participant becomes valid
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ParticipantRuntimeDataCompleteTelemetryDataKeepsBufferOfInvalidParticipant()
    {
        var lapEntity = CreateLap(6);

        var sessionRuntimeData = new SessionRuntimeData(2025, TestSessionUniqueId, SessionType.Race);

        using (var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData))
        {
            participantRuntimeData.IsValidObject = false;
            participantRuntimeData.ParticipantDbId = lapEntity.ParticipantId;

            var telemetryEntity = new CarTelemetryEntity
                                  {
                                      PacketNumber = 1,
                                      Speed = 290
                                  };

            participantRuntimeData.AddTelemetryData(lapEntity.LapNumber, telemetryEntity);

            participantRuntimeData.CompleteTelemetryData(lapEntity.LapNumber);

            Assert.IsEmpty(GetStoredTelemetryRows(lapEntity.Id), "No rows may be written for an invalid participant!");

            participantRuntimeData.IsValidObject = true;

            participantRuntimeData.CompleteTelemetryData(lapEntity.LapNumber);
        }

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        var storedRows = GetStoredTelemetryRows(lapEntity.Id);

        Assert.HasCount(1, storedRows, "The buffered telemetry row should be written once the participant is valid!");
    }

    /// <summary>
    /// Verifies that completing telemetry of a still unfinished lap uses the in-memory lap id
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ParticipantRuntimeDataCompleteTelemetryDataUsesInMemoryLapId()
    {
        var sessionRuntimeData = new SessionRuntimeData(2025, TestSessionUniqueId, SessionType.Race);
        var lapDbId = 0L;

        using (var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData))
        {
            participantRuntimeData.IsValidObject = true;
            participantRuntimeData.ParticipantDbId = _participantDbId;

            var lapEntity = new LapEntity
                            {
                                LapNumber = 8,
                                ParticipantId = _participantDbId,
                                SessionId = _sessionDbId
                            };

            Assert.IsTrue(participantRuntimeData.AddLap(lapEntity), "The lap could not be added to the participant runtime data!");
            Assert.AreNotEqual(0, lapEntity.Id, "The lap id should be populated when the lap is added!");

            lapDbId = lapEntity.Id;

            var telemetryEntity = new CarTelemetryEntity
                                  {
                                      PacketNumber = 2,
                                      Speed = 310
                                  };

            participantRuntimeData.AddTelemetryData(lapEntity.LapNumber, telemetryEntity);

            participantRuntimeData.CompleteTelemetryData(lapEntity.LapNumber);
        }

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        var storedRows = GetStoredTelemetryRows(lapDbId);

        Assert.HasCount(1, storedRows, "The telemetry row should be stored with the in-memory lap id!");
    }

    /// <summary>
    /// Verifies that all buffered telemetry rows of a completed lap are persisted with the lap id of the lap
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    public async Task ParticipantRuntimeDataCompleteTelemetryDataPersistsAllBufferedRows()
    {
        var lapEntity = CreateLap(9);

        var sessionRuntimeData = new SessionRuntimeData(2025, TestSessionUniqueId, SessionType.Race);

        using (var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData))
        {
            participantRuntimeData.IsValidObject = true;
            participantRuntimeData.ParticipantDbId = lapEntity.ParticipantId;

            foreach (var telemetryEntity in CreateTelemetryRows(5))
            {
                participantRuntimeData.AddTelemetryData(lapEntity.LapNumber, telemetryEntity);
            }

            participantRuntimeData.CompleteTelemetryData(lapEntity.LapNumber);
        }

        await DatabaseWriter.FlushAsync().ConfigureAwait(false);

        var storedRows = GetStoredTelemetryRows(lapEntity.Id);

        Assert.HasCount(5, storedRows, "All buffered telemetry rows should be persisted with the lap id of the completed lap!");
    }

    /// <summary>
    /// Verifies that adding a new lap populates the lap id from the insert and stores exactly one row
    /// </summary>
    [TestMethod]
    public void ParticipantRuntimeDataAddLapPopulatesLapIdWithSingleRow()
    {
        var sessionRuntimeData = new SessionRuntimeData(2025, TestSessionUniqueId, SessionType.Race);

        using (var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData))
        {
            participantRuntimeData.IsValidObject = true;
            participantRuntimeData.ParticipantDbId = _participantDbId;

            var lapEntity = new LapEntity
                            {
                                LapNumber = 10,
                                ParticipantId = _participantDbId,
                                SessionId = _sessionDbId
                            };

            Assert.IsTrue(participantRuntimeData.AddLap(lapEntity), "The lap could not be added to the participant runtime data!");
            Assert.AreNotEqual(0, lapEntity.Id, "The lap id should be populated from the insert when the lap is added!");

            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                var storedLaps = dbFactory.GetRepository<LapRepository>()
                                          ?.GetQuery(ignoreAutoIncludes: true)
                                          ?.Where(l => l.ParticipantId == _participantDbId && l.LapNumber == 10)
                                          .ToList() ?? [];

                Assert.HasCount(1, storedLaps, "Exactly one lap row should exist for the added lap!");
                Assert.AreEqual(lapEntity.Id, storedLaps[0].Id, "The populated lap id should match the stored lap row!");
            }
        }
    }

    /// <summary>
    /// Verifies that removing an unfinished lap deletes the lap row after draining the writer
    /// </summary>
    [TestMethod]
    public void ParticipantRuntimeDataRemoveLapDeletesLapRow()
    {
        var sessionRuntimeData = new SessionRuntimeData(2025, TestSessionUniqueId, SessionType.Race);

        using (var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData))
        {
            participantRuntimeData.IsValidObject = true;
            participantRuntimeData.ParticipantDbId = _participantDbId;

            var lapEntity = new LapEntity
                            {
                                LapNumber = 7,
                                ParticipantId = _participantDbId,
                                SessionId = _sessionDbId
                            };

            Assert.IsTrue(participantRuntimeData.AddLap(lapEntity), "The lap could not be added to the participant runtime data!");

            var telemetryEntity = new CarTelemetryEntity
                                  {
                                      PacketNumber = 1,
                                      Speed = 300
                                  };

            participantRuntimeData.AddTelemetryData(lapEntity.LapNumber, telemetryEntity);

            Assert.IsTrue(participantRuntimeData.RemoveLap(lapEntity.LapNumber), "The lap could not be removed!");
        }

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var lapRows = dbFactory.GetRepository<LapRepository>()
                                   ?.GetQuery()
                                   ?.Count(l => l.ParticipantId == _participantDbId && l.LapNumber == 7) ?? -1;

            Assert.AreEqual(0, lapRows, "The removed lap should no longer exist in the database!");
        }
    }

    #endregion // Methods
}