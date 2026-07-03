using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test car telemetry packet files
/// </summary>
[TestClass]
public class PacketCarTelemetry2023Tests
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
        var isFile = File.Exists(@"SampleData/F1-2023-CarTelemetry.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2023-CarTelemetry.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car telemetry packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2023-CarTelemetry.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2023

    /// <summary>
    /// Check whether the given file has a correct car telemetry data content
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2023IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarTelemetry;

        Assert.IsTrue(isCorrect, "Packet is not a car telemetry data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2020 packet
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2023IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2023;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2023 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryCheckCarTelemetry2023IsCarTelemetryDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length == ConstData.F12023CarTelemetrySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry packetData)
            {
                isCorrect = packetData.PacketData is CarTelemetry2023;
            }

            Assert.IsTrue(isCorrect, "Packet is not a car telemetry data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check tyre pressure FL (2023)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryTyrePressure2023ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023CarTelemetrySize + ConstData.F12023HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2023)
            {
                var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].TyresPressure.FrontLeft == 23.0082989F;

                Assert.IsTrue(isCorrect, "Incorrect tyre pressure (FL)!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check speed (2023)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetrySpeed2023ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023CarTelemetrySize + ConstData.F12023HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2023)
            {
                var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].Speed == 192;

                Assert.IsTrue(isCorrect, "Incorrect speed!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check tyre inner temperature RR (2023)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryTyreInnerTemperature2023ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023CarTelemetrySize + ConstData.F12023HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2023)
            {
                var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].TyresInnerTemperature.RearRight == 100;

                Assert.IsTrue(isCorrect, "Incorrect tyre inner temperature (RR)!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check wheel surface type FR (2023)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryWheelSurface2023ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023CarTelemetrySize + ConstData.F12023HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is ICarTelemetry2023)
            {
                var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].SurfaceType.FrontRight == SurfaceType.Tarmac;

                Assert.IsTrue(isCorrect, "Incorrect wheel surface type (FR)!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check engine RPM (2023)
    /// </summary>
    [TestMethod]
    public void PacketCarTelemetryEngineRpm2023ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023CarTelemetrySize + ConstData.F12023HeaderSize)
        {
            var carTelemetry = _packetAnalyzer.GetCarTelemetry(_packetData.PacketHeader, _packetContent);

            if (carTelemetry is CarTelemetry telemetryData && telemetryData.PacketData is CarTelemetry2023)
            {
                var isCorrect = telemetryData.PacketData?.CarTelemetryData[0].EngineRPM == 11865;

                Assert.IsTrue(isCorrect, "Incorrect engine rpm!");
            }
            else
            {
                Assert.Fail("Invalid car telemetry format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    #endregion // Methods F1 2023
}