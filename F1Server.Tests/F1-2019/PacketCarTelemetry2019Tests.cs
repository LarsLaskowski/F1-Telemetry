using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test car telemetry packet files
/// </summary>
[TestClass]
public class PacketCarTelemetry2019Tests
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
        var file = File.Exists(@"SampleData/F1-2019-CarTelemetry.packet");

        if (file)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2019-CarTelemetry.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car telemetry packets failed!");
        }
        else
        {
            Assert.IsTrue(file, "File F1-2019-CarTelemetry.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2019

    /// <summary>
    /// Check whether the given file has a correct car telemetry data content
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2019IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarTelemetry;

        Assert.IsTrue(isCorrect, "Packet is not a car telemetry data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2019 packet
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2019IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2019;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2019 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2019IsCarTelemetryDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length == ConstData.F12019CarTelemetrySize + ConstData.F12019HeaderSize)
        {
            var isCorrect = false;
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry packetData)
            {
                isCorrect = packetData.PacketData is CarTelemetry2019;
            }

            Assert.IsTrue(isCorrect, "Packet is not a car telemetry data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2019 packet header or content!");
        }
    }

    /// <summary>
    /// Check speed (2019)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetrySpeed2019ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12019CarTelemetrySize + ConstData.F12019HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2019)
            {
                var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].Speed == 300;

                Assert.IsTrue(isCorrect, "Incorrect speed!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2019!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2019 packet header or content!");
        }
    }

    /// <summary>
    /// Check tyre surface temperature RL is parsed as uint16, not uint8 (2019)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryTyreSurfaceTemperature2019ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12019CarTelemetrySize + ConstData.F12019HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2019)
            {
                var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].TyresSurfaceTemperature.RearLeft == 300;

                Assert.IsTrue(isCorrect, "Incorrect tyre surface temperature (RL)!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2019!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2019 packet header or content!");
        }
    }

    /// <summary>
    /// Check tyre inner temperature RR is parsed as uint16, not uint8 (2019)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryTyreInnerTemperature2019ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12019CarTelemetrySize + ConstData.F12019HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2019)
            {
                var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].TyresInnerTemperature.RearRight == 311;

                Assert.IsTrue(isCorrect, "Incorrect tyre inner temperature (RR)!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2019!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2019 packet header or content!");
        }
    }

    /// <summary>
    /// Check wheel surface type FR (2019)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryWheelSurface2019ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12019CarTelemetrySize + ConstData.F12019HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2019)
            {
                var isCorrect = telemetryData.PacketData.CarTelemetryData[0].SurfaceType.FrontRight == SurfaceType.Rock;

                Assert.IsTrue(isCorrect, "Incorrect wheel surface type (FR)!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2019!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2019 packet header or content!");
        }
    }

    /// <summary>
    /// Check the button status, read after all 20 cars have been parsed, is not discarded (2019)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryButtonStatus2019ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12019CarTelemetrySize + ConstData.F12019HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2019 carTelemetry2019)
            {
                var isCorrect = carTelemetry2019.ButtonStatus == 1;

                Assert.IsTrue(isCorrect, "Incorrect button status!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2019!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2019 packet header or content!");
        }
    }

    #endregion // Methods F1 2019
}