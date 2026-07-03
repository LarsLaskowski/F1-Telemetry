using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test car status packet files
/// </summary>
[TestClass]
public class PacketCarStatus2020Tests
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
    public static void PacketCarStatusInit(TestContext testContext)
    {
        var file = File.Exists(@"SampleData/F1-2020-CarStatus.packet");

        if (file)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2020-CarStatus.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car status packets failed!");
        }
        else
        {
            Assert.IsTrue(file, "File F1-2020-CarStatus.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2020

    /// <summary>
    /// Check whether the given file has a correct car status data content
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2020IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarStatus;

        Assert.IsTrue(isCorrect, "Packet is not a car status data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2020 packet
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2020IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2020;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2020 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2020IsCarStatusDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length == ConstData.F12020CarStatusSize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus packetData)
            {
                isCorrect = packetData.PacketData is CarStatus2020;
            }

            Assert.IsTrue(isCorrect, "Packet is not a car status data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel remaining laps (2020)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelRemainingLaps2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2020)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[0].FuelRemainingLaps == 13.1783037F;

                Assert.IsTrue(isCorrect, "Incorrect fuel remaining laps!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check visual tyre compound (2020)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusVisualTyreCompound2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2020)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[0].VisualTyreCompound == VisualTyreCompound.Hard;

                Assert.IsTrue(isCorrect, "Incorrect visual tyre compound!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check ERS deployed this lap (2020)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusERSDeployedThisLap2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2020)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[0].ERSDeployedThisLap == 0;

                Assert.IsTrue(isCorrect, "Incorrect ERS deployed this lap value!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel capacity (2020)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelCapacity2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2020)
            {
                var isCorrect = statusData.PacketData.CarStatusData[0].FuelCapacity == 110;

                Assert.IsTrue(isCorrect, "Incorrect fuel capacity!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check engine max RPM (2020)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusEngineMaxRpm2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2020)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[0].MaxRPM == 13000;

                Assert.IsTrue(isCorrect, "Incorrect engine rpm!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    #endregion // Methods F1 2020
}