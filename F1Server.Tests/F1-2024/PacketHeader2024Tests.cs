using System.IO;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Test of packet headers
/// </summary>
[TestClass]
public class PacketHeader2024Tests
{
    #region Methods F1 2024

    /// <summary>
    /// Test to verify, that the sample file is existing
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckFile2024IsExisting()
    {
        var isExists = File.Exists(@"SampleData/F1-2024-HeaderCheck.packet");

        Assert.IsTrue(isExists, "Expected file is missing!");
    }

    /// <summary>
    /// Test to verify the correct game version
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckGameVersionReturns2024()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2024-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual((ushort)2024, packetData.PacketHeader.GameVersion, "Wrong game version!");
    }

    /// <summary>
    /// Test to verify the correct unique session id
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckSessionUniqueId2024ReturnsExpectedNumber()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2024-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual(12246351741849144044, packetData.PacketHeader.UniqueSessionId, "Wrong unique session id!");
    }

    /// <summary>
    /// Test to verify the correct packet type
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckPacketType2024IsEventPacket()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2024-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "Packet header is null!");
        Assert.AreEqual(PacketTypes.Event, packetData.PacketHeader.PacketType, "Wrong packet type!");
    }

    #endregion // Methods F1 2024
}