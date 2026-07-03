using System.IO;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Test of packet headers
/// </summary>
[TestClass]
public class PacketHeader2023Tests
{
    #region Methods F1 2023

    /// <summary>
    /// Test to verify, that the sample file is existing
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckFile2023IsExisting()
    {
        var isExists = File.Exists(@"SampleData/F1-2023-HeaderCheck.packet");

        Assert.IsTrue(isExists, "Expected file is missing!");
    }

    /// <summary>
    /// Test to verify the correct game version
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckGameVersionReturns2023()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2023-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual((ushort)2023, packetData.PacketHeader.GameVersion, "Wrong game version!");
    }

    /// <summary>
    /// Test to verify the correct unique session id
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckSessionUniqueId2023ReturnsExpectedNumber()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2023-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual(16234025100940339161, packetData.PacketHeader.UniqueSessionId, "Wrong unique session id!");
    }

    /// <summary>
    /// Test to verify the correct packet type
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckPacketType2023IsEventPacket()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2023-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual(PacketTypes.Event, packetData.PacketHeader.PacketType, "Wrong packet type!");
    }

    #endregion // Methods F1 2023
}