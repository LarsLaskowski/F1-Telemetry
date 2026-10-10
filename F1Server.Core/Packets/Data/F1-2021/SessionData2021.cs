using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session data of F1 2021
/// </summary>
public class SessionData2021 : ISessionData2021
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionData2021()
    {
        MarshalZones = new MarshalZone[21];
        WeatherForecastSamples = new WeatherForecastSample[56];
    }

    #endregion // Constructors

    #region ISessionDataBase

    /// <inheritdoc/>
    public bool IsRecordable { get; set; }

    /// <inheritdoc/>
    public WeatherCondition Weather { get; set; }

    /// <inheritdoc/>
    public short TrackTemperature { get; set; }

    /// <inheritdoc/>
    public short AirTemperature { get; set; }

    /// <inheritdoc/>
    public ushort TotalLaps { get; set; }

    /// <inheritdoc/>
    public int TrackLength { get; set; }

    /// <inheritdoc/>
    public SessionType SessionType { get; set; }

    /// <inheritdoc/>
    public short TrackId { get; set; }

    /// <inheritdoc/>
    public string TrackName { get; set; }

    /// <inheritdoc/>
    public Formula FormulaType { get; set; }

    /// <inheritdoc/>
    public int SessionTimeLeft { get; set; }

    /// <inheritdoc/>
    public int SessionDuration { get; set; }

    /// <inheritdoc/>
    public ushort PitSpeedLimit { get; set; }

    /// <inheritdoc/>
    public bool IsGamePaused { get; set; }

    /// <inheritdoc/>
    public bool IsSpectating { get; set; }

    /// <inheritdoc/>
    public ushort SpectatorCarIndex { get; set; }

    /// <inheritdoc/>
    public bool IsSliProNativeSupport { get; set; }

    /// <inheritdoc/>
    public ushort NumberOfMarshalZones { get; set; }

    /// <inheritdoc/>
    public MarshalZone[] MarshalZones { get; set; }

    /// <inheritdoc/>
    public SafetyCarStatus SafetyCar { get; set; }

    /// <inheritdoc/>
    public bool IsNetworkGame { get; set; }

    #endregion // ISessionDataBase

    #region ISessionData2020

    /// <inheritdoc/>
    public ushort NumberWeatherForecastSamples { get; set; }

    /// <inheritdoc/>
    public WeatherForecastSample[] WeatherForecastSamples { get; }

    #endregion // ISessionData2020

    #region ISessionData2021

    /// <inheritdoc/>
    public ForecastAccuracy ForecastAccuracy { get; set; }

    /// <inheritdoc/>
    public ushort AiDifficulty { get; set; }

    /// <inheritdoc/>
    public uint SeasonLinkIdentifier { get; set; }

    /// <inheritdoc/>
    public uint WeekendLinkIdentifier { get; set; }

    /// <inheritdoc/>
    public uint SessionLinkIdentifier { get; set; }

    /// <inheritdoc/>
    public ushort PitStopWindowIdealLap { get; set; }

    /// <inheritdoc/>
    public ushort PitStopWindowLatestLap { get; set; }

    /// <inheritdoc/>
    public ushort PitStopRejoinPosition { get; set; }

    /// <inheritdoc/>
    public bool IsSteeringAssist { get; set; }

    /// <inheritdoc/>
    public BrakingAssist BrakingAssist { get; set; }

    /// <inheritdoc/>
    public GearboxAssist GearboxAssist { get; set; }

    /// <inheritdoc/>
    public bool IsPitAssist { get; set; }

    /// <inheritdoc/>
    public bool IsPitReleaseAssist { get; set; }

    /// <inheritdoc/>
    public bool IsERSAssist { get; set; }

    /// <inheritdoc/>
    public bool IsDRSAssist { get; set; }

    /// <inheritdoc/>
    public DynamicRaceLine DynamicRaceLine { get; set; }

    /// <inheritdoc/>
    public DynamicRaceLineType DynamicRaceLineType { get; set; }

    #endregion // ISessionData2021
}