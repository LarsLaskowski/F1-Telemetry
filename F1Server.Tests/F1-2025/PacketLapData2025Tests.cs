using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test lap data packet files
/// </summary>
[TestClass]
public class PacketLapData2025Tests
{
    #region Fields

    private static PacketAnalyzer _packetAnalyzer;
    private static ReceivedPacketData _packetData;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketLapDataInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2025-LapData.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2025-LapData.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of lap data packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2025-LapData.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2025

    /// <summary>
    /// Check whether the given file has a correct lap data content
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.LapData;

        Assert.IsTrue(isCorrect, "Packet is not a lap data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2025 packet
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2025IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2025;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2025 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2025IsLapDataObject()
    {
        var cars = GetLapDataCars();

        Assert.IsNotNull(cars, "Packet is not a lap data packet!");
    }

    /// <summary>
    /// Check the correct number of cars
    /// </summary>
    [TestMethod]
    public void PacketLapDataCarsOnLap2025Expected3()
    {
        var cars = GetLapDataCars();

        Assert.AreEqual(3, cars.Count(l => l.IsEmpty == false), "Number of cars is wrong!");
    }

    /// <summary>
    /// Check the last lap time of a car with a completed lap (2025)
    /// </summary>
    [TestMethod]
    public void PacketLapDataLastLapTime2025ExpectedValue()
    {
        var car = (ILapData2023)GetLapDataCars()[17];

        Assert.AreEqual(89963U, car.LastLapTime, "Incorrect last lap time!");
    }

    /// <summary>
    /// Check sector 1 and sector 2 time of the current lap (2025)
    /// </summary>
    [TestMethod]
    public void PacketLapDataSectorTimes2025ExpectedValue()
    {
        var car = (ILapData2023)GetLapDataCars()[9];

        Assert.AreEqual((ushort)31686, car.Sector1Time, "Incorrect sector 1 time!");
        Assert.AreEqual((ushort)19056, car.Sector2Time, "Incorrect sector 2 time!");
    }

    /// <summary>
    /// Check delta to race leader including the whole minute part (2025)
    /// </summary>
    [TestMethod]
    public void PacketLapDataDeltaToRaceLeader2025ExpectedValue()
    {
        var car = GetLapDataCars()[19];
        var car23 = (ILapData2023)car;

        Assert.AreEqual((ushort)41605, car23.DeltaToRaceLeader, "Incorrect delta to race leader!");
        Assert.AreEqual((ushort)2, car.DeltaToRaceLeaderMinutes, "Incorrect delta to race leader whole minute part!");
    }

    /// <summary>
    /// Check accumulated warnings of a car without infringements (2025)
    /// </summary>
    [TestMethod]
    public void PacketLapDataWarnings2025ExpectedZero()
    {
        var car = (ILapData2023)GetLapDataCars()[9];

        Assert.AreEqual((ushort)0, car.Warnings, "Incorrect warnings!");
        Assert.AreEqual((ushort)0, car.CornerCuttingWarnings, "Incorrect corner cutting warnings!");
    }

    #endregion // Methods F1 2025

    #region Private methods

    /// <summary>
    /// Reads the lap data cars of the sample packet
    /// </summary>
    /// <returns>Lap data cars of the sample packet</returns>
    private static ILapData2025[] GetLapDataCars()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");

        var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-LapData.packet"));

        Assert.IsInstanceOfType<LapData>(lapData, "Packet is not a lap data packet!");

        var dataComplete = ((LapData)lapData).PacketData;

        Assert.IsInstanceOfType<ILapDataComplete>(dataComplete, "Packet is not a lap data packet!");

        var cars = ((ILapDataComplete)dataComplete).LapData;

        Assert.IsInstanceOfType<ILapData2025[]>(cars, "Packet is not a F1 2025 lap data packet!");

        return (ILapData2025[])cars;
    }

    #endregion // Private methods
}