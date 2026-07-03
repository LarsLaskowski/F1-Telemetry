using F1Server.Core.Interfaces;

namespace F1Server.Data;

/// <summary>
/// Lap data, independed from game version
/// </summary>
public class IndependentLapData : IIndependentLapData
{
    #region IIndependentLapData properties

    /// <summary>
    /// Current lap time
    /// </summary>
    public uint CurrentLapTime { get; set; }

    /// <summary>
    /// Last lap time
    /// </summary>
    public uint LastLapTime { get; set; }

    /// <summary>
    /// Sector 1 time
    /// </summary>
    public uint Sector1Time { get; set; }

    /// <summary>
    /// Sector 2 time
    /// </summary>
    public uint Sector2Time { get; set; }

    /// <summary>
    /// Sector 3 time
    /// </summary>
    public uint Sector3Time { get; set; }

    #endregion // IIndependentLapData properties
}