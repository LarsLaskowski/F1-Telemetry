using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session data of F1 2024
/// </summary>
public class SessionData2024 : SessionData2023, ISessionData2024
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionData2024()
        : base(64)
    {
        WeekendStructure = new SessionType[12];
    }

    #endregion // Constructors

    #region ISessionData2024 implemenation

    /// <summary>
    /// Equal car performance?
    /// </summary>
    public bool EqualCarPerformance { get; set; }

    /// <summary>
    /// Recovery mode
    /// </summary>
    public RecoveryMode RecoveryMode { get; set; }

    /// <summary>
    /// Flashback limit
    /// </summary>
    public FlashbackLimit FlashbackLimit { get; set; }

    /// <summary>
    /// Realistic (true) surface type or simplified (false)
    /// </summary>
    public bool IsRealisticSurfaceType { get; set; }

    /// <summary>
    /// Hard low fuel mode (true) or easy (false)
    /// </summary>
    public bool IsHardLowFuelMode { get; set; }

    /// <summary>
    /// Assisted race starts or manual (false)?
    /// </summary>
    public bool AssistedRaceStarts { get; set; }

    /// <summary>
    /// Tyre temperature with carcass (true) or surface only (false)
    /// </summary>
    public bool TyreTemperatureWithCarcass { get; set; }

    /// <summary>
    /// With formation lap?
    /// </summary>
    public bool PitLaneTyreSimulation { get; set; }

    /// <summary>
    /// Car damage
    /// </summary>
    public CarDamage CarDamage { get; set; }

    /// <summary>
    /// Car damage rate
    /// </summary>
    public CarDamageRate CarDamageRate { get; set; }

    /// <summary>
    /// Collions
    /// </summary>
    public Collisions Collisions { get; set; }

    /// <summary>
    /// Collisions off for first lap only
    /// </summary>
    public bool CollisionsOffForFirstLapOnly { get; set; }

    /// <summary>
    /// Unsafe pit release (multi player)
    /// </summary>
    public bool MultiPlayerUnsafePitRelease { get; set; }

    /// <summary>
    /// Off for griefing (multi player)
    /// </summary>
    public bool MultiPlayerOffForGriefing { get; set; }

    /// <summary>
    /// Strict corner cutting stringency or regular
    /// </summary>
    public bool IsStrictCornerCuttingStringency { get; set; }

    /// <summary>
    /// Parc ferme rules?
    /// </summary>
    public bool ParcFermeRules { get; set; }

    /// <summary>
    /// Pit stop experience
    /// </summary>
    public PitStopExperience PitStopExperience { get; set; }

    /// <summary>
    /// Safety car
    /// </summary>
    public SafetyCarAppearance SafetyCarAppearance { get; set; }

    /// <summary>
    /// Immersive safety car experience or broadcast?
    /// </summary>
    public bool IsImmersiveSafetyCarExperience { get; set; }

    /// <summary>
    /// With formation lap?
    /// </summary>
    public bool WithFormationLap { get; set; }

    /// <summary>
    /// Immersive formation lap experience or broadcast
    /// </summary>
    public bool IsImmersiveFormationLapExperience { get; set; }

    /// <summary>
    /// Red flags
    /// </summary>
    public RedFlagAppearance RedFlagAppearance { get; set; }

    /// <summary>
    /// Affects license level in solo
    /// </summary>
    public bool AffectsLicenceLevelSolo { get; set; }

    /// <summary>
    /// Affects license level in multi player
    /// </summary>
    public bool AffectsLicenceLevelMultiPlayer { get; set; }

    /// <summary>
    /// Number of sessions in weekend
    /// </summary>
    public short SessionsInWeekend { get; set; }

    /// <summary>
    /// List of session types to show weekend
    /// </summary>
    public SessionType[] WeekendStructure { get; }

    /// <summary>
    /// Distance in meters around track where sector 2 starts
    /// </summary>
    public float Sector2LapDistanceStart { get; set; }

    /// <summary>
    /// Distance in meters around track where sector 3 starts
    /// </summary>
    public float Sector3LapDistanceStart { get; set; }

    #endregion // ISessionData2024 implemenation
}