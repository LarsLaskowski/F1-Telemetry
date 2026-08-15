using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test event packet files (F1 2026)
/// </summary>
[TestClass]
public class PacketEvent2026Tests
{
    #region Fields

    private static PacketAnalyzer _packetAnalyzer;
    private static ReceivedPacketData _packetData;
    private static byte[] _packetContent;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketEventInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2026-MelbourneRace-Event-SessionStart.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2026-MelbourneRace-Event-SessionStart.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of event packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-MelbourneRace-Event-SessionStart.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file has a correct event data content
    /// </summary>
    [TestMethod]
    public void PacketEventCheckEvent2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Event;

        Assert.IsTrue(isCorrect, "Packet is not an event packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketEventCheckEvent2026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet");
    }

    /// <summary>
    /// Check that the session start event code is parsed (2026)
    /// </summary>
    [TestMethod]
    public void PacketEventSessionStartCode2026ExpectedValue()
    {
        var eventData = _packetAnalyzer.GetEventData(_packetData.PacketHeader!, _packetContent);

        if (eventData is EventData data && data.PacketData is IEventDataBase baseData)
        {
            Assert.AreEqual("SSTA", baseData.EventCode, "Incorrect event code!");
        }
        else
        {
            Assert.Fail("Invalid event packet, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check that the event data class recognizes the sample as a session start event and not as any
    /// other convenience event flag (2026)
    /// </summary>
    [TestMethod]
    public void PacketEventSessionStart2026IsRecognizedAsSessionStart()
    {
        var eventData = _packetAnalyzer.GetEventData(_packetData.PacketHeader!, _packetContent);

        if (eventData is EventData data)
        {
            Assert.IsTrue(data.IsSessionStart, "Event data must be recognized as a session start event!");
            Assert.IsFalse(data.IsSessionEnd, "Event data must not be recognized as a session end event!");
            Assert.IsFalse(data.IsFlashback, "Event data must not be recognized as a flashback event!");
        }
        else
        {
            Assert.Fail("Invalid event packet, expected F1 2026!");
        }
    }

    /// <summary>
    /// The session start event carries no additional event details in any F1 game version, so the
    /// detail fields must stay at their unset default values instead of leftover data from a previous
    /// packet
    /// </summary>
    [TestMethod]
    public void PacketEventSessionStartDetails2026ExpectedDefaultValues()
    {
        var eventData = _packetAnalyzer.GetEventData(_packetData.PacketHeader!, _packetContent);

        if (eventData is EventData data && data.PacketData is IEventDataBase baseData)
        {
            Assert.AreEqual(EventType.Unknown, baseData.EventDetails.EventType, "Session start must not carry an event type!");
            Assert.AreEqual((ushort)0, baseData.EventDetails.VehicleIndex, "Session start must not carry a vehicle index!");
            Assert.AreEqual(0F, baseData.EventDetails.FastestLap, 0.0001F, "Session start must not carry a fastest lap time!");
        }
        else
        {
            Assert.Fail("Invalid event packet, expected F1 2026!");
        }
    }

    #endregion // Methods F1 2026
}