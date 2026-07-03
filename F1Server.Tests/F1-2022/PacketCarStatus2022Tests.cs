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
public class PacketCarStatus2022Tests
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
        var isFile = File.Exists(@"SampleData/F1-2022-CarStatus.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2022-CarStatus.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car status packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2022-CarStatus.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2022

    /// <summary>
    /// Check whether the given file has a correct car status data content
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2022IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarStatus;

        Assert.IsTrue(isCorrect, "Packet is not a car status data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2022 packet
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2022IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2022;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2022 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2022IsCarStatusDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length == ConstData.F12022CarStatusSize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus packetData)
            {
                isCorrect = packetData.PacketData is CarStatus2022;
            }

            Assert.IsTrue(isCorrect, "Packet is not a car status data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel remaining laps (2022)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelRemainingLaps2022ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2022)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[0].FuelRemainingLaps == 1.65665889F;

                Assert.IsTrue(isCorrect, "Incorrect fuel remaining laps!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2022!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check visual tyre compound (2022)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusVisualTyreCompound2022ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2022)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[0].VisualTyreCompound == VisualTyreCompound.Wet;

                Assert.IsTrue(isCorrect, "Incorrect visual tyre compound!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2022!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check ERS deployed this lap (2022)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusERSDeployedThisLap2022ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2022)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[0].ERSDeployedThisLap == 0;

                Assert.IsTrue(isCorrect, "Incorrect ERS deployed this lap value!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2022!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel capacity (2022)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelCapacity2022ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2022)
            {
                var isCorrect = statusData.PacketData.CarStatusData[0].FuelCapacity == 94;

                Assert.IsTrue(isCorrect, "Incorrect fuel capacity!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2022!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check engine max RPM (2022)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusEngineMaxRpm2022ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022CarStatusSize + ConstData.F12020HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2022)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[0].MaxRPM == 9000;

                Assert.IsTrue(isCorrect, "Incorrect engine rpm!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2022!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    #endregion // Methods F1 2022
}