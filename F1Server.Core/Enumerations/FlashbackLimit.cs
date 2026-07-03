namespace F1Server.Core.Enumerations;

/// <summary>
/// Flashback limit from session data (since F1 2024)
/// </summary>
public enum FlashbackLimit
{
    /// <summary>
    /// Low
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium
    /// </summary>
    Medium,

    /// <summary>
    /// High
    /// </summary>
    High,

    /// <summary>
    /// Unlimited
    /// </summary>
    Unlimited
}