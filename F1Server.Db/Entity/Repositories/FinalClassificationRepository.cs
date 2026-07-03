using F1Server.Db.Entity.Queryable;
using F1Server.Db.Entity.Repositories.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Repositories;

/// <summary>
/// Repository for accessing <see cref="Tables.FinalClassificationEntity"/>
/// </summary>
public class FinalClassificationRepository : RepositoryBase<FinalClassificationQueryable, FinalClassificationEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext">DbContext</param>
    public FinalClassificationRepository(F1ServerDbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion // Constructors
}