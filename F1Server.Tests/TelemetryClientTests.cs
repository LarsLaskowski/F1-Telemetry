using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Observability;
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
    #region Constants

    /// <summary>
    /// UDP port used by the replay listener test, the TCP replay port is the next port number
    /// </summary>
    private const int ReplayTestPort = 47311;

    #endregion // Constants

    #region Properties

    /// <summary>
    /// Gets or sets the test context that provides the cancellation token for the running test
    /// </summary>
    public TestContext TestContext { get; set; }

    #endregion // Properties

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
            var packetLength = await TelemetryClient.ReadReplayPacketLengthAsync(stream, lengthPrefix, TestContext.CancellationToken);

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
            var packetLength = await TelemetryClient.ReadReplayPacketLengthAsync(stream, lengthPrefix, TestContext.CancellationToken);

            Assert.AreEqual(0, packetLength, "A closed connection must be reported as a zero length!");
        }
    }

    /// <summary>
    /// Test to verify that the length prefix reader returns zero when the connection is closed inside a partial prefix
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReadReplayPacketLengthWithPartialPrefixReturnsZero()
    {
        var lengthPrefix = new byte[sizeof(int)];
        var partialPrefix = new byte[]
                            {
                                42,
                                0
                            };

        using (var stream = new MemoryStream(partialPrefix))
        {
            var packetLength = await TelemetryClient.ReadReplayPacketLengthAsync(stream, lengthPrefix, TestContext.CancellationToken);

            Assert.AreEqual(0, packetLength, "A connection closed inside the prefix must be reported as a zero length!");
        }
    }

    /// <summary>
    /// Test to verify that the packet reader returns the payload of a single framed packet
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReadReplayPacketWithSingleFrameReturnsPayload()
    {
        var lengthPrefix = new byte[sizeof(int)];
        var recvBuf = new byte[ConstData.MaxReplayPacketLength];
        var frame = CreateReplayFrame([11, 22, 33]);

        using (var stream = new MemoryStream(frame))
        {
            var packetLength = await TelemetryClient.ReadReplayPacketAsync(stream, lengthPrefix, recvBuf, TestContext.CancellationToken);

            Assert.AreEqual(3, packetLength, "The reader must return the announced payload length!");
            Assert.AreSequenceEqual(new byte[] { 11, 22, 33 }, recvBuf.Take(packetLength), "The reader must return the payload bytes of the frame!");
        }
    }

    /// <summary>
    /// Test to verify that the packet reader returns two consecutive framed packets from one stream
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReadReplayPacketWithTwoFramesReturnsBothPayloads()
    {
        var lengthPrefix = new byte[sizeof(int)];
        var recvBuf = new byte[ConstData.MaxReplayPacketLength];
        var frames = CreateReplayFrame([1, 2]).Concat(CreateReplayFrame([3, 4, 5])).ToArray();

        using (var stream = new MemoryStream(frames))
        {
            var firstLength = await TelemetryClient.ReadReplayPacketAsync(stream, lengthPrefix, recvBuf, TestContext.CancellationToken);

            Assert.AreEqual(2, firstLength, "The first frame must announce two payload bytes!");
            Assert.AreSequenceEqual(new byte[] { 1, 2 }, recvBuf.Take(firstLength), "The first payload must be returned unchanged!");

            var secondLength = await TelemetryClient.ReadReplayPacketAsync(stream, lengthPrefix, recvBuf, TestContext.CancellationToken);

            Assert.AreEqual(3, secondLength, "The second frame must announce three payload bytes!");
            Assert.AreSequenceEqual(new byte[] { 3, 4, 5 }, recvBuf.Take(secondLength), "The second payload must be returned unchanged!");

            var thirdLength = await TelemetryClient.ReadReplayPacketAsync(stream, lengthPrefix, recvBuf, TestContext.CancellationToken);

            Assert.AreEqual(0, thirdLength, "The end of the stream must be reported as a zero length!");
        }
    }

    /// <summary>
    /// Test to verify that the packet reader treats a negative announced length as a framing error
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReadReplayPacketWithNegativeLengthReturnsZero()
    {
        var lengthPrefix = new byte[sizeof(int)];
        var recvBuf = new byte[ConstData.MaxReplayPacketLength];
        var frame = new byte[sizeof(int)];

        BinaryPrimitives.WriteInt32LittleEndian(frame, -5);

        using (var stream = new MemoryStream(frame))
        {
            var packetLength = await TelemetryClient.ReadReplayPacketAsync(stream, lengthPrefix, recvBuf, TestContext.CancellationToken);

            Assert.AreEqual(0, packetLength, "A negative announced length must be treated as a framing error!");
        }
    }

    /// <summary>
    /// Test to verify that the packet reader treats an announced length above the buffer size as a framing error
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReadReplayPacketWithOversizedLengthReturnsZero()
    {
        var lengthPrefix = new byte[sizeof(int)];
        var recvBuf = new byte[ConstData.MaxReplayPacketLength];
        var frame = new byte[sizeof(int)];

        BinaryPrimitives.WriteInt32LittleEndian(frame, ConstData.MaxReplayPacketLength + 1);

        using (var stream = new MemoryStream(frame))
        {
            var packetLength = await TelemetryClient.ReadReplayPacketAsync(stream, lengthPrefix, recvBuf, TestContext.CancellationToken);

            Assert.AreEqual(0, packetLength, "An announced length above the buffer size must be treated as a framing error!");
        }
    }

    /// <summary>
    /// Test to verify that the packet reader reports a payload truncated by an unclean disconnect as an end of stream
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReadReplayPacketWithTruncatedPayloadThrowsEndOfStreamException()
    {
        var lengthPrefix = new byte[sizeof(int)];
        var recvBuf = new byte[ConstData.MaxReplayPacketLength];
        var frame = CreateReplayFrame([7, 8, 9]);

        // Expose the frame without its last payload byte so the payload read hits the end of the stream
        using (var stream = new MemoryStream(frame, 0, frame.Length - 1))
        {
            await Assert.ThrowsExactlyAsync<EndOfStreamException>(() => TelemetryClient.ReadReplayPacketAsync(stream, lengthPrefix, recvBuf, TestContext.CancellationToken), "A truncated payload must surface as an end of stream!");
        }
    }

    /// <summary>
    /// Test to verify that enqueuing a replay packet updates the received packet statistics and the queue counter
    /// </summary>
    [TestMethod]
    public void TelemetryClientEnqueueReplayPacketUpdatesStatistics()
    {
        var services = new ServiceCollection();
        var applicationData = new F1ServerApplicationData();

        services.AddSingleton(applicationData);
        services.AddSingleton(new PacketAnalyzer());
        services.AddSingleton(new TelemetryConfiguration());

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var telemetryClient = new TelemetryClient(serviceProvider, false, false);

            try
            {
                var recvBuf = new byte[]
                              {
                                  1,
                                  2,
                                  3,
                                  4
                              };

                telemetryClient.EnqueueReplayPacket(recvBuf, recvBuf.Length);

                Assert.AreEqual(1, applicationData.Statistics.PacketsReceivedTotal, "The received packet counter must be incremented!");
                Assert.AreEqual(1, applicationData.Statistics.PacketsInQueue, "The queue counter must reflect the enqueued packet!");
            }
            finally
            {
                telemetryClient.Dispose();
            }
        }
    }

    /// <summary>
    /// Test to verify that the replay TCP listener processes framed packets and keeps accepting connections after an unclean disconnect
    /// </summary>
    /// <returns>A task representing the asynchronous operation</returns>
    [TestMethod]
    public async Task TelemetryClientReceiveReplayPacketsSurvivesUncleanDisconnect()
    {
        var services = new ServiceCollection();
        var applicationData = new F1ServerApplicationData();

        services.AddSingleton(applicationData);
        services.AddSingleton(new PacketAnalyzer());
        services.AddSingleton(new TelemetryConfiguration());

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var telemetryClient = new TelemetryClient(serviceProvider, false, false, ReplayTestPort);

            try
            {
                Assert.IsTrue(telemetryClient.StartReceiving(), "The telemetry client must start receiving on the test port!");

                using (var firstConnection = new TcpClient())
                {
                    await firstConnection.ConnectAsync(IPAddress.Loopback, ReplayTestPort + 1, TestContext.CancellationToken);

                    using (var stream = firstConnection.GetStream())
                    {
                        await stream.WriteAsync(CreateReplayFrame([1, 2, 3, 4]), TestContext.CancellationToken);

                        // Announce more payload bytes than are sent so closing the connection triggers the unclean disconnect path
                        await stream.WriteAsync(CreateReplayFrame(new byte[32]).AsMemory(0, 12), TestContext.CancellationToken);
                    }
                }

                await WaitForReceivedPacketsAsync(applicationData, 1, TestContext.CancellationToken);

                Assert.AreEqual(1, applicationData.Statistics.PacketsReceivedTotal, "The framed packet of the first connection must be counted!");

                using (var secondConnection = new TcpClient())
                {
                    await secondConnection.ConnectAsync(IPAddress.Loopback, ReplayTestPort + 1, TestContext.CancellationToken);

                    using (var stream = secondConnection.GetStream())
                    {
                        await stream.WriteAsync(CreateReplayFrame([5, 6, 7, 8]), TestContext.CancellationToken);
                    }
                }

                await WaitForReceivedPacketsAsync(applicationData, 2, TestContext.CancellationToken);

                Assert.AreEqual(2, applicationData.Statistics.PacketsReceivedTotal, "The listener must accept a new connection after an unclean disconnect!");
            }
            finally
            {
                telemetryClient.Dispose();
            }
        }
    }

    /// <summary>
    /// Test to verify that a fast, successfully processed high-frequency packet does not record a tracing span
    /// </summary>
    [TestMethod]
    public void TelemetryClientRecordSlowPacketActivityFastSuccessDoesNotRecordActivity()
    {
        var recordedActivity = InvokeRecordSlowPacketActivityWithListener(TimeSpan.FromMilliseconds(1), true);

        Assert.IsNull(recordedActivity, "A fast, successfully processed packet recorded a tracing span!");
    }

    /// <summary>
    /// Test to verify that a slow but successfully processed high-frequency packet records an Ok tracing span
    /// </summary>
    [TestMethod]
    public void TelemetryClientRecordSlowPacketActivitySlowSuccessRecordsOkActivity()
    {
        var recordedActivity = InvokeRecordSlowPacketActivityWithListener(TimeSpan.FromMilliseconds(ConstData.SlowPacketProcessingThresholdMs + 1), true);

        Assert.IsNotNull(recordedActivity, "A slow, successfully processed packet did not record a tracing span!");
        Assert.AreEqual(ActivityStatusCode.Ok, recordedActivity.Status, "The recorded span status was not Ok!");
    }

    /// <summary>
    /// Test to verify that a fast but failed high-frequency packet still records an Error tracing span
    /// </summary>
    [TestMethod]
    public void TelemetryClientRecordSlowPacketActivityFastFailureRecordsErrorActivity()
    {
        var recordedActivity = InvokeRecordSlowPacketActivityWithListener(TimeSpan.FromMilliseconds(1), false);

        Assert.IsNotNull(recordedActivity, "A failed packet did not record a tracing span!");
        Assert.AreEqual(ActivityStatusCode.Error, recordedActivity.Status, "The recorded span status was not Error!");
    }

    #endregion // Methods

    #region Static methods

    /// <summary>
    /// Creates one length-prefixed replay frame for the given payload
    /// </summary>
    /// <param name="payload">Payload bytes of the frame</param>
    /// <returns>The frame consisting of the four byte little endian length prefix followed by the payload</returns>
    private static byte[] CreateReplayFrame(byte[] payload)
    {
        var frame = new byte[sizeof(int) + payload.Length];

        BinaryPrimitives.WriteInt32LittleEndian(frame, payload.Length);

        payload.CopyTo(frame, sizeof(int));

        return frame;
    }

    /// <summary>
    /// Waits until the given number of packets was counted as received or a timeout elapses
    /// </summary>
    /// <param name="applicationData">Application data holding the statistics</param>
    /// <param name="expectedPackets">Number of received packets to wait for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task WaitForReceivedPacketsAsync(F1ServerApplicationData applicationData, long expectedPackets, CancellationToken cancellationToken)
    {
        var timeout = DateTime.UtcNow.AddSeconds(10);

        while (applicationData.Statistics.PacketsReceivedTotal < expectedPackets && DateTime.UtcNow < timeout)
        {
            await Task.Delay(25, cancellationToken);
        }
    }

    /// <summary>
    /// Attaches an <see cref="ActivityListener"/> to <see cref="AppActivity.SrvSource"/>, calls
    /// <see cref="TelemetryClient.RecordSlowPacketActivity"/> and returns the span it recorded, if any
    /// </summary>
    /// <param name="elapsed">Elapsed processing time to pass through</param>
    /// <param name="isProcessed">Whether the packet was processed successfully</param>
    /// <returns>The recorded span, or null if none was recorded</returns>
    private static Activity? InvokeRecordSlowPacketActivityWithListener(TimeSpan elapsed, bool isProcessed)
    {
        Activity? recordedActivity = null;

        using var listener = new ActivityListener
                             {
                                 ShouldListenTo = source => source.Name == AppActivity.SrvSource.Name,
                                 Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
                                 ActivityStopped = activity => recordedActivity = activity
                             };

        ActivitySource.AddActivityListener(listener);

        TelemetryClient.RecordSlowPacketActivity(new ReceivedPacketData(), elapsed, isProcessed);

        return recordedActivity;
    }

    #endregion // Static methods
}