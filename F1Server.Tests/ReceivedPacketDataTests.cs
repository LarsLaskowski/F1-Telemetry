using F1Server.Core.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Test of packet header bounds checking in <see cref="ReceivedPacketData"/>
/// </summary>
[TestClass]
public class ReceivedPacketDataTests
{
    #region Methods

    /// <summary>
    /// Test to verify that a packet reporting game version 2023 but shorter than the 2023 header size
    /// does not read past the end of the raw data and leaves the header unset
    /// </summary>
    [TestMethod]
    public void SetRawDataTruncated2023HeaderReturnsNullHeader()
    {
        for (var length = ConstData.F12019HeaderSize; length < ConstData.F12023HeaderSize; length++)
        {
            var rawData = BuildRawPacket(2023, length);

            var packetData = new ReceivedPacketData();

            packetData.SetRawData(rawData);

            Assert.IsNull(packetData.PacketHeader, $"Header should stay null for a {length} byte 2023 packet!");
        }
    }

    /// <summary>
    /// Test to verify that a packet reporting game version 2023 with exactly the 2023 header size
    /// is parsed successfully
    /// </summary>
    [TestMethod]
    public void SetRawDataFullSize2023HeaderReturnsHeader()
    {
        var rawData = BuildRawPacket(2023, ConstData.F12023HeaderSize);

        var packetData = new ReceivedPacketData();

        packetData.SetRawData(rawData);

        Assert.IsNotNull(packetData.PacketHeader, "Header should be set for a full size 2023 packet!");
        Assert.AreEqual((ushort)2023, packetData.PacketHeader.GameVersion, "Wrong game version!");
    }

    /// <summary>
    /// Builds a raw packet of the given length whose first two bytes encode the given game version
    /// </summary>
    /// <param name="gameVersion">Game version to encode in the header</param>
    /// <param name="length">Total length of the raw packet</param>
    /// <returns>Raw packet bytes</returns>
    private static byte[] BuildRawPacket(ushort gameVersion, int length)
    {
        var rawData = new byte[length];

        rawData[0] = (byte)(gameVersion & 0xFF);
        rawData[1] = (byte)((gameVersion >> 8) & 0xFF);

        return rawData;
    }

    #endregion // Methods
}