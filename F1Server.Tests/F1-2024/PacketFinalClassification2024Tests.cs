using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test final classification packet files
/// </summary>
[TestClass]
public class PacketFinalClassification2024Tests
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
        var isFile = File.Exists(@"SampleData/F1-2024-FinalClassification.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2024-FinalClassification.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of final classification packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2024-FinalClassification.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2024

    /// <summary>
    /// Check whether the given file is a final classification packet
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2024IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.FinalClassification;

        Assert.IsTrue(isCorrect, "Packet is not a final classification packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2024IsFinalClassificationObject()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12024FinalClassificationSize + ConstData.F12024HeaderSize, "Packet content too short!");

        var isCorrect = false;
        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData)
        {
            isCorrect = finalClassificationData.PacketData is not null;
        }

        Assert.IsTrue(isCorrect, "Packet is not a final classification packet");
    }

    /// <summary>
    /// Check finishing position, laps completed and points of the first car (2024)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2024FirstCarResultExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12024FinalClassificationSize + ConstData.F12024HeaderSize, "Packet content too short!");

        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData && finalClassificationData.PacketData is FinalClassificationData2024 data)
        {
            var carResult = data.FinalClassifications[0];

            Assert.AreEqual(1, carResult.Position, "Incorrect finishing position!");
            Assert.AreEqual(4, carResult.LapsCompleted, "Incorrect number of laps completed!");
            Assert.AreEqual(1, carResult.GridPosition, "Incorrect grid position!");
            Assert.AreEqual(0, carResult.Points, "Incorrect number of points!");
            Assert.AreEqual(ResultStatus.Finished, carResult.ResultStatus, "Incorrect result status!");
            Assert.AreEqual((uint)67741, carResult.BestLapTimeInMs, "Incorrect best lap time!");
            Assert.AreEqual(408.4129028320, carResult.TotalRaceTime, 0.0001, "Incorrect total race time!");
        }
        else
        {
            Assert.Fail("Invalid final classification format, expected F1 2024!");
        }
    }

    /// <summary>
    /// Check tyre stint data of the first car (2024)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2024FirstCarTyreStintsExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12024FinalClassificationSize + ConstData.F12024HeaderSize, "Packet content too short!");

        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData && finalClassificationData.PacketData is FinalClassificationData2024 data && data.FinalClassifications[0] is IFinalClassification2024 carResult2024)
        {
            var carResult = data.FinalClassifications[0];

            Assert.AreEqual(1, carResult.NumTyreStints, "Incorrect number of tyre stints!");
            Assert.AreEqual(16, carResult.TyreStintsActual[0], "Incorrect actual tyre compound of first stint!");
            Assert.AreEqual(16, carResult.TyreStintsVisual[0], "Incorrect visual tyre compound of first stint!");
            Assert.AreEqual(255, carResult2024.TyreStintsEndLaps[0], "Incorrect end lap of first tyre stint!");
        }
        else
        {
            Assert.Fail("Invalid final classification format, expected F1 2024!");
        }
    }

    #endregion // Methods F1 2024
}