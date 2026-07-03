namespace F1Server.Core.Enumerations;

/// <summary>
/// Enumeration of infringement types (the value from game is 0 based)
/// </summary>
public enum InfringementType
{
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Blocking by slow driving
    /// </summary>
    BlockingBySlowDriving,

    /// <summary>
    /// Blocking by wrong way driving
    /// </summary>
    BlockingByWrongWayDriving,

    /// <summary>
    /// Reversing off the start line
    /// </summary>
    ReversingOffTheStartLine,

    /// <summary>
    /// Big collision
    /// </summary>
    BigCollision,

    /// <summary>
    /// Small collision
    /// </summary>
    SmallCollision,

    /// <summary>
    /// Collision failed to hand back position single
    /// </summary>
    CollisionFailedSingle,

    /// <summary>
    /// Collision failed to hand back position multiple
    /// </summary>
    CollisionFailedMultiple,

    /// <summary>
    /// Corner cutting gained time
    /// </summary>
    CornerCuttingTime,

    /// <summary>
    /// Corner cutting overtake single
    /// </summary>
    CornerCuttingSingle,

    /// <summary>
    /// Corner cutting overtake multiple
    /// </summary>
    CornerCuttingMultiple,

    /// <summary>
    /// Crossed pit exit lane
    /// </summary>
    CrossedPitExitLane,

    /// <summary>
    /// Ignoring blue flags
    /// </summary>
    IgnoringBlueFlag,

    /// <summary>
    /// Ignoring yellow flags
    /// </summary>
    IgnoringYellowFlag,

    /// <summary>
    /// Ignoring drive through
    /// </summary>
    IgnoringDriveThrough,

    /// <summary>
    /// Too many drive throughs
    /// </summary>
    TooManyDriveThroughs,

    /// <summary>
    /// Drive through reminder serve within n laps
    /// </summary>
    DriveThroughReminder,

    /// <summary>
    /// Drive through reminder serve this lap
    /// </summary>
    DriveThrouhgReminderThisLap,

    /// <summary>
    /// Pit lane speeding
    /// </summary>
    PitLaneSpeeding,

    /// <summary>
    /// Parked for too long
    /// </summary>
    ParkedTooLong,

    /// <summary>
    /// Ignoring tyre regulations
    /// </summary>
    IgnoringTyreRegulations,

    /// <summary>
    /// Too many penalties
    /// </summary>
    TooManyPenalties,

    /// <summary>
    /// Multiple warnings
    /// </summary>
    MultipleWarnings,

    /// <summary>
    /// Approaching disqualification
    /// </summary>
    ApproachingDisqualification,

    /// <summary>
    /// Tyre regulations select single
    /// </summary>
    TyreRegulationsSingle,

    /// <summary>
    /// Tyre regulations select multiple
    /// </summary>
    TyreRegulationsMultiple,

    /// <summary>
    /// Lap invalidated corner cutting
    /// </summary>
    LapInvalidCorner,

    /// <summary>
    /// Lap invalidated running wide
    /// </summary>
    LapInvalidRunningWide,

    /// <summary>
    /// Corner cutting ran wide gained time minor
    /// </summary>
    CornerCuttingGainedMinor,

    /// <summary>
    /// Corner cutting ran wide gained time significant
    /// </summary>
    CornerCuttingGainedSignificant,

    /// <summary>
    /// Corner cutting ran wide gained time extreme
    /// </summary>
    CornerCuttingGainedExtreme,

    /// <summary>
    /// Lap invalidated wall riding
    /// </summary>
    LapInvalidWall,

    /// <summary>
    /// Lap invalidated flashback used
    /// </summary>
    LapInvalidFlashback,

    /// <summary>
    /// Lap invalidated reset to track
    /// </summary>
    LapInvalidReset,

    /// <summary>
    /// Blocking the pitlane
    /// </summary>
    BlockingPitLane,

    /// <summary>
    /// Jump start
    /// </summary>
    JumpStart,

    /// <summary>
    /// Safety car to car collision
    /// </summary>
    SafetyCarCollision,

    /// <summary>
    /// Safety car illegal overtake
    /// </summary>
    SafetyCarOvertake,

    /// <summary>
    /// Safety car exceeding allowed pace
    /// </summary>
    SafetyCarExceedingPace,

    /// <summary>
    /// Virtual safety car exceeding allowed pace
    /// </summary>
    VirtualSafetyCarExceedingPace,

    /// <summary>
    /// Formation lap below allowed speed
    /// </summary>
    FormationLapLowSpeed,

    /// <summary>
    /// Formation lap parking
    /// </summary>
    FormationLapParking,

    /// <summary>
    /// Retired mechanical failure
    /// </summary>
    RetiredMechanicalFailure,

    /// <summary>
    /// Retired terminally damaged
    /// </summary>
    RetiredDamaged,

    /// <summary>
    /// Safety car falling too far back
    /// </summary>
    SafetyCarTooFarBack,

    /// <summary>
    /// Black flag timer
    /// </summary>
    BlackFlagTimer,

    /// <summary>
    /// Unserved stop go penalty
    /// </summary>
    UnservedStopAndGo,

    /// <summary>
    /// Unserved drive through penalty
    /// </summary>
    UnservedDriveThrough,

    /// <summary>
    /// Engine component change
    /// </summary>
    EngineComponentChange,

    /// <summary>
    /// Gearbox change
    /// </summary>
    GearboxChange,

    /// <summary>
    /// Parc Fermé change
    /// </summary>
    ParcFermeChange,

    /// <summary>
    /// League grid penalty
    /// </summary>
    LeagueGridPenalty,

    /// <summary>
    /// Retry penalty
    /// </summary>
    RetryPenalty,

    /// <summary>
    /// Illegal time gain
    /// </summary>
    IllegalTimeGain,

    /// <summary>
    /// Mandatory pitstop
    /// </summary>
    MandatoryPitStop,

    /// <summary>
    /// Attribute assigned
    /// </summary>
    AttributeAssigned
}