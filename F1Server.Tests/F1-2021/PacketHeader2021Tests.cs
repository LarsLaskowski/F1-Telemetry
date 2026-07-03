using System.IO;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Test of packet headers
/// </summary>
[TestClass]
public class PacketHeader2021Tests
{
    #region Methods F1 2021

    /// <summary>
    /// Test to verify, that the sample file is existing
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckFile2021IsExisting()
    {
        var isExists = File.Exists(@"SampleData/F1-2021-HeaderCheck.packet");

        Assert.IsTrue(isExists, "Expected file is missing!");
    }

    /// <summary>
    /// Test to verify the correct game version
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckGameVersionReturns2021()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2021-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is missing!");
        Assert.AreEqual((ushort)2021, packetData.PacketHeader.GameVersion, "Wrong game version!");
    }

    /// <summary>
    /// Test to verify the correct unique session id
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckSessionUniqueId2021ReturnsExpectedNumber()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2021-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is missing!");
        Assert.AreEqual(11812237980383671120, packetData.PacketHeader.UniqueSessionId, "Wrong unique session id!");
    }

    /// <summary>
    /// Test to verify the correct packet type
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckPacketType2021IsLapDataPacket()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2021-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is missing!");
        Assert.AreEqual(PacketTypes.LapData, packetData.PacketHeader.PacketType, "Wrong packet type!");
    }

    #endregion // Methods F1 2021
}