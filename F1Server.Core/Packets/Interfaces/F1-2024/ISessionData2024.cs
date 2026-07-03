using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Session data for F1 2024
/// </summary>
public interface ISessionData2024 : ISessionData2023
{
    #region Properties

    /// <summary>
    /// Equal car performance?
    /// </summary>
    bool EqualCarPerformance { get; set; }

    /// <summary>
    /// Recovery mode
    /// </summary>
    RecoveryMode RecoveryMode { get; set; }

    /// <summary>
    /// Flashback limit
    /// </summary>
    FlashbackLimit FlashbackLimit { get; set; }

    /// <summary>
    /// Realistic (true) surface type or simplified (false)
    /// </summary>
    bool IsRealisticSurfaceType { get; set; }

    /// <summary>
    /// Hard low fuel mode (true) or easy (false)
    /// </summary>
    bool IsHardLowFuelMode { get; set; }

    /// <summary>
    /// Assisted race starts or manual (false)?
    /// </summary>
    bool AssistedRaceStarts { get; set; }

    /// <summary>
    /// Tyre temperature with carcass (true) or surface only (false)
    /// </summary>
    bool TyreTemperatureWithCarcass { get; set; }

    /// <summary>
    /// With formation lap?
    /// </summary>
    bool PitLaneTyreSimulation { get; set; }

    /// <summary>
    /// Car damage
    /// </summary>
    CarDamage CarDamage { get; set; }

    /// <summary>
    /// Car damage rate
    /// </summary>
    CarDamageRate CarDamageRate { get; set; }

    /// <summary>
    /// Collions
    /// </summary>
    Collisions Collisions { get; set; }

    /// <summary>
    /// Collisions off for first lap only
    /// </summary>
    bool CollisionsOffForFirstLapOnly { get; set; }

    /// <summary>
    /// Unsafe pit release (multi player)
    /// </summary>
    bool MultiPlayerUnsafePitRelease { get; set; }

    /// <summary>
    /// Off for griefing (multi player)
    /// </summary>
    bool MultiPlayerOffForGriefing { get; set; }

    /// <summary>
    /// Strict corner cutting stringency or regular
    /// </summary>
    bool IsStrictCornerCuttingStringency { get; set; }

    /// <summary>
    /// Parc ferme rules?
    /// </summary>
    bool ParcFermeRules { get; set; }

    /// <summary>
    /// Pit stop experience
    /// </summary>
    PitStopExperience PitStopExperience { get; set; }

    /// <summary>
    /// Safety car
    /// </summary>
    SafetyCarAppearance SafetyCarAppearance { get; set; }

    /// <summary>
    /// Immersive safety car experience or broadcast?
    /// </summary>
    bool IsImmersiveSafetyCarExperience { get; set; }

    /// <summary>
    /// With formation lap?
    /// </summary>
    bool WithFormationLap { get; set; }

    /// <summary>
    /// Immersive formation lap experience or broadcast
    /// </summary>
    bool IsImmersiveFormationLapExperience { get; set; }

    /// <summary>
    /// Red flags
    /// </summary>
    RedFlagAppearance RedFlagAppearance { get; set; }

    /// <summary>
    /// Affects license level in solo
    /// </summary>
    bool AffectsLicenceLevelSolo { get; set; }

    /// <summary>
    /// Affects license level in multi player
    /// </summary>
    bool AffectsLicenceLevelMultiPlayer { get; set; }

    /// <summary>
    /// Number of sessions in weekend
    /// </summary>
    short SessionsInWeekend { get; set; }

    /// <summary>
    /// List of session types to show weekend
    /// </summary>
    SessionType[] WeekendStructure { get; }

    /// <summary>
    /// Distance in meters around track where sector 2 starts
    /// </summary>
    float Sector2LapDistanceStart { get; set; }

    /// <summary>
    /// Distance in meters around track where sector 3 starts
    /// </summary>
    float Sector3LapDistanceStart { get; set; }

    #endregion // Properties
}