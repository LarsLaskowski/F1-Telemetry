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
public class PacketParticipants2021Tests
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
        var is2021File = File.Exists(@"SampleData/F1-2021-Participants.packet");

        if (is2021File)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2021-Participants.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of participants packets failed!");
        }
        else
        {
            Assert.IsTrue(is2021File, "File F1-2021-Participants.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2021

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2021IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Participants;

        Assert.IsTrue(isCorrect, "Packet is not a participants packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2021 packet
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2021IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2021;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2021 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketParticipantsCheckParticipants2021IsParticipantsObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12021ParticipantsSize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants data)
            {
                isCorrect = data.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2021[];
            }

            Assert.IsTrue(isCorrect, "Packet is not a participants packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2021 packet header or content!");
        }
    }

    /// <summary>
    /// Check active cars (2021)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsActiveCars2021ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12021ParticipantsSize + ConstData.F12020HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2021[])
            {
                var isCorrect = baseData.ActiveCars == 22;

                Assert.IsTrue(isCorrect, "Incorrect active cars!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2021 packet header or content!");
        }
    }

    /// <summary>
    /// Check human controlled (2021)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsIsHumanControlled2021ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12021ParticipantsSize + ConstData.F12020HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2021[] data)
            {
                var isCorrect = data[21].IsAIControlled == false;

                Assert.IsTrue(isCorrect, "Driver is not controlled by human!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2021 packet header or content!");
        }
    }

    /// <summary>
    /// Check active cars (2021)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsDriverName2021ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12021ParticipantsSize + ConstData.F12020HeaderSize)
        {
            var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

            if (participants is Participants participantsData && participantsData.PacketData is IParticipantsBase baseData && baseData.Participants is IParticipantData2021[] data)
            {
                var isCorrect = data[21].DriverName.Contains("Iceman");

                Assert.IsTrue(isCorrect, "Driver name is invalid!");
            }
            else
            {
                Assert.Fail("Invalid participants packet, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2021 packet header or content!");
        }
    }

    #endregion // Methods F1 2021
}