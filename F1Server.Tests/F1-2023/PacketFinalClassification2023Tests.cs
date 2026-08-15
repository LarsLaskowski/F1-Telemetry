using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;

namespace F1Server.Tests;

/// <summary>
/// Class to test final classification packet files
/// </summary>
[TestClass]
public class PacketFinalClassification2023Tests
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
        var isFile = File.Exists(@"SampleData/F1-2023-FinalClassification.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2023-FinalClassification.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of final classification packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2023-FinalClassification.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2023

    /// <summary>
    /// Check whether the given file is a final classification packet
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2023IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.FinalClassification;

        Assert.IsTrue(isCorrect, "Packet is not a final classification packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2023IsFinalClassificationObject()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12023FinalClassificationSize + ConstData.F12023HeaderSize, "Packet content too short!");

        var isCorrect = false;
        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData)
        {
            isCorrect = finalClassificationData.PacketData is not null;
        }

        Assert.IsTrue(isCorrect, "Packet is not a final classification packet");
    }

    /// <summary>
    /// Check finishing position, points, laps completed, pit stops, result status and tyre stint
    /// data of the first car in the final classification content (2023)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2023ContentValues()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12023FinalClassificationSize + ConstData.F12023HeaderSize, "Packet content too short!");

        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData && finalClassificationData.PacketData is FinalClassificationData2023 packetData2023)
        {
            var car = (FinalClassificationCarData2023)packetData2023.FinalClassifications[0];

            Assert.AreEqual((ushort)22, packetData2023.NumberOfCars, "Incorrect number of cars!");
            Assert.AreEqual((ushort)10, car.Position, "Incorrect finishing position!");
            Assert.AreEqual((ushort)1, car.LapsCompleted, "Incorrect number of laps completed!");
            Assert.AreEqual((ushort)0, car.Points, "Incorrect number of points!");
            Assert.AreEqual((ushort)2, car.PitStops, "Incorrect number of pit stops!");
            Assert.AreEqual(ResultStatus.Finished, car.ResultStatus, "Incorrect result status!");
            Assert.AreEqual((ushort)1, car.NumTyreStints, "Incorrect number of tyre stints!");
            Assert.AreEqual((ushort)255, car.TyreStintsEndLaps[0], "Incorrect tyre stint end lap!");
        }
        else
        {
            Assert.Fail("Invalid final classification packet, expected F1 2023!");
        }
    }

    #endregion // Methods F1 2023
}