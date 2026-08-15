using System.Text;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test event packets from F1 2025
/// </summary>
[TestClass]
public class PacketEvent2025Tests
{
    #region Fields

    private static PacketAnalyzer _packetAnalyzer;
    private static ReceivedPacketData _packetDataTopSpeed;
    private static ReceivedPacketData _packetDataFlashback;
    private static ReceivedPacketData _packetDataPenalty;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketEvent2025Init(TestContext testContext)
    {
        var file1 = File.Exists(@"SampleData/F1-2025-Event-TopSpeed.packet");
        var file2 = File.Exists(@"SampleData/F1-2025-Event-Flashback.packet");
        var file3 = File.Exists(@"SampleData/F1-2025-Event-Penalty.packet");

        if (file1 && file2 && file3)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet");

            _packetDataTopSpeed = new ReceivedPacketData();

            _packetDataTopSpeed.SetRawData(fileContent);

            fileContent = File.ReadAllBytes(@"SampleData/F1-2025-Event-Flashback.packet");

            _packetDataFlashback = new ReceivedPacketData();

            _packetDataFlashback.SetRawData(fileContent);

            fileContent = File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet");

            _packetDataPenalty = new ReceivedPacketData();

            _packetDataPenalty.SetRawData(fileContent);

            var isCorrect = _packetDataTopSpeed.PacketHeader != null && _packetDataFlashback.PacketHeader != null && _packetDataPenalty.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of event packets failed!");
        }
        else
        {
            Assert.IsTrue(file1, "File F1-2025-Event-TopSpeed.packet is missing!");
            Assert.IsTrue(file2, "File F1-2025-Event-Flashback.packet is missing!");
            Assert.IsTrue(file3, "File F1-2025-Event-Penalty.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Builds a synthetic F1 2025 event packet by taking a real sample packet's header and
    /// overwriting its event code and payload, so branches without a dedicated sample file can be tested
    /// </summary>
    /// <param name="eventCode">Four letter event code to inject</param>
    /// <param name="payloadBytes">Payload bytes following the event code</param>
    /// <returns>Synthetic packet content</returns>
    private static byte[] BuildSyntheticEventPacket(string eventCode, params byte[] payloadBytes)
    {
        var data = File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet");
        var headerSize = ConstData.F12025HeaderSize;
        var codeBytes = Encoding.ASCII.GetBytes(eventCode);

        Array.Copy(codeBytes, 0, data, headerSize, codeBytes.Length);
        Array.Copy(payloadBytes, 0, data, headerSize + 4, payloadBytes.Length);

        return data;
    }

    /// <summary>
    /// Reads the event data for the given source packet and raw payload
    /// </summary>
    /// <param name="source">Received sample packet supplying the header</param>
    /// <param name="rawData">Raw packet bytes to analyze</param>
    /// <returns>Event data of the packet</returns>
    private static EventData GetEventData(ReceivedPacketData source, byte[] rawData)
    {
        Assert.IsNotNull(source.PacketHeader, "Invalid F1 2025 packet header!");

        var packetData = _packetAnalyzer.GetEventData(source.PacketHeader, rawData);

        Assert.IsInstanceOfType<EventData>(packetData, "Invalid packet data, expected F1 2025!");

        return (EventData)packetData;
    }

    /// <summary>
    /// Reads the F1 2025 event details for the given source packet and raw payload
    /// </summary>
    /// <param name="source">Received sample packet supplying the header</param>
    /// <param name="rawData">Raw packet bytes to analyze</param>
    /// <returns>Event details of the packet</returns>
    private static IEventDataDetails2025 GetEventDetails(ReceivedPacketData source, byte[] rawData)
    {
        var eventData = GetEventData(source, rawData);

        Assert.IsInstanceOfType<IEventDataDetails2025>(eventData.PacketData?.EventDetails, "Invalid packet data, expected F1 2025!");

        return (IEventDataDetails2025)eventData.PacketData.EventDetails;
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Check correct packet type
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsCorrectPacketType()
    {
        var isCorrect = _packetDataTopSpeed.PacketHeader?.PacketType == PacketTypes.Event;

        Assert.IsTrue(isCorrect, "Packet is not a event packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2025 packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsCorrectGameVersion()
    {
        var isCorrect = _packetDataFlashback.PacketHeader?.GameVersion == 2025;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2025 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketEvent2025CheckEventDataIsEventDataObject()
    {
        var eventData = GetEventData(_packetDataPenalty, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

        Assert.IsInstanceOfType<EventData2025>(eventData.PacketData, "Packet is not a event data packet");
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsTopSpeedPacketExpectedTrue()
    {
        var eventData = GetEventData(_packetDataTopSpeed, File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet"));

        Assert.IsTrue(eventData.EventCode.Equals("SPTP"), "No top speed event packet!");
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsTopSpeedExpectedValue()
    {
        var eventDetails = GetEventDetails(_packetDataTopSpeed, File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet"));

        Assert.AreEqual(307.768463F, eventDetails.TopSpeed, 0.0001F, "Incorrect top speed value!");
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsTopSpeedVehicleExpectedTwenty()
    {
        var eventDetails = GetEventDetails(_packetDataTopSpeed, File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet"));

        Assert.AreEqual(20, eventDetails.VehicleIndex, "Incorrect vehicle value!");
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsFlashbackPacketExpectedTrue()
    {
        var eventData = GetEventData(_packetDataFlashback, File.ReadAllBytes(@"SampleData/F1-2025-Event-Flashback.packet"));

        Assert.IsTrue(eventData.EventCode.Equals("FLBK"), "No flashback event packet!");
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsFlashbackExpectedValue()
    {
        var eventDetails = GetEventDetails(_packetDataFlashback, File.ReadAllBytes(@"SampleData/F1-2025-Event-Flashback.packet"));

        Assert.AreEqual(1280U, eventDetails.FlashbackFrame, "Incorrect flashback value!");
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyPacketExpectedTrue()
    {
        var eventData = GetEventData(_packetDataPenalty, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

        Assert.IsTrue(eventData.EventCode.Equals("PENA"), "No penalty event packet!");
    }

    /// <summary>
    /// Check penalty type from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyTypeExpectedValue()
    {
        var eventDetails = GetEventDetails(_packetDataPenalty, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

        Assert.AreEqual(PenaltyType.Warning, eventDetails.PenaltyType, "Incorrect penalty type value!");
    }

    /// <summary>
    /// Check penalty infringement type from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyInfringementTypeExpectedLapInvalidRunningWide()
    {
        var eventDetails = GetEventDetails(_packetDataPenalty, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

        Assert.AreEqual(InfringementType.SmallCollision, eventDetails.PenaltyInfringementType, "Incorrect infringement value!");
    }

    /// <summary>
    /// Check penalty vehicle from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyVehicleExpectedTwentyOne()
    {
        var eventDetails = GetEventDetails(_packetDataPenalty, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

        Assert.AreEqual(21, eventDetails.VehicleIndex, "Incorrect vehicle value!");
    }

    /// <summary>
    /// Check penalty lap number from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyLapNumberExpectedSixteen()
    {
        var eventDetails = GetEventDetails(_packetDataPenalty, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

        Assert.AreEqual(16, eventDetails.PenaltyLapNumber, "Incorrect lap number!");
    }

    /// <summary>
    /// Check event type from top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsTopSpeedEventTypeExpectedSpeedTrap()
    {
        var eventDetails = GetEventDetails(_packetDataTopSpeed, File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet"));

        Assert.AreEqual(EventType.SpeedTrap, eventDetails.EventType, "Incorrect event type!");
    }

    /// <summary>
    /// Check event type from flashback packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsFlashbackEventTypeExpectedFlashback()
    {
        var eventDetails = GetEventDetails(_packetDataFlashback, File.ReadAllBytes(@"SampleData/F1-2025-Event-Flashback.packet"));

        Assert.AreEqual(EventType.Flashback, eventDetails.EventType, "Incorrect event type!");
    }

    /// <summary>
    /// Check event type from penalty packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyEventTypeExpectedPenalty()
    {
        var eventDetails = GetEventDetails(_packetDataPenalty, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

        Assert.AreEqual(EventType.Penalty, eventDetails.EventType, "Incorrect event type!");
    }

    /// <summary>
    /// Check event type and reason of a synthetic DRS disabled packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsDrsDisabledEventTypeExpectedDrsDisabled()
    {
        var eventDetails = GetEventDetails(_packetDataPenalty, BuildSyntheticEventPacket("DRSD", 2));

        Assert.AreEqual(EventType.DrsDisabled, eventDetails.EventType, "Incorrect event type!");
        Assert.AreEqual(DrsDisabledReason.RedFlag, eventDetails.DrsDisabledReason, "Incorrect DRS disabled reason!");
    }

    #endregion // Methods
}