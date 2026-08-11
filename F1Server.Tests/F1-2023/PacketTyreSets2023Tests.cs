using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;

namespace F1Server.Tests;

/// <summary>
/// Class to test tyre sets packet files (F1 2023). The tyre sets packet is deliberately not
/// decoded field by field, it is mapped to <see cref="UnknownData"/>, so these tests cover the
/// header handling and that dispatching the packet neither throws nor produces a typed object
/// </summary>
[TestClass]
public class PacketTyreSets2023Tests
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
    public static void PacketTyreSetsInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2023-TyreSets.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2023-TyreSets.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of tyre sets packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2023-TyreSets.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2023

    /// <summary>
    /// Check whether the given file is a tyre sets packet
    /// </summary>
    [TestMethod]
    public void PacketTyreSetsCheckTyreSets2023IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.TyreSets;

        Assert.IsTrue(isCorrect, "Packet is not a tyre sets packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2023 packet
    /// </summary>
    [TestMethod]
    public void PacketTyreSetsCheckTyreSets2023IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2023;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2023 packet!");
    }

    /// <summary>
    /// Check that the tyre sets packet is dispatched to the unknown data placeholder, which is
    /// the documented handling for packets that are received but not decoded
    /// </summary>
    [TestMethod]
    public void PacketTyreSets2023ReturnsUnknownData()
    {
        var packetData = _packetAnalyzer.GetPacketData(_packetData);

        Assert.IsInstanceOfType<UnknownData>(packetData, "Tyre sets packet must be mapped to unknown data!");
    }

    /// <summary>
    /// Check that the unknown data placeholder keeps the header of the received packet, so the
    /// session and packet type stay available even though the payload is not decoded
    /// </summary>
    [TestMethod]
    public void PacketTyreSets2023UnknownDataKeepsPacketHeader()
    {
        var packetData = _packetAnalyzer.GetPacketData(_packetData);

        if (packetData is UnknownData unknownData)
        {
            Assert.AreEqual(PacketTypes.TyreSets, unknownData.PacketHeader?.PacketType, "Unknown data lost the packet type!");
            Assert.AreEqual<ushort?>(2023, unknownData.PacketHeader?.GameVersion, "Unknown data lost the game version!");
        }
        else
        {
            Assert.Fail("Tyre sets packet was not mapped to unknown data!");
        }
    }

    /// <summary>
    /// Check that dispatching the tyre sets packet reports no error, so an undecoded packet is
    /// not mistaken for a failed transformation
    /// </summary>
    [TestMethod]
    public void PacketTyreSets2023ReportsNoError()
    {
        _packetAnalyzer.GetPacketData(_packetData);

        Assert.IsTrue(string.IsNullOrEmpty(_packetAnalyzer.LastError), "Tyre sets packet must not report an error!");
    }

    #endregion // Methods F1 2023
}