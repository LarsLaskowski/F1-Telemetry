using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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

    #endregion // Methods F1 2026
}