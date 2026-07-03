using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Table with participant data
/// </summary>
[Table("Participants")]
[Comment("Table with participant data, depends on the game version")]
[Index(nameof(SessionId))]
[Index(nameof(DriverId))]
[Index(nameof(NationalityId))]
[Index(nameof(TeamId))]
[Index(nameof(SessionId), nameof(DriverId))]
[Index(nameof(SessionId), nameof(TeamId))]
public class ParticipantEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id of associated session
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// Id of driver
    /// </summary>
    public long DriverId { get; set; }

    /// <summary>
    /// Id of nationality
    /// </summary>
    public long NationalityId { get; set; }

    /// <summary>
    /// Is human controlled driver? - database only value
    /// </summary>
    [Column("IsHumanControlled")]
    public int DbIsHumanControlled { get; set; }

    /// <summary>
    /// Is human controlled driver?
    /// </summary>
    [NotMapped]
    public bool IsHumanControlled
    {
        get => DbIsHumanControlled != 0;
        set => DbIsHumanControlled = value ? 1 : 0;
    }

    /// <summary>
    /// Race number of the car
    /// </summary>
    public int CarRaceNumber { get; set; }

    /// <summary>
    /// Id of the team
    /// </summary>
    public long TeamId { get; set; }

    /// <summary>
    /// Is my team? Available since F1 2021 - database only value
    /// </summary>
    [Column("IsMyTeam")]
    public int? DbIsMyTeam { get; set; }

    /// <summary>
    /// Is my team? Available since F1 2021
    /// </summary>
    [NotMapped]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0045:Convert to conditional expression", Justification = "It's ok here")]
    public bool? IsMyTeam
    {
        get => DbIsMyTeam == null ? null : DbIsMyTeam.Value != 0;
        set
        {
            if (value == null)
            {
                DbIsMyTeam = null;
            }
            else
            {
                DbIsMyTeam = value != true ? 0 : 1;
            }
        }
    }

    /// <summary>
    /// Drivers name
    /// </summary>
    public string DriverName { get; set; }

    /// <summary>
    /// Index within array, needed for laps and other packets
    /// </summary>
    public ushort ArrayIndex { get; set; }

    #endregion // Properties

    #region Navigation properties

    /// <summary>
    /// Session
    /// </summary>
    [ForeignKey(nameof(SessionId))]
    public virtual SessionEntity Session { get; set; }

    /// <summary>
    /// Team information
    /// </summary>
    [ForeignKey(nameof(TeamId))]
    public virtual TeamEntity Team { get; set; }

    /// <summary>
    /// Driver
    /// </summary>
    [ForeignKey(nameof(DriverId))]
    public virtual DriverEntity Driver { get; set; }

    /// <summary>
    /// Nationality
    /// </summary>
    [ForeignKey(nameof(NationalityId))]
    public virtual NationalityEntity Nationality { get; set; }

    /// <summary>
    /// Laps of the participant
    /// </summary>
    public virtual ICollection<LapEntity> Laps { get; } = new List<LapEntity>();

    #endregion // Navigation properties
}