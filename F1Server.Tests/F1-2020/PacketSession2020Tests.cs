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
public class PacketSession2020Tests
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
        var is2020File = File.Exists(@"SampleData/F1-2020-Session.packet");

        if (is2020File)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2020-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session packets failed!");
        }
        else
        {
            Assert.IsTrue(is2020File, "File F1-2020-Session.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2020

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2020IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Session;

        Assert.IsTrue(isCorrect, "Packet is not a session data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2020 packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2020IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2020;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2020 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSession2020IsSessionDataObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length == ConstData.F12020SessionSize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData)
            {
                isCorrect = sessionData.PacketData is ISessionData2020;
            }

            Assert.IsTrue(isCorrect, "Packet is not a valid session data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check F1 session (2020)
    /// </summary>
    [TestMethod]
    public void PacketSessionActiveCars2020ExpectedFormulaOne()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020SessionSize + ConstData.F12020HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2020)
            {
                var isCorrect = sessionData.PacketData?.FormulaType == Formula.F1Modern;

                Assert.IsTrue(isCorrect, "Incorrect formula type!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check track (2020)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrack2020ExpectedMonza()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020SessionSize + ConstData.F12020HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2020)
            {
                var isCorrect = sessionData.PacketData?.TrackName.Equals("Monza");

                Assert.IsTrue(isCorrect, "Incorrect track id!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check track length (2020)
    /// </summary>
    [TestMethod]
    public void PacketSessionTrackLength2020ExpectedValue()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020SessionSize + ConstData.F12020HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2020)
            {
                var isCorrect = sessionData.PacketData?.TrackLength == 5798;

                Assert.IsTrue(isCorrect, "Incorrect track id!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    /// <summary>
    /// Check safety car status (2020)
    /// </summary>
    [TestMethod]
    public void PacketSessionSafetyCar2020ExpectedNo()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12020SessionSize + ConstData.F12020HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2020)
            {
                var isCorrect = sessionData.PacketData?.SafetyCar == SafetyCarStatus.NoSafetyCar;

                Assert.IsTrue(isCorrect, "Incorrect safety car status!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header or content!");
        }
    }

    #endregion // Methods F1 2020
}