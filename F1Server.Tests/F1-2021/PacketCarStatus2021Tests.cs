using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;

namespace F1Server.Tests;

/// <summary>
/// Class to test car status packet files
/// </summary>
[TestClass]
public class PacketCarStatus2021Tests
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
        var file = File.Exists(@"SampleData/F1-2021-CarStatus.packet");

        if (file)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2021-CarStatus.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of car status packets failed!");
        }
        else
        {
            Assert.IsTrue(file, "File F1-2021-CarStatus.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2021

    /// <summary>
    /// Check whether the given file has a correct car status data content
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2021IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.CarStatus;

        Assert.IsTrue(isCorrect, "Packet is not a car status packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2021 packet
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2021IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2021;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2021 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketCarStatusCheckCarStatus2021IsCarStatusObject()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021CarStatusSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var isCorrect = false;
        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

        if (carStatus is CarStatus packetData)
        {
            isCorrect = packetData.PacketData is CarStatus2021;
        }

        Assert.IsTrue(isCorrect, "Packet is not a car status packet");
    }

    /// <summary>
    /// Check fuel remaining laps (2021)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelRemainingLaps2021ExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021CarStatusSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2021 carStatusData2021)
        {
            Assert.AreEqual(3.035119F, carStatusData2021.CarStatusData[0].FuelRemainingLaps, 0.0001F, "Incorrect fuel remaining laps!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2021!");
        }
    }

    /// <summary>
    /// Check visual tyre compound (2021)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusVisualTyreCompound2021ExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021CarStatusSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2021)
        {
            var isCorrect = statusData.PacketData?.CarStatusData[0].VisualTyreCompound == VisualTyreCompound.Soft;

            Assert.IsTrue(isCorrect, "Incorrect visual tyre compound!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2021!");
        }
    }

    /// <summary>
    /// Check ERS deployed this lap (2021)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusERSDeployedThisLap2021ExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021CarStatusSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2021 carStatusData2021)
        {
            Assert.AreEqual(2215349.5F, carStatusData2021.CarStatusData[0].ERSDeployedThisLap, 0.0001F, "Incorrect ERS deployed this lap value!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2021!");
        }
    }

    /// <summary>
    /// Check fuel capacity (2021)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelCapacity2021ExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021CarStatusSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2021 carStatusData2021)
        {
            Assert.AreEqual(110F, carStatusData2021.CarStatusData[0].FuelCapacity, 0.0001F, "Incorrect fuel capacity!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2021!");
        }
    }

    /// <summary>
    /// Check engine max RPM (2021)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusEngineMaxRpm2021ExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021CarStatusSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

        if (carStatus is CarStatus statusData && statusData.PacketData is CarStatus2021)
        {
            var isCorrect = statusData.PacketData?.CarStatusData[0].MaxRPM == 13000;

            Assert.IsTrue(isCorrect, "Incorrect engine rpm!");
        }
        else
        {
            Assert.Fail("Invalid car status format, expected F1 2021!");
        }
    }

    #endregion // Methods F1 2021
}