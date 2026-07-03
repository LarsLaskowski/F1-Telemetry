namespace F1Server.Core.Enumerations;

/// <summary>
/// Safety car status
/// </summary>
public enum SafetyCarStatus
{
    /// <summary>
    /// No safety car
    /// </summary>
    NoSafetyCar = 0,

    /// <summary>
    /// Full safety car
    /// </summary>
    SafetyCar,

    /// <summary>
    /// Virtual safety car
    /// </summary>
    VirtualSafetyCar,

    /// <summary>
    /// Formation lap
    /// </summary>
    FormationLap
}