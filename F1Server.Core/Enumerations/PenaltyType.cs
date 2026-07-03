namespace F1Server.Core.Enumerations;

/// <summary>
/// Type of Penalty (Value from game is 0 based)
/// </summary>
public enum PenaltyType
{
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Drive through
    /// </summary>
    DriveThrough,

    /// <summary>
    /// Stop and go
    /// </summary>
    StopAndGo,

    /// <summary>
    /// Grid penalty
    /// </summary>
    GridPenalty,

    /// <summary>
    /// Penalty reminder
    /// </summary>
    PenaltyReminder,

    /// <summary>
    /// Time penalty
    /// </summary>
    TimePenalty,

    /// <summary>
    /// Warning
    /// </summary>
    Warning,

    /// <summary>
    /// Disqualified
    /// </summary>
    Disqualified,

    /// <summary>
    /// Removed from formation lap
    /// </summary>
    RemovedFromFormationLap,

    /// <summary>
    /// Parked too long timer
    /// </summary>
    ParkedTooLongTimer,

    /// <summary>
    /// Tyre regulations
    /// </summary>
    TyreRegulations,

    /// <summary>
    /// This lap invalidated
    /// </summary>
    ThisLapInvalid,

    /// <summary>
    /// This and next lap invalidated
    /// </summary>
    ThisAndNextLapInvalid,

    /// <summary>
    /// This lap invalidated without reason
    /// </summary>
    ThisLapInvalidWithoutReason,

    /// <summary>
    /// This and next lap invalidated without reason
    /// </summary>
    ThisAndNextLapsInvalidWithoutReason,

    /// <summary>
    /// This and previous lap invalidated
    /// </summary>
    ThisAndPreviousLapInvalid,

    /// <summary>
    /// This and previous lap invalidated without reason
    /// </summary>
    ThisAndPreviousLapInvalidWithoutReason,

    /// <summary>
    /// Retired
    /// </summary>
    Retired,

    /// <summary>
    /// Black flag timer
    /// </summary>
    BlackFlag
}