using System.Diagnostics.Metrics;

using F1Server.Core.Enumerations;
using F1Server.Core.Interfaces;
using F1Server.Core.Observability;

namespace F1Server.Observability;

/// <summary>
/// Provides metrics for tracking application performance
/// </summary>
public class AppMetrics : IAppMetrics
{
    #region Constants

    /// <summary>
    /// The default metric name used for telemetry purposes
    /// </summary>
    private const string DefaultMetricName = "f1telemetry";

    /// <summary>
    /// Represents the default type identifier for packets
    /// </summary>
    private const string DefaultTypePacket = "packets";

    /// <summary>
    /// Represents the default unit of time measurement, specified as milliseconds
    /// </summary>
    private const string DefaultTypeMilliseconds = "ms";

    #endregion // Constants

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AppMetrics"/> class with the specified meter
    /// </summary>
    public AppMetrics()
    {
        var instrumentAdvice = new InstrumentAdvice<double>
                               {
                                   HistogramBucketBoundaries = [0.001, 0.01, 0.5, 1, 5, 10, 15, 20, 35, 50, 75, 100, 150]
                               };

        PacketsReceived = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.packets_received", DefaultTypePacket, "Total number of packets received from the game");
        PacketsProcessed = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.packets_processed", DefaultTypePacket, "Total number of packets processed");
        PacketsInQueue = MetricsMeter.CreateGauge<long>($"{DefaultMetricName}.packets_in_queue", DefaultTypePacket, "Number of packets in queue to be processed");
        PacketProcessingTime = MetricsMeter.CreateHistogram<double>($"{DefaultMetricName}.packet_processing_time", DefaultTypeMilliseconds, "Time taken to process packets", null, instrumentAdvice);
        ProcessingErrors = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.processing_errors", "errors", "Number of packet processing errors encountered");
        DbErrorCount = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.db_errors", "errors", "Total number of database errors encountered");
        PacketsLogged = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.packets_logged", DefaultTypePacket, "Number of packets logged");
        PacketLoggingTime = MetricsMeter.CreateHistogram<double>($"{DefaultMetricName}.packet_logging_time", DefaultTypeMilliseconds, "Time taken for packet logging operations", null, instrumentAdvice);
        EventPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.event_packets", DefaultTypePacket, "Number of event packets received");
        EventPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.event_packet_processing_time", DefaultTypeMilliseconds, "Processing time for event packets");
        SessionPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.session_packets", DefaultTypePacket, "Number of session packets received");
        SessionPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.session_packet_processing_time", DefaultTypeMilliseconds, "Processing time for session packets");
        ParticipantsPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.participants_packets", DefaultTypePacket, "Number of participants packets received");
        ParticipantsPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.participants_packet_processing_time", DefaultTypeMilliseconds, "Processing time for participants packets");
        LapDataPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.lapdata_packets", DefaultTypePacket, "Number of lap data packets received");
        LapDataPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.lapdata_packet_processing_time", DefaultTypeMilliseconds, "Processing time for lap data packets");
        CarStatusPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.carstatus_packets", DefaultTypePacket, "Number of car status packets received");
        CarStatusPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.carstatus_packet_processing_time", DefaultTypeMilliseconds, "Processing time for car status packets");
        CarTelemetryPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.cartelemetry_packets", DefaultTypePacket, "Number of car telemetry packets received");
        CarTelemetryPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.cartelemetry_packet_processing_time", DefaultTypeMilliseconds, "Processing time for car telemetry packets");
        CarDamagePackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.cardamage_packets", DefaultTypePacket, "Number of car damage packets received");
        CarDamagePacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.cardamage_packet_processing_time", DefaultTypeMilliseconds, "Processing time for car damage packets");
        CarSetupPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.carsetup_packets", DefaultTypePacket, "Number of car setup packets received");
        CarSetupPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.carsetup_packet_processing_time", DefaultTypeMilliseconds, "Processing time for car setup packets");
        MotionPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.motion_packets", DefaultTypePacket, "Number of motion packets received");
        MotionPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.motion_packet_processing_time", DefaultTypeMilliseconds, "Processing time for motion packets");
        MotionExPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.motionex_packets", DefaultTypePacket, "Number of extended motion packets received");
        MotionExPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.motionex_packet_processing_time", DefaultTypeMilliseconds, "Processing time for extended motion packets");
        FinalClassificationPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.finalclassification_packets", DefaultTypePacket, "Number of final classification packets received");
        FinalClassificationPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.finalclassification_packet_processing_time", DefaultTypeMilliseconds, "Processing time for final classification packets");
        SessionHistoryPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.sessionhistory_packets", DefaultTypePacket, "Number of session history packets received");
        SessionHistoryPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.sessionhistory_packet_processing_time", DefaultTypeMilliseconds, "Processing time for session history packets");
        TyreSetsPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.tyresets_packets", DefaultTypePacket, "Number of tyre sets packets received");
        TyreSetsPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.tyresets_packet_processing_time", DefaultTypeMilliseconds, "Processing time for tyre sets packets");
        LapPositionsPackets = MetricsMeter.CreateCounter<long>($"{DefaultMetricName}.lappositions_packets", DefaultTypePacket, "Number of lap positions packets received");
        LapPositionsPacketProcessingTime = MetricsMeter.CreateGauge<double>($"{DefaultMetricName}.lappositions_packet_processing_time", DefaultTypeMilliseconds, "Processing time for lap positions packets");

        InitMeters();
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Gets the <see cref="Meter"/> instance used for recording and tracking telemetry metrics
    /// </summary>
    public Meter MetricsMeter { get; } = new Meter(MetricsName.F1MeterName);

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Initializes the meters to zero so they are visible in Prometheus
    /// </summary>
    private void InitMeters()
    {
        PacketsReceived.Add(0);
        PacketsProcessed.Add(0);
        PacketsInQueue.Record(0);
        PacketProcessingTime.Record(0);
        ProcessingErrors.Add(0);
        DbErrorCount.Add(0);
        PacketsLogged.Add(0);
        PacketLoggingTime.Record(0);
        EventPackets.Add(0);
        EventPacketProcessingTime.Record(0);
        SessionPackets.Add(0);
        SessionPacketProcessingTime.Record(0);
        ParticipantsPackets.Add(0);
        ParticipantsPacketProcessingTime.Record(0);
        LapDataPackets.Add(0);
        LapDataPacketProcessingTime.Record(0);
        CarStatusPackets.Add(0);
        CarStatusPacketProcessingTime.Record(0);
        CarTelemetryPackets.Add(0);
        CarTelemetryPacketProcessingTime.Record(0);
        CarDamagePackets.Add(0);
        CarDamagePacketProcessingTime.Record(0);
        CarSetupPackets.Add(0);
        CarSetupPacketProcessingTime.Record(0);
        MotionPackets.Add(0);
        MotionPacketProcessingTime.Record(0);
        MotionExPackets.Add(0);
        MotionExPacketProcessingTime.Record(0);
        FinalClassificationPackets.Add(0);
        FinalClassificationPacketProcessingTime.Record(0);
        SessionHistoryPackets.Add(0);
        SessionHistoryPacketProcessingTime.Record(0);
        TyreSetsPackets.Add(0);
        TyreSetsPacketProcessingTime.Record(0);
        LapPositionsPackets.Add(0);
        LapPositionsPacketProcessingTime.Record(0);
    }

    #endregion // Methods

    #region IAppMetrics

    /// <inheritdoc/>
    public Counter<long> PacketsReceived { get; }

    /// <inheritdoc/>
    public Counter<long> PacketsProcessed { get; }

    /// <inheritdoc/>
    public Gauge<long> PacketsInQueue { get; }

    /// <inheritdoc/>
    public Histogram<double> PacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> ProcessingErrors { get; }

    /// <inheritdoc/>
    public Counter<long> DbErrorCount { get; }

    /// <inheritdoc/>
    public Counter<long> PacketsLogged { get; }

    /// <inheritdoc/>
    public Histogram<double> PacketLoggingTime { get; }

    /// <inheritdoc/>
    public Counter<long> EventPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> EventPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> SessionPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> SessionPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> ParticipantsPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> ParticipantsPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> LapDataPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> LapDataPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> CarStatusPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> CarStatusPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> CarTelemetryPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> CarTelemetryPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> CarDamagePackets { get; }

    /// <inheritdoc/>
    public Gauge<double> CarDamagePacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> CarSetupPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> CarSetupPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> MotionPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> MotionPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> MotionExPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> MotionExPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> FinalClassificationPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> FinalClassificationPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> SessionHistoryPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> SessionHistoryPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> TyreSetsPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> TyreSetsPacketProcessingTime { get; }

    /// <inheritdoc/>
    public Counter<long> LapPositionsPackets { get; }

    /// <inheritdoc/>
    public Gauge<double> LapPositionsPacketProcessingTime { get; }

    /// <inheritdoc/>
    public void RecordProcessedPacket(PacketTypes? packetType, double processingTimeMs)
    {
        if (packetType != null)
        {
            PacketsProcessed.Add(1, new KeyValuePair<string, object?>("PacketType", packetType));
            PacketProcessingTime.Record(processingTimeMs, new KeyValuePair<string, object?>("PacketType", packetType));

            switch (packetType)
            {
                case PacketTypes.Event:
                    {
                        EventPackets.Add(1);
                        EventPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.Session:
                    {
                        SessionPackets.Add(1);
                        SessionPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.Participants:
                    {
                        ParticipantsPackets.Add(1);
                        ParticipantsPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.LapData:
                    {
                        LapDataPackets.Add(1);
                        LapDataPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.CarStatus:
                    {
                        CarStatusPackets.Add(1);
                        CarStatusPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.CarTelemetry:
                    {
                        CarTelemetryPackets.Add(1);
                        CarTelemetryPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.CarDamage:
                    {
                        CarDamagePackets.Add(1);
                        CarDamagePacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.CarSetups:
                    {
                        CarSetupPackets.Add(1);
                        CarSetupPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.Motion:
                    {
                        MotionPackets.Add(1);
                        MotionPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.MotionEx:
                    {
                        MotionExPackets.Add(1);
                        MotionExPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.FinalClassification:
                    {
                        FinalClassificationPackets.Add(1);
                        FinalClassificationPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.SessionHistory:
                    {
                        SessionHistoryPackets.Add(1);
                        SessionHistoryPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.TyreSets:
                    {
                        TyreSetsPackets.Add(1);
                        TyreSetsPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;

                case PacketTypes.LapPositions:
                    {
                        LapPositionsPackets.Add(1);
                        LapPositionsPacketProcessingTime.Record(processingTimeMs);
                    }
                    break;
            }
        }
    }

    #endregion // IAppMetrics
}