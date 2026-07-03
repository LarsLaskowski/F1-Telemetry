using System.Collections.Concurrent;

using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Service.Cache;

/// <summary>
/// Caches DriverEntity objects for fast lookup by DriverGameId or Id
/// </summary>
internal static class DriverRepositoryCache
{
    #region Fields

    private static readonly ConcurrentDictionary<int, DriverEntity> _byGameId = new();
    private static readonly ConcurrentDictionary<long, DriverEntity> _byId = new();
    private static readonly Lock _lock = new();
    private static bool _initialized = false;

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Ensures that the necessary drivers are loaded and initialized
    /// </summary>
    /// <returns><see langword="true"/> if the drivers are successfully initialized; otherwise, <see langword="false"/></returns>
    public static bool LoadDrivers()
    {
        EnsureInitialized();

        return _initialized;
    }

    /// <summary>
    /// Gets a driver by DriverGameId from cache, or null if not found
    /// </summary>
    /// <param name="driverGameId">The DriverGameId to look up</param>
    /// <returns>The DriverEntity or null</returns>
    public static DriverEntity? GetByGameId(int driverGameId)
    {
        EnsureInitialized();

        _byGameId.TryGetValue(driverGameId, out var driver);

        return driver;
    }

    /// <summary>
    /// Gets a driver by Id from cache, or null if not found
    /// </summary>
    /// <param name="id">The database Id to look up</param>
    /// <returns>The DriverEntity or null</returns>
    public static DriverEntity? GetById(long id)
    {
        EnsureInitialized();

        _byId.TryGetValue(id, out var driver);

        return driver;
    }

    /// <summary>
    /// Adds or updates a driver in the cache
    /// </summary>
    /// <param name="driver">The DriverEntity to add or update</param>
    public static void AddOrUpdate(DriverEntity driver)
    {
        EnsureInitialized();

        _byGameId[driver.DriverGameId] = driver;
        _byId[driver.Id] = driver;
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

                    var allDrivers = dbFactory.GetRepository<DriverRepository>()?.GetQuery()?.ToList();

                    if (allDrivers != null)
                    {
                        foreach (var driver in allDrivers)
                        {
                            _byGameId[driver.DriverGameId] = driver;
                            _byId[driver.Id] = driver;
                        }
                    }

                    _initialized = true;
                }
            }
        }
    }

    #endregion // Methods
}