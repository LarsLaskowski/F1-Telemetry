using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test session packet files
/// </summary>
[TestClass]
public class PacketSession2024Tests
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
        var isFile = File.Exists(@"SampleData/F1-2024-Session.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2024-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2024-Session.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2024

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2024IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Session;

        Assert.IsTrue(isCorrect, "Packet is not a session packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2024 packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2024IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2024;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2024 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2024IsSessionObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024SessionSize + ConstData.F12024HeaderSize)
        {
            var isCorrect = false;
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData)
            {
                isCorrect = sessionData.PacketData is ISessionData2024;
            }

            Assert.IsTrue(isCorrect, "Packet is not a session packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check ai difficulty (2024)
    /// </summary>
    [TestMethod]
    public void PacketSessionAiDifficulty2024ExpectedThirtyOne()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024SessionSize + ConstData.F12024HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2024 sessionData2024)
            {
                var isCorrect = sessionData2024.AiDifficulty == 31;

                Assert.IsTrue(isCorrect, "Incorrect ai difficulty!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check track (2024)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrack2024ExpectedBahrain()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024SessionSize + ConstData.F12024HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2024)
            {
                var isCorrect = sessionData.PacketData?.TrackName.Equals("Sakhir (Bahrain)");

                Assert.IsTrue(isCorrect, "Invalid track id!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check track length (2024)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrackLength2024ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024SessionSize + ConstData.F12024HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2024)
            {
                var isCorrect = sessionData.PacketData?.TrackLength == 5408;

                Assert.IsTrue(isCorrect, "Invalid track length!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    /// <summary>
    /// Check formula type (2024)
    /// </summary>
    [TestMethod]
    public void PacketSessionFormulaType2024ExpectedF2()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12024SessionSize + ConstData.F12024HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2024)
            {
                var isCorrect = sessionData.PacketData?.FormulaType == Formula.F2;

                Assert.IsTrue(isCorrect, "Invalid formula type!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header or content!");
        }
    }

    #endregion // Methods F1 2024
}