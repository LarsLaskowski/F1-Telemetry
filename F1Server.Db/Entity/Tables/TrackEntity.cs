using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Game version
/// </summary>
[Table("Tracks")]
[Comment("Tracks table containing track information from the game.")]
[Index(nameof(TrackNumber))]
public class TrackEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Track number from the game
    /// </summary>
    public int TrackNumber { get; set; }

    /// <summary>
    /// Name of track
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Lap reference time
    /// </summary>
    public uint LapReferenceTime { get; set; }

    /// <summary>
    /// Reference time sector 1
    /// </summary>
    public uint Sector1ReferenceTime { get; set; }

    /// <summary>
    /// Reference time sector 2
    /// </summary>
    public uint Sector2ReferenceTime { get; set; }

    /// <summary>
    /// Reference time sector 3
    /// </summary>
    public uint Sector3ReferenceTime { get; set; }

    #endregion // Properties
}