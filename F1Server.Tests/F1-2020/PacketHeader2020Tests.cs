using System.IO;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Test of packet headers
/// </summary>
[TestClass]
public class PacketHeader2020Tests
{
    #region Methods F1 2020

    /// <summary>
    /// Test to verify, that the sample file is existing
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckFile2020IsExisting()
    {
        var isExists = File.Exists(@"SampleData/F1-2020-HeaderCheck.packet");

        Assert.IsTrue(isExists, "Expected file is missing!");
    }

    /// <summary>
    /// Test to verify, that a correct packet header returns
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckHeaderReturnsObject()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2020-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "No packet header!");
    }

    /// <summary>
    /// Test to verify the correct game version
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckGameVersionReturns2020()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2020-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "No packet header!");
        Assert.AreEqual((ushort)2020, packetData.PacketHeader.GameVersion, "Wrong game version!");
    }

    /// <summary>
    /// Test to verify the correct unique session id
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckSessionUniqueId2020ReturnsExpectedNumber()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2020-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "No packet header!");
        Assert.AreEqual(1661982855633685893UL, packetData.PacketHeader.UniqueSessionId, "Wrong unique session id!");
    }

    /// <summary>
    /// Test to verify the correct packet type
    /// </summary>
    [TestMethod]
    public void PacketHeaderCheckPacketType2020IsLapDataPacket()
    {
        var packetContent = File.ReadAllBytes(@"SampleData/F1-2020-HeaderCheck.packet");

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(packetContent);

        Assert.IsNotNull(packetData.PacketHeader, "No packet header!");
        Assert.AreEqual(PacketTypes.LapData, packetData.PacketHeader.PacketType, "Wrong packet type!");
    }

    #endregion // Methods F1 2020
}