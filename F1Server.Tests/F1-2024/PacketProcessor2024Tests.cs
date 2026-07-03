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

    #endregion // Methods
}