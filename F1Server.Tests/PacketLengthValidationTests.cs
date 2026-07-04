using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.PacketData;
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

    /// <summary>
    /// Creates a synthetic packet header for game versions without sample packet files
    /// </summary>
    /// <param name="gameVersion">Game version to encode in the header</param>
    /// <param name="headerSize">Header size of the game version</param>
    /// <returns>Parsed packet header</returns>
    private static PacketHeader CreatePacketHeader(int gameVersion, int headerSize)
    {
        var rawData = new byte[headerSize];

        rawData[0] = (byte)(gameVersion & 0xFF);
        rawData[1] = (byte)((gameVersion >> 8) & 0xFF);

        var receivedData = new ReceivedPacketData();

        receivedData.SetRawData(rawData);

        Assert.IsNotNull(receivedData.PacketHeader, $"Synthetic header for game version {gameVersion} could not be created!");

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

        var carStatus = packetAnalyzer.GetCarStatus(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

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

        var carTelemetry = packetAnalyzer.GetCarTelemetry(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

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

        var eventData = packetAnalyzer.GetEventData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

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

        var participants = packetAnalyzer.GetParticipantsData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

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

        var sessionData = packetAnalyzer.GetSessionData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

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

        var sessionHistory = packetAnalyzer.GetSessionHistoryData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

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

        var finalClassification = packetAnalyzer.GetFinalClassificationData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

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

        var timeTrial = packetAnalyzer.GetTimeTrialData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

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

    /// <summary>
    /// Test to verify that lap data conversion tolerates truncated packets via the per car checks
    /// and skips the trailing time trial car indexes without reading past the packet end
    /// </summary>
    [TestMethod]
    public void GetLapDataTruncatedPacketReturnsObjectWithDefaults()
    {
        var packetHeader = GetPacketHeader("F1-2025-LapData.packet", out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var lapData = packetAnalyzer.GetLapData(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

        Assert.IsNotNull(lapData, "Truncated lap data packet must still produce an object with default entries!");
    }

    /// <summary>
    /// Test to verify that a truncated F1 2019 packet is rejected by every converter with 2019 support
    /// </summary>
    [TestMethod]
    public void Get2019DataTruncatedPacketReturnsNull()
    {
        var packetHeader = CreatePacketHeader(2019, ConstData.F12019HeaderSize);

        var packetAnalyzer = new PacketAnalyzer();

        var truncatedContent = new byte[TruncatedPacketLength];

        Assert.IsNull(packetAnalyzer.GetCarStatus(packetHeader, truncatedContent), "Truncated F1 2019 car status packet must not produce an object!");
        Assert.IsNull(packetAnalyzer.GetCarTelemetry(packetHeader, truncatedContent), "Truncated F1 2019 car telemetry packet must not produce an object!");
        Assert.IsNull(packetAnalyzer.GetParticipantsData(packetHeader, truncatedContent), "Truncated F1 2019 participants packet must not produce an object!");
        Assert.IsNull(packetAnalyzer.GetSessionData(packetHeader, truncatedContent), "Truncated F1 2019 session packet must not produce an object!");
        Assert.IsNull(packetAnalyzer.GetEventData(packetHeader, new byte[ConstData.F12019HeaderSize]), "Truncated F1 2019 event packet must not produce an object!");
    }

    /// <summary>
    /// Test to verify that full size F1 2019 packets are still converted successfully
    /// </summary>
    [TestMethod]
    public void Get2019DataFullPacketReturnsObject()
    {
        var packetHeader = CreatePacketHeader(2019, ConstData.F12019HeaderSize);

        var packetAnalyzer = new PacketAnalyzer();

        var carStatusContent = new byte[ConstData.F12019HeaderSize + ConstData.F12019CarStatusSize];
        var participantsContent = new byte[ConstData.F12019HeaderSize + ConstData.F12019ParticipantsSize];
        var sessionContent = new byte[ConstData.F12019HeaderSize + ConstData.F12019SessionSize];
        var eventContent = new byte[ConstData.F12019HeaderSize + ConstData.F12019EventSize];

        Assert.IsNotNull(packetAnalyzer.GetCarStatus(packetHeader, carStatusContent), "Full size F1 2019 car status packet must produce an object!");
        Assert.IsNotNull(packetAnalyzer.GetParticipantsData(packetHeader, participantsContent), "Full size F1 2019 participants packet must produce an object!");
        Assert.IsNotNull(packetAnalyzer.GetSessionData(packetHeader, sessionContent), "Full size F1 2019 session packet must produce an object!");
        Assert.IsNotNull(packetAnalyzer.GetEventData(packetHeader, eventContent), "Full size F1 2019 event packet must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated lap positions packet is rejected instead of reading past the packet end
    /// </summary>
    /// <param name="gameVersion">Game version to encode in the header</param>
    /// <param name="headerSize">Header size of the game version</param>
    [TestMethod]
    [DataRow(2025, ConstData.F12025HeaderSize)]
    [DataRow(2026, ConstData.F12026HeaderSize)]
    public void GetLapPositionsDataTruncatedPacketReturnsNull(int gameVersion, int headerSize)
    {
        var packetHeader = CreatePacketHeader(gameVersion, headerSize);

        var packetAnalyzer = new PacketAnalyzer();

        var lapPositions = packetAnalyzer.GetLapPositionsData(packetHeader, new byte[TruncatedPacketLength]);

        Assert.IsNull(lapPositions, $"Truncated F1 {gameVersion} lap positions packet must not produce an object!");
    }

    /// <summary>
    /// Test to verify that full size lap positions packets are converted successfully
    /// </summary>
    /// <param name="gameVersion">Game version to encode in the header</param>
    /// <param name="headerSize">Header size of the game version</param>
    /// <param name="lapPositionSize">Expected lap positions payload size of the game version</param>
    [TestMethod]
    [DataRow(2025, ConstData.F12025HeaderSize, ConstData.F12025LapPositionSize)]
    [DataRow(2026, ConstData.F12026HeaderSize, ConstData.F12026LapPositionSize)]
    public void GetLapPositionsDataFullPacketReturnsObject(int gameVersion, int headerSize, int lapPositionSize)
    {
        var packetHeader = CreatePacketHeader(gameVersion, headerSize);

        var packetAnalyzer = new PacketAnalyzer();

        var packetContent = new byte[headerSize + lapPositionSize];

        var lapPositions = packetAnalyzer.GetLapPositionsData(packetHeader, packetContent);

        Assert.IsNotNull(lapPositions, $"Full size F1 {gameVersion} lap positions packet must produce an object!");
    }

    /// <summary>
    /// Test to verify that a manipulated lap count in a lap positions packet is clamped to the fixed packet layout
    /// </summary>
    [TestMethod]
    public void GetLapPositionsDataManipulatedLapCountReturnsObject()
    {
        var packetHeader = CreatePacketHeader(2025, ConstData.F12025HeaderSize);

        var packetContent = new byte[ConstData.F12025HeaderSize + ConstData.F12025LapPositionSize];

        // Lap count far above the fixed 50 lap layout of the packet
        packetContent[ConstData.F12025HeaderSize] = byte.MaxValue;

        var packetAnalyzer = new PacketAnalyzer();

        var lapPositions = packetAnalyzer.GetLapPositionsData(packetHeader, packetContent);

        Assert.IsNotNull(lapPositions, "Lap positions packet with a manipulated lap count must be clamped and still produce an object!");
    }

    /// <summary>
    /// Test to verify that car positions are read from the byte right after LapStartIndex instead of overlapping it
    /// </summary>
    /// <param name="gameVersion">Game version to encode in the header</param>
    /// <param name="headerSize">Header size of the game version</param>
    /// <param name="lapPositionSize">Expected lap positions payload size of the game version</param>
    [TestMethod]
    [DataRow(2025, ConstData.F12025HeaderSize, ConstData.F12025LapPositionSize)]
    [DataRow(2026, ConstData.F12026HeaderSize, ConstData.F12026LapPositionSize)]
    public void GetLapPositionsDataFirstCarPositionIsNotShiftedByLapStartIndex(int gameVersion, int headerSize, int lapPositionSize)
    {
        var packetHeader = CreatePacketHeader(gameVersion, headerSize);

        var packetContent = new byte[headerSize + lapPositionSize];

        packetContent[headerSize] = 1;
        packetContent[headerSize + 1] = 99;
        packetContent[headerSize + 2] = 5;

        var packetAnalyzer = new PacketAnalyzer();

        var lapPositions = packetAnalyzer.GetLapPositionsData(packetHeader, packetContent);

        var carPosition = lapPositions switch
                          {
                              LapPositions { PacketData: LapPositions2025 lapPositions2025 } => lapPositions2025.CarPositionOnLaps[0, 0],
                              LapPositions { PacketData: LapPositions2026 lapPositions2026 } => lapPositions2026.CarPositionOnLaps[0, 0],
                              _ => -1
                          };

        Assert.AreEqual(5, carPosition, $"First car position of F1 {gameVersion} lap positions packet must not be shifted by the LapStartIndex byte!");
    }

    /// <summary>
    /// Test to verify that a truncated additional car telemetry packet is rejected instead of reading past the packet end
    /// </summary>
    [TestMethod]
    public void GetCarTelemetry2TruncatedPacketReturnsNull()
    {
        var packetHeader = CreatePacketHeader(2026, ConstData.F12026HeaderSize);

        var packetAnalyzer = new PacketAnalyzer();

        var carTelemetry2 = packetAnalyzer.GetCarTelemetry2(packetHeader, new byte[TruncatedPacketLength]);

        Assert.IsNull(carTelemetry2, "Truncated additional car telemetry packet must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size additional car telemetry packet is still converted successfully
    /// </summary>
    [TestMethod]
    public void GetCarTelemetry2FullPacketReturnsObject()
    {
        var packetHeader = CreatePacketHeader(2026, ConstData.F12026HeaderSize);

        var packetAnalyzer = new PacketAnalyzer();

        var packetContent = new byte[ConstData.F12026HeaderSize + ConstData.F12026CarTelemetry2Size];

        var carTelemetry2 = packetAnalyzer.GetCarTelemetry2(packetHeader, packetContent);

        Assert.IsNotNull(carTelemetry2, "Full size additional car telemetry packet must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated time trial packet is rejected for the game versions without sample packet files
    /// </summary>
    /// <param name="gameVersion">Game version to encode in the header</param>
    /// <param name="headerSize">Header size of the game version</param>
    [TestMethod]
    [DataRow(2025, ConstData.F12025HeaderSize)]
    [DataRow(2026, ConstData.F12026HeaderSize)]
    public void GetTimeTrialDataTruncatedSyntheticPacketReturnsNull(int gameVersion, int headerSize)
    {
        var packetHeader = CreatePacketHeader(gameVersion, headerSize);

        var packetAnalyzer = new PacketAnalyzer();

        var timeTrial = packetAnalyzer.GetTimeTrialData(packetHeader, new byte[TruncatedPacketLength]);

        Assert.IsNull(timeTrial, $"Truncated F1 {gameVersion} time trial packet must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size time trial packet is still converted for the game versions without sample packet files
    /// </summary>
    /// <param name="gameVersion">Game version to encode in the header</param>
    /// <param name="headerSize">Header size of the game version</param>
    /// <param name="payloadSize">Time trial payload size of the game version</param>
    [TestMethod]
    [DataRow(2025, ConstData.F12025HeaderSize, ConstData.F12025TimeTrialSize)]
    [DataRow(2026, ConstData.F12026HeaderSize, ConstData.F12026TimeTrialSize)]
    public void GetTimeTrialDataFullSyntheticPacketReturnsObject(int gameVersion, int headerSize, int payloadSize)
    {
        var packetHeader = CreatePacketHeader(gameVersion, headerSize);

        var packetAnalyzer = new PacketAnalyzer();

        var timeTrial = packetAnalyzer.GetTimeTrialData(packetHeader, new byte[headerSize + payloadSize]);

        Assert.IsNotNull(timeTrial, $"Full size F1 {gameVersion} time trial packet must produce an object!");
    }

    /// <summary>
    /// Test to verify that a truncated F1 2020 final classification packet is rejected instead of reading past the packet end
    /// </summary>
    [TestMethod]
    public void GetFinalClassificationDataTruncated2020PacketReturnsNull()
    {
        var packetHeader = CreatePacketHeader(2020, ConstData.F12020HeaderSize);

        var packetAnalyzer = new PacketAnalyzer();

        var finalClassification = packetAnalyzer.GetFinalClassificationData(packetHeader, new byte[TruncatedPacketLength]);

        Assert.IsNull(finalClassification, "Truncated F1 2020 final classification packet must not produce an object!");
    }

    /// <summary>
    /// Test to verify that a full size F1 2020 final classification packet is still converted successfully
    /// </summary>
    [TestMethod]
    public void GetFinalClassificationDataFull2020PacketReturnsObject()
    {
        var packetHeader = CreatePacketHeader(2020, ConstData.F12020HeaderSize);

        var packetAnalyzer = new PacketAnalyzer();

        var packetContent = new byte[ConstData.F12020HeaderSize + ConstData.F12020FinalClassificationSize];

        var finalClassification = packetAnalyzer.GetFinalClassificationData(packetHeader, packetContent);

        Assert.IsNotNull(finalClassification, "Full size F1 2020 final classification packet must produce an object!");
    }

    #endregion // Methods
}