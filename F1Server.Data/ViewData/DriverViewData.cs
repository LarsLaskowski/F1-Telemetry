namespace F1Server.Data.ViewData;

/// <summary>
/// View data of one driver
/// </summary>
public class DriverViewData
{
    #region Properties

    /// <summary>
    /// Participant database id
    /// </summary>
    public long ParticipantId { get; set; }

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
    /// Starting grid position (valid on races)
    /// </summary>
    public int StartingGridPosition { get; set; }

    /// <summary>
    /// Position finished
    /// </summary>
    public int FinishPosition { get; set; }

    /// <summary>
    /// Nationality
    /// </summary>
    public string Nationality { get; set; }

    /// <summary>
    /// Team name
    /// </summary>
    public string TeamName { get; set; }

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
    /// Total race time (valid on races)
    /// </summary>
    public uint TotalRaceTime { get; set; }

    #endregion // Properties
}