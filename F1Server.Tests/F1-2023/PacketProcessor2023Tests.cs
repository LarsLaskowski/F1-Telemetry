using F1Server.Core.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Tests.Data;

namespace F1Server.Tests;

/// <summary>
/// Test the packet processor class
/// </summary>
[TestClass]
public class PacketProcessor2023Tests
{
    #region Fields

    private static ReceivedPacketData _packetData22;
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
        var is2022File = File.Exists(@"SampleData/F1-2022-Session.packet");
        var is2023File = File.Exists(@"SampleData/F1-2023-Session.packet");

        if (is2022File && is2023File)
        {
            var packetContent22 = File.ReadAllBytes(@"SampleData/F1-2022-Session.packet");

            _packetData22 = new ReceivedPacketData();

            _packetData22.SetRawData(packetContent22);

            var packetContent23 = File.ReadAllBytes(@"SampleData/F1-2023-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(packetContent23);

            var isCorrect = _packetData22.PacketHeader != null && _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialization of test packets failed!");

            _processorFactory = new ProcessorFactory(TestData.ServiceProvider);
        }
        else
        {
            Assert.IsTrue(is2022File, "File F1-2022-Session.packet is missing!");
            Assert.IsTrue(is2023File, "File F1-2023-Session.packet is missing!");
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
        if (_packetData22.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData22.PacketHeader.GameVersion
                           };

            var processor = _processorFactory.GetProcessor(_packetData22.PacketHeader, gameData);

            Assert.IsNotNull(processor, "No processor object!");
            Assert.AreEqual(typeof(SessionProcessor), processor.GetType(), "No session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData22.PacketHeader, "Missing header object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test receiving correct session processor object
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessorIsNewSessionProcessor()
    {
        if (_packetData22.PacketHeader != null && _packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData22.PacketHeader.GameVersion
                           };

            var processor22 = _processorFactory.GetProcessor(_packetData22.PacketHeader, gameData);

            Assert.IsNotNull(processor22, "No processor (2022) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor22.GetType(), "No session processor (2022) object!");

            gameData.GameVersion = _packetData.PacketHeader.GameVersion;

            var processor23 = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(processor23, "No processor (2023) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor23.GetType(), "No session processor (2023) object!");

            Assert.AreNotEqual(processor23, processor22, "Same session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData22.PacketHeader, "Missing header 2022 object!");
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2023 object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test that repeated requests for the same 2023 session reuse the cached processor instance,
    /// instead of creating a new one on every call
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessorReturns2023SameInstanceForSameSession()
    {
        if (_packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData.PacketHeader.GameVersion
                           };

            var firstProcessor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);
            var secondProcessor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(firstProcessor, "No processor (first call) object!");
            Assert.IsNotNull(secondProcessor, "No processor (second call) object!");
            Assert.AreSame(firstProcessor, secondProcessor, "Processor was not reused for the same session!");
        }
        else
        {
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2023 object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test that the frame and session timestamp of the returned 2023 processor are updated from
    /// the packet header of the requesting packet
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessor2023UpdatesFrameIdentifier()
    {
        if (_packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData.PacketHeader.GameVersion
                           };

            var processor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(processor, "No processor object!");
            Assert.AreEqual(_packetData.PacketHeader.FrameIdentifier, processor.CurrentFrameIdentifier, "Current frame identifier was not updated!");
            Assert.AreEqual(_packetData.PacketHeader.SessionTimeNum, processor.SessionTimestampNum, "Session timestamp was not updated!");
        }
        else
        {
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2023 object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    #endregion // Methods
}