using F1Server.Db.Entity.Queryable;
using F1Server.Db.Entity.Repositories.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Repositories;

/// <summary>
/// Repository for accessing <see cref="Tables.ChampionshipPointsEntity"/>
/// </summary>
public class ChampionshipPointsRepository : RepositoryBase<ChampionshipPointsQueryable, ChampionshipPointsEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext">DbContext</param>
    public ChampionshipPointsRepository(F1ServerDbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion // Constructors
}