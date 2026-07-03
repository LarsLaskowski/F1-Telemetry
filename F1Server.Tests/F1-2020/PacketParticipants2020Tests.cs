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
public class PacketParticipants2020Tests
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
        var is2020File = File.Exists(@"SampleData/F1-2020-Participants.packet");

        if (is2020File)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2020-Participants.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of participants packets failed!");
        }
        else
        {
            Assert.IsTrue(is2020File, "File F1-2020-Participants.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2020

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2020IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Participants;

        Assert.IsTrue(isCorrect, "Packet is not a participants data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2020 packet
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2020IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2020;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2020 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2020IsParticipantsDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length == ConstData.F12020ParticipantsSize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants data)
            {
                isCorrect = data.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2020[];
            }

            Assert.IsTrue(isCorrect, "Packet is not a participants data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check active cars (2020)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsActiveCars2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020ParticipantsSize + ConstData.F12020HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2020[])
            {
                var isCorrect = participantsData.PacketData.ActiveCars == 1;

                Assert.IsTrue(isCorrect, "Incorrect active cars!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check human controlled (2020)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsIsHumanControlled2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020ParticipantsSize + ConstData.F12020HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2020[] data)
            {
                var isCorrect = data[0].IsAIControlled == false;

                Assert.IsTrue(isCorrect, "First driver is not controlled by human!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check driver nationality (2020)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsDriverNationality2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020ParticipantsSize + ConstData.F12020HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2020[] data)
            {
                var isCorrect = data[0].Nationality == 29;

                Assert.IsTrue(isCorrect, "Driver nationality is invalid!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    #endregion // Methods F1 2020
}