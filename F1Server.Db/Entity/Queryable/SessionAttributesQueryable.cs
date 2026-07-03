using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing session entity
/// </summary>
public class SessionAttributesQueryable : QueryableBase<SessionAttributesEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public SessionAttributesQueryable(IQueryable<SessionAttributesEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}