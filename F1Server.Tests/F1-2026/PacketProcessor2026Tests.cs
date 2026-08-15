using F1Server.Core.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Tests.Data;

namespace F1Server.Tests;

/// <summary>
/// Test the packet processor class
/// </summary>
[TestClass]
public class PacketProcessor2026Tests
{
    #region Fields

    private static ReceivedPacketData _packetData23;
    private static ReceivedPacketData _packetData;
    private static ProcessorFactory? _processorFactory;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketProcessorTestsInit(TestContext testContext)
    {
        var is2023File = File.Exists(@"SampleData/F1-2023-Session.packet");
        var is2026File = File.Exists(@"SampleData/F1-2026-Session.packet");

        if (is2023File && is2026File)
        {
            var packetContent23 = File.ReadAllBytes(@"SampleData/F1-2023-Session.packet");

            _packetData23 = new ReceivedPacketData();

            _packetData23.SetRawData(packetContent23);

            var packetContent26 = File.ReadAllBytes(@"SampleData/F1-2026-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(packetContent26);

            var isCorrect = _packetData23.PacketHeader != null && _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialization of test packets failed!");

            _processorFactory = new ProcessorFactory(TestData.ServiceProvider);
        }
        else
        {
            Assert.IsTrue(is2023File, "File F1-2023-Session.packet is missing!");
            Assert.IsTrue(is2026File, "File F1-2026-Session.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods

    /// <summary>
    /// Test receiving correct session processor object
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessorIsSessionProcessor()
    {
        if (_packetData23.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData23.PacketHeader.GameVersion
                           };

            var processor = _processorFactory.GetProcessor(_packetData23.PacketHeader, gameData);

            Assert.IsNotNull(processor, "No processor object!");
            Assert.AreEqual(typeof(SessionProcessor), processor.GetType(), "No session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData23.PacketHeader, "Missing header object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test receiving correct session processor object
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessorIsNewSessionProcessor()
    {
        if (_packetData23.PacketHeader != null && _packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData23.PacketHeader.GameVersion
                           };

            var processor23 = _processorFactory.GetProcessor(_packetData23.PacketHeader, gameData);

            Assert.IsNotNull(processor23, "No processor (2023) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor23.GetType(), "No session processor (2023) object!");

            gameData.GameVersion = _packetData.PacketHeader.GameVersion;

            var processor26 = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(processor26, "No processor (2026) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor26.GetType(), "No session processor (2026) object!");

            Assert.AreNotEqual(processor26, processor23, "Same session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData23.PacketHeader, "Missing header 2023 object!");
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2026 object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test that the factory selects the correct processor type for every F1 2026 packet type that has
    /// a dedicated processor, using real F1 2026 sample packets instead of only the session packet
    /// </summary>
    [TestMethod]
    public void PacketProcessorGetProcessor2026ReturnsCorrectProcessorPerPacketType()
    {
        if (_packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData.PacketHeader.GameVersion
                           };

            (string SampleFile, Type ProcessorType)[] expectedProcessors = [
                                                                               (@"SampleData/F1-2026-Participants.packet", typeof(ParticipantsProcessor)),
                                                                               (@"SampleData/F1-2026-LapData.packet", typeof(LapDataProcessor)),
                                                                               (@"SampleData/F1-2026-CarStatus.packet", typeof(CarStatusProcessor)),
                                                                               (@"SampleData/F1-2026-CarTelemetry.packet", typeof(CarTelemetryProcessor)),
                                                                               (@"SampleData/F1-2026-SessionHistory.packet", typeof(SessionHistoryProcessor)),
                                                                               (@"SampleData/F1-2026-FinalClassification.packet", typeof(FinalClassificationProcessor)),
                                                                               (@"SampleData/F1-2026-TimeTrial.packet", typeof(TimeTrialProcessor)),
                                                                               (@"SampleData/F1-2026-LapPositions.packet", typeof(LapPositionsProcessor))
                                                                           ];

            foreach (var (sampleFile, processorType) in expectedProcessors)
            {
                var samplePacketData = new ReceivedPacketData();

                samplePacketData.SetRawData(File.ReadAllBytes(sampleFile));

                Assert.IsNotNull(samplePacketData.PacketHeader, $"Missing header for sample file {sampleFile}!");

                var processor = _processorFactory.GetProcessor(samplePacketData.PacketHeader, gameData);

                Assert.IsNotNull(processor, $"No processor object for sample file {sampleFile}!");
                Assert.AreEqual(processorType, processor.GetType(), $"Wrong processor type for sample file {sampleFile}!");
            }
        }
        else
        {
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header (2026) object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test that the factory tracks the frame identifier and session timestamp of the real F1 2026
    /// packet header on the created processor, so downstream logic can rely on both reflecting the
    /// actually received packet rather than a stale or default value
    /// </summary>
    [TestMethod]
    public void PacketProcessorGetProcessor2026TracksFrameIdentifierOfPacket()
    {
        if (_packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData.PacketHeader.GameVersion
                           };

            var processor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(processor, "No processor (2026) object!");
            Assert.AreEqual(_packetData.PacketHeader.FrameIdentifier, processor.CurrentFrameIdentifier, "Processor did not track the F1 2026 packet frame identifier!");
            Assert.AreEqual(_packetData.PacketHeader.SessionTimeNum, processor.SessionTimestampNum, "Processor did not track the F1 2026 packet session timestamp!");
        }
        else
        {
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header (2026) object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    #endregion // Methods
}