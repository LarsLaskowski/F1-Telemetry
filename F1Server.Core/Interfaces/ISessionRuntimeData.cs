using F1Server.Core.Enumerations;

namespace F1Server.Core.Interfaces;

/// <summary>
/// Interface for session runtime data
/// </summary>
public interface ISessionRuntimeData
{
    #region Properties

    /// <summary>
    /// Database id of session
    /// </summary>
    long SessionDbId { get; }

    /// <summary>
    /// Current game session id
    /// </summary>
    ulong CurrentSessionId { get; }

    /// <summary>
    /// Actual game version
    /// </summary>
    int GameVersion { get; }

    /// <summary>
    /// Current type of session
    /// </summary>
    SessionType CurrentSessionType { get; }

    /// <summary>
    /// Flag, if participants available
    /// </summary>
    bool HasParticipants { get; }

    /// <summary>
    /// Telemetry recording active?
    /// </summary>
    bool IsTelemetryRecording { get; }

    /// <summary>
    /// Air temperature
    /// </summary>
    int AirTemperature { get; }

    /// <summary>
    /// Track temperature
    /// </summary>
    int TrackTemperature { get; }

    /// <summary>
    /// Fastest sector 1 time in milliseconds
    /// </summary>
    uint FastestSector1 { get; }

    /// <summary>
    /// Fastest sector 2 time in milliseconds
    /// </summary>
    uint FastestSector2 { get; }

    /// <summary>
    /// Fastest sector 3 time in milliseconds
    /// </summary>
    uint FastestSector3 { get; }

    /// <summary>
    /// Fastest lap time in milliseconds
    /// </summary>
    uint FastestLap { get; }

    #endregion // Properties
}