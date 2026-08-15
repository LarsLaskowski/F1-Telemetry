using F1Server.Core.Data;
using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Tests.Data;

namespace F1Server.Tests;

/// <summary>
/// Test the packet processor class
/// </summary>
[TestClass]
public class PacketProcessor2025Tests
{
    #region Fields

    private static ReceivedPacketData _packetData24;
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
        var is2024File = File.Exists(@"SampleData/F1-2024-Session.packet");
        var is2025File = File.Exists(@"SampleData/F1-2025-Session.packet");

        if (is2024File && is2025File)
        {
            var packetContent24 = File.ReadAllBytes(@"SampleData/F1-2024-Session.packet");

            _packetData24 = new ReceivedPacketData();

            _packetData24.SetRawData(packetContent24);

            var packetContent25 = File.ReadAllBytes(@"SampleData/F1-2025-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(packetContent25);

            var isCorrect = _packetData24.PacketHeader != null && _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialization of test packets failed!");

            _processorFactory = new ProcessorFactory(TestData.ServiceProvider);
        }
        else
        {
            Assert.IsTrue(is2024File, "File F1-2024-Session.packet is missing!");
            Assert.IsTrue(is2025File, "File F1-2025-Session.packet is missing!");
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
        Assert.IsNotNull(_packetData24.PacketHeader, "Missing header object!");
        Assert.IsNotNull(_processorFactory, "Missing processor object!");

        var gameData = new LiveGameData()
                       {
                           GameVersion = _packetData24.PacketHeader.GameVersion
                       };

        var processor = _processorFactory.GetProcessor(_packetData24.PacketHeader, gameData);

        Assert.IsNotNull(processor, "No processor object!");
        Assert.AreEqual(typeof(SessionProcessor), processor.GetType(), "No session processor object!");
    }

    /// <summary>
    /// Test receiving correct session processor object
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessorIsNewSessionProcessor()
    {
        Assert.IsNotNull(_packetData24.PacketHeader, "Missing header 2024 object!");
        Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2025 object!");
        Assert.IsNotNull(_processorFactory, "Missing processor object!");

        var gameData = new LiveGameData()
                       {
                           GameVersion = _packetData24.PacketHeader.GameVersion
                       };

        var processor24 = _processorFactory.GetProcessor(_packetData24.PacketHeader, gameData);

        Assert.IsNotNull(processor24, "No processor (2024) object!");
        Assert.AreEqual(typeof(SessionProcessor), processor24.GetType(), "No session processor (2024) object!");

        gameData.GameVersion = _packetData.PacketHeader.GameVersion;

        var processor25 = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

        Assert.IsNotNull(processor25, "No processor (2025) object!");
        Assert.AreEqual(typeof(SessionProcessor), processor25.GetType(), "No session processor (2025) object!");

        Assert.AreNotEqual(processor25, processor24, "Same session processor object!");
    }

    /// <summary>
    /// Test that repeated calls for the same F1 2025 session reuse the cached processor instance
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessor2025SameSessionReturnsSameInstance()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2025 object!");
        Assert.IsNotNull(_processorFactory, "Missing processor object!");

        var gameData = new LiveGameData()
                       {
                           GameVersion = _packetData.PacketHeader.GameVersion
                       };

        var firstProcessor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);
        var secondProcessor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

        Assert.IsNotNull(firstProcessor, "No processor object on first call!");
        Assert.IsNotNull(secondProcessor, "No processor object on second call!");
        Assert.AreEqual(firstProcessor, secondProcessor, "Processor was not reused within the same session!");
    }

    /// <summary>
    /// Test that the processor tracks the current frame identifier of the F1 2025 header
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessor2025TracksCurrentFrameIdentifier()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2025 object!");
        Assert.IsNotNull(_processorFactory, "Missing processor object!");

        var gameData = new LiveGameData()
                       {
                           GameVersion = _packetData.PacketHeader.GameVersion
                       };

        var processor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

        Assert.IsNotNull(processor, "No processor object!");
        Assert.AreEqual(_packetData.PacketHeader.FrameIdentifier, processor.CurrentFrameIdentifier, "Processor did not track the current frame identifier!");
    }

    /// <summary>
    /// Test that a new F1 2025 session id causes the factory to create a new processor instance
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessor2025NewSessionIdReturnsNewInstance()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2025 object!");
        Assert.IsNotNull(_processorFactory, "Missing processor object!");

        var gameData = new LiveGameData()
                       {
                           GameVersion = _packetData.PacketHeader.GameVersion
                       };

        var firstProcessor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

        var secondSessionHeader = new PacketHeader
                                  {
                                      GameVersion = _packetData.PacketHeader.GameVersion,
                                      PacketType = _packetData.PacketHeader.PacketType,
                                      FrameIdentifier = _packetData.PacketHeader.FrameIdentifier,
                                      UniqueSessionId = _packetData.PacketHeader.UniqueSessionId + 1
                                  };

        var secondProcessor = _processorFactory.GetProcessor(secondSessionHeader, gameData);

        Assert.IsNotNull(firstProcessor, "No processor object for the first session!");
        Assert.IsNotNull(secondProcessor, "No processor object for the second session!");
        Assert.AreEqual(typeof(SessionProcessor), secondProcessor.GetType(), "No session processor object for the second session!");
        Assert.AreNotEqual(firstProcessor, secondProcessor, "Processor was not recreated after a session change!");
    }

    #endregion // Methods
}