using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of all teams, depends on the game version
/// </summary>
[Table("Teams")]
[Comment("Entity of all teams, depends on the game version")]
[Index(nameof(TeamGameId))]
public class TeamEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id of the team in the game
    /// </summary>
    public int TeamGameId { get; set; }

    /// <summary>
    /// Name of the team
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; }

    #endregion // Properties
}