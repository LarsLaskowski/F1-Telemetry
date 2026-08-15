using F1Server.Core.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Tests.Data;

namespace F1Server.Tests;

/// <summary>
/// Test the packet processor class
/// </summary>
[TestClass]
public class PacketProcessor2024Tests
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
        var is2024File = File.Exists(@"SampleData/F1-2024-Session.packet");

        if (is2023File && is2024File)
        {
            var packetContent23 = File.ReadAllBytes(@"SampleData/F1-2023-Session.packet");

            _packetData23 = new ReceivedPacketData();

            _packetData23.SetRawData(packetContent23);

            var packetContent24 = File.ReadAllBytes(@"SampleData/F1-2024-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(packetContent24);

            var isCorrect = _packetData23.PacketHeader != null && _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialization of test packets failed!");

            _processorFactory = new ProcessorFactory(TestData.ServiceProvider);
        }
        else
        {
            Assert.IsTrue(is2023File, "File F1-2023-Session.packet is missing!");
            Assert.IsTrue(is2024File, "File F1-2024-Session.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Load the given F1 2024 packet file and assert that the processor factory returns a processor object of the expected type
    /// </summary>
    /// <param name="samplePacketPath">Path of the sample packet file</param>
    /// <param name="expectedProcessorType">Expected type of the processor</param>
    /// <param name="failureMessage">Message used when the returned processor does not match the expected type</param>
    private static void AssertProcessorType(string samplePacketPath, Type expectedProcessorType, string failureMessage)
    {
        var isFile = File.Exists(samplePacketPath);

        Assert.IsTrue(isFile, $"File {samplePacketPath} is missing!");

        var packetContent = File.ReadAllBytes(samplePacketPath);
        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Missing packet header!");

        using var processorFactory = new ProcessorFactory(TestData.ServiceProvider);

        var gameData = new LiveGameData()
                       {
                           GameVersion = packetData.PacketHeader.GameVersion
                       };

        var processor = processorFactory.GetProcessor(packetData.PacketHeader, gameData);

        Assert.IsNotNull(processor, "No processor object!");
        Assert.AreEqual(expectedProcessorType, processor.GetType(), failureMessage);
    }

    #endregion // Static methods

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

            var processor24 = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(processor24, "No processor (2024) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor24.GetType(), "No session processor (2024) object!");

            Assert.AreNotEqual(processor24, processor23, "Same session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData23.PacketHeader, "Missing header 2023 object!");
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2024 object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test receiving a cached processor instance when the factory is asked twice for the same session (2024)
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessorIsCachedForSameSession2024()
    {
        if (_packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData.PacketHeader.GameVersion
                           };

            var firstProcessor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);
            var secondProcessor = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(firstProcessor, "No processor object!");
            Assert.AreSame(firstProcessor, secondProcessor, "Processor was not reused for the same session!");
        }
        else
        {
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test receiving correct participants processor object (2024)
    /// </summary>
    [TestMethod]
    public void PacketProcessorParticipantsProcessorIsParticipantsProcessor2024()
    {
        AssertProcessorType(@"SampleData/F1-2024-Participants.packet", typeof(ParticipantsProcessor), "No participants processor object!");
    }

    /// <summary>
    /// Test receiving correct lap data processor object (2024)
    /// </summary>
    [TestMethod]
    public void PacketProcessorLapDataProcessorIsLapDataProcessor2024()
    {
        AssertProcessorType(@"SampleData/F1-2024-LapData.packet", typeof(LapDataProcessor), "No lap data processor object!");
    }

    /// <summary>
    /// Test receiving correct session history processor object (2024)
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionHistoryProcessorIsSessionHistoryProcessor2024()
    {
        AssertProcessorType(@"SampleData/F1-2024-SessionHistory.packet", typeof(SessionHistoryProcessor), "No session history processor object!");
    }

    /// <summary>
    /// Test receiving correct car status processor object (2024)
    /// </summary>
    [TestMethod]
    public void PacketProcessorCarStatusProcessorIsCarStatusProcessor2024()
    {
        AssertProcessorType(@"SampleData/F1-2024-CarStatus.packet", typeof(CarStatusProcessor), "No car status processor object!");
    }

    /// <summary>
    /// Test receiving correct car telemetry processor object (2024)
    /// </summary>
    [TestMethod]
    public void PacketProcessorCarTelemetryProcessorIsCarTelemetryProcessor2024()
    {
        AssertProcessorType(@"SampleData/F1-2024-CarTelemetry.packet", typeof(CarTelemetryProcessor), "No car telemetry processor object!");
    }

    /// <summary>
    /// Test receiving correct final classification processor object (2024)
    /// </summary>
    [TestMethod]
    public void PacketProcessorFinalClassificationProcessorIsFinalClassificationProcessor2024()
    {
        AssertProcessorType(@"SampleData/F1-2024-FinalClassification.packet", typeof(FinalClassificationProcessor), "No final classification processor object!");
    }

    /// <summary>
    /// Test receiving correct time trial processor object (2024)
    /// </summary>
    [TestMethod]
    public void PacketProcessorTimeTrialProcessorIsTimeTrialProcessor2024()
    {
        AssertProcessorType(@"SampleData/F1-2024-TimeTrial.packet", typeof(TimeTrialProcessor), "No time trial processor object!");
    }

    #endregion // Methods
}