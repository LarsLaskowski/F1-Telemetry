using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Tables;

/// <summary>
/// Entity of all nationalities
/// </summary>
[Table("Nationalities")]
[Comment("Entity of all nationalities")]
[Index(nameof(NationalityGameId))]
public class NationalityEntity
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id of the nationality in the game
    /// </summary>
    public ushort NationalityGameId { get; set; }

    /// <summary>
    /// Name of the nationality
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; }

    #endregion // Properties
}