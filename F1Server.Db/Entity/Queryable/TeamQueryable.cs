using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing team entity
/// </summary>
public class TeamQueryable : QueryableBase<TeamEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public TeamQueryable(IQueryable<TeamEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}