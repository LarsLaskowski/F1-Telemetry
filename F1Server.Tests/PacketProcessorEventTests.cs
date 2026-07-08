using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.EventArgs;
using F1Server.Data;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;

namespace F1Server.Tests;

/// <summary>
/// Tests of the packet received event behavior of the packet processor
/// </summary>
[TestClass]
public class PacketProcessorEventTests
{
    #region Static methods

    /// <summary>
    /// Creates a packet processor with an isolated service provider and without database usage
    /// </summary>
    /// <returns>Packet processor instance</returns>
    private static PacketProcessor CreatePacketProcessor()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new F1ServerApplicationData());
        services.AddSingleton(new PacketAnalyzer());

        return new PacketProcessor(services.BuildServiceProvider(), false);
    }

    /// <summary>
    /// Reads a sample packet file into received packet data
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    /// <returns>Received packet data</returns>
    private static ReceivedPacketData GetPacketData(string fileName)
    {
        var packetData = new ReceivedPacketData();

        packetData.SetRawData(File.ReadAllBytes(Path.Combine("SampleData", fileName)));

        Assert.IsNotNull(packetData.PacketHeader, $"Header of {fileName} could not be parsed!");

        return packetData;
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Repeated packets of the same game and session must raise the packet received event only once
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketRepeatedPacketRaisesEventOnce()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            var receivedEvents = new List<PacketReceivedEventArgs>();

            packetProcessor.PacketReceived += (sender, args) => receivedEvents.Add(args);

            var firstPacket = GetPacketData("F1-2025-Session.packet");
            var secondPacket = GetPacketData("F1-2025-Session.packet");

            var isFirstProcessed = packetProcessor.ProcessPacket(firstPacket);
            var isSecondProcessed = packetProcessor.ProcessPacket(secondPacket);

            Assert.IsTrue(isFirstProcessed, "First packet not correctly processed!");
            Assert.IsTrue(isSecondProcessed, "Second packet not correctly processed!");

            Assert.HasCount(1, receivedEvents, "Event must be raised exactly once for repeated packets!");
            Assert.AreEqual(2025, receivedEvents[0].GameVersion, "Event argument contains wrong game version!");
            Assert.AreEqual(firstPacket.PacketHeader?.UniqueSessionId, receivedEvents[0].SessionId, "Event argument contains wrong session id!");
        }
    }

    /// <summary>
    /// A packet of a different game version and session must raise the packet received event again
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketGameVersionChangeRaisesEventAgain()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            var receivedEvents = new List<PacketReceivedEventArgs>();

            packetProcessor.PacketReceived += (sender, args) => receivedEvents.Add(args);

            packetProcessor.ProcessPacket(GetPacketData("F1-2024-Session.packet"));
            packetProcessor.ProcessPacket(GetPacketData("F1-2025-Session.packet"));

            Assert.HasCount(2, receivedEvents, "Event must be raised again for a changed game version or session!");
            Assert.AreEqual(2024, receivedEvents[0].GameVersion, "First event argument contains wrong game version!");
            Assert.AreEqual(2025, receivedEvents[1].GameVersion, "Second event argument contains wrong game version!");
        }
    }

    /// <summary>
    /// An exception in an event subscriber must be observed and must not stop packet processing
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketSubscriberExceptionKeepsProcessing()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            packetProcessor.PacketReceived += (sender, args) => throw new InvalidOperationException("Test subscriber failure!");

            var isProcessed = packetProcessor.ProcessPacket(GetPacketData("F1-2025-Session.packet"));

            Assert.IsTrue(isProcessed, "Packet must be processed although a subscriber has thrown an exception!");
        }
    }

    #endregion // Methods
}