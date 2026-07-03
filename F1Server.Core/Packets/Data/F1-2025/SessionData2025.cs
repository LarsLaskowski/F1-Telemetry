using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session data of F1 2025
/// </summary>
public class SessionData2025 : SessionData2023, ISessionData2025
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionData2025()
        : base(64)
    {
        WeekendStructure = new SessionType[12];
    }

    #endregion // Constructors

    #region ISessionData2025 implemenation

    /// <inheritdoc/>
    public bool EqualCarPerformance { get; set; }

    /// <inheritdoc/>
    public RecoveryMode RecoveryMode { get; set; }

    /// <inheritdoc/>
    public FlashbackLimit FlashbackLimit { get; set; }

    /// <inheritdoc/>
    public bool IsRealisticSurfaceType { get; set; }

    /// <inheritdoc/>
    public bool IsHardLowFuelMode { get; set; }

    /// <inheritdoc/>
    public bool AssistedRaceStarts { get; set; }

    /// <inheritdoc/>
    public bool TyreTemperatureWithCarcass { get; set; }

    /// <inheritdoc/>
    public bool PitLaneTyreSimulation { get; set; }

    /// <inheritdoc/>
    public CarDamage CarDamage { get; set; }

    /// <inheritdoc/>
    public CarDamageRate CarDamageRate { get; set; }

    /// <inheritdoc/>
    public Collisions Collisions { get; set; }

    /// <inheritdoc/>
    public bool CollisionsOffForFirstLapOnly { get; set; }

    /// <inheritdoc/>
    public bool MultiPlayerUnsafePitRelease { get; set; }

    /// <inheritdoc/>
    public bool MultiPlayerOffForGriefing { get; set; }

    /// <inheritdoc/>
    public bool IsStrictCornerCuttingStringency { get; set; }

    /// <inheritdoc/>
    public bool ParcFermeRules { get; set; }

    /// <inheritdoc/>
    public PitStopExperience PitStopExperience { get; set; }

    /// <inheritdoc/>
    public SafetyCarAppearance SafetyCarAppearance { get; set; }

    /// <inheritdoc/>
    public bool IsImmersiveSafetyCarExperience { get; set; }

    /// <inheritdoc/>
    public bool WithFormationLap { get; set; }

    /// <inheritdoc/>
    public bool IsImmersiveFormationLapExperience { get; set; }

    /// <inheritdoc/>
    public RedFlagAppearance RedFlagAppearance { get; set; }

    /// <inheritdoc/>
    public bool AffectsLicenceLevelSolo { get; set; }

    /// <inheritdoc/>
    public bool AffectsLicenceLevelMultiPlayer { get; set; }

    /// <inheritdoc/>
    public short SessionsInWeekend { get; set; }

    /// <inheritdoc/>
    public SessionType[] WeekendStructure { get; }

    /// <inheritdoc/>
    public float Sector2LapDistanceStart { get; set; }

    /// <inheritdoc/>
    public float Sector3LapDistanceStart { get; set; }

    #endregion // ISessionData2025 implemenation
}