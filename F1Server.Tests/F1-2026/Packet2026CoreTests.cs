using F1Server.Core.Data;
using F1Server.Core.Enumerations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Tests for the F1 2026 foundations that do not require a binary sample packet
/// </summary>
[TestClass]
public class Packet2026CoreTests
{
    #region Methods F1 2026

    /// <summary>
    /// The new CarTelemetry2 packet type must have the enum value 17
    /// </summary>
    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MSTEST0032:Assertion condition is always true", Justification = "Valid test. If someone changes the enum value, the test will fail")]
    public void PacketTypesCarTelemetry2HasValue17()
    {
        Assert.AreEqual(17, (int)PacketTypes.CarTelemetry2, "CarTelemetry2 must have enum value 17!");
    }

    /// <summary>
    /// The raw packet id 16 must resolve to the CarTelemetry2 packet type (offset by one)
    /// </summary>
    [TestMethod]
    public void PacketHeaderRawPacketId16ResolvesToCarTelemetry2()
    {
        var content = new byte[ConstData.F12026HeaderSize];

        // Packet format 2026 (uint16 little endian) -> 0x07EA
        content[0] = 0xEA;
        content[1] = 0x07;

        // Game year
        content[2] = 26;

        // Raw packet id 16 (CarTelemetry2)
        content[6] = 16;

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(content);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual(PacketTypes.CarTelemetry2, packetData.PacketHeader.PacketType, "Raw packet id 16 must map to CarTelemetry2!");
    }

    /// <summary>
    /// The documented 2026 packet sizes must match the constants
    /// </summary>
    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MSTEST0032:Assertion condition is always true", Justification = "Valid test. If someone changes the enum value, the test will fail")]
    public void ConstData2026SizesAreCorrect()
    {
        Assert.AreEqual(29, ConstData.F12026HeaderSize, "Wrong header size!");
        Assert.AreEqual(24, ConstData.F12026MaxCars, "Wrong maximum number of cars!");
        Assert.AreEqual(897, ConstData.F12026SessionSize, "Wrong session body size!");
        Assert.AreEqual(1368, ConstData.F12026TotalLapSize, "Wrong total lap size!");
        Assert.AreEqual(240, ConstData.F12026CarTelemetry2Size, "Wrong car telemetry 2 size!");
    }

    #endregion // Methods F1 2026
}