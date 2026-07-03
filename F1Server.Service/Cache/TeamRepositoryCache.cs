using System.Collections.Concurrent;

using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Service.Cache;

/// <summary>
/// Caches TeamEntity objects for fast lookup by TeamGameId or Id
/// </summary>
internal static class TeamRepositoryCache
{
    #region Fields

    private static readonly ConcurrentDictionary<int, TeamEntity> _byGameId = new();
    private static readonly ConcurrentDictionary<long, TeamEntity> _byId = new();
    private static readonly Lock _lock = new();
    private static bool _initialized = false;

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Ensures that the teams are loaded and initialized
    /// </summary>
    /// <returns><see langword="true"/> if the teams have been successfully initialized; otherwise, <see langword="false"/></returns>
    public static bool LoadTeams()
    {
        EnsureInitialized();

        return _initialized;
    }

    /// <summary>
    /// Gets a team by TeamGameId from cache, or null if not found
    /// </summary>
    /// <param name="teamGameId">The TeamGameId to look up</param>
    /// <returns>The TeamEntity or null</returns>
    public static TeamEntity? GetByGameId(int teamGameId)
    {
        EnsureInitialized();

        _byGameId.TryGetValue(teamGameId, out var team);

        return team;
    }

    /// <summary>
    /// Gets a team by Id from cache, or null if not found
    /// </summary>
    /// <param name="id">The database Id to look up</param>
    /// <returns>The TeamEntity or null</returns>
    public static TeamEntity? GetById(long id)
    {
        EnsureInitialized();

        _byId.TryGetValue(id, out var team);

        return team;
    }

    /// <summary>
    /// Clears the cache (e.g. after DB changes)
    /// </summary>
    public static void Clear()
    {
        lock (_lock)
        {
            _byGameId.Clear();
            _byId.Clear();

            _initialized = false;
        }
    }

    /// <summary>
    /// Ensures the cache is loaded from the database (lazy loading)
    /// </summary>
    private static void EnsureInitialized()
    {
        if (_initialized == false)
        {
            lock (_lock)
            {
                if (_initialized == false)
                {
                    using var dbFactory = RepositoryFactory.CreateInstance();

                    var allTeams = dbFactory.GetRepository<TeamRepository>()?.GetQuery()?.ToList();

                    if (allTeams != null)
                    {
                        foreach (var team in allTeams)
                        {
                            _byGameId[team.TeamGameId] = team;
                            _byId[team.Id] = team;
                        }
                    }

                    _initialized = true;
                }
            }
        }
    }

    #endregion // Methods
}