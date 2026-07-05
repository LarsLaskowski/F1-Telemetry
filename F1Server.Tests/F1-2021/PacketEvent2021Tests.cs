using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test event packets from F1 2021
/// </summary>
[TestClass]
public class PacketEvent2021Tests
{
    #region Fields

    private static PacketAnalyzer _packetAnalyzer;
    private static ReceivedPacketData _packetDataTopSpeed;
    private static ReceivedPacketData _packetDataFlashback;
    private static ReceivedPacketData _packetDataPenalty;

    #endregion // Fields

    #region Initializer/Cleanup

    /// <summary>
    /// Class initializer
    /// </summary>
    /// <param name="testContext">Context</param>
    [ClassInitialize]
    public static void PacketEvent2021Init(TestContext testContext)
    {
        var file1 = File.Exists(@"SampleData/F1-2021-Event-TopSpeed.packet");
        var file2 = File.Exists(@"SampleData/F1-2021-Event-Flashback.packet");
        var file3 = File.Exists(@"SampleData/F1-2021-Event-Penalty.packet");

        if (file1 && file2 && file3)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2021-Event-TopSpeed.packet");

            _packetDataTopSpeed = new ReceivedPacketData();

            _packetDataTopSpeed.SetRawData(fileContent);

            fileContent = File.ReadAllBytes(@"SampleData/F1-2021-Event-Flashback.packet");

            _packetDataFlashback = new ReceivedPacketData();

            _packetDataFlashback.SetRawData(fileContent);

            fileContent = File.ReadAllBytes(@"SampleData/F1-2021-Event-Penalty.packet");

            _packetDataPenalty = new ReceivedPacketData();

            _packetDataPenalty.SetRawData(fileContent);

            var isCorrect = _packetDataTopSpeed.PacketHeader != null && _packetDataFlashback.PacketHeader != null && _packetDataPenalty.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of event packets failed!");
        }
        else
        {
            Assert.IsTrue(file1, "File F1-2021-Event-TopSpeed.packet is missing!");
            Assert.IsTrue(file2, "File F1-2021-Event-Flashback.packet is missing!");
            Assert.IsTrue(file3, "File F1-2021-Event-Penalty.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods

    /// <summary>
    /// Check correct packet type
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsCorrectPacketType()
    {
        var isCorrect = _packetDataTopSpeed.PacketHeader?.PacketType == PacketTypes.Event;

        Assert.IsTrue(isCorrect, "Packet is not a event packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2020 packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsCorrectGameVersion()
    {
        var isCorrect = _packetDataFlashback.PacketHeader?.GameVersion == 2021;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2021 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketEvent2021CheckEventDataIsEventDataObject()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var isCorrect = false;
            var eventData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Penalty.packet"));

            if (eventData is EventData data && data.PacketData is EventData2021)
            {
                isCorrect = true;
            }

            Assert.IsTrue(isCorrect, "Packet is not a event data packet");
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsTopSpeedPacketExpectedTrue()
    {
        if (_packetDataTopSpeed.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataTopSpeed.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-TopSpeed.packet"));

            if (packetData is EventData eventData)
            {
                Assert.IsTrue(eventData.EventCode.Equals("SPTP"), "No top speed event packet!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataTopSpeed.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsTopSpeedExpectedValue()
    {
        if (_packetDataTopSpeed.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataTopSpeed.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(246.577454F, eventDetails.TopSpeed, "Incorrect top speed value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataTopSpeed.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsTopSpeedVehicleExpectedFourteen()
    {
        if (_packetDataTopSpeed.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataTopSpeed.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual((ushort)14, eventDetails.VehicleIndex, "Incorrect vehicle value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataTopSpeed.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsFlashbackPacketExpectedTrue()
    {
        if (_packetDataFlashback.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataFlashback.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Flashback.packet"));

            if (packetData is EventData eventData)
            {
                Assert.IsTrue(eventData.EventCode.Equals("FLBK"), "No flashback event packet!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataFlashback.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsFlashbackExpectedValue()
    {
        if (_packetDataFlashback.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataFlashback.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Flashback.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(14944U, eventDetails.FlashbackFrame, "Incorrect flashback value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataFlashback.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsPenaltyPacketExpectedTrue()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Penalty.packet"));

            if (packetData is EventData eventData)
            {
                Assert.IsTrue(eventData.EventCode.Equals("PENA"), "No penalty event packet!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check penalty type from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsPenaltyTypeExpectedValue()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(PenaltyType.Retired, eventDetails.PenaltyType, "Incorrect penalty type value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check penalty infringement type from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsPenaltyInfringementTypeExpectedRetiredDamage()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(InfringementType.RetiredDamaged, eventDetails.PenaltyInfringementType, "Incorrect infringement value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check penalty vehicle from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsPenaltyVehicleExpectedNineteen()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(19, eventDetails.VehicleIndex, "Incorrect vehicle value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check penalty lap number from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsPenaltyLapNumberExpectedTen()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(10, eventDetails.PenaltyLapNumber, "Incorrect lap number!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check event type from top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsTopSpeedEventTypeExpectedSpeedTrap()
    {
        if (_packetDataTopSpeed.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataTopSpeed.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(EventType.SpeedTrap, eventDetails.EventType, "Incorrect event type!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataTopSpeed.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check event type from flashback packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsFlashbackEventTypeExpectedFlashback()
    {
        if (_packetDataFlashback.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataFlashback.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Flashback.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(EventType.Flashback, eventDetails.EventType, "Incorrect event type!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataFlashback.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    /// <summary>
    /// Check event type from penalty packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2021IsPenaltyEventTypeExpectedPenalty()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2021-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails)
            {
                Assert.AreEqual(EventType.Penalty, eventDetails.EventType, "Incorrect event type!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2021!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2021 packet header!");
        }
    }

    #endregion // Methods
}