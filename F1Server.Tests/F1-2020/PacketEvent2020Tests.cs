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
/// Class to test event packets from F1 2020
/// </summary>
[TestClass]
public class PacketEvent2020Tests
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
    public static void PacketEvent2020Init(TestContext testContext)
    {
        var file1 = File.Exists(@"SampleData/F1-2020-Event-TopSpeed.packet");

        if (file1)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2020-Event-TopSpeed.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of event packets failed!");
        }
        else
        {
            Assert.IsTrue(file1, "File F1-2020-Event-TopSpeed.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods

    /// <summary>
    /// Check correct packet type
    /// </summary>
    [TestMethod]
    public void PacketEvent2020IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.Event;

        Assert.IsTrue(isCorrect, "Packet is not a event packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2020 packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2020IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2020;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2020 packet");
    }

    /// <summary>
    /// Check whether the analyzer construct the correct class object
    /// </summary>
    [TestMethod]
    public void PacketEvent2020CheckEventDataIsEventDataObject()
    {
        if (_packetData.PacketHeader != null)
        {
            var isCorrect = false;
            var eventData = _packetAnalyzer.GetEventData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2020-Event-TopSpeed.packet"));

            if (eventData is EventData data && data.PacketData is EventData2020)
            {
                isCorrect = true;
            }

            Assert.IsTrue(isCorrect, "Packet is not a event data packet");
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header!");
        }
    }

    /// <summary>
    /// Check if event packet is a top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2020IsTopSpeedPacketExpectedTrue()
    {
        if (_packetData.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2020-Event-TopSpeed.packet"));

            if (packetData is EventData eventData)
            {
                Assert.IsTrue(eventData.EventCode.Equals("SPTP"), "No top speed event packet!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header!");
        }
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2020IsTopSpeedExpectedValue()
    {
        if (_packetData.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2020-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2020 eventDetails)
            {
                Assert.AreEqual(319.895874F, eventDetails.TopSpeed, "Incorrect top speed value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header!");
        }
    }

    /// <summary>
    /// Check top speed from packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2020IsTopSpeedVehicleExpectedNineteen()
    {
        if (_packetData.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2020-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2020 eventDetails)
            {
                Assert.AreEqual(19, eventDetails.VehicleIndex, "Incorrect vehicle value!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header!");
        }
    }

    /// <summary>
    /// Check event type from top speed packet
    /// </summary>
    [TestMethod]
    public void PacketEvent2020IsTopSpeedEventTypeExpectedSpeedTrap()
    {
        if (_packetData.PacketHeader != null)
        {
            var packetData = _packetAnalyzer.GetEventData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2020-Event-TopSpeed.packet"));

            if (packetData is EventData eventData && eventData.PacketData?.EventDetails is IEventDataDetails2020 eventDetails)
            {
                Assert.AreEqual(EventType.SpeedTrap, eventDetails.EventType, "Incorrect event type!");
            }
            else
            {
                Assert.Fail("Invalid packet data, expected F1 2020!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2020 packet header!");
        }
    }

    #endregion // Methods
}