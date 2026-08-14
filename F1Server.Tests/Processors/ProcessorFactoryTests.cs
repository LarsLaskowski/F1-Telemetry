using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Tests.Data;

namespace F1Server.Tests.Processors;

/// <summary>
/// Class to test the processor caching of the processor factory class
/// </summary>
[TestClass]
public class ProcessorFactoryTests
{
    #region Test methods

    /// <summary>
    /// Two packets of the same type within the same session must be handled by the same processor instance
    /// </summary>
    [TestMethod]
    public void ProcessorFactoryGetProcessorSameSessionReturnsSameInstance()
    {
        using (var processorFactory = new ProcessorFactory(TestData.ServiceProvider))
        {
            var gameInfo = CreateGameInfo();

            var firstProcessor = processorFactory.GetProcessor(CreatePacketHeader(PacketTypes.Session, 1, 1), gameInfo);
            var secondProcessor = processorFactory.GetProcessor(CreatePacketHeader(PacketTypes.Session, 1, 2), gameInfo);

            Assert.IsNotNull(firstProcessor, "No processor was created for the first packet!");
            Assert.AreSame(firstProcessor, secondProcessor, "The second packet of the same session was handled by a different processor instance!");
        }
    }

    /// <summary>
    /// A packet of a new session must be handled by a freshly created processor instance
    /// </summary>
    [TestMethod]
    public void ProcessorFactoryGetProcessorNewSessionReturnsNewInstance()
    {
        using (var processorFactory = new ProcessorFactory(TestData.ServiceProvider))
        {
            var gameInfo = CreateGameInfo();

            var firstProcessor = processorFactory.GetProcessor(CreatePacketHeader(PacketTypes.Session, 1, 1), gameInfo);
            var secondProcessor = processorFactory.GetProcessor(CreatePacketHeader(PacketTypes.Session, 2, 1), gameInfo);

            Assert.IsNotNull(firstProcessor, "No processor was created for the first session!");
            Assert.IsNotNull(secondProcessor, "No processor was created for the second session!");
            Assert.AreNotSame(firstProcessor, secondProcessor, "The packet of the new session was handled by the processor instance of the previous session!");
        }
    }

    /// <summary>
    /// Every supported packet type must be handled by its own cached processor instance
    /// </summary>
    [TestMethod]
    public void ProcessorFactoryGetProcessorSupportedPacketTypesReturnsMatchingProcessors()
    {
        using (var processorFactory = new ProcessorFactory(TestData.ServiceProvider))
        {
            var gameInfo = CreateGameInfo();

            var expectedProcessors = new[]
                                     {
                                         (PacketType: PacketTypes.Session, ProcessorType: typeof(SessionProcessor)),
                                         (PacketType: PacketTypes.Participants, ProcessorType: typeof(ParticipantsProcessor)),
                                         (PacketType: PacketTypes.LapData, ProcessorType: typeof(LapDataProcessor)),
                                         (PacketType: PacketTypes.SessionHistory, ProcessorType: typeof(SessionHistoryProcessor)),
                                         (PacketType: PacketTypes.CarStatus, ProcessorType: typeof(CarStatusProcessor)),
                                         (PacketType: PacketTypes.CarTelemetry, ProcessorType: typeof(CarTelemetryProcessor)),
                                         (PacketType: PacketTypes.FinalClassification, ProcessorType: typeof(FinalClassificationProcessor)),
                                         (PacketType: PacketTypes.TimeTrial, ProcessorType: typeof(TimeTrialProcessor)),
                                         (PacketType: PacketTypes.LapPositions, ProcessorType: typeof(LapPositionsProcessor))
                                     };

            foreach (var expectedProcessor in expectedProcessors)
            {
                var processor = processorFactory.GetProcessor(CreatePacketHeader(expectedProcessor.PacketType, 1, 1), gameInfo);

                Assert.IsNotNull(processor, $"No processor was created for packet type {expectedProcessor.PacketType}!");
                Assert.IsInstanceOfType(processor, expectedProcessor.ProcessorType, $"Packet type {expectedProcessor.PacketType} was handled by an unexpected processor!");

                var cachedProcessor = processorFactory.GetProcessor(CreatePacketHeader(expectedProcessor.PacketType, 1, 2), gameInfo);

                Assert.AreSame(processor, cachedProcessor, $"The processor of packet type {expectedProcessor.PacketType} was not cached!");
            }
        }
    }

    /// <summary>
    /// A packet type without a specialized processor must not create a processor
    /// </summary>
    [TestMethod]
    public void ProcessorFactoryGetProcessorUnsupportedPacketTypeReturnsNull()
    {
        using (var processorFactory = new ProcessorFactory(TestData.ServiceProvider))
        {
            var processor = processorFactory.GetProcessor(CreatePacketHeader(PacketTypes.Motion, 1, 1), CreateGameInfo());

            Assert.IsNull(processor, "A processor was created for an unsupported packet type!");
        }
    }

    #endregion // Test methods

    #region Private methods

    /// <summary>
    /// Creates a packet header for the given packet type, session and frame
    /// </summary>
    /// <param name="packetType">Packet type</param>
    /// <param name="uniqueSessionId">Unique session id</param>
    /// <param name="frameIdentifier">Frame identifier</param>
    /// <returns>Packet header</returns>
    private static PacketHeader CreatePacketHeader(PacketTypes packetType,
                                                   ulong uniqueSessionId,
                                                   uint frameIdentifier)
    {
        return new PacketHeader
               {
                   PacketType = packetType,
                   UniqueSessionId = uniqueSessionId,
                   FrameIdentifier = frameIdentifier
               };
    }

    /// <summary>
    /// Creates the runtime game information used by the tested processors
    /// </summary>
    /// <returns>Runtime game information</returns>
    private static LiveGameData CreateGameInfo()
    {
        return new LiveGameData
               {
                   GameVersion = 2025
               };
    }

    #endregion // Private methods
}