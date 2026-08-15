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
public class PacketFinalClassification2022Tests
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
        var isFile = File.Exists(@"SampleData/F1-2022-FinalClassification.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2022-FinalClassification.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of final classification packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2022-FinalClassification.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2022

    /// <summary>
    /// Check whether the given file is a final classification packet
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2022IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.FinalClassification;

        Assert.IsTrue(isCorrect, "Packet is not a final classification packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2022IsFinalClassificationObject()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12022FinalClassificationSize + ConstData.F12022HeaderSize, "Packet content too short!");

        var isCorrect = false;
        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData)
        {
            isCorrect = finalClassificationData.PacketData is not null;
        }

        Assert.IsTrue(isCorrect, "Packet is not a final classification packet");
    }

    /// <summary>
    /// Check finishing position and grid position of the player car (2022)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2022PlayerPositionExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12022FinalClassificationSize + ConstData.F12022HeaderSize, "Packet content too short!");

        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData && finalClassificationData.PacketData is FinalClassificationData2022 data2022)
        {
            var playerClassification = data2022.FinalClassifications[_packetData.PacketHeader.PlayerCarIndex];

            Assert.AreEqual((ushort)8, playerClassification.Position, "Incorrect finishing position!");
            Assert.AreEqual((ushort)22, playerClassification.GridPosition, "Incorrect grid position!");
        }
        else
        {
            Assert.Fail("Invalid final classification packet, expected F1 2022!");
        }
    }

    /// <summary>
    /// Check points and number of laps completed by the player car (2022)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2022PlayerPointsAndLapsExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12022FinalClassificationSize + ConstData.F12022HeaderSize, "Packet content too short!");

        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData && finalClassificationData.PacketData is FinalClassificationData2022 data2022)
        {
            var playerClassification = data2022.FinalClassifications[_packetData.PacketHeader.PlayerCarIndex];

            Assert.AreEqual((ushort)6, playerClassification.Points, "Incorrect points!");
            Assert.AreEqual((ushort)5, playerClassification.LapsCompleted, "Incorrect number of completed laps!");
            Assert.AreEqual(ResultStatus.Finished, playerClassification.ResultStatus, "Incorrect result status!");
        }
        else
        {
            Assert.Fail("Invalid final classification packet, expected F1 2022!");
        }
    }

    /// <summary>
    /// Check tyre stint data of the player car (2022)
    /// </summary>
    [TestMethod]
    public void PacketFinalClassificationCheck2022PlayerTyreStintExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12022FinalClassificationSize + ConstData.F12022HeaderSize, "Packet content too short!");

        var finalClassification = _packetAnalyzer.GetFinalClassificationData(_packetData.PacketHeader, _packetContent);

        if (finalClassification is FinalClassificationData finalClassificationData
            && finalClassificationData.PacketData is FinalClassificationData2022 data2022
            && data2022.FinalClassifications[_packetData.PacketHeader.PlayerCarIndex] is FinalClassificationCarData2022 playerClassification)
        {
            Assert.AreEqual((ushort)1, playerClassification.NumTyreStints, "Incorrect number of tyre stints!");
            Assert.AreEqual((ushort)15, playerClassification.TyreStintsActual[0], "Incorrect actual tyre compound of first stint!");
            Assert.AreEqual((ushort)15, playerClassification.TyreStintsVisual[0], "Incorrect visual tyre compound of first stint!");
            Assert.AreEqual((ushort)255, playerClassification.TyreStintsEndLaps[0], "Incorrect end lap of first tyre stint!");
        }
        else
        {
            Assert.Fail("Invalid final classification packet, expected F1 2022!");
        }
    }

    #endregion // Methods F1 2022
}