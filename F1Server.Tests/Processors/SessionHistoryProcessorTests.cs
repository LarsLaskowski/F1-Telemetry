using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;
using F1Server.Tests.Data;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the session history processor class
/// </summary>
[TestClass]
public class SessionHistoryProcessorTests
{
    #region Constants

    /// <summary>
    /// Unique game session id used by the tests in this class
    /// </summary>
    private const ulong TestSessionUniqueId = 419419419419UL;

    /// <summary>
    /// Unique game session id used by the unfinished lap test
    /// </summary>
    private const ulong TestSessionUniqueId2 = 419419419420UL;

    /// <summary>
    /// Car index of the test participant in the game packet arrays
    /// </summary>
    private const ushort TestCarIndex = 7;

    /// <summary>
    /// Car index of the test participant of the unfinished lap test
    /// </summary>
    private const ushort TestCarIndex2 = 8;

    #endregion // Constants

    #region Test methods

    /// <summary>
    /// A lap completed through the normal lap data path must not be inserted a second time
    /// when a following session history packet reports the same lap again
    /// </summary>
    [TestMethod]
    public void SessionHistoryProcessorCompletedLapIsNotDuplicated()
    {
        var (sessionDbId, participantDbId) = CreateTestEntities(419001, 419, 419002, TestSessionUniqueId, TestCarIndex);

        var sessionRuntimeData = new SessionRuntimeData(2025, TestSessionUniqueId, SessionType.Race)
                                 {
                                     HasParticipants = true,
                                     IsRecordable = true,
                                     CurrentSession = new LiveSessionData
                                                      {
                                                          DbId = sessionDbId,
                                                          SessionGameId = TestSessionUniqueId,
                                                          SessionType = SessionType.Race
                                                      }
                                 };

        var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData)
                                     {
                                         IsValidObject = true,
                                         ParticipantDbId = participantDbId,
                                         ArrayIndex = TestCarIndex
                                     };

        Assert.IsTrue(sessionRuntimeData.Participants.TryAdd(TestCarIndex, participantRuntimeData), "Participant runtime data could not be registered!");

        var packetHeader = new PacketHeader
                           {
                               GameVersion = 2025,
                               PacketType = PacketTypes.SessionHistory,
                               UniqueSessionId = TestSessionUniqueId,
                               PlayerCarIndex = TestCarIndex
                           };

        // The processor is created once per session (before any lap is completed), like in the packet processing flow
        var sessionHistoryProcessor = new SessionHistoryProcessor(TestData.ServiceProvider,
                                                                  packetHeader,
                                                                  new LiveGameData
                                                                  {
                                                                      GameVersion = 2025
                                                                  });

        var lapEntity = new LapEntity
                        {
                            LapNumber = 1,
                            ParticipantId = participantDbId,
                            SessionId = sessionDbId,
                            DriverStatus = DriverStatus.OnTrack,
                            PitStatus = PitStatus.None,
                            ResultStatus = ResultStatus.Active
                        };

        Assert.IsTrue(participantRuntimeData.AddLap(lapEntity), "Lap could not be added to the participant runtime data!");

        lapEntity.LapTime = 90000;
        lapEntity.Sector1Time = 30000;
        lapEntity.Sector2Time = 30000;
        lapEntity.Sector3Time = 30000;
        lapEntity.IsCompleted = true;
        lapEntity.IsFinished = true;

        Assert.IsTrue(participantRuntimeData.CompleteLap(lapEntity.LapNumber), "Lap could not be completed!");

        AssertSingleLapRow(participantDbId, "The completed lap must exist exactly once before the session history packet is processed!");

        var sessionHistory = new SessionHistoryData2025
                             {
                                 CarIndex = TestCarIndex,
                                 NumberOfLaps = 1
                             };

        sessionHistory.LapHistory[0] = new SessionHistoryLapData2025
                                       {
                                           LapTime = 90000,
                                           Sector1Time = 30000,
                                           Sector2Time = 30000,
                                           Sector3Time = 30000,
                                           LapValidFlag = 0x0F
                                       };

        var sessionHistoryData = new SessionHistoryData(packetHeader, sessionHistory);

