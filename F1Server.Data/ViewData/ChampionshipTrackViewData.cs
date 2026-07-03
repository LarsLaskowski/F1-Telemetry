namespace F1Server.Data.ViewData;

/// <summary>
/// View data of championship track
/// </summary>
public class ChampionshipTrackViewData
{
    #region Properties

    /// <summary>
    /// Id of championship track entry
    /// </summary>
    public long ChampionshipTrackId { get; set; }

    /// <summary>
    /// Gets or sets the qualifying position of a participant in a race or competition
    /// </summary>
    public int QualifyingPosition { get; set; }

    /// <summary>
    /// Gets or sets the position achieved during sprint qualifying
    /// </summary>
    public int SprintQualifyingPosition { get; set; }

    /// <summary>
    /// Gets or sets the position of the sprint in a sequence or ranking
    /// </summary>
    public int SprintPosition { get; set; }

    /// <summary>
    /// Gets or sets the position of a racer in a race
    /// </summary>
    public int RacePosition { get; set; }

    /// <summary>
    /// Gets or sets the number of points earned in a race
    /// </summary>
    public int RacePoints { get; set; }

    /// <summary>
    /// Gets or sets the number of points earned in a sprint race
    /// </summary>
    public int SprintPoints { get; set; }

    /// <summary>
    /// Gets or sets the difficulty level of the race
    /// </summary>
    public int RaceDifficulty { get; set; }

    /// <summary>
    /// Gets or sets the difficulty level of the sprint
    /// </summary>
    public int SprintDifficulty { get; set; }

    /// <summary>
    /// Gets or sets the difficulty level for qualifying rounds
    /// </summary>
    public int QualifyingDifficulty { get; set; }

    /// <summary>
    /// Gets or sets the difficulty level for sprint qualifying sessions
    /// </summary>
    public int SprintQualifyingDifficulty { get; set; }

    #endregion // Properties
}