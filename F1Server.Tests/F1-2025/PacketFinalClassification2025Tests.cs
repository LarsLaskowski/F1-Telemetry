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
public class PacketFinalClassification2025Tests
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
        var isFile = File.Exists(@"SampleData/F1-2025-FinalClassification.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2025-FinalClassification.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of final classification packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2025-FinalClassification.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2025

    /// <summary>
    /// Check whether the given file is a final classification packet
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.FinalClassification;

        Assert.IsTrue(isCorrect, "Packet is not a final classification packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2025IsFinalClassificationObject()
    {
        var packetData = GetFinalClassificationData();

        Assert.IsNotNull(packetData, "Packet is not a final classification packet");
    }

    /// <summary>
    /// Check the number of cars in the final classification (2025)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationNumberOfCars2025ExpectedValue()
    {
        var packetData = GetFinalClassificationData();

        Assert.AreEqual((ushort)20, packetData.NumberOfCars, "Incorrect number of cars!");
    }

    /// <summary>
    /// Check position and points of the race winner (2025)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationPositionAndPoints2025ExpectedValue()
    {
        var car = GetFinalClassificationData().FinalClassifications[19];

        Assert.AreEqual((ushort)1, car.Position, "Incorrect position!");
        Assert.AreEqual((ushort)25, car.Points, "Incorrect points!");
    }

    /// <summary>
    /// Check completed laps and pit stops of the race winner (2025)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationLapsAndPitStops2025ExpectedValue()
    {
        var car = GetFinalClassificationData().FinalClassifications[19];

        Assert.AreEqual((ushort)27, car.LapsCompleted, "Incorrect laps completed!");
        Assert.AreEqual((ushort)2, car.PitStops, "Incorrect pit stops!");
    }

    /// <summary>
    /// Check accumulated penalties of the race winner (2025)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationPenalties2025ExpectedValue()
    {
        var car = GetFinalClassificationData().FinalClassifications[19];

        Assert.AreEqual((ushort)10, car.PenaltiesTime, "Incorrect penalties time!");
        Assert.AreEqual((ushort)1, car.NumPenalties, "Incorrect number of penalties!");
    }

    /// <summary>
    /// Check tyre stints of the race winner (2025)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationTyreStints2025ExpectedValue()
    {
        var car = GetFinalClassificationData().FinalClassifications[19];

        Assert.AreEqual((ushort)3, car.NumTyreStints, "Incorrect number of tyre stints!");
        Assert.AreEqual((ushort)8, car.TyreStintsActual[0], "Incorrect actual compound of the first tyre stint!");
        Assert.AreEqual((ushort)7, car.TyreStintsActual[1], "Incorrect actual compound of the second tyre stint!");
        Assert.AreEqual((ushort)20, car.TyreStintsActual[2], "Incorrect actual compound of the third tyre stint!");
        Assert.AreEqual((ushort)18, car.TyreStintsVisual[2], "Incorrect visual compound of the third tyre stint!");
    }

    /// <summary>
    /// Check the result status and reason of the race winner (2025)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationResultStatusAndReason2025ExpectedFinished()
    {
        var car = GetFinalClassificationData().FinalClassifications[19];

        Assert.AreEqual(ResultStatus.Finished, car.ResultStatus, "Incorrect result status!");
        Assert.IsInstanceOfType<IFinalClassification2025>(car, "Car data is not a F1 2025 final classification object!");
        Assert.AreEqual(ResultReason.Finished, ((IFinalClassification2025)car).ResultReason, "Incorrect result reason!");
    }

    #endregion // Methods F1 2025

    #region Private methods

    /// <summary>
    /// Reads the final classification data of the sample packet
    /// </summary>
    /// <returns>Final classification data of the sample packet</returns>
    private static IFinalClassificationData GetFinalClassificationData()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12025FinalClassificationSize + ConstData.F12025HeaderSize, "Packet content too short!");

        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        Assert.IsInstanceOfType<FinalClassificationData>(finalClassification, "Packet is not a final classification packet!");

        var packetData = ((FinalClassificationData)finalClassification).PacketData;

        Assert.IsNotNull(packetData, "Final classification packet contains no data!");

        return packetData;
    }

    #endregion // Private methods
}