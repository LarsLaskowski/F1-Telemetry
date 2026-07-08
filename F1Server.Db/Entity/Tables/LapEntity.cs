using System.ComponentModel.DataAnnotations.Schema;

using F1Server.Core.Enumerations;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of all laps
/// </summary>
[Table("Laps")]
[Comment("Table with all laps of participants in the session, depends on the game version")]
[Index(nameof(ParticipantId))]
[Index(nameof(SessionId))]
[Index(nameof(LapNumber))]
[Index(nameof(ParticipantId), nameof(LapNumber))]
[Index(nameof(ParticipantId), nameof(SessionId))]
[Index(nameof(SessionId), nameof(DbIsInvalidLapTime))]
[Index(nameof(SessionId), nameof(LapNumber))]
[Index(nameof(SessionId), nameof(ParticipantId), nameof(LapNumber), IsUnique = true)]
public class LapEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id of participant in the database
    /// </summary>
    public long ParticipantId { get; set; }

    /// <summary>
    /// Id of the session this lap belongs to
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// Lap time in milliseconds
    /// </summary>
    public uint LapTime { get; set; }

    /// <summary>
    /// Time of sector 1 in milliseconds
    /// </summary>
    public uint Sector1Time { get; set; }

    /// <summary>
    /// Time of sector 2 in milliseconds
    /// </summary>
    public uint Sector2Time { get; set; }

    /// <summary>
    /// Time of sector 3 in milliseconds
    /// </summary>
    public uint Sector3Time { get; set; }

    /// <summary>
    /// Lap number
    /// </summary>
    public ushort LapNumber { get; set; }

    /// <summary>
    /// Flag if this lap is invalid - database only value
    /// </summary>
    [Column("IsInvalid")]
    public int DbIsInvalid { get; set; }

    /// <summary>
    /// Flag if this lap is invalid
    /// </summary>
    [NotMapped]
    public bool IsInvalid
    {
        get => DbIsInvalid != 0;
        set => DbIsInvalid = value ? 1 : 0;
    }

    /// <summary>
    /// Flag if this lap time is invalid - database only value
    /// </summary>
    [Column("IsInvalidLapTime")]
    public int DbIsInvalidLapTime { get; set; }

    /// <summary>
    /// Flag if this lap time is invalid
    /// </summary>
    [NotMapped]
    public bool IsInvalidLapTime
    {
        get => DbIsInvalidLapTime != 0;
        set => DbIsInvalidLapTime = value ? 1 : 0;
    }

    /// <summary>
    /// Lap is completed (all times set) - database only value
    /// </summary>
    [Column("IsCompleted")]
    public int DbIsCompleted { get; set; }

    /// <summary>
    /// Lap is completed (all times set)
    /// </summary>
    [NotMapped]
    public bool IsCompleted
    {
        get => DbIsCompleted != 0;
        set => DbIsCompleted = value ? 1 : 0;
    }

    /// <summary>
    /// Current driver status
    /// </summary>
    public DriverStatus DriverStatus { get; set; }

    /// <summary>
    /// Pit status
    /// </summary>
    public PitStatus PitStatus { get; set; }

    /// <summary>
    /// Drivers result status
    /// </summary>
    public ResultStatus ResultStatus { get; set; }

    /// <summary>
    /// Car position
    /// </summary>
    public ushort CarPosition { get; set; }

    /// <summary>
    /// Tyre compound
    /// </summary>
    public VisualTyreCompound TyreCompound { get; set; }

    /// <summary>
    /// Lap is started
    /// </summary>
    [NotMapped]
    public bool IsStarted { get; set; }

    /// <summary>
    /// Lap is finished, but times are not completed
    /// </summary>
    [NotMapped]
    public bool IsFinished { get; set; }

    #endregion // Properties

    #region Navigation properties

    /// <summary>
    /// Participant data
    /// </summary>
    [ForeignKey(nameof(ParticipantId))]
    public virtual ParticipantEntity Participant { get; set; }

    /// <summary>
    /// Telemetry data of this lap
    /// </summary>
    public virtual ICollection<CarTelemetryEntity> Telemetries { get; } = new List<CarTelemetryEntity>();

    #endregion // Navigation properties
}