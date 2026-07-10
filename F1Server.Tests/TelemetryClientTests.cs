using System.Buffers.Binary;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Data;
using F1Server.Service;
using F1Server.Service.Runtime;
using F1Server.Telemetry;

using Microsoft.Extensions.DependencyInjection;

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

    /// <summary>
    /// Test to verify that disposing the client releases the telemetry writer and stops the background database writer
    /// </summary>
    [TestMethod]
    public void TelemetryClientDisposeShutsDownDatabaseWriter()
    {
        var services = new ServiceCollection();
        var applicationData = new F1ServerApplicationData();

        services.AddSingleton(applicationData);
        services.AddSingleton(new PacketAnalyzer());
        services.AddSingleton(new TelemetryConfiguration());

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var telemetryClient = new TelemetryClient(serviceProvider, false, false);

            telemetryClient.Dispose();

            Assert.IsNull(applicationData.TelemetryWriter, "The telemetry writer should be released on dispose!");
            Assert.IsFalse(DatabaseWriter.IsRunning, "The background database writer should be stopped on dispose!");
        }
    }

    /// <summary>
    /// Test to verify that the length prefix reader returns the packet length encoded on the shared connection
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReadReplayPacketLengthWithPrefixReturnsLength()
    {
        var lengthPrefix = new byte[sizeof(int)];
        var payload = new byte[sizeof(int)];

        BinaryPrimitives.WriteInt32LittleEndian(payload, 1347);

        using (var stream = new MemoryStream(payload))
        {
            var packetLength = await TelemetryClient.ReadReplayPacketLengthAsync(stream, lengthPrefix, CancellationToken.None);

            Assert.AreEqual(1347, packetLength, "The reader must return the length encoded in the prefix!");
        }
    }

    /// <summary>
    /// Test to verify that the length prefix reader returns zero once the connection was closed by the client
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReadReplayPacketLengthOnClosedConnectionReturnsZero()
    {
        var lengthPrefix = new byte[sizeof(int)];

        using (var stream = new MemoryStream(Array.Empty<byte>()))
        {
            var packetLength = await TelemetryClient.ReadReplayPacketLengthAsync(stream, lengthPrefix, CancellationToken.None);

            Assert.AreEqual(0, packetLength, "A closed connection must be reported as a zero length!");
        }
    }

    #endregion // Methods
}