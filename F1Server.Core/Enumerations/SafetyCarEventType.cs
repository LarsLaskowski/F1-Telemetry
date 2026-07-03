namespace F1Server.Core.Enumerations;

/// <summary>
/// Safety car event type
/// </summary>
public enum SafetyCarEventType
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,

    /// <summary>
    /// Safety car deployed
    /// </summary>
    Deployed,

    /// <summary>
    /// Safety car returning
    /// </summary>
    Returning,

    /// <summary>
    /// Safety car returned
    /// </summary>
    Returned,

    /// <summary>
    /// Resume race
    /// </summary>
    ResumeRace
}