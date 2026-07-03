using System.Collections.Concurrent;

using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Service.Cache;

/// <summary>
/// Caches NationalityEntity objects for fast lookup by NationalityGameId or Id
/// </summary>
internal static class NationalityRepositoryCache
{
    #region Fields

    private static readonly ConcurrentDictionary<int, NationalityEntity> _byGameId = new();
    private static readonly ConcurrentDictionary<long, NationalityEntity> _byId = new();
    private static readonly Lock _lock = new();
    private static bool _initialized = false;

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Loads the nationalities data and ensures the system is initialized
    /// </summary>
    /// <returns><see langword="true"/> if the system is successfully initialized; otherwise, <see langword="false"/></returns>
    public static bool LoadNationalities()
    {
        EnsureInitialized();

        return _initialized;
    }

    /// <summary>
    /// Gets a nationality by NationalityGameId from cache, or null if not found
    /// </summary>
    /// <param name="nationalityGameId">The NationalityGameId to look up</param>
    /// <returns>The NationalityEntity or null</returns>
    public static NationalityEntity? GetByGameId(int nationalityGameId)
    {
        EnsureInitialized();

        _byGameId.TryGetValue(nationalityGameId, out var nationality);

        return nationality;
    }

    /// <summary>
    /// Gets a nationality by Id from cache, or null if not found
    /// </summary>
    /// <param name="id">The database Id to look up</param>
    /// <returns>The NationalityEntity or null</returns>
    public static NationalityEntity? GetById(long id)
    {
        EnsureInitialized();

        _byId.TryGetValue(id, out var nationality);

        return nationality;
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

                    var allNationalities = dbFactory.GetRepository<NationalityRepository>()?.GetQuery()?.ToList();

                    if (allNationalities != null)
                    {
                        foreach (var nationality in allNationalities)
                        {
                            _byGameId[nationality.NationalityGameId] = nationality;
                            _byId[nationality.Id] = nationality;
                        }
                    }

                    _initialized = true;
                }
            }
        }
    }

    #endregion // Methods
}