using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Base data of final classification data packet
/// </summary>
public interface IFinalClassificationCarBase
{
    #region Properties

    /// <summary>
    /// Finishing position
    /// </summary>
    ushort Position { get; set; }

    /// <summary>
    /// Number of laps completed
    /// </summary>
    ushort LapsCompleted { get; set; }

    /// <summary>
    /// Grid position of the car
    /// </summary>
    ushort GridPosition { get; set; }

    /// <summary>
    /// Number of points scored
    /// </summary>
    ushort Points { get; set; }

    /// <summary>
    /// Number of pit stops made
    /// </summary>
    ushort PitStops { get; set; }

    /// <summary>
    /// Result status
    /// </summary>
    ResultStatus ResultStatus { get; set; }

    /// <summary>
    /// Best lap time of the session in milliseconds
    /// </summary>
    uint BestLapTimeInMs { get; set; }

    /// <summary>
    /// Total race time in seconds without penalties
    /// </summary>
    double TotalRaceTime { get; set; }

    /// <summary>
    /// Total penalties accumulated in seconds
    /// </summary>
    ushort PenaltiesTime { get; set; }

    /// <summary>
    /// Number of penalties applied to this driver
    /// </summary>
    ushort NumPenalties { get; set; }

    /// <summary>
    /// Number of tyre stints up to maximum
    /// </summary>
    ushort NumTyreStints { get; set; }

    /// <summary>
    /// Actual tyres used  by this driver
    /// </summary>
    ushort[] TyreStintsActual { get; set; }

    /// <summary>
    /// Visual tyre stints used by this driver
    /// </summary>
    ushort[] TyreStintsVisual { get; set; }

    #endregion // Properties
}