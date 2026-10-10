using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Service.Runtime;
using F1Server.Tests.Data;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the lap positions processor class
/// </summary>
[TestClass]
public class LapPositionsProcessorTests
{
    #region Static methods

    /// <summary>
    /// Creates a session runtime data object
    /// </summary>
    /// <param name="hasParticipants">Has the session participants?</param>
    /// <returns>Session runtime data</returns>
    private static SessionRuntimeData CreateSessionRuntimeData(bool hasParticipants)
    {
        return new SessionRuntimeData(2026, 450500, SessionType.Race)
               {
                   HasParticipants = hasParticipants
               };
    }

    /// <summary>
    /// Creates a F1 2026 lap positions packet
    /// </summary>
    /// <returns>Lap positions packet</returns>
    private static LapPositions CreateLapPositions()
    {
        var packetHeader = new PacketHeader
                           {
                               GameVersion = 2026,
                               PacketType = PacketTypes.LapPositions,
                               UniqueSessionId = 450500
                           };

        return new LapPositions(packetHeader,
                                new LapPositions2026
                                {
                                    NumberOfLaps = 1
                                });
    }

    /// <summary>
    /// Creates a lap positions processor
    /// </summary>
    /// <returns>Lap positions processor</returns>
    private static LapPositionsProcessor CreateProcessor()
    {
        return new LapPositionsProcessor(TestData.ServiceProvider,
                                         new PacketHeader(),
                                         new LiveGameData
                                         {
                                             GameVersion = 2026
                                         });
    }

    #endregion // Static methods

    #region Test methods

    /// <summary>
    /// A lap positions packet of a session with participants is processed
    /// </summary>
    [TestMethod]
    public void LapPositionsProcessorSessionWithParticipantsReturnsTrue()
    {
        var lapPositionsProcessor = CreateProcessor();

        var isProcessed = lapPositionsProcessor.Process(CreateLapPositions(), CreateSessionRuntimeData(true));

        Assert.IsTrue(isProcessed, "Lap positions packet not correctly processed!");
        Assert.AreEqual(string.Empty, lapPositionsProcessor.LastException, "Processing a lap positions packet must not record an exception!");
    }

    /// <summary>
    /// A lap positions packet of a session without participants is ignored without failing
    /// </summary>
    [TestMethod]
    public void LapPositionsProcessorSessionWithoutParticipantsReturnsTrue()
    {
        var lapPositionsProcessor = CreateProcessor();

        var isProcessed = lapPositionsProcessor.Process(CreateLapPositions(), CreateSessionRuntimeData(false));

        Assert.IsTrue(isProcessed, "A lap positions packet without participants should be ignored without failing!");
        Assert.AreEqual(string.Empty, lapPositionsProcessor.LastException, "Ignoring a lap positions packet must not record an exception!");
    }

    /// <summary>
    /// A data object that is no lap positions packet is ignored without failing
    /// </summary>
    [TestMethod]
    public void LapPositionsProcessorUnexpectedDataObjectReturnsTrue()
    {
        var lapPositionsProcessor = CreateProcessor();

        var isProcessed = lapPositionsProcessor.Process(new object(), null);

        Assert.IsTrue(isProcessed, "An unexpected data object should be ignored without an error!");
        Assert.AreEqual(string.Empty, lapPositionsProcessor.LastException, "Ignoring an unexpected data object must not record an exception!");
    }

    #endregion // Test methods
}