namespace F1Server.Data.ViewData;

/// <summary>
/// View data representing the game version
/// </summary>
public class GameViewData
{
    #region Properties

    /// <summary>
    /// Game id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Game version
    /// </summary>
    public string GameVersion { get; set; }

    /// <summary>
    /// Game version (major.minor)
    /// </summary>
    public string GameVersionCode { get; set; }

    /// <summary>
    /// Last used
    /// </summary>
    public string LastUsed { get; set; }

    /// <summary>
    /// Number of sessions
    /// </summary>
    public int Sessions { get; set; }

    #endregion // Properties
}