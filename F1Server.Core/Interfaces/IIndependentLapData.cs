namespace F1Server.Core.Interfaces;

/// <summary>
/// Represents lap timing data for a racing scenario, including current lap time, last lap time, and sector times
/// </summary>
public interface IIndependentLapData
{
    #region Properties

    /// <summary>
    /// Current lap time
    /// </summary>
    uint CurrentLapTime { get; set; }

    /// <summary>
    /// Last lap time
    /// </summary>
    uint LastLapTime { get; set; }

    /// <summary>
    /// Sector 1 time
    /// </summary>
    uint Sector1Time { get; set; }

    /// <summary>
    /// Sector 2 time
    /// </summary>
    uint Sector2Time { get; set; }

    /// <summary>
    /// Sector 3 time
    /// </summary>
    uint Sector3Time { get; set; }

    #endregion // Properties
}