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
public class PacketSessionHistory2025Tests
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
        var is2025File = File.Exists(@"SampleData/F1-2025-SessionHistory.packet");

        if (is2025File)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2025-SessionHistory.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session history packets failed!");
        }
        else
        {
            Assert.IsTrue(is2025File, "File F1-2025-SessionHistory.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2025

    /// <summary>
    /// Check whether the given file is a session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.SessionHistory;

        Assert.IsTrue(isCorrect, "Packet is not a session history packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025IsSessionHistoryObject()
    {
        var data = GetSessionHistoryData();

        Assert.IsNotNull(data, "Packet is not a session history packet");
    }

    /// <summary>
    /// Check best lap number in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025BestLap()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual((ushort)16, data.BestLapNumber, "Best lap number is invalid!");
    }

    /// <summary>
    /// Check car index in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025BestLapCarIndex()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual((ushort)11, data.CarIndex, "Car index of best lap is invalid!");
    }

    /// <summary>
    /// Check number of laps in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025NumberOfLaps()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual((ushort)28, data.NumberOfLaps, "Number of laps is invalid!");
    }

    /// <summary>
    /// Check lap time time from lap 1 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025LapTime()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual(116270U, data.LapHistory[0].LapTime, "Lap time is invalid!");
    }

    /// <summary>
    /// Check sector 1 time from lap 3 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025Sector1Time()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual((ushort)35013, data.LapHistory[0].Sector1Time, "Sector 1 time is invalid!");
    }

    /// <summary>
    /// Check sector 3 time from lap 4 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025Sector3Time()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual((ushort)46644, data.LapHistory[0].Sector3Time, "Sector 3 time is invalid!");
    }

    /// <summary>
    /// Check number of tyre stints in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025TyreStints()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual((ushort)2, data.NumberOfTyreStints, "Number of tyre stints is invalid!");
    }

    /// <summary>
    /// Check first tyre stint actual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025FirstTyreStintActual()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual((ushort)18, data.TyreStintHistory[0].TyreActualCompound, "Actual compound of first tyre stint is invalid!");
    }

    /// <summary>
    /// Check first tyre stint visual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025FirstTyreStintVisual()
    {
        var data = GetSessionHistoryData();

        Assert.AreEqual((ushort)17, data.TyreStintHistory[0].TyreVisualCompound, "Visual compound of first tyre stint is invalid!");
    }

    /// <summary>
    /// Check first tyre stint visual compound mapping
    /// </summary>
    [TestMethod]
    public void PacketSessionHistoryCheck2025VisualCompoundMapper()
    {
        var data = GetSessionHistoryData();
        var visualCompound = TyreCompoundMapper.MapVisualTyreCompoundToEnum(data.TyreStintHistory[0].TyreVisualCompound);

        Assert.AreEqual(VisualTyreCompound.Medium, visualCompound, "Visual compound mapping is invalid!");
    }

    #endregion // Methods F1 2025

    #region Private methods

    /// <summary>
    /// Reads the session history data of the sample packet
    /// </summary>
    /// <returns>Session history data of the sample packet</returns>
    private static SessionHistoryData2025 GetSessionHistoryData()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize, "Packet content too short!");

        var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

        Assert.IsInstanceOfType<SessionHistoryData>(sessionHistory, "Packet is not a session history packet!");

        var packetData = ((SessionHistoryData)sessionHistory).PacketData;

        Assert.IsInstanceOfType<SessionHistoryData2025>(packetData, "Invalid session history packet, expected F1 2025!");

        return (SessionHistoryData2025)packetData;
    }

    #endregion // Private methods
}