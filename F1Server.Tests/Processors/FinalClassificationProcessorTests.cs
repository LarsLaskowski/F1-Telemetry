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
/// Class to test the final classification processor class
/// </summary>
[TestClass]
public class FinalClassificationProcessorTests
{
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

    #endregion // Test methods
}