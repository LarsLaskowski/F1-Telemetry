using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;
using F1Server.Tests.Data;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the car telemetry processor class
/// </summary>
[TestClass]
public class CarTelemetryProcessorTests
{
    #region Test methods

    /// <summary>
    /// A new telemetry start point completes the previous lap buffer and records the current lap
    /// </summary>
    [TestMethod]
    public void CarTelemetryProcessorNewTelemetryStartsNewLapBuffer()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2025-CarTelemetry.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Analyzing the car telemetry packet failed!");

        var carTelemetry = new PacketAnalyzer().GetCarTelemetry(packetData.PacketHeader, packetContent) as CarTelemetry;

        Assert.IsNotNull(carTelemetry, "The packet is not a car telemetry packet!");

        var sessionRuntimeData = new SessionRuntimeData(2025, packetData.PacketHeader.UniqueSessionId, SessionType.Race)
                                 {
                                     HasParticipants = true,
                                     IsRecordable = true,
                                     IsTelemetryRecording = true,
                                     CurrentSession = new LiveSessionData
                                                      {
                                                          DbId = 421600,
                                                          SessionGameId = packetData.PacketHeader.UniqueSessionId,
                                                          SessionType = SessionType.Race
                                                      }
                                 };

        var participantRuntimeData = new ParticipantRuntimeData(sessionRuntimeData)
                                     {
                                         CurrentLapNumber = 5,
                                         IsNewTelemetry = true,
                                         IsOnRecordableLap = true
                                     };

        Assert.IsTrue(sessionRuntimeData.Participants.TryAdd(0, participantRuntimeData), "Participant runtime data could not be registered!");

        var carTelemetryProcessor = new CarTelemetryProcessor(TestData.ServiceProvider,
                                                              packetData.PacketHeader,
                                                              new LiveGameData
                                                              {
                                                                  GameVersion = 2025
                                                              });

        var isProcessed = carTelemetryProcessor.Process(carTelemetry, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Car telemetry packet not correctly processed!");
        Assert.IsFalse(participantRuntimeData.IsNewTelemetry, "The new telemetry flag should be reset after completing the previous lap!");
        Assert.IsTrue(participantRuntimeData.ClearTelemetryData(5), "A telemetry point of the current lap should be buffered!");
    }

    #endregion // Test methods
}