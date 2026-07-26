using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

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
    /// <param name="withLogger">Whether to attach a no-op logger, to exercise the logger-attached code paths</param>
    /// <returns>Packet processor instance</returns>
    private static PacketProcessor CreatePacketProcessor(bool withLogger = false)
    {
        var services = new ServiceCollection();

        var applicationData = new F1ServerApplicationData();

        if (withLogger)
        {
            applicationData.Logger = NullLogger.Instance;
        }

        services.AddSingleton(applicationData);
        services.AddSingleton(new PacketAnalyzer());

        return new PacketProcessor(services.BuildServiceProvider(), false);
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// A packet shorter than the minimum header size must not be processed and must record the bounded rejection code as the last error,
    /// with a logger attached so the rejection warning is actually logged
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketTooShortHeaderSetsRejectionCodeAsLastError()
    {
        using (var packetProcessor = CreatePacketProcessor(withLogger: true))
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