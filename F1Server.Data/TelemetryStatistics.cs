using F1Server.Data.Events;

namespace F1Server.Data;

/// <summary>
/// Statistics of running telemetry client
/// </summary>
public class TelemetryStatistics
{
    #region Fields

    private long _packetsReceivedTotal;
    private long _packetsReceivedCurrentSession;

    #endregion // Fields

    #region Events

    /// <summary>
    /// Event raised when session has changed
    /// </summary>
    public event SessionChangedEventHandler SessionChanged;

    #endregion // Events

    #region Properties

    /// <summary>
    /// Current session id
    /// </summary>
    public ulong CurrentSessionId { get; private set; }

    /// <summary>
    /// Current session game version
    /// </summary>
    public ushort CurrentSessionGameVersion { get; private set; }

    /// <summary>
    /// Last session id
    /// </summary>
    public ulong LastSessionId { get; private set; }

    /// <summary>
    /// Last session game version
    /// </summary>
    public ushort LastSessionGameVersion { get; private set; }

    /// <summary>
    /// Total numbers of packets received
    /// </summary>
    public long PacketsReceivedTotal => Volatile.Read(ref _packetsReceivedTotal);

    /// <summary>
    /// Number of packets received for current session
    /// </summary>
    public long PacketsReceivedCurrentSession => Volatile.Read(ref _packetsReceivedCurrentSession);

    /// <summary>
    /// Number of packets received in last session
    /// </summary>
    public long PacketsReceivedLastSession { get; set; }

    /// <summary>
    /// Number of packets currently in processing queue
    /// </summary>
    public long PacketsInQueue { get; set; }

    /// <summary>
    /// Number of packets currently queued in packet processor
    /// </summary>
    public long PacketsInProcessorQueue { get; set; }

    /// <summary>
    /// Total time in milliseconds processing all packets
    /// </summary>
    public double TotalPacketProcessingTime { get; set; }

    /// <summary>
    /// Total number of all processed packets
    /// </summary>
    public long TotalPacketsProcessed { get; set; }

    /// <summary>
    /// Current session metrics
    /// </summary>
    public PacketsPerSessionMetrics CurrentSessionMetrics { get; } = new();

    /// <summary>
    /// Last session metrics
    /// </summary>
    public PacketsPerSessionMetrics LastSessionMetrics { get; } = new();

    /// <summary>
    /// Total time in milliseconds for logging all packets to disk
    /// </summary>
    public double TotalPacketLogTime { get; set; }

    /// <summary>
    /// Total number of logged packets
    /// </summary>
    public long TotalPacketsLogged { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Atomically increments the total and the current session packet counters
    /// </summary>
    public void IncrementPacketsReceived()
    {
        Interlocked.Increment(ref _packetsReceivedTotal);
        Interlocked.Increment(ref _packetsReceivedCurrentSession);
    }

    /// <summary>
    /// Check whether the current session has changed
    /// </summary>
    /// <param name="sessionId">Session id from last received packet</param>
    /// <param name="gameVersion">Game version from last received packet</param>
    public void CheckChangeSession(ulong? sessionId, ushort? gameVersion)
    {
        if (sessionId != null && sessionId != CurrentSessionId)
        {
            LastSessionId = CurrentSessionId;

            if (gameVersion != null)
            {
                LastSessionGameVersion = CurrentSessionGameVersion;
                CurrentSessionGameVersion = gameVersion.Value;
            }

            // Copy current session metrics to last session metrics
            if (CurrentSessionId > 0)
            {
                PacketsReceivedLastSession = Interlocked.Exchange(ref _packetsReceivedCurrentSession, 0);

                LastSessionMetrics.CopyFrom(CurrentSessionMetrics);

                CurrentSessionMetrics.Reset();
            }

            CurrentSessionId = sessionId.Value;

            SessionChanged?.Invoke(this, new(CurrentSessionGameVersion, LastSessionGameVersion, CurrentSessionId, LastSessionId, LastSessionMetrics));
        }
    }

    #endregion // Methods
}