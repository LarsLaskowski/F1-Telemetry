using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test session packet files (F1 2026)
/// </summary>
[TestClass]
public class PacketSession2026Tests
{
    #region Constants

    /// <summary>
    /// Absolute offset of the F1 2026 session core block (fields introduced with F1 2026) within the packet content
    /// </summary>
    private const int SessionCore2026Offset = ConstData.F12026HeaderSize + ConstData.F12025SessionSize;

    /// <summary>
    /// Relative offset of ActiveAeroTrackStatus within the F1 2026 session core block
    /// </summary>
    private const int ActiveAeroTrackStatusOffset = 0;

    /// <summary>
    /// Relative offset of StartReactionTime within the F1 2026 session core block
    /// </summary>
    private const int StartReactionTimeOffset = 164;

    /// <summary>
    /// Relative offset of DynamicRacingLineColourBlind within the F1 2026 session core block
    /// </summary>
    private const int DynamicRacingLineColourBlindOffset = 171;

    /// <summary>
    /// Relative offset of RecurringRewindPrompt within the F1 2026 session core block
    /// </summary>
    private const int RecurringRewindPromptOffset = 172;

    #endregion // Constants

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
        var isFile = File.Exists(@"SampleData/F1-2026-MelbourneQ3-Session.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2026-MelbourneQ3-Session.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-MelbourneQ3-Session.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Creates a synthetic F1 2026 session packet with distinguishable, non-default values for the new
    /// F1 2026 fields, so their parser offsets can be verified without relying on the up-converted 2025 sample
    /// </summary>
    /// <returns>Packet header and content of the synthetic packet</returns>
    private static (PacketHeader Header, byte[] Content) CreateSyntheticSession2026Packet()
    {
        var content = new byte[ConstData.F12026HeaderSize + ConstData.F12026SessionSize];

        content[0] = (byte)(2026 & 0xFF);
        content[1] = (byte)((2026 >> 8) & 0xFF);

        content[SessionCore2026Offset + ActiveAeroTrackStatusOffset] = 1;

        BitConverter.GetBytes(0.5234f).CopyTo(content, SessionCore2026Offset + StartReactionTimeOffset);

        content[SessionCore2026Offset + DynamicRacingLineColourBlindOffset] = 2;

        content[SessionCore2026Offset + RecurringRewindPromptOffset] = 1;

        var receivedData = new ReceivedPacketData();

        receivedData.SetRawData(content);

        Assert.IsNotNull(receivedData.PacketHeader, "Synthetic F1 2026 session header could not be created!");

        return (receivedData.PacketHeader, content);
    }

    #endregion // Static methods

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file has a correct session data content
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Session;

        Assert.IsTrue(isCorrect, "Packet is not a session packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet");
    }

    /// <summary>
    /// Check whether the analyzer constructs a F1 2026 session data object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheck2026IsSessionDataObject()
    {
        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader!, _packetContent);

