using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;
using F1Server.Tests.Data;

namespace F1Server.Tests;

/// <summary>
/// Class to test the participants processor with F1 2026 packets
/// </summary>
[TestClass]
public class ParticipantsProcessor2026Tests
{
    #region Fields

    /// <summary>
    /// Packet analyzer instance
    /// </summary>
    private static PacketAnalyzer _packetAnalyzer;

    /// <summary>
    /// Received participants packet data
    /// </summary>
    private static ReceivedPacketData _packetData;

    /// <summary>
    /// Analyzed participants data object
    /// </summary>
    private static Participants? _participantsData;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void ParticipantsProcessor2026Init(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2026-Participants.packet");

        Assert.IsTrue(isFile, "File F1-2026-Participants.packet is missing!");

        _packetAnalyzer = new PacketAnalyzer();

        var fileContent = File.ReadAllBytes(@"SampleData/F1-2026-Participants.packet");

        _packetData = new ReceivedPacketData();

        _packetData.SetRawData(fileContent);

        Assert.IsNotNull(_packetData.PacketHeader, "Test packet file (2026) is not a participants file!");
        Assert.AreEqual(PacketTypes.Participants, _packetData.PacketHeader.PacketType, "Test packet file (2026) is not a participants file!");

        var participantsData = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetData.PacketRawData);

        if (participantsData is Participants participantsDataObj)
        {
            _participantsData = participantsDataObj;
        }

        Assert.IsNotNull(_participantsData, "Participants data object (2026) is null!");
    }

    #endregion // Initializer/Cleanup

    #region Test methods

    /// <summary>
    /// Method to test that F1 2026 participants are matched against the seeded drivers and written to the database
    /// </summary>
    [TestMethod]
    public void ProcessParticipantsPacket2026ExpectedValidParticipants()
    {
        var sessionRuntimeData = ProcessParticipantsPacket();

        var validParticipants = sessionRuntimeData.Participants.Values.Where(p => p.IsValidObject).ToList();

        Assert.IsNotEmpty(validParticipants, "No valid participants created (2026)!");
    }

    /// <summary>
    /// Method to test that the human player sending the uint16 driver id 65535 is created as participant
    /// </summary>
    [TestMethod]
    public void ProcessParticipantsPacket2026ExpectedHumanParticipantCreated()
    {
        var sessionRuntimeData = ProcessParticipantsPacket();

        var humanParticipants = sessionRuntimeData.Participants.Values.Where(p => p.IsValidObject && p.IsHumanDriver).ToList();

        Assert.IsNotEmpty(humanParticipants, "No human participant created (2026)!");
    }

    #endregion // Test methods

    #region Private methods

    /// <summary>
    /// Process the participants packet on the F1 2026 live session
    /// </summary>
    /// <returns>Session runtime data of the live session</returns>
    private SessionRuntimeData ProcessParticipantsPacket()
    {
        var packetHeader = _packetData.PacketHeader;
        var sessionRuntimeData = TestData.LiveSessionProcessor2026.Session;

        Assert.IsNotNull(packetHeader, "Packet header variable (2026) is null!");
        Assert.IsNotNull(sessionRuntimeData, "Session runtime data (2026) is null!");
        Assert.IsNotNull(sessionRuntimeData.CurrentSession, "Current session data (2026) is null!");

        var participantsProcessor = TestData.LiveSessionProcessor2026.GetProcessor(packetHeader);

        Assert.IsInstanceOfType<ParticipantsProcessor>(participantsProcessor, "Processor is not a participants processor (2026)!");

        // First packet creates participants, second packet associates the runtime objects held in the session
        var isProcessed = participantsProcessor.Process(_participantsData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Participants packet (2026) not correctly processed!");

        isProcessed = participantsProcessor.Process(_participantsData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, "Participants packet (2026) not correctly processed (second pass)!");

        return sessionRuntimeData;
    }

    #endregion // Private methods
}