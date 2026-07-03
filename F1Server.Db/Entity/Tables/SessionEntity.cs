using System.ComponentModel.DataAnnotations.Schema;

using F1Server.Core.Enumerations;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Session table
/// </summary>
[Table("Sessions")]
[Comment("Session table containing information about game sessions, including track, game version, and session details.")]
[Index(nameof(TrackId))]
[Index(nameof(GameVersionId))]
[Index(nameof(SessionId))]
[Index(nameof(TrackId), nameof(GameVersionId))]
public class SessionEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Session unique identifier from the game
    /// </summary>
    public ulong SessionId { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreationTimestamp { get; set; }

    /// <summary>
    /// Type of formula
    /// </summary>
    public Formula FormulaType { get; set; }

    /// <summary>
    /// Id of track
    /// </summary>
    public long TrackId { get; set; }

    /// <summary>
    /// Type of session
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Id of the game
    /// </summary>
    public long GameVersionId { get; set; }

    /// <summary>
    /// Online or offline session - database only value
    /// </summary>
    [Column("IsNetworkGame")]
    public int DbIsNetworkGame { get; set; }

    /// <summary>
    /// Online or offline session
    /// </summary>
    [NotMapped]
    public bool IsNetworkGame
    {
        get => DbIsNetworkGame != 0;
        set => DbIsNetworkGame = value ? 1 : 0;
    }

    /// <summary>
    /// Active cars in session
    /// </summary>
    public int ActiveCars { get; set; }

    /// <summary>
    /// AI difficulty (0-110)
    /// </summary>
    public ushort AiDifficulty { get; set; }

    /// <summary>
    /// Session length
    /// </summary>
    public SessionLength SessionLength { get; set; }

    /// <summary>
    /// Flag, if session is finished correctly - database only value
    /// </summary>
    [Column("IsFinished")]
    public int DbIsFinished { get; set; }

    /// <summary>
    /// Flag, if session is finished correctly
    /// </summary>
    [NotMapped]
    public bool IsFinished
    {
        get => DbIsFinished != 0;
        set => DbIsFinished = value ? 1 : 0;
    }

    #endregion // Properties

    #region Navigation properties

    /// <summary>
    /// Track information
    /// </summary>
    [ForeignKey(nameof(TrackId))]
    public virtual TrackEntity Track { get; set; }

    /// <summary>
    /// Game information
    /// </summary>
    [ForeignKey(nameof(GameVersionId))]
    public virtual GameVersionEntity GameVersion { get; set; }

    /// <summary>
    /// Participants of current session
    /// </summary>
    public virtual ICollection<ParticipantEntity> Participants { get; } = new List<ParticipantEntity>();

    /// <summary>
    /// Final classifications of current session
    /// </summary>
    public virtual ICollection<FinalClassificationEntity> FinalClassifications { get; } = new List<FinalClassificationEntity>();

    #endregion // Navigation properties
}