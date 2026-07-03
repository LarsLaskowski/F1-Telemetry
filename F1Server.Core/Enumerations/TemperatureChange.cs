namespace F1Server.Core.Enumerations;

/// <summary>
/// Enumeration of temperature changes
/// </summary>
public enum TemperatureChange
{
    /// <summary>
    /// Not available in this game
    /// </summary>
    NotAvailable = -1,

    /// <summary>
    /// Temperature goes up
    /// </summary>
    Up,

    /// <summary>
    /// Temperature goes down
    /// </summary>
    Down,

    /// <summary>
    /// No changes
    /// </summary>
    NoChange
}