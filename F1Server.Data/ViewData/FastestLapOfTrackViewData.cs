using F1Server.Core.Enumerations;

namespace F1Server.Data.ViewData;

/// <summary>
/// Viewdata of a fastest lap of specific track
/// </summary>
public class FastestLapOfTrackViewData
{
    #region Properties

    /// <summary>
    /// Id of track
    /// </summary>
    public long TrackId { get; set; }

    /// <summary>
    /// Id of game version
    /// </summary>
    public long GameVersionId { get; set; }

    /// <summary>
    /// Game name
    /// </summary>
    public string GameVersionName { get; set; }

    /// <summary>
    /// Id of driver
    /// </summary>
    public long DriverId { get; set; }

    /// <summary>
    /// Type of formula
    /// </summary>
    public Formula FormulaType { get; set; }

    /// <summary>
    /// Driver
    /// </summary>
    public string DriverName { get; set; }

    /// <summary>
    /// Time of fastest lap
    /// </summary>
    public uint LapTime { get; set; }

    /// <summary>
    /// Track reference time
    /// </summary>
    public uint ReferenceTime { get; set; }

    /// <summary>
    /// Difference between lap and reference time
    /// </summary>
    public uint DiffReference { get; set; }

    /// <summary>
    /// Type of session for this lap
    /// </summary>
    public FastestLapSessionType LapSessionType { get; set; }

    #endregion // Properties
}