using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Packets.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Tests of the packet length validation in the packet to object converters
/// </summary>
[TestClass]
public class PacketLengthValidationTests
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
    /// Test to verify that a truncated car status packet is rejected instead of reading past the packet end
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-CarStatus.packet")]
    [DataRow("F1-2021-CarStatus.packet")]
    [DataRow("F1-2022-CarStatus.packet")]
    [DataRow("F1-2023-CarStatus.packet")]
    [DataRow("F1-2024-CarStatus.packet")]
    [DataRow("F1-2025-CarStatus.packet")]
    [DataRow("F1-2026-CarStatus.packet")]
    public void GetCarStatusTruncatedPacketReturnsNull(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var carStatus = packetAnalyzer.GetCarStatus(packetHeader, packetContent[..TruncatedPacketLength]);

        Assert.IsNull(carStatus, $"Truncated car status packet {fileName} must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size car status packet is still converted successfully
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-CarStatus.packet")]
    [DataRow("F1-2021-CarStatus.packet")]
    [DataRow("F1-2022-CarStatus.packet")]
    [DataRow("F1-2023-CarStatus.packet")]
    [DataRow("F1-2024-CarStatus.packet")]
    [DataRow("F1-2025-CarStatus.packet")]
    [DataRow("F1-2026-CarStatus.packet")]
    public void GetCarStatusFullPacketReturnsObject(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var carStatus = packetAnalyzer.GetCarStatus(packetHeader, packetContent);

        Assert.IsNotNull(carStatus, $"Full size car status packet {fileName} must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated car telemetry packet is rejected instead of reading past the packet end
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
    public void GetCarTelemetryTruncatedPacketReturnsNull(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var carTelemetry = packetAnalyzer.GetCarTelemetry(packetHeader, packetContent[..TruncatedPacketLength]);

        Assert.IsNull(carTelemetry, $"Truncated car telemetry packet {fileName} must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size car telemetry packet is still converted successfully
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
    public void GetCarTelemetryFullPacketReturnsObject(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var carTelemetry = packetAnalyzer.GetCarTelemetry(packetHeader, packetContent);

        Assert.IsNotNull(carTelemetry, $"Full size car telemetry packet {fileName} must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated event packet is rejected instead of reading past the packet end
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-Event-TopSpeed.packet")]
    [DataRow("F1-2021-Event-TopSpeed.packet")]
    [DataRow("F1-2022-Event-TopSpeed.packet")]
    [DataRow("F1-2023-Event-TopSpeed.packet")]
    [DataRow("F1-2024-Event-TopSpeed.packet")]
    [DataRow("F1-2025-Event-TopSpeed.packet")]
    [DataRow("F1-2026-MelbourneRace-Event-SessionStart.packet")]
    public void GetEventDataTruncatedPacketReturnsNull(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var eventData = packetAnalyzer.GetEventData(packetHeader, packetContent[..TruncatedPacketLength]);

        Assert.IsNull(eventData, $"Truncated event packet {fileName} must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size event packet is still converted successfully
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-Event-TopSpeed.packet")]
    [DataRow("F1-2021-Event-TopSpeed.packet")]
    [DataRow("F1-2022-Event-TopSpeed.packet")]
    [DataRow("F1-2023-Event-TopSpeed.packet")]
    [DataRow("F1-2024-Event-TopSpeed.packet")]
    [DataRow("F1-2025-Event-TopSpeed.packet")]
    [DataRow("F1-2026-MelbourneRace-Event-SessionStart.packet")]
    public void GetEventDataFullPacketReturnsObject(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var eventData = packetAnalyzer.GetEventData(packetHeader, packetContent);

        Assert.IsNotNull(eventData, $"Full size event packet {fileName} must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated participants packet is rejected instead of reading past the packet end
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-Participants.packet")]
    [DataRow("F1-2021-Participants.packet")]
    [DataRow("F1-2022-Participants.packet")]
    [DataRow("F1-2023-Participants.packet")]
    [DataRow("F1-2024-Participants.packet")]
    [DataRow("F1-2025-Participants.packet")]
    [DataRow("F1-2026-Participants.packet")]
    public void GetParticipantsDataTruncatedPacketReturnsNull(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var participants = packetAnalyzer.GetParticipantsData(packetHeader, packetContent[..TruncatedPacketLength]);

        Assert.IsNull(participants, $"Truncated participants packet {fileName} must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size participants packet is still converted successfully
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-Participants.packet")]
    [DataRow("F1-2021-Participants.packet")]
    [DataRow("F1-2022-Participants.packet")]
    [DataRow("F1-2023-Participants.packet")]
    [DataRow("F1-2024-Participants.packet")]
    [DataRow("F1-2025-Participants.packet")]
    [DataRow("F1-2026-Participants.packet")]
    public void GetParticipantsDataFullPacketReturnsObject(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var participants = packetAnalyzer.GetParticipantsData(packetHeader, packetContent);

        Assert.IsNotNull(participants, $"Full size participants packet {fileName} must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated session packet is rejected instead of reading past the packet end
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
    public void GetSessionDataTruncatedPacketReturnsNull(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var sessionData = packetAnalyzer.GetSessionData(packetHeader, packetContent[..TruncatedPacketLength]);

        Assert.IsNull(sessionData, $"Truncated session packet {fileName} must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size session packet is still converted successfully
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
    public void GetSessionDataFullPacketReturnsObject(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var sessionData = packetAnalyzer.GetSessionData(packetHeader, packetContent);

        Assert.IsNotNull(sessionData, $"Full size session packet {fileName} must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated session history packet is rejected instead of reading past the packet end
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2021-SessionHistory.packet")]
    [DataRow("F1-2022-SessionHistory.packet")]
    [DataRow("F1-2023-SessionHistory.packet")]
    [DataRow("F1-2024-SessionHistory.packet")]
    [DataRow("F1-2025-SessionHistory.packet")]
    [DataRow("F1-2026-SessionHistory.packet")]
    public void GetSessionHistoryDataTruncatedPacketReturnsNull(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var sessionHistory = packetAnalyzer.GetSessionHistoryData(packetHeader, packetContent[..TruncatedPacketLength]);

        Assert.IsNull(sessionHistory, $"Truncated session history packet {fileName} must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size session history packet is still converted successfully
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2021-SessionHistory.packet")]
    [DataRow("F1-2022-SessionHistory.packet")]
    [DataRow("F1-2023-SessionHistory.packet")]
    [DataRow("F1-2024-SessionHistory.packet")]
    [DataRow("F1-2025-SessionHistory.packet")]
    [DataRow("F1-2026-SessionHistory.packet")]
    public void GetSessionHistoryDataFullPacketReturnsObject(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var sessionHistory = packetAnalyzer.GetSessionHistoryData(packetHeader, packetContent);

        Assert.IsNotNull(sessionHistory, $"Full size session history packet {fileName} must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated final classification packet is rejected instead of reading past the packet end
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2021-FinalClassification.packet")]
    [DataRow("F1-2022-FinalClassification.packet")]
    [DataRow("F1-2023-FinalClassification.packet")]
    [DataRow("F1-2024-FinalClassification.packet")]
    [DataRow("F1-2025-FinalClassification.packet")]
    [DataRow("F1-2026-FinalClassification.packet")]
    public void GetFinalClassificationDataTruncatedPacketReturnsNull(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var finalClassification = packetAnalyzer.GetFinalClassificationData(packetHeader, packetContent[..TruncatedPacketLength]);

        Assert.IsNull(finalClassification, $"Truncated final classification packet {fileName} must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size final classification packet is still converted successfully
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2021-FinalClassification.packet")]
    [DataRow("F1-2022-FinalClassification.packet")]
    [DataRow("F1-2023-FinalClassification.packet")]
    [DataRow("F1-2024-FinalClassification.packet")]
    [DataRow("F1-2025-FinalClassification.packet")]
    [DataRow("F1-2026-FinalClassification.packet")]
    public void GetFinalClassificationDataFullPacketReturnsObject(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var finalClassification = packetAnalyzer.GetFinalClassificationData(packetHeader, packetContent);

        Assert.IsNotNull(finalClassification, $"Full size final classification packet {fileName} must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated time trial packet is rejected instead of reading past the packet end
    /// </summary>
    [TestMethod]
    public void GetTimeTrialDataTruncatedPacketReturnsNull()
    {
        var packetHeader = GetPacketHeader("F1-2024-TimeTrial.packet", out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var timeTrial = packetAnalyzer.GetTimeTrialData(packetHeader, packetContent[..TruncatedPacketLength]);

        Assert.IsNull(timeTrial, "Truncated time trial packet must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size time trial packet is still converted successfully
    /// </summary>
    [TestMethod]
    public void GetTimeTrialDataFullPacketReturnsObject()
    {
        var packetHeader = GetPacketHeader("F1-2024-TimeTrial.packet", out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var timeTrial = packetAnalyzer.GetTimeTrialData(packetHeader, packetContent);

        Assert.IsNotNull(timeTrial, "Full size time trial packet must produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size lap data packet including the trailing time trial car indexes is still converted successfully
    /// </summary>
    /// <param name="fileName">Name of the sample packet file</param>
    [TestMethod]
    [DataRow("F1-2020-LapData.packet")]
    [DataRow("F1-2021-LapData.packet")]
    [DataRow("F1-2022-LapData.packet")]
    [DataRow("F1-2023-LapData.packet")]
    [DataRow("F1-2024-LapData.packet")]
    [DataRow("F1-2025-LapData.packet")]
    [DataRow("F1-2026-LapData.packet")]
    public void GetLapDataFullPacketReturnsObject(string fileName)
    {
        var packetHeader = GetPacketHeader(fileName, out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var lapData = packetAnalyzer.GetLapData(packetHeader, packetContent);

        Assert.IsNotNull(lapData, $"Full size lap data packet {fileName} must produce an object!");
    }

    #endregion // Methods
}