        var isCorrect = session is SessionData data && data.PacketData is ISessionData2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 session data object!");
    }

    /// <summary>
    /// Check track id and name (2026) - the Melbourne race capture maps to track id 0
    /// </summary>
    [TestMethod]
    public void PacketSessionTrack2026ExpectedValue()
    {
        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader!, _packetContent);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.AreEqual((short)0, sessionData.TrackId, "Incorrect track id!");
            Assert.AreEqual("Melbourne", sessionData.TrackName, "Incorrect track name!");
        }
        else
        {
            Assert.Fail("Invalid session format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the new active aero track status (2026)
    /// </summary>
    [TestMethod]
    public void PacketSessionActiveAeroTrackStatus2026ExpectedValue()
    {
        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader!, _packetContent);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.AreEqual(0, sessionData.ActiveAeroTrackStatus, "Incorrect active aero track status!");
        }
        else
        {
            Assert.Fail("Invalid session format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the new start reaction time (2026)
    /// </summary>
    [TestMethod]
    public void PacketSessionStartReactionTime2026ExpectedValue()
    {
        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader!, _packetContent);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.AreEqual(0, sessionData.StartReactionTime, "Incorrect start reaction time!");
        }
        else
        {
            Assert.Fail("Invalid session format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the new traction control assist (2026)
    /// </summary>
    [TestMethod]
    public void PacketSessionTractionControlAssist2026ExpectedValue()
    {
        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader!, _packetContent);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.AreEqual((ushort)2, sessionData.TractionControlAssist, "Incorrect traction control assist!");
        }
        else
        {
            Assert.Fail("Invalid session format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the new dynamic racing line colour blind mode (2026)
    /// </summary>
    [TestMethod]
    public void PacketSessionDynamicRacingLineColourBlind2026ExpectedValue()
    {
        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader!, _packetContent);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.AreEqual((ushort)0, sessionData.DynamicRacingLineColourBlind, "Incorrect dynamic racing line colour blind mode!");
        }
        else
        {
            Assert.Fail("Invalid session format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the new recurring rewind prompt flag (2026)
    /// </summary>
    [TestMethod]
    public void PacketSessionRecurringRewindPrompt2026ExpectedValue()
    {
        var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader!, _packetContent);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.IsFalse(sessionData.RecurringRewindPrompt, "Incorrect recurring rewind prompt flag!");
        }
        else
        {
            Assert.Fail("Invalid session format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check formula type (2026)
    /// </summary>
    [TestMethod]
    public void PacketSessionFormulaType2026ExpectedF12026()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12026SessionSize + ConstData.F12026HeaderSize)
        {
            var session = _packetAnalyzer.GetSessionData(_packetData.PacketHeader, _packetContent);

            if (session is SessionData sessionData && sessionData.PacketData is ISessionData2026)
            {
                var isCorrect = sessionData.PacketData?.FormulaType == Formula.F12026;

                Assert.IsTrue(isCorrect, "Invalid formula type!");
            }
            else
            {
                Assert.Fail("Invalid session packet, expected F1 2026!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2026 packet header or content!");
        }
    }

    /// <summary>
    /// A non-default active aero track status must round-trip through the parser at its documented offset,
    /// so a wrong offset does not stay hidden behind the placeholder value 0 in the up-converted 2025 sample
    /// </summary>
    [TestMethod]
    public void PacketSessionActiveAeroTrackStatus2026SyntheticValueRoundTrips()
    {
        var (header, content) = CreateSyntheticSession2026Packet();

        var session = _packetAnalyzer.GetSessionData(header, content);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.AreEqual((ushort)1, sessionData.ActiveAeroTrackStatus, "ActiveAeroTrackStatus must decode the synthetic partial-status value!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2026 session packet did not produce an ISessionData2026 object!");
        }
    }

    /// <summary>
    /// A non-default start reaction time must round-trip through the parser at its documented offset,
    /// so a wrong offset does not stay hidden behind the placeholder value 0 in the up-converted 2025 sample
    /// </summary>
    [TestMethod]
    public void PacketSessionStartReactionTime2026SyntheticValueRoundTrips()
    {
        var (header, content) = CreateSyntheticSession2026Packet();

        var session = _packetAnalyzer.GetSessionData(header, content);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.AreEqual(0.5234f, sessionData.StartReactionTime, 0.0001f, "StartReactionTime must decode the synthetic reaction time value!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2026 session packet did not produce an ISessionData2026 object!");
        }
    }

    /// <summary>
    /// A non-default dynamic racing line colour blind mode must round-trip through the parser at its documented
    /// offset, so a wrong offset does not stay hidden behind the placeholder value 0 in the up-converted 2025 sample
    /// </summary>
    [TestMethod]
    public void PacketSessionDynamicRacingLineColourBlind2026SyntheticValueRoundTrips()
    {
        var (header, content) = CreateSyntheticSession2026Packet();

        var session = _packetAnalyzer.GetSessionData(header, content);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.AreEqual((ushort)2, sessionData.DynamicRacingLineColourBlind, "DynamicRacingLineColourBlind must decode the synthetic Deuteranopia value!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2026 session packet did not produce an ISessionData2026 object!");
        }
    }

    /// <summary>
    /// A non-default recurring rewind prompt flag must round-trip through the parser at its documented offset,
    /// so a wrong offset does not stay hidden behind the placeholder value false in the up-converted 2025 sample
    /// </summary>
    [TestMethod]
    public void PacketSessionRecurringRewindPrompt2026SyntheticValueRoundTrips()
    {
        var (header, content) = CreateSyntheticSession2026Packet();

        var session = _packetAnalyzer.GetSessionData(header, content);

        if (session is SessionData data && data.PacketData is ISessionData2026 sessionData)
        {
            Assert.IsTrue(sessionData.RecurringRewindPrompt, "RecurringRewindPrompt must decode the synthetic true value!");
        }
        else
        {
            Assert.Fail("Synthetic F1 2026 session packet did not produce an ISessionData2026 object!");
        }
    }

    #endregion // Methods F1 2026
}