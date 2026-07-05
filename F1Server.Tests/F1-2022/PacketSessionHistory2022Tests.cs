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
public class PacketSessionHistory2022Tests
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
        var is2022File = File.Exists(@"SampleData/F1-2022-SessionHistory.packet");

        if (is2022File)
        {
            _packetAnalyzer = new PacketAnalyzer();

            _packetContent = File.ReadAllBytes(@"SampleData/F1-2022-SessionHistory.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(_packetContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of session history packets failed!");
        }
        else
        {
            Assert.IsTrue(is2022File, "File F1-2022-SessionHistory.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2022

    /// <summary>
    /// Check whether the given file has a correct participants data content
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.SessionHistory;

        Assert.IsTrue(isCorrect, "Packet is not a session history packet!");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022IsSessionHistoryObject()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
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
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check best lap number in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022BestLap()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.BestLapNumber == 14;
            }

            Assert.IsTrue(isCorrect, "Best lap number is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check car index in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022BestLapCarIndex()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.CarIndex == 7;
            }

            Assert.IsTrue(isCorrect, "Car index of best lap is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check number of laps in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022NumberOfLaps()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.NumberOfLaps == 15;
            }

            Assert.IsTrue(isCorrect, "Number of laps is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check lap time time from lap 1 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022LapTime()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[0].LapTime == 102927;
            }

            Assert.IsTrue(isCorrect, "Lap time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check sector 1 time from lap 3 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022Sector1Time()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[2].Sector1Time == 30601;
            }

            Assert.IsTrue(isCorrect, "Sector 1 time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check sector 3 time from lap 4 in lap history data
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022Sector3Time()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.LapHistory[3].Sector3Time == 23789;
            }

            Assert.IsTrue(isCorrect, "Sector 3 time is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check number of tyre stints in session history packet
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022TyreStints()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.NumberOfTyreStints == 2;
            }

            Assert.IsTrue(isCorrect, "Number of tyre stints is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check first tyre stint actual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022FirstTyreStintActual()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.TyreStintHistory[0].TyreActualCompound == 18;
            }

            Assert.IsTrue(isCorrect, "Actual compound of first tyre stint is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check first tyre stint visual compound
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022FirstTyreStintVisual()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var isCorrect = false;
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                isCorrect = sessionHistoryData.PacketData.TyreStintHistory[0].TyreVisualCompound == 16;
            }

            Assert.IsTrue(isCorrect, "Visual compound of first tyre stint is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    /// <summary>
    /// Check first tyre stint visual compound mapping
    /// </summary>
    [TestMethod]
    public void PacketSessionCheckSessionHistory2022VisualCompoundMapper()
    {
        if (_packetData.PacketHeader != null && _packetContent?.Length >= ConstData.F12022SessionHistorySize + ConstData.F12020HeaderSize)
        {
            var sessionHistory = _packetAnalyzer.GetSessionHistoryData(_packetData.PacketHeader, _packetContent);
            var visualCompound = VisualTyreCompound.Unknown;

            if (sessionHistory is SessionHistoryData sessionHistoryData && sessionHistoryData.PacketData is SessionHistoryData2022)
            {
                visualCompound = TyreCompoundMapper.MapVisualTyreCompoundToEnum(sessionHistoryData.PacketData.TyreStintHistory[0].TyreVisualCompound);
            }

            Assert.AreEqual(VisualTyreCompound.Soft, visualCompound, "Visual compound mapping is invalid!");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2022 packet header or content!");
        }
    }

    #endregion // Methods F1 2022
}