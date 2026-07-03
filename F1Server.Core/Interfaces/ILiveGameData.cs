namespace F1Server.Core.Interfaces;

/// <summary>
/// Interface for live game data at runtime
/// </summary>
public interface ILiveGameData : ILiveBaseData
{
    #region Properties

    /// <summary>
    /// Actual game version
    /// </summary>
    int GameVersion { get; }

    /// <summary>
    /// Major version
    /// </summary>
    int MajorVersion { get; }

    /// <summary>
    /// Minor version
    /// </summary>
    int MinorVersion { get; }

    /// <summary>
    /// Name of the game
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Last usage timestamp
    /// </summary>
    DateTime? LastTimeUsed { get; }

    #endregion // Properties
}