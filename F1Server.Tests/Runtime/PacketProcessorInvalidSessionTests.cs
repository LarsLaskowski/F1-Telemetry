using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;

namespace F1Server.Tests.Runtime;

/// <summary>
/// Tests of the invalid session handling of the packet processor
/// </summary>
[TestClass]
public class PacketProcessorInvalidSessionTests
{
    #region Constants

    /// <summary>
    /// Offset of the unique session identifier inside the packet header
    /// </summary>
    private const int SessionIdOffset = 7;

    /// <summary>
    /// Offset of the team identifier of the first participant inside a F1 2025 participants packet
    /// </summary>
    private const int FirstParticipantTeamIdOffset = 33;

    /// <summary>
    /// Team identifier used by the game for an online session
    /// </summary>
    private const byte NetworkGameTeamId = 41;

    /// <summary>
    /// Unique session identifier used by these tests to work on an own database session
    /// </summary>
    private const ulong TestSessionId = 4915491549154915;

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Creates a packet processor with an isolated service provider and database usage
    /// </summary>
    /// <returns>Packet processor instance</returns>
    private static PacketProcessor CreatePacketProcessor()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new F1ServerApplicationData());
        services.AddSingleton(new PacketAnalyzer());

        return new PacketProcessor(services.BuildServiceProvider(), true);
    }

    /// <summary>
    /// Reads a sample packet file and replaces its unique session identifier
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    /// <returns>Received packet data</returns>
    private static ReceivedPacketData GetPacketData(string fileName)
    {
        return GetPacketData(fileName, null);
    }

    /// <summary>
    /// Reads a sample packet file, replaces its unique session identifier and applies an optional modification
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    /// <param name="modifyRawData">Optional modification of the raw packet data</param>
    /// <returns>Received packet data</returns>
    private static ReceivedPacketData GetPacketData(string fileName, Action<byte[]>? modifyRawData)
    {
        var rawData = File.ReadAllBytes(Path.Combine("SampleData", fileName));

        // All packets have to belong to the same session, otherwise the processor starts a new one
        BitConverter.TryWriteBytes(rawData.AsSpan(SessionIdOffset), TestSessionId);

        modifyRawData?.Invoke(rawData);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(rawData);

        Assert.IsNotNull(packetData.PacketHeader, $"Header of {fileName} could not be parsed!");
        Assert.AreEqual(TestSessionId, packetData.PacketHeader.UniqueSessionId, $"Session identifier of {fileName} was not replaced!");

        return packetData;
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Removing an invalid session must clear the runtime session without reporting a packet processing error
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketInvalidSessionRemovesSessionWithoutError()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            var isProcessed = packetProcessor.ProcessPacket(GetPacketData("F1-2025-MelbourneRace-Event-SessionStart.packet"));

            Assert.IsTrue(isProcessed, "Session start event not correctly processed!");

            isProcessed = packetProcessor.ProcessPacket(GetPacketData("F1-2025-MelbourneRace-SessionFirst.packet"));

            Assert.IsTrue(isProcessed, "Session packet not correctly processed!");
            Assert.IsNotNull(packetProcessor.Session, "No session runtime data after the session packet!");

            var sessionDbId = packetProcessor.Session.SessionDbId;

            Assert.AreNotEqual(0, sessionDbId, "Session was not stored in the database!");

            // A participant of team 41 marks the session as an online session, which has to be removed again
            packetProcessor.ProcessPacket(GetPacketData("F1-2025-Participants.packet", raw => raw[FirstParticipantTeamIdOffset] = NetworkGameTeamId));

            Assert.IsNull(packetProcessor.Session, "Session runtime data was not cleared after the invalid session was removed!");
            Assert.IsTrue(string.IsNullOrWhiteSpace(packetProcessor.LastError), $"Removing an invalid session must not report an error: {packetProcessor.LastError}");

            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                var dbSessionData = dbFactory.GetRepository<SessionRepository>()?.GetQuery()?.FirstOrDefault(s => s.Id == sessionDbId);

                Assert.IsNull(dbSessionData, "Invalid session was not removed from the database!");
            }
        }
    }

    #endregion // Methods
}