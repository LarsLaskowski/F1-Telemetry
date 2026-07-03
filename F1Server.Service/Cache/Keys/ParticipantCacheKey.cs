namespace F1Server.Service.Cache.Keys;

/// <summary>
/// Key for identifying a participant by session and driver
/// </summary>
internal readonly struct ParticipantCacheKey
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ParticipantCacheKey"/> struct
    /// </summary>
    /// <param name="sessionId">Session id</param>
    /// <param name="driverId">Driver id</param>
    public ParticipantCacheKey(long sessionId, long driverId)
    {
        SessionId = sessionId;
        DriverId = driverId;
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Gets the session id
    /// </summary>
    public long SessionId { get; }

    /// <summary>
    /// Gets the driver id
    /// </summary>
    public long DriverId { get; }

    #endregion // Properties

    #region Object

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(SessionId, DriverId);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is ParticipantCacheKey other && other.SessionId == SessionId && other.DriverId == DriverId;
    }

    #endregion // Object
}