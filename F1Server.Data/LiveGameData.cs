using F1Server.Core.Interfaces;

namespace F1Server.Data;

/// <summary>
/// Game information at runtime
/// </summary>
public class LiveGameData : ILiveGameData
{
    #region IRuntimeGameData

    /// <summary>
    /// Database id
    /// </summary>
    public long DbId { get; set; }

    /// <summary>
    /// Actual game version
    /// </summary>
    public int GameVersion { get; set; }

    /// <summary>
    /// Major version
    /// </summary>
    public int MajorVersion { get; set; }

    /// <summary>
    /// Minor version
    /// </summary>
    public int MinorVersion { get; set; }

    /// <summary>
    /// Name of the game
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Last usage timestamp
    /// </summary>
    public DateTime? LastTimeUsed { get; set; }

    #endregion // IRuntimeGameData
}