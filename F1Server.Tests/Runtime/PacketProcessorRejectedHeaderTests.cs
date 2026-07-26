using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;

namespace F1Server.Tests.Runtime;

/// <summary>
/// Tests of the rejected packet header handling of the packet processor
/// </summary>
[TestClass]
public class PacketProcessorRejectedHeaderTests
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

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// A packet shorter than the minimum header size must not be processed and must record the bounded rejection code as the last error
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketTooShortHeaderSetsRejectionCodeAsLastError()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            var packetData = new ReceivedPacketData();

            packetData.SetRawData(new byte[4]);

            var isProcessed = packetProcessor.ProcessPacket(packetData);

            Assert.IsFalse(isProcessed, "A packet with an undersized header must not be reported as processed!");
            Assert.AreEqual(HeaderRejectionCode.PacketTooShort.ToString(), packetProcessor.LastError, "LastError must carry the bounded rejection code!");
        }
    }

    /// <summary>
    /// A packet reporting a 2023+ game version but shorter than the 2023+ header size must not be processed
    /// and must record the bounded rejection code as the last error
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketUndersized2023HeaderSetsRejectionCodeAsLastError()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            var rawData = new byte[ConstData.F12019HeaderSize];

            rawData[0] = 2023 & 0xFF;
            rawData[1] = (2023 >> 8) & 0xFF;

            var packetData = new ReceivedPacketData();

            packetData.SetRawData(rawData);

            var isProcessed = packetProcessor.ProcessPacket(packetData);

            Assert.IsFalse(isProcessed, "A packet with an undersized 2023+ header must not be reported as processed!");
            Assert.AreEqual(HeaderRejectionCode.Undersized2023Header.ToString(), packetProcessor.LastError, "LastError must carry the bounded rejection code!");
        }
    }

    #endregion // Methods
}