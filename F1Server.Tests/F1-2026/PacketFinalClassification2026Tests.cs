using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test final classification packet files
/// </summary>
[TestClass]
public class PacketFinalClassification2026Tests
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
    public static void PacketFinalClassificationInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2026-FinalClassification.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2026-FinalClassification.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of final classification packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-FinalClassification.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Reads the final classification data of the sample packet
    /// </summary>
    /// <returns>Final classification data of the sample packet</returns>
    private static IFinalClassificationData GetFinalClassificationData()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12026FinalClassificationSize + ConstData.F12026HeaderSize, "Packet content too short!");

        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        Assert.IsInstanceOfType<FinalClassificationData>(finalClassification, "Packet was not transformed into final classification data!");

        var packetData = ((FinalClassificationData)finalClassification).PacketData;

        Assert.IsNotNull(packetData, "Final classification packet contains no data!");

        return packetData;
    }

    #endregion // Static methods

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file is a final classification packet
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.FinalClassification;

        Assert.IsTrue(isCorrect, "Packet is not a final classification packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2026IsFinalClassificationObject()
    {
        Assert.IsNotNull(GetFinalClassificationData(), "Packet is not a final classification packet!");
    }

    /// <summary>
    /// Check the total number of classified cars and the finishing details of the first car (2026)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationFirstCar2026ExpectedValues()
    {
        var finalClassificationData = GetFinalClassificationData();

        Assert.AreEqual((ushort)10, finalClassificationData.NumberOfCars, "Wrong number of classified cars!");

        var car = finalClassificationData.FinalClassifications[0];

        Assert.AreEqual((ushort)2, car.Position, "Wrong finishing position of the first car!");
        Assert.AreEqual((ushort)2, car.LapsCompleted, "Wrong number of completed laps of the first car!");
        Assert.AreEqual((ushort)1, car.PitStops, "Wrong number of pit stops of the first car!");
        Assert.AreEqual(ResultStatus.Finished, car.ResultStatus, "Wrong result status of the first car!");
        Assert.AreEqual(81893u, car.BestLapTimeInMs, "Wrong best lap time of the first car!");
        Assert.AreEqual(165.38888549804688, car.TotalRaceTime, 0.0001, "Wrong total race time of the first car!");
    }

    /// <summary>
    /// Check the tyre stint history of the first car, which made one pit stop and used two stints (2026)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationFirstCarTyreStints2026ExpectedValues()
    {
        var finalClassificationData = GetFinalClassificationData();

        var car = finalClassificationData.FinalClassifications[0];

        Assert.AreEqual((ushort)2, car.NumTyreStints, "Wrong number of tyre stints of the first car!");
        Assert.AreEqual((ushort)16, car.TyreStintsActual[0], "Wrong actual compound of the first tyre stint!");
        Assert.AreEqual((ushort)16, car.TyreStintsActual[1], "Wrong actual compound of the second tyre stint!");
        Assert.AreEqual((ushort)16, car.TyreStintsVisual[0], "Wrong visual compound of the first tyre stint!");
    }

    /// <summary>
    /// Check the race winner, who finished on a single tyre stint without any pit stop (2026)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationRaceWinner2026ExpectedValues()
    {
        var finalClassificationData = GetFinalClassificationData();

        var car = finalClassificationData.FinalClassifications[9];

        Assert.AreEqual((ushort)1, car.Position, "Race winner must be classified in first position!");
        Assert.AreEqual((ushort)0, car.PitStops, "Race winner must not have any pit stops!");
        Assert.AreEqual((ushort)1, car.NumTyreStints, "Race winner must have a single tyre stint!");
        Assert.AreEqual(81559u, car.BestLapTimeInMs, "Wrong best lap time of the race winner!");
    }

    #endregion // Methods F1 2026
}