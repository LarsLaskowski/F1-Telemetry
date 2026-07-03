namespace F1Server.Core.Enumerations;

/// <summary>
/// Car damage rate from session data (since F1 2024)
/// </summary>
public enum CarDamageRate
{
    /// <summary>
    /// Reduced
    /// </summary>
    Reduced = 0,

    /// <summary>
    /// Standard
    /// </summary>
    Standard,

    /// <summary>
    /// Simulation
    /// </summary>
    Simulation
}