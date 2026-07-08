using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Service.Processors;
using F1Server.Tests.Data;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the participants processor class
/// </summary>
[TestClass]
public class ParticipantsProcessorTests
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
    public static void ParticipantsProcessorInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2025-Participants.packet");

        Assert.IsTrue(isFile, "File F1-2025-Participants.packet is missing!");

        _packetAnalyzer = new PacketAnalyzer();

        var fileContent = File.ReadAllBytes(@"SampleData/F1-2025-Participants.packet");

        _packetData = new ReceivedPacketData();

        _packetData.SetRawData(fileContent);

        Assert.IsNotNull(_packetData.PacketHeader, "Test packet file (2025) is not a participants file!");
        Assert.AreEqual(PacketTypes.Participants, _packetData.PacketHeader.PacketType, "Test packet file (2025) is not a participants file!");

        var participantsData = _packetAnalyzer.GetParticipantsData(_packetData.PacketHeader, _packetData.PacketRawData);

        if (participantsData is Participants participantsDataObj)
        {
            _participantsData = participantsDataObj;
        }

        Assert.IsNotNull(_participantsData, "Participants data object (2025) is null!");
    }

    #endregion // Initializer/Cleanup

    #region Test methods

    /// <summary>
    /// Method to test that processed participants cache their live driver data reference
    /// </summary>
    [TestMethod]
    public void ProcessParticipantsPacketExpectedCachedLiveDataReference()
    {
        var packetHeader = _packetData.PacketHeader;
        var sessionRuntimeData = TestData.LiveSessionProcessor.Session;

        Assert.IsNotNull(packetHeader, $"Packet header variable ({TestData.LiveSessionGameVersion}) is null!");
        Assert.IsNotNull(sessionRuntimeData?.CurrentSession, $"Current session data ({TestData.LiveSessionGameVersion}) is null!");

        var participantsProcessor = TestData.LiveSessionProcessor.GetProcessor(packetHeader);

        Assert.IsInstanceOfType<ParticipantsProcessor>(participantsProcessor);

        // First packet creates participants, second packet associates the runtime objects held in the session
        var isProcessed = participantsProcessor.Process(_participantsData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, $"Participants packet ({TestData.LiveSessionGameVersion}) not correctly processed!");

        isProcessed = participantsProcessor.Process(_participantsData, sessionRuntimeData);

        Assert.IsTrue(isProcessed, $"Participants packet ({TestData.LiveSessionGameVersion}) not correctly processed (second pass)!");

        var validParticipants = sessionRuntimeData.Participants.Values.Where(p => p.IsValidObject).ToList();

        Assert.IsFalse(validParticipants.Count == 0, $"No valid participants created ({TestData.LiveSessionGameVersion})!");

        foreach (var participantData in validParticipants)
        {
            var driverListEntry = sessionRuntimeData.CurrentSession.Drivers.Find(d => d.DbId == participantData.ParticipantDbId);

            Assert.IsNotNull(participantData.LiveData, $"Live driver data is not cached for participant {participantData.ParticipantDbId}!");
            Assert.AreSame(driverListEntry, participantData.LiveData, $"Cached live driver data is not the list entry for participant {participantData.ParticipantDbId}!");
        }
    }

    #endregion // Test methods
}