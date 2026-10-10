using F1Server.Core.Interfaces;

namespace F1Server.Data;

/// <summary>
/// Game information at runtime
/// </summary>
public class LiveGameData : ILiveGameData
{
    #region ILiveBaseData

    /// <inheritdoc/>
    public long DbId { get; set; }

    #endregion // ILiveBaseData

    #region ILiveGameData

    /// <inheritdoc/>
    public int GameVersion { get; set; }

    /// <inheritdoc/>
    public int MajorVersion { get; set; }

    /// <inheritdoc/>
    public int MinorVersion { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public DateTime? LastTimeUsed { get; set; }

    #endregion // ILiveGameData
}