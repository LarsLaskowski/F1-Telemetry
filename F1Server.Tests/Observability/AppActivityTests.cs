using F1Server.Core.Enumerations;
using F1Server.Core.Observability;

namespace F1Server.Tests.Observability;

/// <summary>
/// Tests for <see cref="AppActivity"/>
/// </summary>
[TestClass]
public class AppActivityTests
{
    #region Test methods

    /// <summary>
    /// High-frequency packet types must be reported as such
    /// </summary>
    /// <param name="packetType">Packet type under test</param>
    [TestMethod]
    [DataRow(PacketTypes.CarTelemetry)]
    [DataRow(PacketTypes.Motion)]
    [DataRow(PacketTypes.LapData)]
    [DataRow(PacketTypes.CarStatus)]
    [DataRow(PacketTypes.SessionHistory)]
    public void AppActivityIsHighFrequencyPacketTypeHighFrequencyTypeReturnsTrue(PacketTypes packetType)
    {
        Assert.IsTrue(AppActivity.IsHighFrequencyPacketType(packetType), "High-frequency packet type was not recognized as such!");
    }

    /// <summary>
    /// Low-frequency packet types must not be reported as high-frequency
    /// </summary>
    /// <param name="packetType">Packet type under test</param>
    [TestMethod]
    [DataRow(PacketTypes.Session)]
    [DataRow(PacketTypes.Event)]
    [DataRow(PacketTypes.Participants)]
    [DataRow(PacketTypes.FinalClassification)]
    public void AppActivityIsHighFrequencyPacketTypeLowFrequencyTypeReturnsFalse(PacketTypes packetType)
    {
        Assert.IsFalse(AppActivity.IsHighFrequencyPacketType(packetType), "Low-frequency packet type was incorrectly recognized as high-frequency!");
    }

    /// <summary>
    /// A missing packet type must not be reported as high-frequency
    /// </summary>
    [TestMethod]
    public void AppActivityIsHighFrequencyPacketTypeNullReturnsFalse()
    {
        Assert.IsFalse(AppActivity.IsHighFrequencyPacketType(null), "A missing packet type was incorrectly recognized as high-frequency!");
    }

    #endregion // Test methods
}