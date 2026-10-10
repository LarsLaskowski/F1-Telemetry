using F1Server.Core.Enumerations;
using F1Server.Core.Interfaces;

namespace F1Server.Data;

/// <summary>
/// Live data of participant in a live session
/// </summary>
public class LiveDriverData : ILiveDriverData
{
    #region ILiveBaseData

    /// <inheritdoc/>
    public long DbId { get; set; }

    #endregion // ILiveBaseData

    #region ILiveDriverData

    /// <inheritdoc/>
    public int ArrayIndex { get; set; }

    /// <inheritdoc/>
    public string DriverName { get; set; }

    /// <inheritdoc/>
    public int CarNumber { get; set; }

    /// <inheritdoc/>
    public int GridPosition { get; set; }

    /// <inheritdoc/>
    public int CarPosition { get; set; }

    /// <inheritdoc/>
    public string Nationality { get; set; }

    /// <inheritdoc/>
    public string TeamName { get; set; }

    /// <inheritdoc/>
    public DriverStatus CurrentDriverStatus { get; set; }

    /// <inheritdoc/>
    public uint CurrentLapTime { get; set; }

    /// <inheritdoc/>
    public uint FastestSector1 { get; set; }

    /// <inheritdoc/>
    public uint FastestSector2 { get; set; }

    /// <inheritdoc/>
    public uint FastestSector3 { get; set; }

    /// <inheritdoc/>
    public uint FastestLapTime { get; set; }

    /// <inheritdoc/>
    public int LapsDriven { get; set; }

    /// <inheritdoc/>
    public VisualTyreCompound CurrentUsedTyre { get; set; }

    #endregion // ILiveDriverData
}