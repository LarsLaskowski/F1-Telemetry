using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test car telemetry packet files (F1 2026)
/// </summary>
[TestClass]
public class PacketCarTelemetry2026Tests
{
    #region Fields

    private static PacketAnalyzer _packetAnalyzer;
    private static ReceivedPacketData _packetData;
    private static byte[] _packetContent;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketCarTelemetryInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2026-CarTelemetry.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2026-CarTelemetry.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car telemetry packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-CarTelemetry.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file has a correct car telemetry data content
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarTelemetry;

        Assert.IsTrue(isCorrect, "Packet is not a car telemetry data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2026IsCarTelemetryDataObject()
    {
        var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader!, _packetContent);

        var isCorrect = carTelemetry is CarTelemetry packetData && packetData.PacketData is CarTelemetry2026;

        Assert.IsTrue(isCorrect, "Packet is not a car telemetry data packet");
    }

    /// <summary>
    /// Check tyre pressure FL (2026)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryTyrePressure2026ExpectedValue()
    {
        var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader!, _packetContent);

        if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2026)
        {
            var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].TyresPressure.FrontLeft == 24.2F;

            Assert.IsTrue(isCorrect, "Incorrect tyre pressure (FL)!");
        }
        else
        {
            Assert.Fail("Invalid car telemetry format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check speed (2026)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetrySpeed2026ExpectedValue()
    {
        var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader!, _packetContent);

        if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2026)
        {
            var isCorrect = telemetryData.PacketData?.CarTelemetryData[17].Speed == 248;

            Assert.IsTrue(isCorrect, "Incorrect speed!");
        }
        else
        {
            Assert.Fail("Invalid car telemetry format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check engine temperature (2026) - now read from a single byte
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryEngineTemperature2026ExpectedValue()
    {
        var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader!, _packetContent);

        if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2026)
        {
            var isCorrect = telemetryData.PacketData?.CarTelemetryData[19].EngineTemperature == 109;

            Assert.IsTrue(isCorrect, "Incorrect engine temperature!");
        }
        else
        {
            Assert.Fail("Invalid car telemetry format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check wheel surface type FR (2026)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryWheelSurface2026ExpectedValue()
    {
        var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader!, _packetContent);

        if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2026)
        {
            var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].SurfaceType.FrontRight == SurfaceType.Tarmac;

            Assert.IsTrue(isCorrect, "Incorrect wheel surface type (FR)!");
        }
        else
        {
            Assert.Fail("Invalid car telemetry format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check engine RPM (2026)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryEngineRpm2026ExpectedValue()
    {
        var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader!, _packetContent);

        if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2026)
        {
            var isCorrect = telemetryData.PacketData?.CarTelemetryData[9].EngineRPM == 10514;

            Assert.IsTrue(isCorrect, "Incorrect engine rpm!");
        }
        else
        {
            Assert.Fail("Invalid car telemetry format, expected F1 2026!");
        }
    }

    #endregion // Methods F1 2026
}