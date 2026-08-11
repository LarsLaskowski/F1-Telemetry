using F1Server.Core.Enumerations;
using F1Server.Data;

namespace F1Server.Tests.Data;

/// <summary>
/// Class to test the packets-per-session metrics class
/// </summary>
[TestClass]
public class PacketsPerSessionMetricsTests
{
    #region Methods

    /// <summary>
    /// Verifies that a known packet type increments its dedicated counter and total processing time
    /// </summary>
    [TestMethod]
    public void PacketsPerSessionMetricsUpdatePacketStatisticsKnownTypeUpdatesDedicatedCounter()
    {
        var metrics = new PacketsPerSessionMetrics();

        metrics.UpdatePacketStatistics(PacketTypes.Motion, 12.5);

        Assert.AreEqual(1UL, metrics.Motion.Received, "The Motion counter should be incremented!");
        Assert.AreEqual(12.5, metrics.Motion.TotalProcessingTime, "The Motion processing time should be recorded!");
        Assert.AreEqual(0UL, metrics.Unknown.Received, "The Unknown counter must not be incremented for a known type!");
        Assert.AreEqual(1UL, metrics.TotalPacketsReceived, "TotalPacketsReceived should include the known type!");
    }

    /// <summary>
    /// Verifies that PacketTypes.Unknown is routed into the Unknown bucket instead of being silently dropped
    /// </summary>
    [TestMethod]
    public void PacketsPerSessionMetricsUpdatePacketStatisticsUnknownTypeUpdatesUnknownCounter()
    {
        var metrics = new PacketsPerSessionMetrics();

        metrics.UpdatePacketStatistics(PacketTypes.Unknown, 3.0);

        Assert.AreEqual(1UL, metrics.Unknown.Received, "The Unknown counter should be incremented for PacketTypes.Unknown!");
        Assert.AreEqual(3.0, metrics.Unknown.TotalProcessingTime, "The Unknown processing time should be recorded!");
        Assert.AreEqual(1UL, metrics.TotalPacketsReceived, "TotalPacketsReceived should include the Unknown bucket!");
    }

    /// <summary>
    /// Verifies that a null packet type is ignored entirely, matching the guard at the start of the method
    /// </summary>
    [TestMethod]
    public void PacketsPerSessionMetricsUpdatePacketStatisticsNullTypeIsIgnored()
    {
        var metrics = new PacketsPerSessionMetrics();

        metrics.UpdatePacketStatistics(null, 5.0);

        Assert.AreEqual(0UL, metrics.TotalPacketsReceived, "A null packet type must not update any counter!");
        Assert.AreEqual(0UL, metrics.Unknown.Received, "A null packet type must not update the Unknown counter!");
    }

    /// <summary>
    /// Verifies that Reset clears every per-type counter including the Unknown bucket
    /// </summary>
    [TestMethod]
    public void PacketsPerSessionMetricsResetClearsAllCountersIncludingUnknown()
    {
        var metrics = new PacketsPerSessionMetrics();

        metrics.UpdatePacketStatistics(PacketTypes.Motion, 1.0);
        metrics.UpdatePacketStatistics(PacketTypes.Unknown, 2.0);
        metrics.UnsuccessfullyProcessed = 4;
        metrics.Errors = 2;

        metrics.Reset();

        Assert.AreEqual(0UL, metrics.Motion.Received, "Motion counter should be reset!");
        Assert.AreEqual(0UL, metrics.Unknown.Received, "Unknown counter should be reset!");
        Assert.AreEqual(0L, metrics.UnsuccessfullyProcessed, "UnsuccessfullyProcessed should be reset!");
        Assert.AreEqual(0L, metrics.Errors, "Errors should be reset!");
    }

    /// <summary>
    /// Verifies that CopyFrom copies every per-type counter including the Unknown bucket as an independent instance
    /// </summary>
    [TestMethod]
    public void PacketsPerSessionMetricsCopyFromCopiesUnknownBucketIndependently()
    {
        var source = new PacketsPerSessionMetrics();

        source.UpdatePacketStatistics(PacketTypes.Unknown, 7.0);

        var target = new PacketsPerSessionMetrics();

        target.CopyFrom(source);

        Assert.AreEqual(1UL, target.Unknown.Received, "The copied Unknown counter should match the source!");
        Assert.AreEqual(7.0, target.Unknown.TotalProcessingTime, "The copied Unknown processing time should match the source!");

        source.UpdatePacketStatistics(PacketTypes.Unknown, 9.0);

        Assert.AreEqual(1UL, target.Unknown.Received, "The target's Unknown bucket must be an independent copy, not a shared reference!");
    }

    #endregion // Methods
}