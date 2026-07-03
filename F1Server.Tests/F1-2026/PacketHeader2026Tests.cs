using System.IO;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Test of packet headers (F1 2026)
/// </summary>
[TestClass]
public class PacketHeader2026Tests
{
    #region Methods F1 2026

    /// <summary>
    /// Test to verify, that the sample file is existing
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckFile2026IsExisting()
    {
        var isExists = File.Exists(@"SampleData/F1-2026-HeaderCheck.packet");

        Assert.IsTrue(isExists, "Expected file is missing!");
    }

    /// <summary>
    /// Test to verify the correct game version
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckGameVersionReturns2026()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2026-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual((ushort)2026, packetData.PacketHeader.GameVersion, "Wrong game version!");
    }

    /// <summary>
    /// Test to verify the correct game year
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckGameYear2026Returns26()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2026-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual((ushort)26, packetData.PacketHeader.GameYear, "Wrong game year!");
    }

    /// <summary>
    /// Test to verify the correct packet type
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckPacketType2026IsEventPacket()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2026-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual(PacketTypes.Event, packetData.PacketHeader.PacketType, "Wrong packet type!");
    }

    #endregion // Methods F1 2026
}