        var isProcessed = sessionHistoryProcessor.Process(sessionHistoryData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Session history packet not correctly processed!");

        AssertSingleLapRow(participantDbId, "The completed lap was inserted a second time by the session history processor!");

        // A repeated history packet for the same lap must not create another row either
        isProcessed = sessionHistoryProcessor.Process(sessionHistoryData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Repeated session history packet not correctly processed!");

        AssertSingleLapRow(participantDbId, "The completed lap was duplicated by a repeated session history packet!");
    }

    /// <summary>
    /// An unfinished lap with complete times is completed and an inconsistent lap is invalidated after the final classification
    /// </summary>
    [TestMethod]
    public void SessionHistoryProcessorUnfinishedLapsAreCompletedAndInvalidated()
    {
        var (sessionDbId, participantDbId) = CreateTestEntities(419003, 4191, 419004, TestSessionUniqueId2, TestCarIndex2);

        var sessionRuntimeData = new SessionRuntimeData(2025, TestSessionUniqueId2, SessionType.Race)
                                 {
                                     HasParticipants = true,
                                     IsRecordable = true,
                                     FinalClassificationReceived = true,
                                     CurrentSession = new LiveSessionData
                                                      {
                                                          DbId = sessionDbId,
                                                          SessionGameId = TestSessionUniqueId2,
                                                          SessionType = SessionType.Race
                                                      }
                                 };

        var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData)
                                     {
                                         IsValidObject = true,
                                         ParticipantDbId = participantDbId,
                                         ArrayIndex = TestCarIndex2
                                     };

        Assert.IsTrue(sessionRuntimeData.Participants.TryAdd(TestCarIndex2, participantRuntimeData), "Participant runtime data could not be registered!");

        var packetHeader = new PacketHeader
                           {
                               GameVersion = 2025,
                               PacketType = PacketTypes.SessionHistory,
                               UniqueSessionId = TestSessionUniqueId2,
                               PlayerCarIndex = TestCarIndex2
                           };

        var sessionHistoryProcessor = new SessionHistoryProcessor(TestData.ServiceProvider,
                                                                  packetHeader,
                                                                  new LiveGameData
                                                                  {
                                                                      GameVersion = 2025
                                                                  });

        var lapOne = new LapEntity
                     {
                         LapNumber = 1,
                         ParticipantId = participantDbId,
                         SessionId = sessionDbId
                     };

        Assert.IsTrue(participantRuntimeData.AddLap(lapOne), "The first lap could not be added to the participant runtime data!");

        var lapTwo = new LapEntity
                     {
                         LapNumber = 2,
                         ParticipantId = participantDbId,
                         SessionId = sessionDbId
                     };

        Assert.IsTrue(participantRuntimeData.AddLap(lapTwo), "The second lap could not be added to the participant runtime data!");

        var sessionHistory = new SessionHistoryData2025
                             {
                                 CarIndex = TestCarIndex2,
                                 NumberOfLaps = 2
                             };

        sessionHistory.LapHistory[0] = new SessionHistoryLapData2025
                                       {
                                           LapTime = 90000,
                                           Sector1Time = 30000,
                                           Sector2Time = 30000,
                                           Sector3Time = 30000,
                                           LapValidFlag = 0x0F
                                       };

        sessionHistory.LapHistory[1] = new SessionHistoryLapData2025
                                       {
                                           LapTime = 90000,
                                           Sector1Time = 40000,
                                           Sector2Time = 40000,
                                           Sector3Time = 40000,
                                           LapValidFlag = 0x0F
                                       };

        var sessionHistoryData = new SessionHistoryData(packetHeader, sessionHistory);

        var isProcessed = sessionHistoryProcessor.Process(sessionHistoryData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Session history packet not correctly processed!");
        Assert.IsNull(participantRuntimeData.GetLap(1), "The lap with consistent times should be removed from the unfinished laps!");
        Assert.IsNotNull(participantRuntimeData.GetLap(2), "The lap with inconsistent times should stay in the unfinished laps!");
        Assert.IsTrue(lapTwo.IsInvalid, "The lap with inconsistent times should be marked as invalid after the final classification!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var completedLaps = dbFactory.GetRepository<LapRepository>()
                                         ?.GetQuery()
                                         ?.Count(l => l.ParticipantId == participantDbId && l.DbIsCompleted == 1) ?? -1;

            Assert.AreEqual(1, completedLaps, "Exactly the lap with consistent times should be completed in the database!");
        }
    }

    #endregion // Test methods

    #region Methods

    /// <summary>
    /// Creates the driver, nationality, team, session and participant entities used by a test
    /// </summary>
    /// <param name="driverGameId">Game id of the driver</param>
    /// <param name="nationalityGameId">Game id of the nationality</param>
    /// <param name="teamGameId">Game id of the team</param>
    /// <param name="sessionUniqueId">Unique game session id</param>
    /// <param name="carIndex">Car index of the participant in the game packet arrays</param>
    /// <returns>Tuple with the database ids of the created session and participant</returns>
    private static (long SessionDbId, long ParticipantDbId) CreateTestEntities(int driverGameId, ushort nationalityGameId, int teamGameId, ulong sessionUniqueId, ushort carIndex)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var driverEntity = new DriverEntity
                               {
                                   DriverGameId = driverGameId,
                                   Name = "Test Driver"
                               };

            Assert.IsTrue(dbFactory.GetRepository<DriverRepository>()?.Add(driverEntity), "Driver entity could not be added to the database!");

            var nationalityEntity = new NationalityEntity
                                    {
                                        NationalityGameId = nationalityGameId,
                                        Name = "Test Nationality"
                                    };

            Assert.IsTrue(dbFactory.GetRepository<NationalityRepository>()?.Add(nationalityEntity), "Nationality entity could not be added to the database!");

            var teamEntity = new TeamEntity
                             {
                                 TeamGameId = teamGameId,
                                 Name = "Test Team"
                             };

            Assert.IsTrue(dbFactory.GetRepository<TeamRepository>()?.Add(teamEntity), "Team entity could not be added to the database!");

            var sessionEntity = new SessionEntity
                                {
                                    SessionId = sessionUniqueId,
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
                                        DriverName = "Test Driver",
                                        ArrayIndex = carIndex
                                    };

            Assert.IsTrue(dbFactory.GetRepository<ParticipantRepository>()?.Add(participantEntity), "Participant entity could not be added to the database!");

            return (sessionEntity.Id, participantEntity.Id);
        }
    }

    /// <summary>
    /// Asserts that exactly one lap row exists for the test participant and lap number 1
    /// </summary>
    /// <param name="participantDbId">Database id of the test participant</param>
    /// <param name="message">Assert message shown when the row count is wrong</param>
    private static void AssertSingleLapRow(long participantDbId, string message)
    {
        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var lapRows = dbFactory.GetRepository<LapRepository>()
                                   ?.GetQuery()
                                   ?.Where(l => l.ParticipantId == participantDbId && l.LapNumber == 1)
                                   .ToList();

            Assert.IsNotNull(lapRows, "Lap query returned no result!");
            Assert.HasCount(1, lapRows, message);
        }
    }

    #endregion // Methods
}