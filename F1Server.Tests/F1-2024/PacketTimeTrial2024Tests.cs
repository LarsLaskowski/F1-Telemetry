using System.IO;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace F1Server.Tests;

/// <summary>
/// Class to test time trial packet files
/// </summary>
[TestClass]
public class PacketTimeTrial2024Tests
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
    public static void PacketTimeTrialInit(TestContext testContext)
    {
        var isFile = File.Exists(@"SampleData/F1-2024-TimeTrial.packet");

        if (isFile)
        {
            _packetAnalyzer = new PacketAnalyzer();

            var fileContent = File.ReadAllBytes(@"SampleData/F1-2024-TimeTrial.packet");

            _packetData = new ReceivedPacketData();

            _packetData.SetRawData(fileContent);

            var isCorrect = _packetData.PacketHeader != null;

            Assert.IsTrue(isCorrect, "Initialize of time trial packets failed!");
        }
        else
        {
            Assert.IsTrue(isFile, "File F1-2024-TimeTrial.packet is missing!");
        }
    }

    #endregion // Initializer/Cleanup

    #region Methods F1 2024

    /// <summary>
    /// Check whether the given file has a correct time trial content
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialCheckTimeTrial2024IsCorrectPacketType()
    {
        var isCorrect = _packetData.PacketHeader?.PacketType == PacketTypes.TimeTrial;

        Assert.IsTrue(isCorrect, "Packet is not a time trial packet!");
    }

    /// <summary>
    /// Check whether the given file is a F1 2024 packet
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialCheckTimeTrial2024IsCorrectGameVersion()
    {
        var isCorrect = _packetData.PacketHeader?.GameVersion == 2024;

        Assert.IsTrue(isCorrect, "Packet is not a F1 2024 packet");
    }

    /// <summary>
    /// Check that Sector3Time is read as a full uint value and not truncated to a single byte
    /// </summary>
    [TestMethod]
    public void PacketTimeTrialPlayerSessionBestSector3Time2024IsNotTruncatedToByte()
    {
        if (_packetData.PacketHeader != null)
        {
            var timeTrialData = _packetAnalyzer.GetTimeTrialData(_packetData.PacketHeader, File.ReadAllBytes(@"SampleData/F1-2024-TimeTrial.packet"));

            if (timeTrialData is TimeTrialData data && data.PacketData is ITimeTrialDataBase dataComplete)
            {
                Assert.AreEqual(20991u, dataComplete.PlayerSessionBestDataSet.Sector3Time, "Sector3Time was not read as a full uint value!");
            }
            else
            {
                Assert.Fail("Invalid time trial format, expected F1 2024!");
            }
        }
        else
        {
            Assert.IsNull(_packetData.PacketHeader, "Invalid F1 2024 packet header!");
        }
    }

    #endregion // Methods F1 2024
}