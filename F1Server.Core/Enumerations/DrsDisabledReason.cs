namespace F1Server.Core.Enumerations;

/// <summary>
/// Reasons why DRS is disabled by race control
/// </summary>
public enum DrsDisabledReason
{
    /// <summary>
    /// No reason given
    /// </summary>
    NoReason = 0,

    /// <summary>
    /// Wet track conditions
    /// </summary>
    WetTrack,

    /// <summary>
    /// Safety car deployed
    /// </summary>
    SafetyCarDeployed,

    /// <summary>
    /// Red flag
    /// </summary>
    RedFlag,

    /// <summary>
    /// Minimal lap not reached
    /// </summary>
    MinLapNotReached
}