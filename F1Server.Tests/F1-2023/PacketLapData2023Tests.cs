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
public class PacketLapData2023Tests
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
        var isFile = File.Exists(@"SampleData/F1-2023-LapData.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2023-LapData.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of lap data packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2023-LapData.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2023

    /// <summary>
    /// Check whether the given file has a correct lap data content
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2023IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.LapData;

        Assert.IsTrue(isCorrect, "Packet is not a lap data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2023 packet
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2023IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2023;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2023 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2023IsLapDataObject()
    {
        if (_packetData.PacketHeader != null)
        {
            var isCorrect = false;
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2023-LapData.packet"));

            if (lapData is LapData data && data.PacketData is ILapDataComplete dataComplete)
            {
                isCorrect = dataComplete.LapData is ILapData2023[];
            }

            Assert.IsTrue(isCorrect, "Packet is not a lap data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header!");
        }
    }

    /// <summary>
    /// Check the correct number of cars
    /// </summary>
    [TestMethod]
    public void PacketLapDataCarsOnLap2023Expected15()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2023-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2023[])
            {
                var cars = data.LapData.Count(l => l.IsEmpty == false);

                Assert.AreEqual(15, cars, "Number of cars is wrong!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header!");
        }
    }

    /// <summary>
    /// Check sector 1 and 2 whole minute parts (2023)
    /// </summary>
    [TestMethod]
    public void PacketLapDataSectorTimeMinutes2023ExpectedZero()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2023-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2023[] cars)
            {
                Assert.AreEqual((ushort)0, cars[0].Sector1TimeMinutes, "Incorrect sector 1 whole minutes part!");
                Assert.AreEqual((ushort)0, cars[0].Sector2TimeMinutes, "Incorrect sector 2 whole minutes part!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header!");
        }
    }

    /// <summary>
    /// Check delta to car in front and race leader (2023)
    /// </summary>
    [TestMethod]
    public void PacketLapDataDeltas2023ExpectedZero()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2023-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2023[] cars)
            {
                Assert.AreEqual((ushort)0, cars[0].DeltaToCarInFront, "Incorrect delta to car in front!");
                Assert.AreEqual((ushort)0, cars[0].DeltaToRaceLeader, "Incorrect delta to race leader!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header!");
        }
    }

    /// <summary>
    /// Check accumulated corner cutting warnings (2023)
    /// </summary>
    [TestMethod]
    public void PacketLapDataCornerCuttingWarnings2023ExpectedZero()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2023-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2023[] cars)
            {
                Assert.AreEqual((ushort)0, cars[0].CornerCuttingWarnings, "Incorrect accumulated corner cutting warnings!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2023!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header!");
        }
    }

    #endregion // Methods F1 2023
}