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
public class PacketSession2021Tests
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
        var isFile = File.Exists(@"SampleData/F1-2021-Session.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2021-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2021-Session.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2021

    /// <summary>
    /// Check whether the given file is a session packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2021IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Session;

        Assert.IsTrue(isCorrect, "Packet is not a session packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2021 packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2021IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2021;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2021 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2021IsSessionObject()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021SessionSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var isCorrect = false;
        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

        if (session is SessionData sessionData)
        {
            isCorrect = sessionData.PacketData is ISessionData2021;
        }

        Assert.IsTrue(isCorrect, "Packet is not a session packet");
    }

    /// <summary>
    /// Check ai difficulty (2021)
    /// </summary>
    [TestMethod]
    public void PacketSessionAiDifficulty2021ExpectedSixty()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021SessionSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

        if (session is SessionData sessionData && sessionData.PacketData is ISessionData2021 sessionData2021)
        {
            var isCorrect = sessionData2021.AiDifficulty == 60;

            Assert.IsTrue(isCorrect, "Incorrect ai difficulty!");
        }
        else
        {
            Assert.Fail("Invalid session packet, expected F1 2021!");
        }
    }

    /// <summary>
    /// Check track (2021)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrack2021ExpectedAbuDhabi()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021SessionSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

        if (session is SessionData sessionData && sessionData.PacketData is ISessionData2021)
        {
            var isCorrect = sessionData.PacketData?.TrackName.Equals("Abu Dhabi");

            Assert.IsTrue(isCorrect, "Invalid track!");
        }
        else
        {
            Assert.Fail("Invalid session packet, expected F1 2021!");
        }
    }

    /// <summary>
    /// Check track length (2021)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrackLength2021ExpectedValue()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021SessionSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

        if (session is SessionData sessionData && sessionData.PacketData is ISessionData2021)
        {
            var isCorrect = sessionData.PacketData?.TrackLength == 5547;

            Assert.IsTrue(isCorrect, "Invalid track length!");
        }
        else
        {
            Assert.Fail("Invalid session packet, expected F1 2021!");
        }
    }

    /// <summary>
    /// Check formula type (2021)
    /// </summary>
    [TestMethod]
    public void PacketSessionFormulaType2021ExpectedF2()
    {
        Assert.IsNotNull(_packetData.PacketHeader, "Missing packet header!");
        Assert.IsTrue(_packetContent?.Length >= ConstData.F12021SessionSize + ConstData.F12021HeaderSize, "Packet content too short!");

        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

        if (session is SessionData sessionData && sessionData.PacketData is ISessionData2021)
        {
            var isCorrect = sessionData.PacketData?.FormulaType == Formula.F2;

            Assert.IsTrue(isCorrect, "Invalid formula type!");
        }
        else
        {
            Assert.Fail("Invalid session packet, expected F1 2021!");
        }
    }

    #endregion // Methods F1 2021
}