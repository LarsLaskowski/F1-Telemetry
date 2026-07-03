using System.IO;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Test of packet headers
/// </summary>
[TestClass]
public class PacketHeader2022Tests
{
    #region Methods F1 2022

    /// <summary>
    /// Test to verify, that the sample file is existing
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckFile2022IsExisting()
    {
        var isExists = File.Exists(@"SampleData/F1-2022-HeaderCheck.packet");

        Assert.IsTrue(isExists, "Expected file is missing!");
    }

    /// <summary>
    /// Test to verify the correct game version
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckGameVersionReturns2022()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2022-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual((ushort)2022, packetData.PacketHeader.GameVersion, "Wrong game version!");
    }

    /// <summary>
    /// Test to verify the correct unique session id
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckSessionUniqueId2022ReturnsExpectedNumber()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2022-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual(9317899283514315051, packetData.PacketHeader.UniqueSessionId, "Wrong unique session id!");
    }

    /// <summary>
    /// Test to verify the correct packet type
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckPacketType2022IsEventPacket()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2022-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual(PacketTypes.Event, packetData.PacketHeader.PacketType, "Wrong packet type!");
    }

    #endregion // Methods F1 2022
}