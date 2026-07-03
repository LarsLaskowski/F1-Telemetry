using System.Diagnostics.Metrics;

using F1Server.Core.Enumerations;

namespace F1Server.Core.Interfaces;

/// <summary>
/// Defines the interface for application metrics used to track and report performance data
/// </summary>
public interface IAppMetrics
{
    #region Properties

    /// <summary>
    /// Gets the total number of packets received from the app
    /// </summary>
    Counter<long> PacketsReceived { get; }

    /// <summary>
    /// Gets the counter that tracks the total number of packets processed
    /// </summary>
    Counter<long> PacketsProcessed { get; }

    /// <summary>
    /// Gets the gauge that tracks the number of packets currently in the queue to be processed
    /// </summary>
    Gauge<long> PacketsInQueue { get; }

    /// <summary>
    /// Gets the histogram that measures the time taken to process packets
    /// </summary>
    Histogram<double> PacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of processing errors encountered
    /// </summary>
    Counter<long> ProcessingErrors { get; }

    /// <summary>
    /// Gets the total number of database errors encountered
    /// </summary>
    Counter<long> DbErrorCount { get; }

    /// <summary>
    /// Gets the counter that tracks the number of packets logged
    /// </summary>
    Counter<long> PacketsLogged { get; }

    /// <summary>
    /// Gets the histogram representing the time taken for packet logging operations
    /// </summary>
    Histogram<double> PacketLoggingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of event packets received
    /// </summary>
    Counter<long> EventPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for event packets
    /// </summary>
    Gauge<double> EventPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of session packets received
    /// </summary>
    Counter<long> SessionPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for session packets
    /// </summary>
    Gauge<double> SessionPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of participants packets received
    /// </summary>
    Counter<long> ParticipantsPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for participants packets
    /// </summary>
    Gauge<double> ParticipantsPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of lap data packets received
    /// </summary>
    Counter<long> LapDataPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for lap data packets
    /// </summary>
    Gauge<double> LapDataPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of car status packets received
    /// </summary>
    Counter<long> CarStatusPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for car status packets
    /// </summary>
    Gauge<double> CarStatusPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of car telemetry packets received
    /// </summary>
    Counter<long> CarTelemetryPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for car telemetry packets
    /// </summary>
    Gauge<double> CarTelemetryPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of car damage packets received
    /// </summary>
    Counter<long> CarDamagePackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for car damage packets
    /// </summary>
    Gauge<double> CarDamagePacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of car setup packets received
    /// </summary>
    Counter<long> CarSetupPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for car setup packets
    /// </summary>
    Gauge<double> CarSetupPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of motion packets received
    /// </summary>
    Counter<long> MotionPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for motion packets
    /// </summary>
    Gauge<double> MotionPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of extended motion packets received
    /// </summary>
    Counter<long> MotionExPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for extended motion packets
    /// </summary>
    Gauge<double> MotionExPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of final classification packets received
    /// </summary>
    Counter<long> FinalClassificationPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for final classification packets
    /// </summary>
    Gauge<double> FinalClassificationPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of session history packets received
    /// </summary>
    Counter<long> SessionHistoryPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for session history packets
    /// </summary>
    Gauge<double> SessionHistoryPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of tyre sets packets received
    /// </summary>
    Counter<long> TyreSetsPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for tyre sets packets
    /// </summary>
    Gauge<double> TyreSetsPacketProcessingTime { get; }

    /// <summary>
    /// Gets the counter that tracks the number of lap positions packets received
    /// </summary>
    Counter<long> LapPositionsPackets { get; }

    /// <summary>
    /// Gets the gauge that measures the processing time for lap positions packets
    /// </summary>
    Gauge<double> LapPositionsPacketProcessingTime { get; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Records the processing details of a packet, including its type and the time taken to process it
    /// </summary>
    /// <param name="packetType">The type of the packet being processed. Can be <see langword="null"/> if the packet type is unknown</param>
    /// <param name="processingTimeMs">The time, in milliseconds, taken to process the packet. Must be greater than or equal to 0</param>
    void RecordProcessedPacket(PacketTypes? packetType, double processingTimeMs);

    #endregion // Methods
}