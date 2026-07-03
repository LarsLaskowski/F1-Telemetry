using F1Server.Core.Enumerations;

namespace F1Server.Shared.Data;

/// <summary>
/// Data class with session information
/// </summary>
public class SessionDataInfo
{
    #region Properties

    /// <summary>
    /// Session type
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Formula type
    /// </summary>
    public Formula FormulaType { get; set; }

    /// <summary>
    /// Track name
    /// </summary>
    public string TrackName { get; set; }

    /// <summary>
    /// Total laps
    /// </summary>
    public int TotalLaps { get; set; }

    /// <summary>
    /// Session id
    /// </summary>
    public ulong SessionId { get; set; }

    /// <summary>
    /// AI difficulty
    /// </summary>
    public short AiDifficulty { get; set; }

    /// <summary>
    /// Weather conditions
    /// </summary>
    public WeatherCondition Weather { get; set; }

    #endregion // Properties
}