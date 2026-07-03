using F1Server.Core.Enumerations;

namespace F1Server.Core.Interfaces;

/// <summary>
/// Live participants data in live session
/// </summary>
public interface ILiveDriverData : ILiveBaseData
{
    #region Properties

    /// <summary>
    /// Index in game packet
    /// </summary>
    int ArrayIndex { get; }

    /// <summary>
    /// Driver name
    /// </summary>
    string DriverName { get; }

    /// <summary>
    /// Car number
    /// </summary>
    int CarNumber { get; }

    /// <summary>
    /// Grid position
    /// </summary>
    int GridPosition { get; }

    /// <summary>
    /// Car position on track
    /// </summary>
    int CarPosition { get; }

    /// <summary>
    /// Nationality
    /// </summary>
    string Nationality { get; }

    /// <summary>
    /// Team name
    /// </summary>
    string TeamName { get; }

    /// <summary>
    /// Current driver status - in garage or out lap or something else
    /// </summary>
    DriverStatus CurrentDriverStatus { get; }

    /// <summary>
    /// Current lap time in milliseconds (since F1 2020)
    /// </summary>
    uint CurrentLapTime { get; }

    /// <summary>
    /// Fastest time needed for sector 1 in milliseconds
    /// </summary>
    uint FastestSector1 { get; }

    /// <summary>
    /// Fastest time needed for sector 2 in milliseconds
    /// </summary>
    uint FastestSector2 { get; }

    /// <summary>
    /// Fastest time needed for sector 3 in milliseconds
    /// </summary>
    uint FastestSector3 { get; }

    /// <summary>
    /// Fastest lap in milliseconds
    /// </summary>
    uint FastestLapTime { get; }

    /// <summary>
    /// Number of laps driven by this driver
    /// </summary>
    int LapsDriven { get; }

    /// <summary>
    /// Currently used type of tyre
    /// </summary>
    VisualTyreCompound CurrentUsedTyre { get; }

    #endregion // Properties
}