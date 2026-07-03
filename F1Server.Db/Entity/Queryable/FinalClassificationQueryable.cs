using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing final classification entity
/// </summary>
public class FinalClassificationQueryable : QueryableBase<FinalClassificationEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public FinalClassificationQueryable(IQueryable<FinalClassificationEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}