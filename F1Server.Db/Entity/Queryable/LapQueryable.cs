using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing lap entity
/// </summary>
public class LapQueryable : QueryableBase<LapEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public LapQueryable(IQueryable<LapEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}