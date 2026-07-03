namespace F1Server.Data.ViewData;

/// <summary>
/// View data of championships
/// </summary>
public class ChampionshipViewData
{
    #region Properties

    /// <summary>
    /// Id of championship
    /// </summary>
    public long ChampionshipId { get; set; }

    /// <summary>
    /// Id of game version
    /// </summary>
    public long GameVersionId { get; set; }

    /// <summary>
    /// Name of game version (F1 2020 for example)
    /// </summary>
    public string GameVersionName { get; set; }

    /// <summary>
    /// Year of game version
    /// </summary>
    public int GameVersionYear { get; set; }

    /// <summary>
    /// Number of championship
    /// </summary>
    public long Number { get; set; }

    /// <summary>
    /// Tracks
    /// </summary>
    public List<ChampionshipTrackViewData> Tracks { get; set; }

    #endregion // Properties
}