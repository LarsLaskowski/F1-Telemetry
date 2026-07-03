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
public class PacketCarStatus2024Tests
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
        var isFile = File.Exists(@"SampleData/F1-2024-CarStatus.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2024-CarStatus.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car status packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2024-CarStatus.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2024

    /// <summary>
    /// Check whether the given file has a correct car status data content
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2024IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarStatus;

        Assert.IsTrue(isCorrect, "Packet is not a car status data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2024 packet
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2024IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2024;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2024 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2024IsCarStatusDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024CarStatusSize + ConstData.F12024HeaderSize)
        {
            var isCorrect = false;
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus packetData)
            {
                isCorrect = packetData.PacketData is CarStatus2024;
            }

            Assert.IsTrue(isCorrect, "Packet is not a car status data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel remaining laps (2024)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelRemainingLaps2024ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024CarStatusSize + ConstData.F12024HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2024)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[4].FuelRemainingLaps == 6.87499952F;

                Assert.IsTrue(isCorrect, "Incorrect fuel remaining laps!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check visual tyre compound (2024)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusVisualTyreCompound2024ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024CarStatusSize + ConstData.F12024HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2024)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[9].VisualTyreCompound == VisualTyreCompound.Soft;

                Assert.IsTrue(isCorrect, "Incorrect visual tyre compound!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check ERS deployed this lap (2024)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusERSDeployedThisLap2024ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024CarStatusSize + ConstData.F12024HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2024)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[8].ERSDeployedThisLap == 0;

                Assert.IsTrue(isCorrect, "Incorrect ERS deployed this lap value!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel capacity (2024)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelCapacity2024ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024CarStatusSize + ConstData.F12024HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2024)
            {
                var isCorrect = statusData.PacketData.CarStatusData[1].FuelCapacity == 110;

                Assert.IsTrue(isCorrect, "Incorrect fuel capacity!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check engine max RPM (2024)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusEngineMaxRpm2024ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024CarStatusSize + ConstData.F12024HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2024)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[3].MaxRPM == 13999;

                Assert.IsTrue(isCorrect, "Incorrect engine rpm!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    #endregion // Methods F1 2024
}