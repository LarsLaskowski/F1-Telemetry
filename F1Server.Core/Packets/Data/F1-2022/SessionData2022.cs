using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session data of F1 2022
/// </summary>
public class SessionData2022 : ISessionData2022
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionData2022()
    {
        MarshalZone = new MarshalZone[21];
        WeatherForecastSamples = new WeatherForecastSample[56];
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
    public short TrackTemperature { get; set; }

    /// <summary>
    /// Air temperature
    /// </summary>
    public short AirTemperature { get; set; }

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

    #region ISessionData2022

    /// <summary>
    /// Forecast accuracy
    /// </summary>
    public ForecastAccuracy ForecastAccuracy { get; set; }

    /// <summary>
    /// AI difficulty rating (0 - 110)
    /// </summary>
    public ushort AiDifficulty { get; set; }

    /// <summary>
    /// Identifier for season - persists across saves
    /// </summary>
    public uint SeasonLinkIdentifier { get; set; }

    /// <summary>
    /// Identifier for weekend - persists across saves
    /// </summary>
    public uint WeekendLinkIdentifier { get; set; }

    /// <summary>
    /// Identifier for session - persists across saves
    /// </summary>
    public uint SessionLinkIdentifier { get; set; }

    /// <summary>
    /// Ideal lap to pit on current strategy
    /// </summary>
    public ushort PitStopWindowIdealLap { get; set; }

    /// <summary>
    /// Latest lap to pit on current strategy
    /// </summary>
    public ushort PitStopWindowLatestLap { get; set; }

    /// <summary>
    /// Predicted position to rejoin at
    /// </summary>
    public ushort PitStopRejoinPosition { get; set; }

    /// <summary>
    /// Steering assist is on or off
    /// </summary>
    public bool IsSteeringAssist { get; set; }

    /// <summary>
    /// Braking assist setting
    /// </summary>
    public BrakingAssist BrakingAssist { get; set; }

    /// <summary>
    /// Gearbox assist setting
    /// </summary>
    public GearboxAssist GearboxAssist { get; set; }

    /// <summary>
    /// Pit assist on or off
    /// </summary>
    public bool IsPitAssist { get; set; }

    /// <summary>
    /// Pit release assist on or off
    /// </summary>
    public bool IsPitReleaseAssist { get; set; }

    /// <summary>
    /// ERS assist on or off
    /// </summary>
    public bool IsERSAssist { get; set; }

    /// <summary>
    /// DRS assist on or off
    /// </summary>
    public bool IsDRSAssist { get; set; }

    /// <summary>
    /// Dynamic race line settings
    /// </summary>
    public DynamicRaceLine DynamicRaceLine { get; set; }

    /// <summary>
    /// Type of dynamic race line
    /// </summary>
    public DynamicRaceLineType DynamicRaceLineType { get; set; }

    /// <summary>
    /// Game mode
    /// </summary>
    public GameMode GameMode { get; set; }

    /// <summary>
    /// Ruleset
    /// </summary>
    public RuleSet RuleSet { get; set; }

    /// <summary>
    /// Local time of day - minutes since midnight
    /// </summary>
    public uint LocalTimeOfDay { get; set; }

    /// <summary>
    /// Length of session
    /// </summary>
    public SessionLength SessionLength { get; set; }

    #endregion // ISessionData2022
}