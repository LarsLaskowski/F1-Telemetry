using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing championship points entity
/// </summary>
public class ChampionshipPointsQueryable : QueryableBase<ChampionshipPointsEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public ChampionshipPointsQueryable(IQueryable<ChampionshipPointsEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}