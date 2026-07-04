using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session data of F1 2020
/// </summary>
public class SessionData2020 : ISessionData2020
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionData2020()
    {
        MarshalZone = new MarshalZone[21];
        WeatherForecastSamples = new WeatherForecastSample[20];
    }

    #endregion // Constructors

    #region ISessionDataBase

    /// <summary>
    /// Is session recordable? Network games or time trial are not
    /// </summary>
    public bool IsRecordable { get; set; }

    /// <summary>
    /// Weather condition
    /// </summary>
    public WeatherCondition Weather { get; set; }

    /// <summary>
    /// Track temperature
    /// </summary>
    public ushort TrackTemperature { get; set; }

    /// <summary>
    /// Air temperature
    /// </summary>
    public ushort AirTemperature { get; set; }

    /// <summary>
    /// Total laps
    /// </summary>
    public ushort TotalLaps { get; set; }

    /// <summary>
    /// Length of track
    /// </summary>
    public int TrackLength { get; set; }

    /// <summary>
    /// Session type
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Track id
    /// </summary>
    public ushort TrackId { get; set; }

    /// <summary>
    /// Name of the track
    /// </summary>
    public string TrackName { get; set; }

    /// <summary>
    /// Formula type - F1 and so on
    /// </summary>
    public Formula FormulaType { get; set; }

    /// <summary>
    /// Session time left in seconds
    /// </summary>
    public int SessionTimeLeft { get; set; }

    /// <summary>
    /// Duration of current session
    /// </summary>
    public int SessionDuration { get; set; }

    /// <summary>
    /// Pit speed limit
    /// </summary>
    public ushort PitSpeedLimit { get; set; }

    /// <summary>
    /// Flag if game is paused
    /// </summary>
    public bool IsGamePaused { get; set; }

    /// <summary>
    /// Flag if user is spectating
    /// </summary>
    public bool IsSpectating { get; set; }

    /// <summary>
    /// Car index spectating
    /// </summary>
    public ushort SpectatorCarIndex { get; set; }

    /// <summary>
    /// SLI Pro support
    /// </summary>
    public bool IsSliProNativeSupport { get; set; }

    /// <summary>
    /// Number of marshal zones
    /// </summary>
    public ushort MarshalZones { get; set; }

    /// <summary>
    /// Marshal zone data
    /// </summary>
    public MarshalZone[] MarshalZone { get; set; }

    /// <summary>
    /// Safety car status
    /// </summary>
    public SafetyCarStatus SafetyCar { get; set; }

    /// <summary>
    /// Online or offline
    /// </summary>
    public bool IsNetworkGame { get; set; }

    #endregion // ISessionDataBase

    #region ISessionData2020

    /// <summary>
    /// Number of weather samples to follow
    /// </summary>
    public ushort NumberWeatherForecastSamples { get; set; }

    /// <summary>
    /// Weather forecast data
    /// </summary>
    public WeatherForecastSample[] WeatherForecastSamples { get; }

    #endregion // ISessionData2020
}