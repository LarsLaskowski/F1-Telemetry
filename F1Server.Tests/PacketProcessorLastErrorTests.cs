using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.Observability;
using F1Server.Service.Runtime;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests;

/// <summary>
/// Tests of the error reporting of the packet processor
/// </summary>
[TestClass]
public class PacketProcessorLastErrorTests
{
    #region Constants

    /// <summary>
    /// Length of truncated test packets, longer than every packet header but shorter than every expected packet size
    /// </summary>
    private const int TruncatedPacketLength = 32;

    /// <summary>
    /// Offset of the packet type inside a packet header of a game version before 2023
    /// </summary>
    private const int PacketTypeOffset = 5;

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Creates a packet processor with an isolated service provider and without database usage
    /// </summary>
    /// <returns>Packet processor instance</returns>
    private static PacketProcessor CreatePacketProcessor()
    {
        var applicationData = new F1ServerApplicationData
                              {
                                  AppMetrics = new AppMetrics(),
                                  Logger = NullLogger.Instance
                              };

        var services = new ServiceCollection();

        services.AddSingleton(applicationData);
        services.AddSingleton(new PacketAnalyzer());

        return new PacketProcessor(services.BuildServiceProvider(), false);
    }

    /// <summary>
    /// Reads a sample packet file into received packet data
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    /// <param name="isTruncated">Truncate the packet to a length that is too short for its payload?</param>
    /// <returns>Received packet data</returns>
    private static ReceivedPacketData GetPacketData(string fileName, bool isTruncated)
    {
        var packetContent = File.ReadAllBytes(Path.Combine("SampleData", fileName));

        var receivedData = new ReceivedPacketData();

        receivedData.SetRawData(isTruncated ? packetContent.AsSpan(0, TruncatedPacketLength).ToArray() : packetContent);

        Assert.IsNotNull(receivedData.PacketHeader, $"Header of {fileName} could not be parsed!");

        return receivedData;
    }

    /// <summary>
    /// Reads a sample packet file and replaces its packet type, so that a packet type can be tested
    /// for which the reported game version provides no transformation
    /// </summary>
    /// <param name="fileName">Name of the sample packet file of a game version before 2023</param>
    /// <param name="packetType">Packet type to write into the packet header</param>
    /// <returns>Received packet data</returns>
    private static ReceivedPacketData GetPacketDataWithPacketType(string fileName, PacketTypes packetType)
    {
        var packetContent = File.ReadAllBytes(Path.Combine("SampleData", fileName));

        packetContent[PacketTypeOffset] = (byte)(packetType - 1);

        var receivedData = new ReceivedPacketData();

        receivedData.SetRawData(packetContent);

        Assert.AreEqual(packetType, receivedData.PacketHeader?.PacketType, $"Packet type of {fileName} could not be replaced!");

        return receivedData;
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// A packet that cannot be converted must report the reason of the packet analyzer
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2025-Session.packet")]
    [DataRow("F1-2025-CarTelemetry.packet")]
    [DataRow("F1-2025-Participants.packet")]
    public void PacketProcessorProcessPacketTruncatedPacketReportsLastError(string fileName)
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            var isProcessed = packetProcessor.ProcessPacket(GetPacketData(fileName, true));

            Assert.IsFalse(isProcessed, $"Truncated packet {fileName} must not be processed!");
            Assert.IsNotEmpty(packetProcessor.LastError, $"Truncated packet {fileName} must report an error!");
        }
    }

    /// <summary>
    /// A converted packet must not report an error, also not the error of a previously rejected packet
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketAfterRejectedPacketClearsLastError()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            packetProcessor.ProcessPacket(GetPacketData("F1-2025-Session.packet", true));

            Assert.IsNotEmpty(packetProcessor.LastError, "Truncated session packet must report an error!");

            var isProcessed = packetProcessor.ProcessPacket(GetPacketData("F1-2025-Session.packet", false));

            Assert.IsTrue(isProcessed, "Full size session packet must be processed!");
            Assert.IsEmpty(packetProcessor.LastError, "Error of the previous packet must not be reported again!");
        }
    }

    /// <summary>
    /// A packet type that is received but not converted must not report an error
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketUnconvertedPacketTypeKeepsLastErrorEmpty()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            packetProcessor.ProcessPacket(GetPacketData("F1-2023-TyreSets.packet", false));

            Assert.IsEmpty(packetProcessor.LastError, "A packet type that is not converted must not report an error!");
        }
    }

    /// <summary>
    /// A packet type that the reported game version does not support must not report an error
    /// </summary>
    [TestMethod]
    public void PacketProcessorProcessPacketUnsupportedGameVersionKeepsLastErrorEmpty()
    {
        using (var packetProcessor = CreatePacketProcessor())
        {
            // The session history packet exists since F1 2021, so the transformation of a F1 2020 packet returns no object and no reason
            var isProcessed = packetProcessor.ProcessPacket(GetPacketDataWithPacketType("F1-2020-Session.packet", PacketTypes.SessionHistory));

            Assert.IsFalse(isProcessed, "A packet without a transformation for its game version must not be processed!");
            Assert.IsEmpty(packetProcessor.LastError, "A packet type that the game version does not support must not report an error!");
        }
    }

    #endregion // Methods
}