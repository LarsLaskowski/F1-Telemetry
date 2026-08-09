namespace F1Server.Service.Championships;

/// <summary>
/// Result of the creation of a new championship
/// </summary>
public enum ChampionshipCreateResult
{
    /// <summary>
    /// The given championship data is not usable
    /// </summary>
    InvalidData = 0,

    /// <summary>
    /// The championship was created
    /// </summary>
    Created,

    /// <summary>
    /// The championship could not be stored
    /// </summary>
    NotCreated
}