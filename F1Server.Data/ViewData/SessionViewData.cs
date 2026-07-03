using F1Server.Core.Enumerations;

namespace F1Server.Data.ViewData;

/// <summary>
/// Class to view sessions
/// </summary>
public class SessionViewData
{
    #region Properties

    /// <summary>
    /// Database id of session
    /// </summary>
    public long SessionDbId { get; set; }

    /// <summary>
    /// Database unique identifier of game
    /// </summary>
    public long GameVersionId { get; set; }

    /// <summary>
    /// Game version
    /// </summary>
    public string GameVersion { get; set; }

    /// <summary>
    /// Database id of track
    /// </summary>
    public long TrackId { get; set; }

    /// <summary>
    /// Track
    /// </summary>
    public string Track { get; set; }

    /// <summary>
    /// Type of formula
    /// </summary>
    public Formula FormulaType { get; set; }

    /// <summary>
    /// Type of session
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Number of cars
    /// </summary>
    public int Cars { get; set; }

    /// <summary>
    /// AI difficulty level
    /// </summary>
    public int AiDifficulty { get; set; }

    /// <summary>
    /// Weather condition
    /// </summary>
    public WeatherCondition Weather { get; set; }

    #endregion // Properties
}