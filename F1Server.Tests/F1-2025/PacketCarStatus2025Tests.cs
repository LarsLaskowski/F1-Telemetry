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
public class PacketCarStatus2025Tests
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
        var isFile = File.Exists(@"SampleData/F1-2025-CarStatus.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2025-CarStatus.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car status packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2025-CarStatus.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2025

    /// <summary>
    /// Check whether the given file has a correct car status data content
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarStatus;

        Assert.IsTrue(isCorrect, "Packet is not a car status data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2025 packet
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2025IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2025;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2025 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2025IsCarStatusDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025CarStatusSize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus packetData)
            {
                isCorrect = packetData.PacketData is CarStatus2025;
            }

            Assert.IsTrue(isCorrect, "Packet is not a car status data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel remaining laps (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelRemainingLaps2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025CarStatusSize + ConstData.F12025HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2025)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[4].FuelRemainingLaps == 1.14814556F;

                Assert.IsTrue(isCorrect, "Incorrect fuel remaining laps!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check visual tyre compound (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusVisualTyreCompound2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025CarStatusSize + ConstData.F12025HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2025)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[9].VisualTyreCompound == VisualTyreCompound.Medium;

                Assert.IsTrue(isCorrect, "Incorrect visual tyre compound!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check ERS deployed this lap (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusERSDeployedThisLap2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025CarStatusSize + ConstData.F12025HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2025)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[19].ERSDeployedThisLap == 842641.6F;

                Assert.IsTrue(isCorrect, "Incorrect ERS deployed this lap value!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel capacity (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelCapacity2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025CarStatusSize + ConstData.F12025HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2025)
            {
                var isCorrect = statusData.PacketData.CarStatusData[1].FuelCapacity == 110;

                Assert.IsTrue(isCorrect, "Incorrect fuel capacity!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check engine max RPM (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusEngineMaxRpm2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025CarStatusSize + ConstData.F12025HeaderSize)
        {
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

            if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2025)
            {
                var isCorrect = statusData.PacketData?.CarStatusData[3].MaxRPM == 13099;

                Assert.IsTrue(isCorrect, "Incorrect engine rpm!");
            }
            else
            {
                Assert.Fail("Invalid car status format, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    #endregion // Methods F1 2025
}