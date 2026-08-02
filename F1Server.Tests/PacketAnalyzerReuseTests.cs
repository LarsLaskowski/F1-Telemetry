using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Tests of the transformation instances the packet analyzer reuses for every received packet
/// </summary>
[TestClass]
public class PacketAnalyzerReuseTests
{
    #region Constants

    /// <summary>
    /// Length of a truncated test packet, longer than every packet header but shorter than every expected packet size
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
    /// Compares the lap data of a reused transformation with the lap data of a transformation used only once
    /// </summary>
    /// <param name="expectedLapData">Lap data of a packet analyzer that converted only this packet</param>
    /// <param name="reusedLapData">Lap data of the packet analyzer that converted several packets</param>
    /// <param name="packetName">Name of the converted packet for the assert messages</param>
    private static void AssertSameLapData(ILapDataComplete? expectedLapData,
                                          ILapDataComplete? reusedLapData,
                                          string packetName)
    {
        Assert.IsNotNull(expectedLapData, $"Lap data of {packetName} could not be converted!");
        Assert.IsNotNull(reusedLapData, $"Lap data of {packetName} could not be converted by the reused transformation!");

        Assert.HasCount(expectedLapData.LapData.Length,
                        reusedLapData.LapData,
                        $"Reused transformation returned another number of cars for {packetName}!");

        for (var carIndex = 0; carIndex < expectedLapData.LapData.Length; carIndex++)
        {
            var expectedCar = expectedLapData.LapData[carIndex];
            var reusedCar = reusedLapData.LapData[carIndex];

            Assert.AreEqual(expectedCar.CarPosition, reusedCar.CarPosition, $"Car position of car {carIndex} in {packetName} differs!");
            Assert.AreEqual(expectedCar.CurrentLapNumber, reusedCar.CurrentLapNumber, $"Current lap of car {carIndex} in {packetName} differs!");
            Assert.AreEqual(expectedCar.LapDistance, reusedCar.LapDistance, $"Lap distance of car {carIndex} in {packetName} differs!");
            Assert.AreEqual(expectedCar.TotalDistance, reusedCar.TotalDistance, $"Total distance of car {carIndex} in {packetName} differs!");
        }
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Test to verify that the reused car telemetry transformation creates the data object of the game version
    /// of the packet that is converted at the moment and not of the previously converted packet
    /// </summary>
    [TestMethod]
    public void PacketAnalyzerGetCarTelemetryDifferentGameVersionsReturnsVersionSpecificObjects()
    {
        var header2021 = GetPacketHeader("F1-2021-CarTelemetry.packet", out var content2021);
        var header2025 = GetPacketHeader("F1-2025-CarTelemetry.packet", out var content2025);

        var packetAnalyzer = new PacketAnalyzer();

        var telemetry2021 = packetAnalyzer.GetCarTelemetry(header2021, content2021) as CarTelemetry;
        var telemetry2025 = packetAnalyzer.GetCarTelemetry(header2025, content2025) as CarTelemetry;

        Assert.IsNotNull(telemetry2021, "F1 2021 car telemetry packet was not converted!");
        Assert.IsNotNull(telemetry2025, "F1 2025 car telemetry packet was not converted!");

        Assert.IsInstanceOfType<CarTelemetry2021>(telemetry2021.PacketData, "Reused transformation did not create a F1 2021 car telemetry object!");
        Assert.IsInstanceOfType<CarTelemetry2025>(telemetry2025.PacketData, "Reused transformation did not create a F1 2025 car telemetry object!");

        Assert.AreSame(header2021, telemetry2021.PacketHeader, "F1 2021 car telemetry object does not carry the header of its own packet!");
        Assert.AreSame(header2025, telemetry2025.PacketHeader, "F1 2025 car telemetry object does not carry the header of its own packet!");
    }

    /// <summary>
    /// Test to verify that a reused lap data transformation converts two packets of different game versions
    /// to the same values as two transformations that are used only once
    /// </summary>
    [TestMethod]
    public void PacketAnalyzerGetLapDataDifferentGameVersionsReturnsSameValuesAsSingleUseTransformation()
    {
        var header2022 = GetPacketHeader("F1-2022-LapData.packet", out var content2022);
        var header2026 = GetPacketHeader("F1-2026-LapData.packet", out var content2026);

        var expected2022 = new PacketAnalyzer().GetLapData(header2022, content2022) as LapData;
        var expected2026 = new PacketAnalyzer().GetLapData(header2026, content2026) as LapData;

        var packetAnalyzer = new PacketAnalyzer();

        var reused2022 = packetAnalyzer.GetLapData(header2022, content2022) as LapData;
        var reused2026 = packetAnalyzer.GetLapData(header2026, content2026) as LapData;

        Assert.IsNotNull(expected2022, "F1 2022 lap data packet was not converted!");
        Assert.IsNotNull(expected2026, "F1 2026 lap data packet was not converted!");
        Assert.IsNotNull(reused2022, "F1 2022 lap data packet was not converted by the reused transformation!");
        Assert.IsNotNull(reused2026, "F1 2026 lap data packet was not converted by the reused transformation!");

        Assert.IsInstanceOfType<LapDataComplete2022>(reused2022.PacketData, "Reused transformation did not create a F1 2022 lap data object!");
        Assert.IsInstanceOfType<LapDataComplete2026>(reused2026.PacketData, "Reused transformation did not create a F1 2026 lap data object!");

        AssertSameLapData(expected2022.PacketData, reused2022.PacketData, "F1 2022 lap data");
        AssertSameLapData(expected2026.PacketData, reused2026.PacketData, "F1 2026 lap data");
    }

    /// <summary>
    /// Test to verify that a reused session transformation converts two packets of different game versions
    /// to the same values as two transformations that are used only once
    /// </summary>
    [TestMethod]
    public void PacketAnalyzerGetSessionDataDifferentGameVersionsReturnsSameValuesAsSingleUseTransformation()
    {
        var header2023 = GetPacketHeader("F1-2023-Session.packet", out var content2023);
        var header2025 = GetPacketHeader("F1-2025-Session.packet", out var content2025);

        var expected2023 = new PacketAnalyzer().GetSessionData(header2023, content2023) as SessionData;
        var expected2025 = new PacketAnalyzer().GetSessionData(header2025, content2025) as SessionData;

        var packetAnalyzer = new PacketAnalyzer();

        var reused2023 = packetAnalyzer.GetSessionData(header2023, content2023) as SessionData;
        var reused2025 = packetAnalyzer.GetSessionData(header2025, content2025) as SessionData;

        Assert.IsNotNull(expected2023, "F1 2023 session packet was not converted!");
        Assert.IsNotNull(expected2025, "F1 2025 session packet was not converted!");
        Assert.IsNotNull(reused2023, "F1 2023 session packet was not converted by the reused transformation!");
        Assert.IsNotNull(reused2025, "F1 2025 session packet was not converted by the reused transformation!");

        Assert.IsNotNull(expected2023.PacketData, "F1 2023 session packet contains no session data!");
        Assert.IsNotNull(expected2025.PacketData, "F1 2025 session packet contains no session data!");
        Assert.IsNotNull(reused2023.PacketData, "F1 2023 session packet of the reused transformation contains no session data!");
        Assert.IsNotNull(reused2025.PacketData, "F1 2025 session packet of the reused transformation contains no session data!");

        Assert.IsInstanceOfType<SessionData2023>(reused2023.PacketData, "Reused transformation did not create a F1 2023 session object!");
        Assert.IsInstanceOfType<SessionData2025>(reused2025.PacketData, "Reused transformation did not create a F1 2025 session object!");

        Assert.AreEqual(expected2023.PacketData.TrackName, reused2023.PacketData.TrackName, "Track name of the F1 2023 session packet differs!");
        Assert.AreEqual(expected2023.PacketData.SessionType, reused2023.PacketData.SessionType, "Session type of the F1 2023 session packet differs!");
        Assert.AreEqual(expected2023.PacketData.TotalLaps, reused2023.PacketData.TotalLaps, "Total laps of the F1 2023 session packet differs!");

        Assert.AreEqual(expected2025.PacketData.TrackName, reused2025.PacketData.TrackName, "Track name of the F1 2025 session packet differs!");
        Assert.AreEqual(expected2025.PacketData.SessionType, reused2025.PacketData.SessionType, "Session type of the F1 2025 session packet differs!");
        Assert.AreEqual(expected2025.PacketData.TotalLaps, reused2025.PacketData.TotalLaps, "Total laps of the F1 2025 session packet differs!");
    }

    /// <summary>
    /// Test to verify that the error of a rejected packet does not leak into the next packet
    /// converted by the same transformation instance
    /// </summary>
    [TestMethod]
    public void PacketAnalyzerGetCarStatusAfterRejectedPacketClearsLastError()
    {
        var packetHeader = GetPacketHeader("F1-2025-CarStatus.packet", out var packetContent);

        var packetAnalyzer = new PacketAnalyzer();

        var rejectedCarStatus = packetAnalyzer.GetCarStatus(packetHeader, packetContent.AsSpan(0, TruncatedPacketLength));

        Assert.IsNull(rejectedCarStatus, "Truncated car status packet must not produce an object!");
        Assert.AreNotEqual(string.Empty, packetAnalyzer.LastError, "Truncated car status packet must report an error!");

        var carStatus = packetAnalyzer.GetCarStatus(packetHeader, packetContent);

        Assert.IsNotNull(carStatus, "Full size car status packet must produce an object!");
        Assert.AreEqual(string.Empty, packetAnalyzer.LastError, "Error of the rejected packet must not leak into the next transformation!");
    }

    #endregion // Methods
}