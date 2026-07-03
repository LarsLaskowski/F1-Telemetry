using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing driver entity
/// </summary>
public class DriverQueryable : QueryableBase<DriverEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public DriverQueryable(IQueryable<DriverEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}