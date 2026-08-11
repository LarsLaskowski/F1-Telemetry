using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests;

/// <summary>
/// Class to test lap data packet files
/// </summary>
[TestClass]
public class PacketLapData2024Tests
{
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
    public static void PacketLapDataInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2024-LapData.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2024-LapData.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of lap data packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2024-LapData.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2024

    /// <summary>
    /// Check whether the given file has a correct lap data content
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2024IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.LapData;

        Assert.IsTrue(isCorrect, "Packet is not a lap data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2024 packet
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2024IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2024;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2024 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2024IsLapDataObject()
    {
        if (_packetData.PacketHeader != null)
        {
            var isCorrect = false;
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2024-LapData.packet"));

            if (lapData is LapData data && data.PacketData is ILapDataComplete dataComplete)
            {
                isCorrect = dataComplete.LapData is ILapData2024[];
            }

            Assert.IsTrue(isCorrect, "Packet is not a lap data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header!");
        }
    }

    /// <summary>
    /// Check the correct number of cars
    /// </summary>
    [TestMethod]
    public void PacketLapDataCarsOnLap2024Expected15()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2024-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2024[])
            {
                var cars = data.LapData.Count(l => l.IsEmpty == false);

                Assert.AreEqual(15, cars, "Number of cars is wrong!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header!");
        }
    }

    /// <summary>
    /// Check delta to car in front and race leader whole minute parts (2024)
    /// </summary>
    [TestMethod]
    public void PacketLapDataDeltaMinutes2024ExpectedZero()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2024-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2024[] cars)
            {
                Assert.AreEqual((ushort)0, cars[0].DeltaToCarInFrontMinutes, "Incorrect delta to car in front whole minute part!");
                Assert.AreEqual((ushort)0, cars[0].DeltaToRaceLeaderMinutes, "Incorrect delta to race leader whole minute part!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header!");
        }
    }

    /// <summary>
    /// Check speed trap fastest speed and lap (2024)
    /// </summary>
    [TestMethod]
    public void PacketLapDataSpeedTrapFastest2024ExpectedValue()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2024-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2024[] cars)
            {
                Assert.AreEqual(316.6052f, cars[0].SpeedTrapFastestSpeed, 0.001f, "Incorrect speed trap fastest speed!");
                Assert.AreEqual((ushort)1, cars[0].SpeedTrapFastestLap, "Incorrect speed trap fastest lap!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header!");
        }
    }

    /// <summary>
    /// Check speed trap fastest lap value for a car that never triggered the speed trap (2024)
    /// </summary>
    [TestMethod]
    public void PacketLapDataSpeedTrapFastestLapNotSet2024ExpectedValue()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2024-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2024[] cars)
            {
                Assert.AreEqual(0f, cars[1].SpeedTrapFastestSpeed, 0.001f, "Incorrect speed trap fastest speed!");
                Assert.AreEqual((ushort)255, cars[1].SpeedTrapFastestLap, "Speed trap fastest lap must decode the documented not-set value 255!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header!");
        }
    }

    #endregion // Methods F1 2024
}