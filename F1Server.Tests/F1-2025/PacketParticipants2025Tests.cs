using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test participants packet files
/// </summary>
[TestClass]
public class PacketParticipants2025Tests
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
        var isFile = File.Exists(@"SampleData/F1-2025-Participants.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2025-Participants.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of participants packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2025-Participants.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2025

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Participants;

        Assert.IsTrue(isCorrect, "Packet is not a participants packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2025 packet
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2025IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2025;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2025 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2025IsParticipantsObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025ParticipantsSize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants data)
            {
                isCorrect = data.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2025[];
            }

            Assert.IsTrue(isCorrect, "Packet is not a participants packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check active cars (2025)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsActiveCars2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025ParticipantsSize + ConstData.F12025HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2025[])
            {
                var isCorrect = baseData.ActiveCars == 20;

                Assert.IsTrue(isCorrect, "Incorrect active cars!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check human controlled (2025)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsIsHumanControlled2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025ParticipantsSize + ConstData.F12025HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2025[] data)
            {
                var isCorrect = data[21].IsAIControlled == false;

                Assert.IsTrue(isCorrect, "Driver is not controlled by human!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check active cars (2025)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsDriverName2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025ParticipantsSize + ConstData.F12025HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2025[] data)
            {
                var isCorrect = data[19].DriverName.Contains("Max Kane");

                Assert.IsTrue(isCorrect, "Driver name is invalid!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    #endregion // Methods F1 2025
}