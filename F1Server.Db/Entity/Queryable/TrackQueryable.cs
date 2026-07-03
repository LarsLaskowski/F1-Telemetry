using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing track entity
/// </summary>
public class TrackQueryable : QueryableBase<TrackEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public TrackQueryable(IQueryable<TrackEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}