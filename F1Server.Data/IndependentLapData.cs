using F1Server.Core.Interfaces;

namespace F1Server.Data;

/// <summary>
/// Lap data, independent from game version
/// </summary>
public class IndependentLapData : IIndependentLapData
{
    #region IIndependentLapData

    /// <inheritdoc/>
    public uint CurrentLapTime { get; set; }

    /// <inheritdoc/>
    public uint LastLapTime { get; set; }

    /// <inheritdoc/>
    public uint Sector1Time { get; set; }

    /// <inheritdoc/>
    public uint Sector2Time { get; set; }

    /// <inheritdoc/>
    public uint Sector3Time { get; set; }

    #endregion // IIndependentLapData
}