using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of points in a championship season
/// </summary>
[Table("ChampionshipPoints")]
[Comment("Entity of points in a championship season.")]
[Index(nameof(ChampionshipId))]
[Index(nameof(TrackId))]
[Index(nameof(DriverId))]
[Index(nameof(ChampionshipId), nameof(DriverId))]
[Index(nameof(ChampionshipId), nameof(TrackId))]
[Index(nameof(TrackId), nameof(DriverId))]
[Index(nameof(ChampionshipId), nameof(TrackId), nameof(DriverId))]
public class ChampionshipPointsEntity
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
    /// Id of driver
    /// </summary>
    [Required]
    public long DriverId { get; set; }

    /// <summary>
    /// Points from race
    /// </summary>
    public int RacePoints { get; set; }

    /// <summary>
    /// Points from sprint races
    /// </summary>
    public int SprintRacePoints { get; set; }

    /// <summary>
    /// Additional points (fastest lap)
    /// </summary>
    public int AdditionalPoints { get; set; }

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
    /// Driver
    /// </summary>
    [ForeignKey(nameof(DriverId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual DriverEntity Driver { get; set; }

    #endregion // Navigation properties

    #endregion // Properties
}