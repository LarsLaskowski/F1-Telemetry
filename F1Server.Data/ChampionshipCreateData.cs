namespace F1Server.Data;

/// <summary>
/// Lap data, independed from game version
/// </summary>
public class ChampionshipCreateData
{
    #region Properties

    /// <summary>
    /// Id of selected game version
    /// </summary>
    public long GameVersionId { get; set; }

    /// <summary>
    /// Selected tracks
    /// </summary>
    public List<long> Tracks { get; set; }

    /// <summary>
    /// Selected championship mode
    /// </summary>
    public int Mode { get; set; }

    #endregion // Properties
}