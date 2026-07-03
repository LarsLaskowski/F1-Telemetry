using F1Server.Core.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Lap positions (F1 2025)
/// </summary>
public class LapPositions2025 : ILapPositionsBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public LapPositions2025()
    {
        CarPositionOnLaps = new int[ConstData.F12025MaxLapPositions, ConstData.F12025MaxCars];
    }

    #endregion // Constructors

    #region ILapPositionsBase

    /// <inheritdoc/>
    public int NumberOfLaps { get; set; }

    /// <inheritdoc/>
    public int LapStartIndex { get; set; }

    /// <inheritdoc/>
    public int[,] CarPositionOnLaps { get; set; }

    #endregion // ILapPositionsBase
}