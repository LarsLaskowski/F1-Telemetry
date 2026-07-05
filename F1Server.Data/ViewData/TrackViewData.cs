namespace F1Server.Data.ViewData;

/// <summary>
/// View data representing track information
/// </summary>
public class TrackViewData
{
    #region Properties

    /// <summary>
    /// Id of track
    /// </summary>
    public long TrackId { get; set; }

    /// <summary>
    /// Number of track
    /// </summary>
    public int TrackNumber { get; set; }

    /// <summary>
    /// Name of track
    /// </summary>
    public string TrackName { get; set; }

    /// <summary>
    /// Flag session exists
    /// </summary>
    public bool HasSession { get; set; }

    /// <summary>
    /// Number of stored sessions
    /// </summary>
    public int Sessions { get; set; }

    /// <summary>
    /// Lap reference time
    /// </summary>
    public string ReferenceLapTime { get; set; }

    #endregion // Properties
}