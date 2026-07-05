using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test session history packet files
/// </summary>
[TestClass]
public class PacketSessionHistory2025Tests
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
    public static void PacketSessionHistoryInit(TestContext testContext)
    {
        var is2025File = File.Exists(@"SampleData/F1-2025-SessionHistory.packet");

        if (is2025File)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2025-SessionHistory.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session history packets failed!");
        }
        else
        {
            Assert.IsTrue(is2025File, "File F1-2025-SessionHistory.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2025

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.SessionHistory;

        Assert.IsTrue(isCorrect, "Packet is not a session history packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025IsSessionHistoryObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData)
            {
                isCorrect = sessionHistoryData.PacketData is not null;
            }

            Assert.IsTrue(isCorrect, "Packet is not a session history packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check best lap number in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025BestLap()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.BestLapNumber == 16;
            }

            Assert.IsTrue(isCorrect, "Best lap number is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check car index in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025BestLapCarIndex()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.CarIndex == 11;
            }

            Assert.IsTrue(isCorrect, "Car index of best lap is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check number of laps in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025NumberOfLaps()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.NumberOfLaps == 28;
            }

            Assert.IsTrue(isCorrect, "Number of laps is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check lap time time from lap 1 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025LapTime()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[0].LapTime == 116270;
            }

            Assert.IsTrue(isCorrect, "Lap time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check sector 1 time from lap 3 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025Sector1Time()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[0].Sector1Time == 35013;
            }

            Assert.IsTrue(isCorrect, "Sector 1 time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check sector 3 time from lap 4 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025Sector3Time()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[0].Sector3Time == 46644;
            }

            Assert.IsTrue(isCorrect, "Sector 3 time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check number of tyre stints in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025TyreStints()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.NumberOfTyreStints == 2;
            }

            Assert.IsTrue(isCorrect, "Number of tyre stints is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check first tyre stint actual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025FirstTyreStintActual()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.TyreStintHistory[0].TyreActualCompound == 18;
            }

            Assert.IsTrue(isCorrect, "Actual compound of first tyre stint is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check second tyre stint visual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025FirstTyreStintVisual()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                isCorrect = sessionHistoryData.PacketData.TyreStintHistory[0].TyreVisualCompound == 17;
            }

            Assert.IsTrue(isCorrect, "Visual compound of first tyre stint is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    /// <summary>
    /// Check first tyre stint visual compound mapping
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2025VisualCompoundMapper()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12025SessionHistorySize + ConstData.F12025HeaderSize)
        {
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);
            var visualCompound = VisualTyreCompound.Unknown;

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2025)
            {
                visualCompound = TyreCompoundMapper.MapVisualTyreCompoundToEnum(sessionHistoryData.PacketData.TyreStintHistory[0].TyreVisualCompound);
            }

            Assert.AreEqual(VisualTyreCompound.Medium, visualCompound, "Visual compound mapping is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2025 packet header or content!");
        }
    }

    #endregion // Methods F1 2025
}