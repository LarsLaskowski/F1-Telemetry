using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Packets.Data;

namespace F1Server.Tests;

/// <summary>
/// Tests of the error reporting of the packet to object converters
/// </summary>
[TestClass]
public class PacketConverterLastErrorTests
{
    #region Constants

    /// <summary>
    /// Length of truncated test packets, longer than every packet header but shorter than every expected packet size
    /// </summary>
    private const int TruncatedPacketLength = 32;

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Reads a sample packet file and parses its packet header
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    /// <param name="packetContent">Raw content of the sample packet file</param>
    /// <returns>Parsed packet header</returns>
    private static PacketHeader GetPacketHeader(string fileName, out byte[] packetContent)
    {
        packetContent = File.ReadAllBytes(Path.Combine("SampleData", fileName));

        var receivedData = new ReceivedPacketData();

        receivedData.SetRawData(packetContent);

        Assert.IsNotNull(receivedData.PacketHeader, $"Header of {fileName} could not be parsed!");

        return receivedData.PacketHeader;
    }

    /// <summary>
    /// Reads a sample packet file and truncates it to a length that is too short for its payload
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    /// <returns>Received packet data with a valid header and a truncated payload</returns>
    private static ReceivedPacketData GetTruncatedPacket(string fileName)
    {
        var packetContent = File.ReadAllBytes(Path.Combine("SampleData", fileName));

        var receivedData = new ReceivedPacketData();

        receivedData.SetRawData(packetContent.AsSpan(0, TruncatedPacketLength).ToArray());

        Assert.IsNotNull(receivedData.PacketHeader, $"Header of the truncated packet {fileName} could not be parsed!");

        return receivedData;
    }

