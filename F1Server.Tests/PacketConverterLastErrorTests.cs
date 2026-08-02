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
        Assert.IsTrue(string.IsNullOrWhiteSpace(packetAnalyzer.LastError), $"Full size car telemetry packet {fileName} must not report an error!");
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
        Assert.IsTrue(string.IsNullOrWhiteSpace(packetAnalyzer.LastError), $"Full size session packet {fileName} must not report an error!");
    }

    #endregion // Methods
}