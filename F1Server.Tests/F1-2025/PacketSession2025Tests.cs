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
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Session;

        Assert.IsTrue(isCorrect, "Packet is not a session packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2025 packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2025IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2025;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2025 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2025IsSessionObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionSize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData)
            {
                isCorrect = sessionData.PacketData is ISessionData2025;
            }

            Assert.IsTrue(isCorrect, "Packet is not a session packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check ai difficulty (2025)
    /// </summary>
    [TestMethod]
    public void PacketSessionAiDifficulty2025ExpectedThirtyOne()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionSize + ConstData.F12025HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2025 sessionData2025)
            {
                var isCorrect = sessionData2025.AiDifficulty == 60;

                Assert.IsTrue(isCorrect, "Incorrect ai difficulty!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check track (2025)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrack2025ExpectedBahrain()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionSize + ConstData.F12025HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2025)
            {
                var isCorrect = sessionData.PacketData?.TrackName.Equals("Suzuka");

                Assert.IsTrue(isCorrect, "Invalid track id!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check track length (2025)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrackLength2025ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionSize + ConstData.F12025HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2025)
            {
                var isCorrect = sessionData.PacketData?.TrackLength == 5809;

                Assert.IsTrue(isCorrect, "Invalid track length!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check formula type (2025)
    /// </summary>
    [TestMethod]
    public void PacketSessionFormulaType2025ExpectedF1Modern()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionSize + ConstData.F12025HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2025)
            {
                var isCorrect = sessionData.PacketData?.FormulaType == Formula.F1Modern;

                Assert.IsTrue(isCorrect, "Invalid formula type!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    #endregion // Methods F1 2025
}