namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Complete lap poitions data
/// </summary>
public interface ILapPositionsBase
{
    #region Properties

    /// <summary>
    /// Number of laps
    /// </summary>
    int NumberOfLaps { get; set; }

    /// <summary>
    /// Index of the lap where the data starts, 0 indexed
    /// </summary>
    int LapStartIndex { get; set; }

    /// <summary>
    /// Car positions on each lap (max number of laps is 100)
    /// </summary>
    int[,] CarPositionOnLaps { get; set; }

    #endregion // Properties
}