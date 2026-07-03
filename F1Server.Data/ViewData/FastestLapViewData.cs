namespace F1Server.Data.ViewData;

/// <summary>
/// Viewdata of a fastest lap
/// </summary>
public class FastestLapViewData
{
    #region Properties

    /// <summary>
    /// Driver
    /// </summary>
    public string DriverName { get; set; }

    /// <summary>
    /// Time of fastest lap
    /// </summary>
    public uint LapTime { get; set; }

    /// <summary>
    /// Sector 1 time
    /// </summary>
    public uint LapTimeSector1 { get; set; }

    /// <summary>
    /// Sector 2 time
    /// </summary>
    public uint LapTimeSector2 { get; set; }

    /// <summary>
    /// Sector 3 time
    /// </summary>
    public uint LapTimeSector3 { get; set; }

    /// <summary>
    /// Number of lap
    /// </summary>
    public int LapNumber { get; set; }

    /// <summary>
    /// Position of car
    /// </summary>
    public int CarPosition { get; set; }

    /// <summary>
    /// Id of session
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// Id of lap
    /// </summary>
    public long LapId { get; set; }

    /// <summary>
    /// Id of participant
    /// </summary>
    public long ParticipantId { get; set; }

    /// <summary>
    /// Id of driver
    /// </summary>
    public long DriverId { get; set; }

    #endregion // Properties
}