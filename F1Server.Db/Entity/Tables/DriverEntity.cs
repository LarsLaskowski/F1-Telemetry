using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of all drivers, depends on the game version
/// </summary>
[Table("Drivers")]
[Comment("Entity of all drivers, depends on the game version.")]
[Index(nameof(DriverGameId))]
public class DriverEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id of the driver in the game
    /// </summary>
    public int DriverGameId { get; set; }

    /// <summary>
    /// Name of the driver
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Flag, if this driver is a human player (only for database)
    /// </summary>
    [Column("IsHumanDriver")]
    public int DbIsHumanDriver { get; set; }

    /// <summary>
    /// Flag, if this driver is a human player
    /// </summary>
    [NotMapped]
    public bool IsHumanDriver
    {
        get => DbIsHumanDriver != 0;
        set => DbIsHumanDriver = value ? 1 : 0;
    }

    #endregion // Properties
}