using System.ComponentModel.DataAnnotations.Schema;

using F1Server.Core.Enumerations;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of final classification of session
/// </summary>
[Table("FinalClassifications")]
[Comment("Entity of final classification of session, contains results of participants after the session ends.")]
[Index(nameof(SessionId))]
[Index(nameof(ParticipantId))]
[Index(nameof(SessionId), nameof(ParticipantId))]
public class FinalClassificationEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Session id
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// Id of participant in the session
    /// </summary>
    public long ParticipantId { get; set; }

    /// <summary>
    /// Starting position
    /// </summary>
    public int GridPosition { get; set; }

    /// <summary>
    /// Finishing position
    /// </summary>
    public int FinishPosition { get; set; }

    /// <summary>
    /// Number of laps completed
    /// </summary>
    public int LapsDriven { get; set; }

    /// <summary>
    /// Number of pit stops
    /// </summary>
    public int PitStops { get; set; }

    /// <summary>
    /// Result status
    /// </summary>
    public ResultStatus ResultStatus { get; set; }

    /// <summary>
    /// Fastest lap time in milliseconds
    /// </summary>
    public uint FastestLapTime { get; set; }

    /// <summary>
    /// Total race time in seconds without penalties
    /// </summary>
    public double TotalRaceTime { get; set; }

    /// <summary>
    /// Total penalties time in seconds
    /// </summary>
    public uint PenaltiesTime { get; set; }

    /// <summary>
    /// Number of penalties
    /// </summary>
    public uint NumberOfPenalties { get; set; }

    #endregion // Properties

    #region Navigation properties

    /// <summary>
    /// Session
    /// </summary>
    [ForeignKey(nameof(SessionId))]
    public virtual SessionEntity Session { get; set; }

    /// <summary>
    /// Participant
    /// </summary>
    [ForeignKey(nameof(ParticipantId))]
    public virtual ParticipantEntity Participant { get; set; }

    #endregion // Navigation properties
}