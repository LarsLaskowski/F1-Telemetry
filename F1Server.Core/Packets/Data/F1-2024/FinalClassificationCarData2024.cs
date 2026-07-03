using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Final classification data - F1 2024
/// </summary>
public class FinalClassificationCarData2024 : IFinalClassificationCarBase, IFinalClassification2024
{
    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public FinalClassificationCarData2024()
    {
        TyreStintsActual = new ushort[8];
        TyreStintsVisual = new ushort[8];
        TyreStintsEndLaps = new ushort[8];
    }

    #endregion // Constructor

    #region IFinalClassificationBase

    /// <summary>
    /// Finishing position
    /// </summary>
    public ushort Position { get; set; }

    /// <summary>
    /// Number of laps completed
    /// </summary>
    public ushort LapsCompleted { get; set; }

    /// <summary>
    /// Grid position of the car
    /// </summary>
    public ushort GridPosition { get; set; }

    /// <summary>
    /// Number of points scored
    /// </summary>
    public ushort Points { get; set; }

    /// <summary>
    /// Number of pit stops made
    /// </summary>
    public ushort PitStops { get; set; }

    /// <summary>
    /// Result status
    /// </summary>
    public ResultStatus ResultStatus { get; set; }

    /// <summary>
    /// Best lap time of the session in milliseconds
    /// </summary>
    public uint BestLapTimeInMs { get; set; }

    /// <summary>
    /// Total race time in seconds without penalties
    /// </summary>
    public double TotalRaceTime { get; set; }

    /// <summary>
    /// Total penalties accumulated in seconds
    /// </summary>
    public ushort PenaltiesTime { get; set; }

    /// <summary>
    /// Number of penalties applied to this driver
    /// </summary>
    public ushort NumPenalties { get; set; }

    /// <summary>
    /// Number of tyre stints up to maximum
    /// </summary>
    public ushort NumTyreStints { get; set; }

    /// <summary>
    /// Actual tyres used  by this driver
    /// </summary>
    public ushort[] TyreStintsActual { get; set; }

    /// <summary>
    /// Visual tyre stints used by this driver
    /// </summary>
    public ushort[] TyreStintsVisual { get; set; }

    #endregion // IFinalClassificationBase

    #region IFinalClassification2024

    /// <summary>
    /// The lap number stints end on
    /// </summary>
    public ushort[] TyreStintsEndLaps { get; set; }

    #endregion // IFinalClassification2024
}