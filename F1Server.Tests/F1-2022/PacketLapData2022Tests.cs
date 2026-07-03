using System.IO;
using System.Linq;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test lap data packet files
/// </summary>
[TestClass]
public class PacketLapData2022Tests
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
        var isFile = File.Exists(@"SampleData/F1-2022-LapData.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2022-LapData.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of lap data packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2022-LapData.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2022

    /// <summary>
    /// Check whether the given file has a correct lap data content
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2022IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.LapData;

        Assert.IsTrue(isCorrect, "Packet is not a lap data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2022 packet
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2022IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2022;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2022 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2022IsLapDataObject()
    {
        if (_packetData.PacketHeader != null)
        {
            var isCorrect = false;
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2022-LapData.packet"));

            if (lapData is LapData data && data.PacketData is ILapDataComplete dataComplete)
            {
                isCorrect = dataComplete.LapData is ILapData2022[];
            }

            Assert.IsTrue(isCorrect, "Packet is not a lap data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header!");
        }
    }

    /// <summary>
    /// Check if only one car is in session
    /// </summary>
    [TestMethod]
    public void PacketLapDataCarsOnLap2022Expected22()
    {
        if (_packetData.PacketHeader != null)
        {
            var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2022-LapData.packet"));

            if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2022[])
            {
                var cars = data.LapData.Count(l => l.IsEmpty == false);

                Assert.AreEqual(22, cars, "Number of cars is wrong!");
            }
            else
            {
                Assert.Fail("Invalid lap format, expected F1 2022!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header!");
        }
    }

    #endregion // Methods F1 2022
}