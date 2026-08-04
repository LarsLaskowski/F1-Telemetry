using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test lap positions packet files (F1 2026)
/// </summary>
[TestClass]
public class PacketLapPositions2026Tests
{
    #region Constants

    private const string SampleFile = @"SampleData/F1-2026-LapPositions.packet";

    #endregion // Constants

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
    public static void PacketLapPositionsInit(TestContext testContext)
    {
        var isFile = File.Exists(SampleFile);

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(SampleFile);

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of lap positions packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-LapPositions.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Reads the lap positions data of the sample packet
    /// </summary>
    /// <returns>Lap positions data of the sample packet</returns>
    private static ILapPositionsBase GetLapPositions()
    {
        var packetHeader = _packetData.PacketHeader;

        Assert.IsNotNull(packetHeader, "Sample packet has no header!");

        var lapPositionsData = _packetAnalyzer.GetLapPositionsData(packetHeader, File.ReadAllBytes(SampleFile));

        Assert.IsInstanceOfType<LapPositions>(lapPositionsData, "Packet was not transformed into lap positions!");

        var packetData = ((LapPositions)lapPositionsData).PacketData;

        Assert.IsNotNull(packetData, "Lap positions packet contains no data!");

        return packetData;
    }

    #endregion // Static methods

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file is a lap positions packet
    /// </summary>
    [TestMethod]
    public void PacketLapPositionsCheckLapPositions2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.LapPositions;

        Assert.IsTrue(isCorrect, "Packet is not a lap positions packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketLapPositionsCheckLapPositions2026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet!");
    }

    /// <summary>
    /// Check that the analyzer constructs the F1 2026 lap positions object
    /// </summary>
    [TestMethod]
    public void PacketLapPositions2026IsLapPositionsObject()
    {
        Assert.IsInstanceOfType<LapPositions2026>(GetLapPositions(), "Lap positions data is not a F1 2026 object!");
    }

    /// <summary>
    /// Check the lap count and start index of the sample packet
    /// </summary>
    [TestMethod]
    public void PacketLapPositions2026ExpectedLapCount()
    {
        var lapPositions = GetLapPositions();

        Assert.AreEqual(5, lapPositions.NumberOfLaps, "Wrong number of laps!");
        Assert.AreEqual(0, lapPositions.LapStartIndex, "Wrong lap start index!");
    }

    /// <summary>
    /// Check that the position array is indexed as [lap, car] and holds 24 cars per lap, which is
    /// the F1 2026 car count
    /// </summary>
    [TestMethod]
    public void PacketLapPositions2026PositionArrayHasLapAndCarDimensions()
    {
        var lapPositions = GetLapPositions();

        Assert.AreEqual(ConstData.F12026MaxLapPositions, lapPositions.CarPositionOnLaps.GetLength(0), "Wrong lap dimension of the position array!");
        Assert.AreEqual(ConstData.F12026MaxCars, lapPositions.CarPositionOnLaps.GetLength(1), "Wrong car dimension of the position array!");
    }

    /// <summary>
    /// Check the positions of the first lap. The sample rotates the field by one position per
    /// lap, so a wrong dimension order or a wrong per-lap stride shows up immediately
    /// </summary>
    [TestMethod]
    public void PacketLapPositionsFirstLap2026ExpectedValues()
    {
        var lapPositions = GetLapPositions();

        Assert.AreEqual(1, lapPositions.CarPositionOnLaps[0, 0], "Wrong position of car 0 on the first lap!");
        Assert.AreEqual(2, lapPositions.CarPositionOnLaps[0, 1], "Wrong position of car 1 on the first lap!");
        Assert.AreEqual(24, lapPositions.CarPositionOnLaps[0, 23], "Wrong position of the last car on the first lap!");
    }

    /// <summary>
    /// Check the positions of a later lap, which verifies that each lap starts at its own offset
    /// inside the flat position block
    /// </summary>
    [TestMethod]
    public void PacketLapPositionsThirdLap2026ExpectedValues()
    {
        var lapPositions = GetLapPositions();

        Assert.AreEqual(3, lapPositions.CarPositionOnLaps[2, 0], "Wrong position of car 0 on the third lap!");
        Assert.AreEqual(4, lapPositions.CarPositionOnLaps[2, 1], "Wrong position of car 1 on the third lap!");
    }

    /// <summary>
    /// Check that laps beyond the reported lap count stay untouched, so the parser does not read
    /// past the number of laps the packet announces
    /// </summary>
    [TestMethod]
    public void PacketLapPositionsBeyondLapCount2026IsNotFilled()
    {
        var lapPositions = GetLapPositions();

        Assert.AreEqual(0, lapPositions.CarPositionOnLaps[5, 0], "Positions behind the reported lap count must stay empty!");
    }

    #endregion // Methods F1 2026
}