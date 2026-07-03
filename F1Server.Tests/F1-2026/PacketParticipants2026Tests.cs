using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test participants packet files (F1 2026)
/// </summary>
[TestClass]
public class PacketParticipants2026Tests
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
    public static void PacketParticipantsInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2026-Participants.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2026-Participants.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of participants packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-Participants.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Participants;

        Assert.IsTrue(isCorrect, "Packet is not a participants packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2026IsParticipantsObject()
    {
        var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader!, _packetContent);

        var isCorrect = participants is Participants data && data.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2026[];

        Assert.IsTrue(isCorrect, "Packet is not a participants packet");
    }

    /// <summary>
    /// Check active cars (2026)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsActiveCars2026ExpectedValue()
    {
        var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader!, _packetContent);

        if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2026[])
        {
            Assert.AreEqual((ushort)20, baseData.ActiveCars, "Incorrect active cars!");
        }
        else
        {
            Assert.Fail("Invalid participants packet, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check that 24 participant slots are present (F1 2026)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCount2026Expected24Slots()
    {
        var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader!, _packetContent);

        if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2026[])
        {
            Assert.HasCount(24, baseData.Participants, "Incorrect number of participant slots!");
        }
        else
        {
            Assert.Fail("Invalid participants packet, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check human controlled (2026)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsIsHumanControlled2026ExpectedValue()
    {
        var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader!, _packetContent);

        if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2026[] data)
        {
            var isCorrect = data[21].IsAIControlled == false;

            Assert.IsTrue(isCorrect, "Driver is not controlled by human!");
        }
        else
        {
            Assert.Fail("Invalid participants packet, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check driver name (2026)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsDriverName2026ExpectedValue()
    {
        var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader!, _packetContent);

        if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2026[] data)
        {
            var isCorrect = data[19].DriverName.Contains("Max Kane");

            Assert.IsTrue(isCorrect, "Driver name is invalid!");
        }
        else
        {
            Assert.Fail("Invalid participants packet, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the uint16 driver id of the human player resolves to the 65535 sentinel (2026)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsDriverId2026ReturnsNetworkHumanSentinel()
    {
        var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader!, _packetContent);

        if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2026[] data)
        {
            Assert.AreEqual((ushort)65535, data[19].DriverId, "Driver id of human player is wrong!");
        }
        else
        {
            Assert.Fail("Invalid participants packet, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the uint16 team id (2026)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsTeamId2026ExpectedValue()
    {
        var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader!, _packetContent);

        if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2026[] data)
        {
            Assert.AreEqual((ushort)3, data[19].TeamId, "Team id is wrong!");
        }
        else
        {
            Assert.Fail("Invalid participants packet, expected F1 2026!");
        }
    }

    #endregion // Methods F1 2026
}