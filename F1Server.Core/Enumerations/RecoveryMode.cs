namespace F1Server.Core.Enumerations;

/// <summary>
/// Recovery mode from session data (since F1 2024)
/// </summary>
public enum RecoveryMode
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,

    /// <summary>
    /// Flashbacks enabled
    /// </summary>
    Flashbacks,

    /// <summary>
    /// Auto-Recovery
    /// </summary>
    AutoRecovery
}