using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test session packet files
/// </summary>
[TestClass]
public class PacketSession2025Tests
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
    public static void PacketSessionInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2025-Session.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2025-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2025-Session.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2025

    /// <summary>
    /// Check whether the given file is a session packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Session;

        Assert.IsTrue(isCorrect, "Packet is not a session packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2025 packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2025IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2025;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2025 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2025IsSessionObject()
    {
        var data = GetSessionData();

        Assert.IsNotNull(data, "Packet is not a session packet");
    }

    /// <summary>
    /// Check ai difficulty (2025)
    /// </summary>
    [TestMethod]
    public void PacketSessionAiDifficulty2025ExpectedSixty()
    {
        var data = GetSessionData();

        Assert.AreEqual((ushort)60, data.AiDifficulty, "Incorrect ai difficulty!");
    }

    /// <summary>
    /// Check track (2025)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrack2025ExpectedSuzuka()
    {
        var data = GetSessionData();

        Assert.IsTrue(data.TrackName.Equals("Suzuka"), "Invalid track id!");
    }

    /// <summary>
    /// Check track length (2025)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrackLength2025ExpectedValue()
    {
        var data = GetSessionData();

        Assert.AreEqual(5809, data.TrackLength, "Invalid track length!");
    }

    /// <summary>
    /// Check formula type (2025)
    /// </summary>
    [TestMethod]
    public void PacketSessionFormulaType2025ExpectedF1Modern()
    {
        var data = GetSessionData();

        Assert.AreEqual(Formula.F1Modern, data.FormulaType, "Invalid formula type!");
    }

    #endregion // Methods F1 2025

    #region Private methods

    /// <summary>
    /// Reads the session data of the sample packet
    /// </summary>
    /// <returns>Session data of the sample packet</returns>
    private static ISessionData2025 GetSessionData()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12025SessionSize + ConstData.F12025HeaderSize, "Packet content too short!");

        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

        Assert.IsInstanceOfType<SessionData>(session, "Packet is not a session packet!");

        var packetData = ((SessionData)session).PacketData;

        Assert.IsInstanceOfType<ISessionData2025>(packetData, "Invalid session packet, expected F1 2025!");

        return (ISessionData2025)packetData;
    }

    #endregion // Private methods
}