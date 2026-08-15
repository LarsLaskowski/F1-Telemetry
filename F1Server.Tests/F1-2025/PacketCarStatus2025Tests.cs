using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

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
        var data = GetCarStatusData();

        Assert.IsNotNull(data, "Packet is not a car status data packet");
    }

    /// <summary>
    /// Check fuel remaining laps (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelRemainingLaps2025ExpectedValue()
    {
        var data = GetCarStatusData();

        Assert.AreEqual(1.14814556F, data.CarStatusData[4].FuelRemainingLaps, 0.0001F, "Incorrect fuel remaining laps!");
    }

    /// <summary>
    /// Check visual tyre compound (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusVisualTyreCompound2025ExpectedValue()
    {
        var data = GetCarStatusData();

        Assert.AreEqual(VisualTyreCompound.Medium, data.CarStatusData[9].VisualTyreCompound, "Incorrect visual tyre compound!");
    }

    /// <summary>
    /// Check ERS deployed this lap (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusERSDeployedThisLap2025ExpectedValue()
    {
        var data = GetCarStatusData();

        Assert.AreEqual(842641.6F, data.CarStatusData[19].ERSDeployedThisLap, 0.0001F, "Incorrect ERS deployed this lap value!");
    }

    /// <summary>
    /// Check fuel capacity (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusFuelCapacity2025ExpectedValue()
    {
        var data = GetCarStatusData();

        Assert.AreEqual(110F, data.CarStatusData[1].FuelCapacity, 0.0001F, "Incorrect fuel capacity!");
    }

    /// <summary>
    /// Check engine max RPM (2025)
    /// </summary>
    [TestMethod]
    public void PacketCarStatusEngineMaxRpm2025ExpectedValue()
    {
        var data = GetCarStatusData();

        Assert.AreEqual(13099U, data.CarStatusData[3].MaxRPM, "Incorrect engine rpm!");
    }

    #endregion // Methods F1 2025

    #region Private methods

    /// <summary>
    /// Reads the car status data of the sample packet
    /// </summary>
    /// <returns>Car status data of the sample packet</returns>
    private static ICarStatus2025 GetCarStatusData()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12025CarStatusSize + ConstData.F12025HeaderSize, "Packet content too short!");

        var carStatus = _packetAnalyzer.GetCarStatus(_packetData.PacketHeader, _packetContent);

        Assert.IsInstanceOfType<CarStatus>(carStatus, "Packet is not a car status data packet!");

        var packetData = ((CarStatus)carStatus).PacketData;

        Assert.IsInstanceOfType<CarStatus2025>(packetData, "Invalid car status format, expected F1 2025!");

        return (ICarStatus2025)packetData;
    }

    #endregion // Private methods
}