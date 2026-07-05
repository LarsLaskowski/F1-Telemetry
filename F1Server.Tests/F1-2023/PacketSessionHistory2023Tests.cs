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
public class PacketSessionHistory2023Tests
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
        var is2023File = File.Exists(@"SampleData/F1-2023-SessionHistory.packet");

        if (is2023File)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2023-SessionHistory.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session history packets failed!");
        }
        else
        {
            Assert.IsTrue(is2023File, "File F1-2023-SessionHistory.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2023

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.SessionHistory;

        Assert.IsTrue(isCorrect, "Packet is not a session history packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023IsSessionHistoryObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
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
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check best lap number in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023BestLap()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.BestLapNumber == 20;
            }

            Assert.IsTrue(isCorrect, "Best lap number is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check car index in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023BestLapCarIndex()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.CarIndex == 8;
            }

            Assert.IsTrue(isCorrect, "Car index of best lap is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check number of laps in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023NumberOfLaps()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.NumberOfLaps == 20;
            }

            Assert.IsTrue(isCorrect, "Number of laps is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check lap time time from lap 1 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023LapTime()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[0].LapTime == 100409;
            }

            Assert.IsTrue(isCorrect, "Lap time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check sector 1 time from lap 3 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023Sector1Time()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[2].Sector1Time == 34353;
            }

            Assert.IsTrue(isCorrect, "Sector 1 time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check sector 3 time from lap 4 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023Sector3Time()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[3].Sector3Time == 27677;
            }

            Assert.IsTrue(isCorrect, "Sector 3 time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check number of tyre stints in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023TyreStints()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.NumberOfTyreStints == 3;
            }

            Assert.IsTrue(isCorrect, "Number of tyre stints is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check first tyre stint actual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023FirstTyreStintActual()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.TyreStintHistory[0].TyreActualCompound == 17;
            }

            Assert.IsTrue(isCorrect, "Actual compound of first tyre stint is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check second tyre stint visual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023FirstTyreStintVisual()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                isCorrect = sessionHistoryData.PacketData.TyreStintHistory[1].TyreVisualCompound == 17;
            }

            Assert.IsTrue(isCorrect, "Visual compound of first tyre stint is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    /// <summary>
    /// Check first tyre stint visual compound mapping
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2023VisualCompoundMapper()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12023SessionHistorySize + ConstData.F12023HeaderSize)
        {
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);
            var visualCompound = VisualTyreCompound.Unknown;

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2023)
            {
                visualCompound = TyreCompoundMapper.MapVisualTyreCompoundToEnum(sessionHistoryData.PacketData.TyreStintHistory[1].TyreVisualCompound);
            }

            Assert.AreEqual(VisualTyreCompound.Medium, visualCompound, "Visual compound mapping is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2023 packet header or content!");
        }
    }

    #endregion // Methods F1 2023
}