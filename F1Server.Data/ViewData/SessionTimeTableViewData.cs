namespace F1Server.Data.ViewData;

/// <summary>
/// Time table of one finished session
/// </summary>
public class SessionTimeTableViewData
{
    #region Properties

    /// <summary>
    /// Fastest sector 1 time in milliseconds
    /// </summary>
    public uint FastestSector1 { get; set; }

    /// <summary>
    /// Fastest sector 1 driver (index from game array)
    /// </summary>
    public int FastestSector1Driver { get; set; }

    /// <summary>
    /// Fastest sector 2 time in milliseconds
    /// </summary>
    public uint FastestSector2 { get; set; }

    /// <summary>
    /// Fastest sector 2 driver (index from game array)
    /// </summary>
    public int FastestSector2Driver { get; set; }

    /// <summary>
    /// Fastest sector3 time in milliseconds
    /// </summary>
    public uint FastestSector3 { get; set; }

    /// <summary>
    /// Fastest sector 3 driver (index from game array)
    /// </summary>
    public int FastestSector3Driver { get; set; }

    /// <summary>
    /// Fastest lap time in milliseconds
    /// </summary>
    public uint FastestLap { get; set; }

    /// <summary>
    /// Fastest lap driver (index from game array)
    /// </summary>
    public int FastestLapDriver { get; set; }

    /// <summary>
    /// Participants in session
    /// </summary>
    public List<DriverViewData> Drivers { get; } = new List<DriverViewData>();

    /// <summary>
    /// Time table
    /// </summary>
    public List<FinalClassificationViewData> TimeTable { get; } = new List<FinalClassificationViewData>();

    #endregion // Properties
}