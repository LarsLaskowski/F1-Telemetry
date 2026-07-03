using System.IO;

using F1Server.Core.Data;
using F1Server.Data;
using F1Server.Service.Processors;
using F1Server.Tests.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Test the packet processor class
/// </summary>
[TestClass]
public class PacketProcessor2021Tests
{
    #region Fields

    private static ReceivedPacketData _packetData20;
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
        var is2020File = File.Exists(@"SampleData/F1-2020-Session.packet");
        var is2021File = File.Exists(@"SampleData/F1-2021-Session.packet");

        if (is2020File && is2021File)
        {
            var packetContent20 = File.ReadAllBytes(@"SampleData/F1-2020-Session.packet");

            _packetData20 = new ReceivedPacketData();

            _packetData20.SetRawData(packetContent20);

            var packetContent21 = File.ReadAllBytes(@"SampleData/F1-2021-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(packetContent21);

            var isCorrect = _packetData20.PacketHeader != null && _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialization of test packets failed!");

            _processorFactory = new ProcessorFactory(TestData.ServiceProvider);
        }
        else
        {
            Assert.IsTrue(is2020File, "File F1-2020-Session.packet is missing!");
            Assert.IsTrue(is2021File, "File F1-2021-Session.packet is missing!");
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
        if (_packetData20.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData20.PacketHeader.GameVersion
                           };

            var processor = _processorFactory.GetProcessor(_packetData20.PacketHeader, gameData);

            Assert.IsNotNull(processor, "No processor object!");
            Assert.AreEqual(typeof(SessionProcessor), processor.GetType(), "No session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData20.PacketHeader, "Missing header object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test receiving correct session processor object
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessorIsNewSessionProcessor()
    {
        if (_packetData20.PacketHeader != null && _packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData20.PacketHeader.GameVersion
                           };

            var processor20 = _processorFactory.GetProcessor(_packetData20.PacketHeader, gameData);

            Assert.IsNotNull(processor20, "No processor (2020) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor20.GetType(), "No session processor (2020) object!");

            gameData.GameVersion = _packetData.PacketHeader.GameVersion;

            var processor21 = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(processor21, "No processor (2021) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor21.GetType(), "No session processor (2021) object!");

            Assert.AreNotEqual(processor21, processor20, "Same session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData20.PacketHeader, "Missing header 2020 object!");
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2021 object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    #endregion // Methods
}