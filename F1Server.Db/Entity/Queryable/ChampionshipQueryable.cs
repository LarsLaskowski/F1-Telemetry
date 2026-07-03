using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing championship entity
/// </summary>
public class ChampionshipQueryable : QueryableBase<ChampionshipEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public ChampionshipQueryable(IQueryable<ChampionshipEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}