namespace F1Server.Data.ViewData;

/// <summary>
/// Class to view participants
/// </summary>
public class ParticipantViewData
{
    #region Properties

    /// <summary>
    /// Id of participant
    /// </summary>
    public long ParticipantDbId { get; set; }

    /// <summary>
    /// Driver name
    /// </summary>
    public string DriverName { get; set; }

    /// <summary>
    /// Nationality
    /// </summary>
    public string DriverNationality { get; set; }

    /// <summary>
    /// Name of team
    /// </summary>
    public string TeamName { get; set; }

    /// <summary>
    /// Is human controlled
    /// </summary>
    public bool IsHumanControlled { get; set; }

    /// <summary>
    /// Number of car
    /// </summary>
    public int CarRaceNumber { get; set; }

    /// <summary>
    /// Flag if this driver is in a custom created team
    /// </summary>
    public bool IsMyTeam { get; set; }

    #endregion // Properties
}