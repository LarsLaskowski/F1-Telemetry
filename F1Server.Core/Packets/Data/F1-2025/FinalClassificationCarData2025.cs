using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Final classification data - F1 2025
/// </summary>
public class FinalClassificationCarData2025 : IFinalClassificationCarBase, IFinalClassification2024, IFinalClassification2025
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public FinalClassificationCarData2025()
    {
        TyreStintsActual = new ushort[8];
        TyreStintsVisual = new ushort[8];
        TyreStintsEndLaps = new ushort[8];
    }

    #endregion // Constructor

    #region IFinalClassificationCarBase

    /// <inheritdoc/>
    public ushort Position { get; set; }

    /// <inheritdoc/>
    public ushort LapsCompleted { get; set; }

    /// <inheritdoc/>
    public ushort GridPosition { get; set; }

    /// <inheritdoc/>
    public ushort Points { get; set; }

    /// <inheritdoc/>
    public ushort PitStops { get; set; }

    /// <inheritdoc/>
    public ResultStatus ResultStatus { get; set; }

    /// <inheritdoc/>
    public uint BestLapTimeInMs { get; set; }

    /// <inheritdoc/>
    public double TotalRaceTime { get; set; }

    /// <inheritdoc/>
    public ushort PenaltiesTime { get; set; }

    /// <inheritdoc/>
    public ushort NumPenalties { get; set; }

    /// <inheritdoc/>
    public ushort NumTyreStints { get; set; }

    /// <inheritdoc/>
    public ushort[] TyreStintsActual { get; set; }

    /// <inheritdoc/>
    public ushort[] TyreStintsVisual { get; set; }

    #endregion // IFinalClassificationCarBase

    #region IFinalClassification2024

    /// <inheritdoc/>
    public ushort[] TyreStintsEndLaps { get; set; }

    #endregion // IFinalClassification2024

    #region IFinalClassification2025

    /// <inheritdoc/>
    public ResultReason ResultReason { get; set; }

    #endregion // IFinalClassification2025
}