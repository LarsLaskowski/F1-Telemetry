using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing participant entity
/// </summary>
public class ParticipantQueryable : QueryableBase<ParticipantEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public ParticipantQueryable(IQueryable<ParticipantEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}