namespace F1Server.Core.Enumerations;

/// <summary>
/// Car pit status
/// </summary>
public enum PitStatus
{
    /// <summary>
    /// Not set
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Car is not in pit
    /// </summary>
    None = 1,

    /// <summary>
    /// Car comes in pit
    /// </summary>
    Pitting,

    /// <summary>
    /// Car is in pit
    /// </summary>
    InPit
}