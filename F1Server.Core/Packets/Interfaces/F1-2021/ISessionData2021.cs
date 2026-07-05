using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Session data for F1 2021
/// </summary>
public interface ISessionData2021 : ISessionData2020
{
    #region Properties

    /// <summary>
    /// Forecast accuracy
    /// </summary>
    ForecastAccuracy ForecastAccuracy { get; }

    /// <summary>
    /// AI difficulty rating (0 - 110)
    /// </summary>
    ushort AiDifficulty { get; }

    /// <summary>
    /// Identifier for season - persists across saves
    /// </summary>
    uint SeasonLinkIdentifier { get; }

    /// <summary>
    /// Identifier for weekend - persists across saves
    /// </summary>
    uint WeekendLinkIdentifier { get; }

    /// <summary>
    /// Identifier for session - persists across saves
    /// </summary>
    uint SessionLinkIdentifier { get; }

    /// <summary>
    /// Ideal lap to pit on current strategy
    /// </summary>
    ushort PitStopWindowIdealLap { get; }

    /// <summary>
    /// Latest lap to pit on current strategy
    /// </summary>
    ushort PitStopWindowLatestLap { get; }

    /// <summary>
    /// Predicted position to rejoin at
    /// </summary>
    ushort PitStopRejoinPosition { get; }

    /// <summary>
    /// Steering assist is on or off
    /// </summary>
    bool IsSteeringAssist { get; }

    /// <summary>
    /// Braking assist setting
    /// </summary>
    BrakingAssist BrakingAssist { get; }

    /// <summary>
    /// Gearbox assist setting
    /// </summary>
    GearboxAssist GearboxAssist { get; }

    /// <summary>
    /// Pit assist on or off
    /// </summary>
    bool IsPitAssist { get; }

    /// <summary>
    /// Pit release assist on or off
    /// </summary>
    bool IsPitReleaseAssist { get; }

    /// <summary>
    /// ERS assist on or off
    /// </summary>
    bool IsERSAssist { get; }

    /// <summary>
    /// DRS assist on or off
    /// </summary>
    bool IsDRSAssist { get; }

    /// <summary>
    /// Dynamic race line settings
    /// </summary>
    DynamicRaceLine DynamicRaceLine { get; }

    /// <summary>
    /// Type of dynamic race line
    /// </summary>
    DynamicRaceLineType DynamicRaceLineType { get; }

    #endregion // Properties
}