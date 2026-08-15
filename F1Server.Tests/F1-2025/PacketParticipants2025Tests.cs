using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

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
        var data = GetParticipantsData();

        Assert.IsNotNull(data, "Packet is not a participants packet");
    }

    /// <summary>
    /// Check active cars (2025)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsActiveCars2025ExpectedValue()
    {
        var data = GetParticipantsData();

        Assert.AreEqual((ushort)20, data.ActiveCars, "Incorrect active cars!");
    }

    /// <summary>
    /// Check human controlled (2025)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsIsHumanControlled2025ExpectedValue()
    {
        var data = (IParticipantData2025[])GetParticipantsData().Participants;

        Assert.IsFalse(data[21].IsAIControlled, "Driver is not controlled by human!");
    }

    /// <summary>
    /// Check driver name (2025)
    /// </summary>
    [TestMethod]
    public void PacketParticipantsDriverName2025ExpectedValue()
    {
        var data = (IParticipantData2025[])GetParticipantsData().Participants;

        Assert.Contains("Max Kane", data[19].DriverName, "Driver name is invalid!");
    }

    #endregion // Methods F1 2025

    #region Private methods

    /// <summary>
    /// Reads the participants data of the sample packet
    /// </summary>
    /// <returns>Participants data of the sample packet</returns>
    private static IParticipantsBase GetParticipantsData()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12025ParticipantsSize + ConstData.F12025HeaderSize, "Packet content too short!");

        var participants = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetContent);

        Assert.IsInstanceOfType<Participants>(participants, "Packet is not a participants packet!");

        var packetData = ((Participants)participants).PacketData;

        Assert.IsInstanceOfType<IParticipantsBase>(packetData, "Packet is not a participants packet!");

        var baseData = (IParticipantsBase)packetData;

        Assert.IsInstanceOfType<IParticipantData2025[]>(baseData.Participants, "Invalid participants packet, expected F1 2025!");

        return baseData;
    }

    #endregion // Private methods
}