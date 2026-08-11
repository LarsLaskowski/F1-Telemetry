namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Complete lap positions data
/// </summary>
public interface ILapPositionsBase
{
    #region Properties

    /// <summary>
    /// Number of laps
    /// </summary>
    ushort NumberOfLaps { get; set; }

    /// <summary>
    /// Index of the lap where the data starts, 0 indexed
    /// </summary>
    ushort LapStartIndex { get; set; }

    /// <summary>
    /// Car positions on each lap. The first dimension is the lap and the second dimension is the
    /// car, so the value at [lap, car] is the position that car held on that lap. The lap
    /// dimension is bounded by the per-year maximum lap count and the car dimension by the
    /// per-year maximum car count
    /// </summary>
    int[,] CarPositionOnLaps { get; }

    #endregion // Properties
}