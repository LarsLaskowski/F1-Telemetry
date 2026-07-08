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
    /// Car index of the test participant in the game packet arrays
    /// </summary>
    private const ushort TestCarIndex = 7;

    #endregion // Constants

    #region Test methods

    /// <summary>
    /// A lap completed through the normal lap data path must not be inserted a second time
    /// when a following session history packet reports the same lap again
    /// </summary>
    [TestMethod]
    public void SessionHistoryProcessorCompletedLapIsNotDuplicated()
    {
        var sessionDbId = 0L;
        var participantDbId = 0L;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var driverEntity = new DriverEntity
                               {
                                   DriverGameId = 419001,
                                   Name = "Test Driver"
                               };

            Assert.IsTrue(dbFactory.GetRepository<DriverRepository>()?.Add(driverEntity), "Driver entity could not be added to the database!");

            var nationalityEntity = new NationalityEntity
                                    {
                                        NationalityGameId = 419,
                                        Name = "Test Nationality"
                                    };

            Assert.IsTrue(dbFactory.GetRepository<NationalityRepository>()?.Add(nationalityEntity), "Nationality entity could not be added to the database!");

            var teamEntity = new TeamEntity
                             {
                                 TeamGameId = 419002,
                                 Name = "Test Team"
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
                                        DriverName = "Test Driver",
                                        ArrayIndex = TestCarIndex
                                    };

            Assert.IsTrue(dbFactory.GetRepository<ParticipantRepository>()?.Add(participantEntity), "Participant entity could not be added to the database!");

            sessionDbId = sessionEntity.Id;
            participantDbId = participantEntity.Id;
        }

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

    #endregion // Test methods

    #region Methods

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