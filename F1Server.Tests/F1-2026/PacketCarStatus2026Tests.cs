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
/// Class to test car status packet files (F1 2026)
/// </summary>
[TestClass]
public class PacketCarStatus2026Tests
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
        var isFile = File.Exists(@"SampleData/F1-2026-CarStatus.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2026-CarStatus.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car status packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-CarStatus.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file has a correct car status data content
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarStatus;

        Assert.IsTrue(isCorrect, "Packet is not a car status data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2026IsCarStatusDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12026CarStatusSize + ConstData.F12026HeaderSize)
        {
            var isCorrect = false;
            var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader!, _packetContent);

            if (carStatus is CarStatus packetData)
            {
                isCorrect = packetData.PacketData is CarStatus2026;
            }

            Assert.IsTrue(isCorrect, "Packet is not a car status data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2026 packet header or content!");
        }
    }

    /// <summary>
    /// Check fuel remaining laps (2026)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelRemainingLaps2026ExpectedValue()
    {
        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader!, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2026)
        {
            var isCorrect = statusData.PacketData?.CarStatusData[4].FuelRemainingLaps == 1.14814556F;

            Assert.IsTrue(isCorrect, "Incorrect fuel remaining laps!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check visual tyre compound (2026)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusVisualTyreCompound2026ExpectedValue()
    {
        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader!, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2026)
        {
            var isCorrect = statusData.PacketData?.CarStatusData[9].VisualTyreCompound == VisualTyreCompound.Medium;

            Assert.IsTrue(isCorrect, "Incorrect visual tyre compound!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check ERS deployed this lap (2026) - read correctly after the inserted ERS harvest limit field
    /// </summary>
    [TestMethod]
    public void PacketCarStatusERSDeployedThisLap2026ExpectedValue()
    {
        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader!, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2026)
        {
            var isCorrect = statusData.PacketData?.CarStatusData[19].ERSDeployedThisLap == 842641.6F;

            Assert.IsTrue(isCorrect, "Incorrect ERS deployed this lap value!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the new ERS harvest limit per lap (2026)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusERSHarvestLimitPerLap2026ExpectedValue()
    {
        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader!, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2026)
        {
            var harvestLimit = (statusData.PacketData?.CarStatusData[19] as ICarStatusData2026)?.ERSHarvestLimitPerLap;

            Assert.AreEqual(4000000F, harvestLimit, "Incorrect ERS harvest limit per lap!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check engine max RPM (2026)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusEngineMaxRpm2026ExpectedValue()
    {
        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader!, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2026)
        {
            var isCorrect = statusData.PacketData?.CarStatusData[3].MaxRPM == 13099;

            Assert.IsTrue(isCorrect, "Incorrect engine rpm!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2026!");
        }
    }

    #endregion // Methods F1 2026
}