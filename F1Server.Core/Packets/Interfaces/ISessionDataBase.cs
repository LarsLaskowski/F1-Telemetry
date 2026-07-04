using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Data;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface with session data, which are equal in all game versions
/// </summary>
public interface ISessionDataBase
{
    #region Properties

    /// <summary>
    /// Is session recordable? Network games or time trial are not
    /// </summary>
    bool IsRecordable { get; set; }

    /// <summary>
    /// Weather condition
    /// </summary>
    WeatherCondition Weather { get; set; }

    /// <summary>
    /// Track temperature
    /// </summary>
    short TrackTemperature { get; set; }

    /// <summary>
    /// Air temperature
    /// </summary>
    short AirTemperature { get; set; }

    /// <summary>
    /// Total laps
    /// </summary>
    ushort TotalLaps { get; set; }

    /// <summary>
    /// Length of track
    /// </summary>
    int TrackLength { get; set; }

    /// <summary>
    /// Session type
    /// </summary>
    SessionType SessionType { get; set; }

    /// <summary>
    /// Track id
    /// </summary>
    ushort TrackId { get; set; }

    /// <summary>
    /// Name of the track
    /// </summary>
    string TrackName { get; set; }

    /// <summary>
    /// Formula type - F1 and so on
    /// </summary>
    Formula FormulaType { get; set; }

    /// <summary>
    /// Session time left in seconds
    /// </summary>
    int SessionTimeLeft { get; set; }

    /// <summary>
    /// Duration of current session
    /// </summary>
    int SessionDuration { get; set; }

    /// <summary>
    /// Pit speed limit
    /// </summary>
    ushort PitSpeedLimit { get; set; }

    /// <summary>
    /// Flag if game is paused
    /// </summary>
    bool IsGamePaused { get; set; }

    /// <summary>
    /// Flag if user is spectating
    /// </summary>
    bool IsSpectating { get; set; }

    /// <summary>
    /// Car index spectating
    /// </summary>
    ushort SpectatorCarIndex { get; set; }

    /// <summary>
    /// SLI Pro support
    /// </summary>
    bool IsSliProNativeSupport { get; set; }

    /// <summary>
    /// Number of marshal zones
    /// </summary>
    ushort MarshalZones { get; set; }

    /// <summary>
    /// Marshal zone data
    /// </summary>
    MarshalZone[] MarshalZone { get; }

    /// <summary>
    /// Safety car status
    /// </summary>
    SafetyCarStatus SafetyCar { get; set; }

    /// <summary>
    /// Online or offline
    /// </summary>
    bool IsNetworkGame { get; set; }

    #endregion // Properties
}