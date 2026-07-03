namespace F1Server.Core.Enumerations;

/// <summary>
/// Car damage from session data (since F1 2024)
/// </summary>
public enum CarDamage
{
    /// <summary>
    /// Off
    /// </summary>
    Off = 0,

    /// <summary>
    /// Reduced
    /// </summary>
    Reduced,

    /// <summary>
    /// Standard
    /// </summary>
    Standard,

    /// <summary>
    /// Simulation
    /// </summary>
    Simulation
}