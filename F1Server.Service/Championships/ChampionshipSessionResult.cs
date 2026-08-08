namespace F1Server.Service.Championships;

/// <summary>
/// Result of adding a session to a running championship
/// </summary>
public enum ChampionshipSessionResult
{
    /// <summary>
    /// The given session id is not usable
    /// </summary>
    InvalidSession = 0,

    /// <summary>
    /// No session exists for the given session id
    /// </summary>
    SessionNotFound,

    /// <summary>
    /// The session was added to the championship
    /// </summary>
    Added,

    /// <summary>
    /// The track of the session is not part of a running championship
    /// </summary>
    TrackNotInChampionship,

    /// <summary>
    /// The session could not be stored at the track of the championship
    /// </summary>
    NotAdded
}