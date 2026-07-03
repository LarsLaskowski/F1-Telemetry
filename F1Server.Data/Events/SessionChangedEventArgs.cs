namespace F1Server.Data.Events;

/// <summary>
/// Event arguments for session changed event
/// </summary>
public class SessionChangedEventArgs : EventArgs
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="currentSessionGameVersion">The current game version</param>
    /// <param name="lastSessionGameVersion">The previous game version</param>
    /// <param name="currentSessionId">The ID of the session that has changed</param>
    /// <param name="lastSessionId">The ID of the previous session</param>
    /// <param name="lastSessionMetrics">Metrics of the last session</param>
    public SessionChangedEventArgs(ushort currentSessionGameVersion, ushort lastSessionGameVersion, ulong currentSessionId, ulong lastSessionId, PacketsPerSessionMetrics lastSessionMetrics)
    {
        CurrentSessionGameVersion = currentSessionGameVersion;
        LastSessionGameVersion = lastSessionGameVersion;
        CurrentSessionId = currentSessionId;
        LastSessionId = lastSessionId;
        LastSessionMetrics = lastSessionMetrics;
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// The current game version
    /// </summary>
    public ushort CurrentSessionGameVersion { get; }

    /// <summary>
    /// The previous game version
    /// </summary>
    public ushort LastSessionGameVersion { get; }

    /// <summary>
    /// The ID of the session that has changed
    /// </summary>
    public ulong CurrentSessionId { get; }

    /// <summary>
    /// The ID of the previous session
    /// </summary>
    public ulong LastSessionId { get; }

    /// <summary>
    /// Metrics of the last session
    /// </summary>
    public PacketsPerSessionMetrics LastSessionMetrics { get; }

    #endregion // Properties
}