using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using F1Server.Core.Enumerations;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of a championship season
/// </summary>
[Table("Championships")]
[Comment("Championships table containing information about championship seasons, including associated tracks and points.")]
[Index(nameof(GameVersionId))]
[Index(nameof(Number))]
[Index(nameof(DbIsFinished))]
[Index(nameof(Mode))]
[Index(nameof(GameVersionId), nameof(DbIsFinished))]
public class ChampionshipEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// Id of game version this season belongs to
    /// </summary>
    [Required]
    public long GameVersionId { get; set; }

    /// <summary>
    /// Number of championship
    /// </summary>
    public ushort Number { get; set; }

    /// <summary>
    /// Championship is finished? (Database value)
    /// </summary>
    [Column("IsFinished")]
    public ushort DbIsFinished { get; set; }

    /// <summary>
    /// Mode of championship
    /// </summary>
    public ChampionshipMode Mode { get; set; }

    /// <summary>
    /// Championship is finished?
    /// </summary>
    [NotMapped]
    public bool IsFinished
    {
        get => DbIsFinished == 1;
        set
        {
            if (value == true)
            {
                DbIsFinished = 1;
            }
            else
            {
                DbIsFinished = 0;
            }
        }
    }

    #region Navigation properties

    /// <summary>
    /// Game version
    /// </summary>
    [ForeignKey(nameof(GameVersionId))]
    public virtual GameVersionEntity GameVersion { get; set; }

    /// <summary>
    /// Tracks in this championship
    /// </summary>
    public virtual ICollection<ChampionshipTrackEntity> Tracks { get; } = new List<ChampionshipTrackEntity>();

    /// <summary>
    /// Points in this championship
    /// </summary>
    public virtual ICollection<ChampionshipPointsEntity> Points { get; } = new List<ChampionshipPointsEntity>();

    #endregion // Navigation properties

    #endregion // Properties
}