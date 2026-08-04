using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test time trial packet files (F1 2026). F1 2026 widened the team id of a time trial
/// data set from uint8 to uint16, which moves every field behind it, so these tests pin the
/// offsets of all three data sets
/// </summary>
[TestClass]
public class PacketTimeTrial2026Tests
{
    #region Constants

    private const string SampleFile = @"SampleData/F1-2026-TimeTrial.packet";

    #endregion // Constants

    #region Fields

    private static PacketAnalyzer _packetAnalyzer;
    private static ReceivedPacketData _packetData;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketTimeTrialInit(TestContext testContext)
    {
        var isFile = File.Exists(SampleFile);

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(SampleFile);

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of time trial packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-TimeTrial.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Reads the time trial data of the sample packet
    /// </summary>
    /// <returns>Time trial data of the sample packet</returns>
    private static ITimeTrialDataBase GetTimeTrialData()
    {
        var packetHeader = _packetData.PacketHeader;

        Assert.IsNotNull(packetHeader, "Sample packet has no header!");

        var timeTrialData = _packetAnalyzer.GetTimeTrialData(packetHeader, File.ReadAllBytes(SampleFile));

        Assert.IsInstanceOfType<TimeTrialData>(timeTrialData, "Packet was not transformed into time trial data!");

        var packetData = ((TimeTrialData)timeTrialData).PacketData;

        Assert.IsNotNull(packetData, "Time trial packet contains no data!");

        return packetData;
    }

    #endregion // Static methods

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file is a time trial packet
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialCheckTimeTrial2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.TimeTrial;

        Assert.IsTrue(isCorrect, "Packet is not a time trial packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialCheckTimeTrial2026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet!");
    }

    /// <summary>
    /// The F1 2026 time trial payload holds three data sets of 25 bytes, because the team id
    /// grew to a uint16
    /// </summary>
    [TestMethod]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MSTEST0032:Assertion condition is always true", Justification = "Valid test. If someone changes the size constant, the test will fail")]
    public void PacketTimeTrial2026PayloadSizeIs75Bytes()
    {
        Assert.AreEqual(75, ConstData.F12026TimeTrialSize, "The F1 2026 time trial payload must be 75 bytes!");
    }

    /// <summary>
    /// Check that the analyzer constructs the F1 2026 time trial data object
    /// </summary>
    [TestMethod]
    public void PacketTimeTrial2026IsTimeTrialDataObject()
    {
        Assert.IsInstanceOfType<TimeTrialData2026>(GetTimeTrialData(), "Time trial data is not a F1 2026 object!");
    }

    /// <summary>
    /// Check that the team id is read as a full uint16. A team id above 255 can only be decoded
    /// when the wider field is honoured, so this fails if the parser falls back to a single byte
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialRivalTeamId2026IsNotTruncatedToByte()
    {
        var dataSet = GetTimeTrialData().RivalDataSet;

        Assert.AreEqual(42, dataSet.TeamId, "Team id was not read as a full uint16 value!");
    }

    /// <summary>
    /// Check the player session best data set against the values of the sample packet
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialPlayerSessionBest2026ExpectedValues()
    {
        var dataSet = GetTimeTrialData().PlayerSessionBestDataSet;

        Assert.AreEqual(19, dataSet.CarIndex, "Wrong car index in the player session best data set!");
        Assert.AreEqual(9, dataSet.TeamId, "Wrong team id in the player session best data set!");
        Assert.AreEqual(91234u, dataSet.LapTime, "Wrong lap time in the player session best data set!");
        Assert.AreEqual(30111u, dataSet.Sector1Time, "Wrong sector 1 time in the player session best data set!");
        Assert.AreEqual(31222u, dataSet.Sector2Time, "Wrong sector 2 time in the player session best data set!");
        Assert.AreEqual(29901u, dataSet.Sector3Time, "Wrong sector 3 time in the player session best data set!");
    }

    /// <summary>
    /// Check the personal best data set against the values of the sample packet. The second data
    /// set starts 25 bytes into the payload, so a wrong team id width shows up here first
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialPersonalBest2026ExpectedValues()
    {
        var dataSet = GetTimeTrialData().PersonalBestDataSet;

        Assert.AreEqual(19, dataSet.CarIndex, "Wrong car index in the personal best data set!");
        Assert.AreEqual(9, dataSet.TeamId, "Wrong team id in the personal best data set!");
        Assert.AreEqual(90567u, dataSet.LapTime, "Wrong lap time in the personal best data set!");
        Assert.AreEqual(29876u, dataSet.Sector1Time, "Wrong sector 1 time in the personal best data set!");
    }

    /// <summary>
    /// Check the rival data set against the values of the sample packet. It starts 50 bytes into
    /// the payload and therefore accumulates the offset error of both preceding data sets
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialRival2026ExpectedValues()
    {
        var dataSet = GetTimeTrialData().RivalDataSet;

        Assert.AreEqual(7, dataSet.CarIndex, "Wrong car index in the rival data set!");
        Assert.AreEqual(89999u, dataSet.LapTime, "Wrong lap time in the rival data set!");
        Assert.AreEqual(29500u, dataSet.Sector1Time, "Wrong sector 1 time in the rival data set!");
        Assert.AreEqual(30800u, dataSet.Sector2Time, "Wrong sector 2 time in the rival data set!");
        Assert.AreEqual(29699u, dataSet.Sector3Time, "Wrong sector 3 time in the rival data set!");
    }

    /// <summary>
    /// Check the assist flags of the player session best data set, they are the last fields of a
    /// data set and therefore fail first when the layout drifts
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialPlayerSessionBestAssists2026ExpectedValues()
    {
        var dataSet = GetTimeTrialData().PlayerSessionBestDataSet;

        Assert.AreEqual(TractionControl.Medium, dataSet.TractionControl, "Wrong traction control in the player session best data set!");
        Assert.AreEqual(GearboxAssist.Manual, dataSet.GearboxAssist, "Wrong gearbox assist in the player session best data set!");
        Assert.IsTrue(dataSet.AntiLockBrakes, "Anti lock brakes must be active in the player session best data set!");
        Assert.IsTrue(dataSet.IsRealisticCarPerformance, "Car performance must be realistic in the player session best data set!");
        Assert.IsTrue(dataSet.IsCustomSetup, "Custom setup must be active in the player session best data set!");
        Assert.IsTrue(dataSet.IsValid, "The player session best lap must be valid!");
    }

    #endregion // Methods F1 2026
}