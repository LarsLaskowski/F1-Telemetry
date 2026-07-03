namespace F1Server.Core.Enumerations;

/// <summary>
/// Current driver status
/// </summary>
public enum DriverStatus
{
    /// <summary>
    /// Not set
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Driver is in garage
    /// </summary>
    InGarage = 1,

    /// <summary>
    /// Flying lap
    /// </summary>
    FlyingLap,

    /// <summary>
    /// In lap
    /// </summary>
    InLap,

    /// <summary>
    /// Out lap
    /// </summary>
    OutLap,

    /// <summary>
    /// On track
    /// </summary>
    OnTrack
}