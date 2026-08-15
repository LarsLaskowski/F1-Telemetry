using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test car telemetry packet files
/// </summary>
[TestClass]
public class PacketCarTelemetry2025Tests
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
        var isFile = File.Exists(@"SampleData/F1-2025-CarTelemetry.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2025-CarTelemetry.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car telemetry packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2025-CarTelemetry.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2025

    /// <summary>
    /// Check whether the given file has a correct car telemetry data content
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarTelemetry;

        Assert.IsTrue(isCorrect, "Packet is not a car telemetry data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2025 packet
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2025IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2025;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2025 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2025IsCarTelemetryDataObject()
    {
        var data = GetCarTelemetryData();

        Assert.IsNotNull(data, "Packet is not a car telemetry data packet");
    }

    /// <summary>
    /// Check tyre pressure FL (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryTyrePressure2025ExpectedValue()
    {
        var data = GetCarTelemetryData();

        Assert.AreEqual(24.2F, data.CarTelemetryData[0].TyresPressure.FrontLeft, 0.0001F, "Incorrect tyre pressure (FL)!");
    }

    /// <summary>
    /// Check speed (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetrySpeed2025ExpectedValue()
    {
        var data = GetCarTelemetryData();

        Assert.AreEqual((ushort)248, data.CarTelemetryData[17].Speed, "Incorrect speed!");
    }

    /// <summary>
    /// Check tyre inner temperature RR (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryTyreInnerTemperature2025ExpectedValue()
    {
        var data = GetCarTelemetryData();

        Assert.AreEqual((ushort)69, data.CarTelemetryData[0].TyresInnerTemperature.RearRight, "Incorrect tyre inner temperature (RR)!");
    }

    /// <summary>
    /// Check wheel surface type FR (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryWheelSurface2025ExpectedValue()
    {
        var data = GetCarTelemetryData();

        Assert.AreEqual(SurfaceType.Tarmac, data.CarTelemetryData[0].SurfaceType.FrontRight, "Incorrect wheel surface type (FR)!");
    }

    /// <summary>
    /// Check engine RPM (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryEngineRpm2025ExpectedValue()
    {
        var data = GetCarTelemetryData();

        Assert.AreEqual((ushort)10514, data.CarTelemetryData[9].EngineRPM, "Incorrect engine rpm!");
    }

    #endregion // Methods F1 2025

    #region Private methods

    /// <summary>
    /// Reads the car telemetry data of the sample packet
    /// </summary>
    /// <returns>Car telemetry data of the sample packet</returns>
    private static ICarTelemetry2025 GetCarTelemetryData()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12025CarTelemetrySize + ConstData.F12025HeaderSize, "Packet content too short!");

        var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

        Assert.IsInstanceOfType<CarTelemetry>(carTelemetry, "Packet is not a car telemetry data packet!");

        var packetData = ((CarTelemetry)carTelemetry).PacketData;

        Assert.IsInstanceOfType<CarTelemetry2025>(packetData, "Invalid car telemetry format, expected F1 2025!");

        return (ICarTelemetry2025)packetData;
    }

    #endregion // Private methods
}