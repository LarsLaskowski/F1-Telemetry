using F1Server.Db.Entity.Queryable;
using F1Server.Db.Entity.Repositories.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Repositories;

/// <summary>
/// Repository for accessing <see cref="Tables.LapEntity"/>
/// </summary>
public class LapRepository : RepositoryBase<LapQueryable, LapEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext">DbContext</param>
    public LapRepository(F1ServerDbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion // Constructors
}