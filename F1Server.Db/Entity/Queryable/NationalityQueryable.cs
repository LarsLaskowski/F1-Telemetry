using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing nationality entity
/// </summary>
public class NationalityQueryable : QueryableBase<NationalityEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public NationalityQueryable(IQueryable<NationalityEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}