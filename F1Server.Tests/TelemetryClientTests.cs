using F1Server.Core.Data;
using F1Server.Service;

namespace F1Server.Tests;

/// <summary>
/// Test of the connection timeout comparison logic in <see cref="TelemetryClient"/>
/// </summary>
[TestClass]
public class TelemetryClientTests
{
    #region Methods

    /// <summary>
    /// Test to verify that no timeout is reported while the last packet is inside the timeout window
    /// </summary>
    [TestMethod]
    public void TelemetryClientIsReceiveTimeoutElapsedWithinWindowReturnsFalse()
    {
        var lastPacketTicks = DateTime.UtcNow.Ticks;
        var currentTicks = lastPacketTicks + ((ConstData.TimeoutInMs - 1) * TimeSpan.TicksPerMillisecond);

        Assert.IsFalse(TelemetryClient.IsReceiveTimeoutElapsed(lastPacketTicks, currentTicks), "No timeout may be reported inside the timeout window!");
    }

    /// <summary>
    /// Test to verify that no timeout is reported when the elapsed time matches the timeout window exactly
    /// </summary>
    [TestMethod]
    public void TelemetryClientIsReceiveTimeoutElapsedExactWindowReturnsFalse()
    {
        var lastPacketTicks = DateTime.UtcNow.Ticks;
        var currentTicks = lastPacketTicks + (ConstData.TimeoutInMs * TimeSpan.TicksPerMillisecond);

        Assert.IsFalse(TelemetryClient.IsReceiveTimeoutElapsed(lastPacketTicks, currentTicks), "No timeout may be reported when the window is matched exactly!");
    }

    /// <summary>
    /// Test to verify that a timeout is reported once the last packet is older than the timeout window
    /// </summary>
    [TestMethod]
    public void TelemetryClientIsReceiveTimeoutElapsedBeyondWindowReturnsTrue()
    {
        var lastPacketTicks = DateTime.UtcNow.Ticks;
        var currentTicks = lastPacketTicks + ((ConstData.TimeoutInMs + 1) * TimeSpan.TicksPerMillisecond);

        Assert.IsTrue(TelemetryClient.IsReceiveTimeoutElapsed(lastPacketTicks, currentTicks), "A timeout must be reported beyond the timeout window!");
    }

    #endregion // Methods
}