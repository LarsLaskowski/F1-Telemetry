namespace F1Server.Core.Enumerations;

/// <summary>
/// Current result reason
/// </summary>
public enum ResultReason
{
    /// <summary>
    /// Not set
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Invalid
    /// </summary>
    Invalid = 1,

    /// <summary>
    /// Retired
    /// </summary>
    Retired,

    /// <summary>
    /// Finished
    /// </summary>
    Finished,

    /// <summary>
    /// Terminal damage
    /// </summary>
    TerminalDamage,

    /// <summary>
    /// Inactive
    /// </summary>
    Inactive,

    /// <summary>
    /// Not enough laps completed
    /// </summary>
    NotEnoughLapsCompleted,

    /// <summary>
    /// Black flagged
    /// </summary>
    BlackFlagged,

    /// <summary>
    /// Red flagged
    /// </summary>
    RedFlagged,

    /// <summary>
    /// Mechanical failure
    /// </summary>
    MechanicalFailure,

    /// <summary>
    /// Session skipped
    /// </summary>
    SessionSkipped,

    /// <summary>
    /// Session simulated
    /// </summary>
    SessionSimulated
}