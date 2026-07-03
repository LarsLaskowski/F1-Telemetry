using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing game version entity
/// </summary>
public class GameVersionQueryable : QueryableBase<GameVersionEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public GameVersionQueryable(IQueryable<GameVersionEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}