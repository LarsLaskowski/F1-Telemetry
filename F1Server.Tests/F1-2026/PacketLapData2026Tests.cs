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
/// Class to test lap data packet files (F1 2026)
/// </summary>
[TestClass]
public class PacketLapData2026Tests
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
    public static void PacketLapDataInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2026-LapData.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2026-LapData.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of lap data packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2026-LapData.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2026

    /// <summary>
    /// Check whether the given file has a correct lap data content
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2026IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.LapData;

        Assert.IsTrue(isCorrect, "Packet is not a lap data packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2026 packet
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2026IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2026;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2026 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketLapDataCheckLapData2026IsLapDataObject()
    {
        var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader!, _packetContent);

        var isCorrect = lapData is LapData data && data.PacketData is ILapDataComplete dataComplete && dataComplete.LapData is ILapData2026[];

        Assert.IsTrue(isCorrect, "Packet is not a lap data packet");
    }

    /// <summary>
    /// Check the array now holds 24 cars (F1 2026)
    /// </summary>
    [TestMethod]
    public void PacketLapDataCount2026Expected24Cars()
    {
        var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader!, _packetContent);

        if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2026[])
        {
            Assert.HasCount(24, data.LapData, "Number of cars in the array is wrong!");
        }
        else
        {
            Assert.Fail("Invalid lap format, expected F1 2026!");
        }
    }

    /// <summary>
    /// Check the correct number of active cars (matches the converted 2025 capture)
    /// </summary>
    [TestMethod]
    public void PacketLapDataCarsOnLap2026Expected3()
    {
        var lapData = _packetAnalyzer.GetLapData(_packetData.PacketHeader!, _packetContent);

        if (lapData is LapData lapInfo && lapInfo.PacketData is ILapDataComplete data && data.LapData is ILapData2026[])
        {
            var cars = data.LapData.Count(l => l.IsEmpty == false);

            Assert.AreEqual(3, cars, "Number of cars is wrong!");
        }
        else
        {
            Assert.Fail("Invalid lap format, expected F1 2026!");
        }
    }

    #endregion // Methods F1 2026
}