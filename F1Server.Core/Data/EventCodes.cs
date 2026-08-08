namespace F1Server.Core.Data;

/// <summary>
/// Event code constants shared across event packet extraction and evaluation
/// </summary>
public static class EventCodes
{
    #region Constants

    /// <summary>
    /// Event code constant for "Session Started"
    /// </summary>
    public const string SessionStart = "SSTA";

    /// <summary>
    /// Event code constant for "Session Ended"
    /// </summary>
    public const string SessionEnd = "SEND";

    /// <summary>
    /// Event code constant for "Flashback Activated"
    /// </summary>
    public const string Flashback = "FLBK";

    #endregion // Constants
}