using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Utils;

namespace F1Server.Tests;

/// <summary>
/// Class to test session history packet files
/// </summary>
[TestClass]
public class PacketSessionHistory2026Tests
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
    public static void PacketSessionHistoryInit(TestContext testContext)
    {
        var is2026File = File.Exists(@"SampleData/F1-2026-SessionHistory.packet");

        if (is2026File)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2026-SessionHistory.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session history packets failed!");
        }
        else
        {
            Assert.IsTrue(is2026File, "File F1-2026-SessionHistory.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Reads the session history data of the sample packet
    /// </summary>
    /// <returns>Session history data of the sample packet</returns>
    private static SessionHistoryData2026 GetSessionHistoryData()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12026SessionHistorySize + ConstData.F12026HeaderSize, "Packet content too short!");

        var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

        Assert.IsInstanceOfType<SessionHistoryData>(sessionHistory, "Packet was not transformed into session history data!");

        var packetData = ((SessionHistoryData)sessionHistory).PacketData;

        Assert.IsInstanceOfType<SessionHistoryData2026>(packetData, "Session history packet is not a F1 2026 object!");

        return (SessionHistoryData2026)packetData;
    }

    #endregion // Static methods

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file is a session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.SessionHistory;

        Assert.IsTrue(isCorrect, "Packet is not a session history packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026IsSessionHistoryObject()
    {
        Assert.IsNotNull(GetSessionHistoryData(), "Packet is not a session history packet!");
    }

    /// <summary>
    /// Check best lap number in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026BestLap()
    {
        Assert.AreEqual((ushort)2, GetSessionHistoryData().BestLapNumber, "Best lap number is invalid!");
    }

    /// <summary>
    /// Check car index in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026BestLapCarIndex()
    {
        Assert.AreEqual((ushort)7, GetSessionHistoryData().CarIndex, "Car index of best lap is invalid!");
    }

    /// <summary>
    /// Check number of laps in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026NumberOfLaps()
    {
        Assert.AreEqual((ushort)3, GetSessionHistoryData().NumberOfLaps, "Number of laps is invalid!");
    }

    /// <summary>
    /// Check lap time time from lap 1 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026LapTime()
    {
        Assert.AreEqual(83435u, GetSessionHistoryData().LapHistory[0].LapTime, "Lap time is invalid!");
    }

    /// <summary>
    /// Check sector 1 time from lap 3 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026Sector1Time()
    {
        Assert.AreEqual((ushort)29372, GetSessionHistoryData().LapHistory[0].Sector1Time, "Sector 1 time is invalid!");
    }

    /// <summary>
    /// Check sector 3 time from lap 4 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026Sector3Time()
    {
        Assert.AreEqual((ushort)35549, GetSessionHistoryData().LapHistory[0].Sector3Time, "Sector 3 time is invalid!");
    }

    /// <summary>
    /// Check number of tyre stints in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026TyreStints()
    {
        Assert.AreEqual((ushort)1, GetSessionHistoryData().NumberOfTyreStints, "Number of tyre stints is invalid!");
    }

    /// <summary>
    /// Check first tyre stint actual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026FirstTyreStintActual()
    {
        Assert.AreEqual((ushort)16, GetSessionHistoryData().TyreStintHistory[0].TyreActualCompound, "Actual compound of first tyre stint is invalid!");
    }

    /// <summary>
    /// Check first tyre stint visual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026FirstTyreStintVisual()
    {
        Assert.AreEqual((ushort)16, GetSessionHistoryData().TyreStintHistory[0].TyreVisualCompound, "Visual compound of first tyre stint is invalid!");
    }

    /// <summary>
    /// Check first tyre stint visual compound mapping
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2026VisualCompoundMapper()
    {
        var visualCompound = TyreCompoundMapper.MapVisualTyreCompoundToEnum(GetSessionHistoryData().TyreStintHistory[0].TyreVisualCompound);

        Assert.AreEqual(VisualTyreCompound.Soft, visualCompound, "Visual compound mapping is invalid!");
    }

    #endregion // Methods F1 2026
}