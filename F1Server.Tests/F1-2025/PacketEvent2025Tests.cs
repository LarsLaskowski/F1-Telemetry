using System.IO;
using System.Text;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test event packets from F1 2025
/// </summary>
[TestClass]
public class PacketEvent2025Tests
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
    public static void PacketEvent2025Init(TestContext testContext)
    {
        var file1 = File.Exists(@"SampleData/F1-2025-Event-TopSpeed.packet");
        var file2 = File.Exists(@"SampleData/F1-2025-Event-Flashback.packet");
        var file3 = File.Exists(@"SampleData/F1-2025-Event-Penalty.packet");

        if (file1 && file2 && file3)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet");

            _packetDataTopSpeed = new ReceivedPacketData();

            _packetDataTopSpeed.SetRawData(fileContent);

            fileContent = File.ReadAllBytes(@"SampleData/F1-2025-Event-Flashback.packet");

            _packetDataFlashback = new ReceivedPacketData();

            _packetDataFlashback.SetRawData(fileContent);

            fileContent = File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet");

            _packetDataPenalty = new ReceivedPacketData();

            _packetDataPenalty.SetRawData(fileContent);

            var isCorrect = _packetDataTopSpeed.PacketHeader != null && _packetDataFlashback.PacketHeader != null && _packetDataPenalty.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of event packets failed!");
        }
        else
        {
            Assert.IsTrue(file1, "File F1-2025-Event-TopSpeed.packet is missing!");
            Assert.IsTrue(file2, "File F1-2025-Event-Flashback.packet is missing!");
            Assert.IsTrue(file3, "File F1-2025-Event-Penalty.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Static methods

    /// <summary>
    /// Builds a synthetic F1 2025 event packet by taking a real sample packet's header and
    /// overwriting its event code and payload, so branches without a dedicated sample file can be tested
    /// </summary>
    /// <param name="eventCode">Four letter event code to inject</param>
    /// <param name="payloadBytes">Payload bytes following the event code</param>
    /// <returns>Synthetic packet content</returns>
    private static byte[] BuildSyntheticEventPacket(string eventCode, params byte[] payloadBytes)
    {
        var data = File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet");
        var headerSize = ConstData.F12025HeaderSize;
        var codeBytes = Encoding.ASCII.GetBytes(eventCode);

        Array.Copy(codeBytes, 0, data, headerSize, codeBytes.Length);
        Array.Copy(payloadBytes, 0, data, headerSize + 4, payloadBytes.Length);

        return data;
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Check correct packet type
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsCorrectPacketType()
    {
        var isCorrect = _packetDataTopSpeed.PacketHeader?.PacketType == PacketTypes.Event;

        Assert.IsTrue(isCorrect, "Packet is not a event packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2020 packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsCorrectGameVersion()
    {
        var isCorrect = _packetDataFlashback.PacketHeader?.GameVersion == 2025;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2025 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketEvent2025CheckEventDataIsEventDataObject()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var isCorrect = false;
            var eventData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

            if (eventData is EventData data && data.PacketData is EventData2025)
            {
                isCorrect = true;
            }

            Assert.IsTrue(isCorrect, "Packet is not a event data packet");
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsTopSpeedPacketExpectedTrue()
    {
        if (_packetDataTopSpeed.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataTopSpeed.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet"));

            if (packetData is EventData eventData)
            {
                Assert.IsTrue(eventData.EventCode.Equals("SPTP"), "No top speed event packet!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataTopSpeed.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsTopSpeedExpectedValue()
    {
        if (_packetDataTopSpeed.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataTopSpeed.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(307.768463F, eventDetails.TopSpeed, "Incorrect top speed value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataTopSpeed.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsTopSpeedVehicleExpectedZero()
    {
        if (_packetDataTopSpeed.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataTopSpeed.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(20, eventDetails.VehicleIndex, "Incorrect vehicle value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataTopSpeed.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsFlashbackPacketExpectedTrue()
    {
        if (_packetDataFlashback.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataFlashback.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Flashback.packet"));

            if (packetData is EventData eventData)
            {
                Assert.IsTrue(eventData.EventCode.Equals("FLBK"), "No flashback event packet!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataFlashback.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsFlashbackExpectedValue()
    {
        if (_packetDataFlashback.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataFlashback.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Flashback.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(1280U, eventDetails.FlashbackFrame, "Incorrect flashback value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataFlashback.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyPacketExpectedTrue()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

            if (packetData is EventData eventData)
            {
                Assert.IsTrue(eventData.EventCode.Equals("PENA"), "No penalty event packet!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check penalty type from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyTypeExpectedValue()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(PenaltyType.Warning, eventDetails.PenaltyType, "Incorrect penalty type value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check penalty infringement type from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyInfringementTypeExpectedLapInvalidRunningWide()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(InfringementType.SmallCollision, eventDetails.PenaltyInfringementType, "Incorrect infringement value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check penalty vehicle from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyVehicleExpectedZero()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(21, eventDetails.VehicleIndex, "Incorrect vehicle value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check penalty lap number from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyLapNumberExpectedThree()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(16, eventDetails.PenaltyLapNumber, "Incorrect lap number!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check event type from top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsTopSpeedEventTypeExpectedSpeedTrap()
    {
        if (_packetDataTopSpeed.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataTopSpeed.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(EventType.SpeedTrap, eventDetails.EventType, "Incorrect event type!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataTopSpeed.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check event type from flashback packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsFlashbackEventTypeExpectedFlashback()
    {
        if (_packetDataFlashback.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataFlashback.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Flashback.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(EventType.Flashback, eventDetails.EventType, "Incorrect event type!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataFlashback.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check event type from penalty packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsPenaltyEventTypeExpectedPenalty()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2025-Event-Penalty.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(EventType.Penalty, eventDetails.EventType, "Incorrect event type!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    /// <summary>
    /// Check event type and reason of a synthetic DRS disabled packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2025IsDrsDisabledEventTypeExpectedDrsDisabled()
    {
        if (_packetDataPenalty.PacketHeader != null)
        {
            var syntheticData = BuildSyntheticEventPacket("DRSD", 2);
            var packetData = _packetAnalyzer.GetEventData(_packetDataPenalty.PacketHeader, syntheticData);

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails)
            {
                Assert.AreEqual(EventType.DrsDisabled, eventDetails.EventType, "Incorrect event type!");
                Assert.AreEqual(DrsDisabledReason.RedFlag, eventDetails.DrsDisabledReason, "Incorrect DRS disabled reason!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2025!");
            }
        }
        else
        {
            Assert.IsNull(_packetDataPenalty.PacketHeader, "Invalid F1 2025 packet header!");
        }
    }

    #endregion // Methods
}