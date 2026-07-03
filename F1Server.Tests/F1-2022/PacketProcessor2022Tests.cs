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
public class PacketProcessor2022Tests
{
    #region Fields

    private static ReceivedPacketData _packetData21;
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
        var is2021File = File.Exists(@"SampleData/F1-2021-Session.packet");
        var is2022File = File.Exists(@"SampleData/F1-2022-Session.packet");

        if (is2021File && is2022File)
        {
            var packetContent21 = File.ReadAllBytes(@"SampleData/F1-2021-Session.packet");

            _packetData21 = new ReceivedPacketData();

            _packetData21.SetRawData(packetContent21);

            var packetContent22 = File.ReadAllBytes(@"SampleData/F1-2022-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(packetContent22);

            var isCorrect = _packetData21.PacketHeader != null && _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialization of test packets failed!");

            _processorFactory = new ProcessorFactory(TestData.ServiceProvider);
        }
        else
        {
            Assert.IsTrue(is2021File, "File F1-2021-Session.packet is missing!");
            Assert.IsTrue(is2022File, "File F1-2022-Session.packet is missing!");
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
        if (_packetData21.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData21.PacketHeader.GameVersion
                           };

            var processor = _processorFactory.GetProcessor(_packetData21.PacketHeader, gameData);

            Assert.IsNotNull(processor, "No processor object!");
            Assert.AreEqual(typeof(SessionProcessor), processor.GetType(), "No session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData21.PacketHeader, "Missing header object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    /// <summary>
    /// Test receiving correct session processor object
    /// </summary>
    [TestMethod]
    public void PacketProcessorSessionProcessorIsNewSessionProcessor()
    {
        if (_packetData21.PacketHeader != null && _packetData.PacketHeader != null && _processorFactory != null)
        {
            var gameData = new LiveGameData()
                           {
                               GameVersion = _packetData21.PacketHeader.GameVersion
                           };

            var processor21 = _processorFactory.GetProcessor(_packetData21.PacketHeader, gameData);

            Assert.IsNotNull(processor21, "No processor (2021) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor21.GetType(), "No session processor (2021) object!");

            gameData.GameVersion = _packetData.PacketHeader.GameVersion;

            var processor22 = _processorFactory.GetProcessor(_packetData.PacketHeader, gameData);

            Assert.IsNotNull(processor22, "No processor (2022) object!");
            Assert.AreEqual(typeof(SessionProcessor), processor22.GetType(), "No session processor (2022) object!");

            Assert.AreNotEqual(processor22, processor21, "Same session processor object!");
        }
        else
        {
            Assert.IsNotNull(_packetData21.PacketHeader, "Missing header 2021 object!");
            Assert.IsNotNull(_packetData.PacketHeader, "Missing header 2022 object!");
            Assert.IsNotNull(_processorFactory, "Missing processor object!");
        }
    }

    #endregion // Methods
}