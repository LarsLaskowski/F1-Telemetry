using F1Server.Core;
using F1Server.Core.Data;
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
/// Class to test the final classification processor class
/// </summary>
[TestClass]
public class FinalClassificationProcessorTests
{
    #region Static methods

    /// <summary>
    /// Reads the F1 2025 final classification sample packet
    /// </summary>
    /// <param name="packetHeader">Parsed packet header</param>
    /// <returns>Final classification packet</returns>
    private static FinalClassificationData GetFinalClassification(out PacketHeader packetHeader)
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2025-FinalClassification.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing the final classification packet failed!");

        packetHeader = packetData.PacketHeader;

        var finalClassification = new PacketAnalyzer().GetFinalClassificationData(packetHeader, packetContent) as FinalClassificationData;

        Assert.IsNotNull(finalClassification, "The packet is not a final classification packet!");

        return finalClassification;
    }

    /// <summary>
    /// Creates a session runtime data object with participants
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="sessionDbId">Database id of the session</param>
    /// <param name="isFinished">Is the session finished?</param>
    /// <returns>Session runtime data</returns>
    private static SessionRuntimeData CreateSessionRuntimeData(PacketHeader packetHeader, long sessionDbId, bool isFinished)
    {
        return new SessionRuntimeData(2025, packetHeader.UniqueSessionId, SessionType.Race)
               {
                   HasParticipants = true,
                   SessionDbId = sessionDbId,
                   CurrentSession = new LiveSessionData
                                    {
                                        DbId = sessionDbId,
                                        SessionGameId = packetHeader.UniqueSessionId,
                                        SessionType = SessionType.Race,
                                        IsFinished = isFinished
                                    }
               };
    }

    /// <summary>
    /// Creates a final classification processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <returns>Final classification processor</returns>
    private static FinalClassificationProcessor CreateProcessor(PacketHeader packetHeader)
    {
        return new FinalClassificationProcessor(TestData.ServiceProvider,
                                                packetHeader,
                                                new LiveGameData
                                                {
                                                    GameVersion = 2025
                                                });
    }

    #endregion // Static methods

    #region Test methods

    /// <summary>
    /// A final classification packet of a finished session is processed after draining the background writer
    /// </summary>
    [TestMethod]
    public void FinalClassificationProcessorFinishedSessionIsProcessed()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2025-FinalClassification.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing the final classification packet failed!");

        var finalClassification = new PacketAnalyzer().GetFinalClassificationData(packetData.PacketHeader, packetContent) as FinalClassificationData;

        Assert.IsNotNull(finalClassification, "The packet is not a final classification packet!");

        var sessionRuntimeData = new SessionRuntimeData(2025, packetData.PacketHeader.UniqueSessionId, SessionType.Race)
                                 {
                                     HasParticipants = true,
                                     SessionDbId = 421700,
                                     CurrentSession = new LiveSessionData
                                                      {
                                                          DbId = 421700,
                                                          SessionGameId = packetData.PacketHeader.UniqueSessionId,
                                                          SessionType = SessionType.Race,
                                                          IsFinished = true
                                                      }
                                 };

        var finalClassificationProcessor = new FinalClassificationProcessor(TestData.ServiceProvider,
                                                                            packetData.PacketHeader,
                                                                            new LiveGameData
                                                                            {
                                                                                GameVersion = 2025
                                                                            });

        var isProcessed = finalClassificationProcessor.Process(finalClassification, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Final classification packet not correctly processed!");
    }

    /// <summary>
    /// The cleanup of a final classification packet removes invalid laps together with their telemetry
    /// </summary>
    [TestMethod]
    public void FinalClassificationProcessorInvalidLapsCleanupRemovesLapsAndTelemetry()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2025-FinalClassification.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing the final classification packet failed!");

        var finalClassification = new PacketAnalyzer().GetFinalClassificationData(packetData.PacketHeader, packetContent) as FinalClassificationData;

        Assert.IsNotNull(finalClassification, "The packet is not a final classification packet!");

        long invalidLapId;
        long validLapId;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var lapRepository = dbFactory.GetRepository<LapRepository>();
            var carTelemetryRepository = dbFactory.GetRepository<CarTelemetryRepository>();

            Assert.IsNotNull(lapRepository, "Lap repository should be resolvable!");
            Assert.IsNotNull(carTelemetryRepository, "Car telemetry repository should be resolvable!");

            var invalidLap = new LapEntity
                             {
                                 SessionId = 430400,
                                 ParticipantId = 430401,
                                 LapNumber = 1,
                                 IsInvalidLapTime = true
                             };

            var validLap = new LapEntity
                           {
                               SessionId = 430400,
                               ParticipantId = 430402,
                               LapNumber = 1,
                               IsInvalidLapTime = false
                           };

            Assert.IsTrue(lapRepository.Add(invalidLap), "Adding the invalid test lap should succeed!");
            Assert.IsTrue(lapRepository.Add(validLap), "Adding the valid test lap should succeed!");

            invalidLapId = invalidLap.Id;
            validLapId = validLap.Id;

            var invalidLapTelemetry = new CarTelemetryEntity
                                      {
                                          LapNumberId = invalidLapId,
                                          PacketNumber = 1
                                      };

            var validLapTelemetry = new CarTelemetryEntity
                                    {
                                        LapNumberId = validLapId,
                                        PacketNumber = 1
                                    };

            Assert.IsTrue(carTelemetryRepository.Add(invalidLapTelemetry), "Adding telemetry for the invalid lap should succeed!");
            Assert.IsTrue(carTelemetryRepository.Add(validLapTelemetry), "Adding telemetry for the valid lap should succeed!");
        }

        var sessionRuntimeData = new SessionRuntimeData(2025, packetData.PacketHeader.UniqueSessionId, SessionType.Race)
                                 {
                                     HasParticipants = true,
                                     SessionDbId = 430400,
                                     CurrentSession = new LiveSessionData
                                                      {
                                                          DbId = 430400,
                                                          SessionGameId = packetData.PacketHeader.UniqueSessionId,
                                                          SessionType = SessionType.Race,
                                                          IsFinished = true
                                                      }
                                 };

        var finalClassificationProcessor = new FinalClassificationProcessor(TestData.ServiceProvider,
                                                                            packetData.PacketHeader,
                                                                            new LiveGameData
                                                                            {
                                                                                GameVersion = 2025
                                                                            });

        var isProcessed = finalClassificationProcessor.Process(finalClassification, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Final classification packet not correctly processed!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var remainingLapIds = dbFactory.GetRepository<LapRepository>()
                                           ?.GetQuery()
                                           ?.Where(l => l.SessionId == 430400)
                                           .Select(l => l.Id)
                                           .ToList();

            Assert.IsNotNull(remainingLapIds, "Lap query should be resolvable!");
            Assert.DoesNotContain(invalidLapId, remainingLapIds, "The invalid lap should be removed by the cleanup!");
            Assert.Contains(validLapId, remainingLapIds, "The valid lap should still be present!");

            var remainingTelemetryLapIds = dbFactory.GetRepository<CarTelemetryRepository>()
                                                    ?.GetQuery()
                                                    ?.Where(t => t.LapNumberId == invalidLapId || t.LapNumberId == validLapId)
                                                    .Select(t => t.LapNumberId)
                                                    .ToList();

            Assert.IsNotNull(remainingTelemetryLapIds, "Telemetry query should be resolvable!");
            Assert.DoesNotContain(invalidLapId, remainingTelemetryLapIds, "Telemetry of the invalid lap should be removed by the cleanup!");
            Assert.Contains(validLapId, remainingTelemetryLapIds, "Telemetry of the valid lap should still be present!");
        }
    }

    /// <summary>
    /// The final classification of a registered participant is stored in the database
    /// </summary>
    [TestMethod]
    public void FinalClassificationProcessorRegisteredParticipantStoresFinalClassification()
    {
        var finalClassification = GetFinalClassification(out var packetHeader);

        Assert.IsNotNull(finalClassification.PacketData, "The final classification packet should contain data!");

        var sessionRuntimeData = CreateSessionRuntimeData(packetHeader, 430600, true);

        Assert.IsTrue(sessionRuntimeData.Participants.TryAdd(0,
                                                             new ParticipantRuntimeData(sessionRuntimeData)
                                                             {
                                                                 ParticipantDbId = 430601
                                                             }),
                      "Participant runtime data could not be registered!");

        var isProcessed = CreateProcessor(packetHeader).Process(finalClassification, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Final classification packet not correctly processed!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var storedClassification = dbFactory.GetRepository<FinalClassificationRepository>()
                                                ?.GetQuery()
                                                ?.FirstOrDefault(f => f.SessionId == 430600 && f.ParticipantId == 430601);

            Assert.IsNotNull(storedClassification, "The final classification of the registered participant should be stored!");
            Assert.AreEqual(finalClassification.PacketData.FinalClassifications[0].Position, storedClassification.FinishPosition, "The finish position should be taken from the packet!");
        }
    }

    /// <summary>
    /// A final classification packet of a session that is not finished is ignored
    /// </summary>
    [TestMethod]
    public void FinalClassificationProcessorUnfinishedSessionIsIgnored()
    {
        var finalClassification = GetFinalClassification(out var packetHeader);
        var sessionRuntimeData = CreateSessionRuntimeData(packetHeader, 430610, false);

        Assert.IsTrue(sessionRuntimeData.Participants.TryAdd(0,
                                                             new ParticipantRuntimeData(sessionRuntimeData)
                                                             {
                                                                 ParticipantDbId = 430611
                                                             }),
                      "Participant runtime data could not be registered!");

        var isProcessed = CreateProcessor(packetHeader).Process(finalClassification, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "A final classification packet of an unfinished session should be ignored without failing!");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var hasClassification = dbFactory.GetRepository<FinalClassificationRepository>()
                                             ?.GetQuery()
                                             ?.Any(f => f.SessionId == 430610);

            Assert.IsFalse(hasClassification, "No final classification should be stored for an unfinished session!");
        }
    }

    /// <summary>
    /// A data object that is no final classification packet is ignored without recording an exception
    /// </summary>
    [TestMethod]
    public void FinalClassificationProcessorUnexpectedDataObjectReturnsTrue()
    {
        GetFinalClassification(out var packetHeader);

        var finalClassificationProcessor = CreateProcessor(packetHeader);

        var isProcessed = finalClassificationProcessor.Process(new object(), CreateSessionRuntimeData(packetHeader, 430620, true));

        Assert.IsTrue(isProcessed, "An unexpected data object should be ignored without an error!");
        Assert.AreEqual(string.Empty, finalClassificationProcessor.LastException, "Ignoring an unexpected data object must not record an exception!");
    }

    #endregion // Test methods
}