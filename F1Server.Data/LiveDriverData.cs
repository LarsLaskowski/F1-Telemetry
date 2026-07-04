using F1Server.Core.Enumerations;
using F1Server.Core.Interfaces;

namespace F1Server.Data;

/// <summary>
/// Live data of participant in a live session
/// </summary>
public class LiveDriverData : ILiveDriverData
{
    #region ILiveBaseData

    /// <summary>
    /// Database id
    /// </summary>
    public long DbId { get; set; }

    #endregion // ILiveBaseData

    #region ILiveDriverData

    /// <summary>
    /// Index in game packet
    /// </summary>
    public int ArrayIndex { get; set; }

    /// <summary>
    /// Driver name
    /// </summary>
    public string DriverName { get; set; }

    /// <summary>
    /// Car number
    /// </summary>
    public int CarNumber { get; set; }

    /// <summary>
    /// Grid position
    /// </summary>
    public int GridPosition { get; set; }

    /// <summary>
    /// Car position on track
    /// </summary>
    public int CarPosition { get; set; }

    /// <summary>
    /// Nationality
    /// </summary>
    public string Nationality { get; set; }

    /// <summary>
    /// Team name
    /// </summary>
    public string TeamName { get; set; }

    /// <summary>
    /// Current driver status - in garage or out lap or something else
    /// </summary>
    public DriverStatus CurrentDriverStatus { get; set; }

    /// <summary>
    /// Current lap time in milliseconds (since F1 2020)
    /// </summary>
    public uint CurrentLapTime { get; set; }

    /// <summary>
    /// Fastest time needed for sector 1 in milliseconds
    /// </summary>
    public uint FastestSector1 { get; set; }

    /// <summary>
    /// Fastest time needed for sector 2 in milliseconds
    /// </summary>
    public uint FastestSector2 { get; set; }

    /// <summary>
    /// Fastest time needed for sector 3 in milliseconds
    /// </summary>
    public uint FastestSector3 { get; set; }

    /// <summary>
    /// Fastest lap in milliseconds
    /// </summary>
    public uint FastestLapTime { get; set; }

    /// <summary>
    /// Number of laps driven by this driver
    /// </summary>
    public int LapsDriven { get; set; }

    /// <summary>
    /// Currently used tyre type
    /// </summary>
    public VisualTyreCompound CurrentUsedTyre { get; set; }

    #endregion // ILiveDriverData
}