using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Game version
/// </summary>
[Table("GameVersions")]
[Comment("Game versions table. Contains information about game versions used in sessions.")]
[Index(nameof(Version))]
public class GameVersionEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// Version of game
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Name of game
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Major game version
    /// </summary>
    public int MajorVersion { get; set; }

    /// <summary>
    /// Minor game version
    /// </summary>
    public int MinorVersion { get; set; }

    /// <summary>
    /// Last usage timestamp
    /// </summary>
    public DateTime? LastUsed { get; set; }

    #endregion // Properties
}