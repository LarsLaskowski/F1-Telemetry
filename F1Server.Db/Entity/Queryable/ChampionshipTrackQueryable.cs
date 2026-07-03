using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing championship track entity
/// </summary>
public class ChampionshipTrackQueryable : QueryableBase<ChampionshipTrackEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public ChampionshipTrackQueryable(IQueryable<ChampionshipTrackEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}