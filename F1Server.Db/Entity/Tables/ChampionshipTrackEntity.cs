using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of a track in a championship season
/// </summary>
[Table("ChampionshipTracks")]
[Comment("ChampionshipTracks table containing information about tracks in a championship season, including associated sessions.")]
[Index(nameof(ChampionshipId))]
[Index(nameof(TrackId))]
[Index(nameof(QualifyingSessionId))]
[Index(nameof(SprintQualifyingSessionId))]
[Index(nameof(SprintSessionId))]
[Index(nameof(RaceSessionId))]
[Index(nameof(ChampionshipId), nameof(TrackId))]
public class ChampionshipTrackEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// Id of champtionship this tracks belongs to
    /// </summary>
    [Required]
    public long ChampionshipId { get; set; }

    /// <summary>
    /// Id of track
    /// </summary>
    [Required]
    public long TrackId { get; set; }

    /// <summary>
    /// Id of qualifying session
    /// </summary>
    public long? QualifyingSessionId { get; set; }

    /// <summary>
    /// Id of sprint qualifying shootout session
    /// </summary>
    public long? SprintQualifyingSessionId { get; set; }

    /// <summary>
    /// Id of sprint session
    /// </summary>
    public long? SprintSessionId { get; set; }

    /// <summary>
    /// Id of race session
    /// </summary>
    public long? RaceSessionId { get; set; }

    #region Navigation properties

    /// <summary>
    /// Championship
    /// </summary>
    [ForeignKey(nameof(ChampionshipId))]
    public virtual ChampionshipEntity Championship { get; set; }

    /// <summary>
    /// Track
    /// </summary>
    [ForeignKey(nameof(TrackId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual TrackEntity Track { get; set; }

    /// <summary>
    /// Qualifying session
    /// </summary>
    [ForeignKey(nameof(QualifyingSessionId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual SessionEntity QualifyingSession { get; set; }

    /// <summary>
    /// Sprint qualifying session
    /// </summary>
    [ForeignKey(nameof(SprintQualifyingSessionId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual SessionEntity SprintQualifyingSession { get; set; }

    /// <summary>
    /// Sprint session
    /// </summary>
    [ForeignKey(nameof(SprintSessionId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual SessionEntity SprintSession { get; set; }

    /// <summary>
    /// Race session
    /// </summary>
    [ForeignKey(nameof(RaceSessionId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual SessionEntity RaceSession { get; set; }

    #endregion // Navigation properties

    #endregion // Properties
}