    /// <summary>
    /// Reads a sample packet file into received packet data
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    /// <returns>Received packet data</returns>
    private static ReceivedPacketData GetFullPacket(string fileName)
    {
        var receivedData = new ReceivedPacketData();

        receivedData.SetRawData(File.ReadAllBytes(Path.Combine("SampleData", fileName)));

        Assert.IsNotNull(receivedData.PacketHeader, $"Header of {fileName} could not be parsed!");

        return receivedData;
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Test to verify that a rejected car telemetry packet reports the reason instead of failing silently
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-CarTelemetry.packet")]
    [DataRow("F1-2021-CarTelemetry.packet")]
    [DataRow("F1-2022-CarTelemetry.packet")]
    [DataRow("F1-2023-CarTelemetry.packet")]
    [DataRow("F1-2024-CarTelemetry.packet")]
    [DataRow("F1-2025-CarTelemetry.packet")]
    [DataRow("F1-2026-CarTelemetry.packet")]
    public void GetCarTelemetryTruncatedPacketSetsLastError(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var carTelemetry = packetAnalyzer.GetCarTelemetry(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

        Assert.IsNull(carTelemetry, $"Truncated car telemetry packet {fileName} must not produce an object!");
        Assert.IsNotEmpty(packetAnalyzer.LastError, $"Truncated car telemetry packet {fileName} must report an error!");
    }

    /// <summary>
    /// Test to verify that a successfully converted car telemetry packet does not report an error
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-CarTelemetry.packet")]
    [DataRow("F1-2021-CarTelemetry.packet")]
    [DataRow("F1-2022-CarTelemetry.packet")]
    [DataRow("F1-2023-CarTelemetry.packet")]
    [DataRow("F1-2024-CarTelemetry.packet")]
    [DataRow("F1-2025-CarTelemetry.packet")]
    [DataRow("F1-2026-CarTelemetry.packet")]
    public void GetCarTelemetryFullPacketKeepsLastErrorEmpty(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var carTelemetry = packetAnalyzer.GetCarTelemetry(packetHeader, packetContent);

        Assert.IsNotNull(carTelemetry, $"Full size car telemetry packet {fileName} must produce an object!");
        Assert.IsEmpty(packetAnalyzer.LastError, $"Full size car telemetry packet {fileName} must not report an error!");
    }

    /// <summary>
    /// Test to verify that a rejected session packet reports the reason instead of failing silently
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-Session.packet")]
    [DataRow("F1-2021-Session.packet")]
    [DataRow("F1-2022-Session.packet")]
    [DataRow("F1-2023-Session.packet")]
    [DataRow("F1-2024-Session.packet")]
    [DataRow("F1-2025-Session.packet")]
    [DataRow("F1-2026-Session.packet")]
    public void GetSessionDataTruncatedPacketSetsLastError(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var sessionData = packetAnalyzer.GetSessionData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

        Assert.IsNull(sessionData, $"Truncated session packet {fileName} must not produce an object!");
        Assert.IsNotEmpty(packetAnalyzer.LastError, $"Truncated session packet {fileName} must report an error!");
    }

    /// <summary>
    /// Test to verify that a successfully converted session packet does not report an error
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-Session.packet")]
    [DataRow("F1-2021-Session.packet")]
    [DataRow("F1-2022-Session.packet")]
    [DataRow("F1-2023-Session.packet")]
    [DataRow("F1-2024-Session.packet")]
    [DataRow("F1-2025-Session.packet")]
    [DataRow("F1-2026-Session.packet")]
    public void GetSessionDataFullPacketKeepsLastErrorEmpty(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var sessionData = packetAnalyzer.GetSessionData(packetHeader, packetContent);

        Assert.IsNotNull(sessionData, $"Full size session packet {fileName} must produce an object!");
        Assert.IsEmpty(packetAnalyzer.LastError, $"Full size session packet {fileName} must not report an error!");
    }

    /// <summary>
    /// Test to verify that every transformation reachable through the packet analyzer reports the reason
    /// of a rejected packet instead of returning no object without an explanation
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-CarStatus.packet")]
    [DataRow("F1-2026-CarStatus.packet")]
    [DataRow("F1-2020-CarTelemetry.packet")]
    [DataRow("F1-2026-CarTelemetry.packet")]
    [DataRow("F1-2020-Event-TopSpeed.packet")]
    [DataRow("F1-2025-Event-Penalty.packet")]
    [DataRow("F1-2021-FinalClassification.packet")]
    [DataRow("F1-2026-FinalClassification.packet")]
    [DataRow("F1-2020-Participants.packet")]
    [DataRow("F1-2026-Participants.packet")]
    [DataRow("F1-2020-Session.packet")]
    [DataRow("F1-2026-Session.packet")]
    [DataRow("F1-2021-SessionHistory.packet")]
    [DataRow("F1-2026-SessionHistory.packet")]
    [DataRow("F1-2024-TimeTrial.packet")]
    public void GetPacketDataTruncatedPacketSetsLastError(string fileName)
    {
        var packetAnalyzer = new PacketAnalyzer();

        var packetData = packetAnalyzer.GetPacketData(GetTruncatedPacket(fileName));

        Assert.IsNull(packetData, $"Truncated packet {fileName} must not produce an object!");
        Assert.IsNotEmpty(packetAnalyzer.LastError, $"Truncated packet {fileName} must report an error!");
    }

    /// <summary>
    /// Test to verify that a successfully converted packet does not report an error
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-CarStatus.packet")]
    [DataRow("F1-2026-CarStatus.packet")]
    [DataRow("F1-2020-CarTelemetry.packet")]
    [DataRow("F1-2026-CarTelemetry.packet")]
    [DataRow("F1-2020-Event-TopSpeed.packet")]
    [DataRow("F1-2025-Event-Penalty.packet")]
    [DataRow("F1-2021-FinalClassification.packet")]
    [DataRow("F1-2026-FinalClassification.packet")]
    [DataRow("F1-2020-LapData.packet")]
    [DataRow("F1-2026-LapData.packet")]
    [DataRow("F1-2020-Participants.packet")]
    [DataRow("F1-2026-Participants.packet")]
    [DataRow("F1-2020-Session.packet")]
    [DataRow("F1-2026-Session.packet")]
    [DataRow("F1-2021-SessionHistory.packet")]
    [DataRow("F1-2026-SessionHistory.packet")]
    [DataRow("F1-2024-TimeTrial.packet")]
    public void GetPacketDataFullPacketKeepsLastErrorEmpty(string fileName)
    {
        var packetAnalyzer = new PacketAnalyzer();

        var packetData = packetAnalyzer.GetPacketData(GetFullPacket(fileName));

        Assert.IsNotNull(packetData, $"Full size packet {fileName} must produce an object!");
        Assert.IsEmpty(packetAnalyzer.LastError, $"Full size packet {fileName} must not report an error!");
    }

    /// <summary>
    /// Test to verify that the error of a rejected packet is not reported again for the next converted packet
    /// </summary>
    [TestMethod]
    public void GetPacketDataAfterRejectedPacketClearsLastError()
    {
        var packetAnalyzer = new PacketAnalyzer();

        packetAnalyzer.GetPacketData(GetTruncatedPacket("F1-2025-Session.packet"));

        Assert.IsNotEmpty(packetAnalyzer.LastError, "Truncated session packet must report an error!");

        var packetData = packetAnalyzer.GetPacketData(GetFullPacket("F1-2025-CarTelemetry.packet"));

        Assert.IsNotNull(packetData, "Full size car telemetry packet must produce an object!");
        Assert.IsEmpty(packetAnalyzer.LastError, "Error of the previous packet must not be reported again!");
    }

    /// <summary>
    /// Test to verify that the lap positions transformation reports a rejected packet, no sample packet exists for this packet type
    /// </summary>
    [TestMethod]
    public void GetLapPositionsDataTruncatedPacketSetsLastError()
    {
        var packetHeader = GetPacketHeader("F1-2026-Session.packet", out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var lapPositionsData = packetAnalyzer.GetLapPositionsData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

        Assert.IsNull(lapPositionsData, "Truncated lap positions packet must not produce an object!");
        Assert.IsNotEmpty(packetAnalyzer.LastError, "Truncated lap positions packet must report an error!");
    }

    /// <summary>
    /// Test to verify that the additional car telemetry transformation reports a rejected packet, no sample packet exists for this packet type
    /// </summary>
    [TestMethod]
    public void GetCarTelemetry2TruncatedPacketSetsLastError()
    {
        var packetHeader = GetPacketHeader("F1-2026-Session.packet", out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var carTelemetry2 = packetAnalyzer.GetCarTelemetry2(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

        Assert.IsNull(carTelemetry2, "Truncated additional car telemetry packet must not produce an object!");
        Assert.IsNotEmpty(packetAnalyzer.LastError, "Truncated additional car telemetry packet must report an error!");
    }

    #endregion // Methods
}