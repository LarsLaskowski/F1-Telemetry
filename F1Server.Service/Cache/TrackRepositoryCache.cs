using System.Collections.Concurrent;

using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Service.Cache;

/// <summary>
/// Caches TrackEntity objects for fast lookup by TrackNumber or Id
/// </summary>
internal static class TrackRepositoryCache
{
    #region Fields

    private static readonly ConcurrentDictionary<int, TrackEntity> _byTrackNumber = new();
    private static readonly ConcurrentDictionary<long, TrackEntity> _byId = new();
    private static readonly Lock _lock = new();
    private static bool _initialized = false;

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Loads the tracks by ensuring the system is initialized
    /// </summary>
    /// <returns><see langword="true"/> if the system is successfully initialized; otherwise, <see langword="false"/></returns>
    public static bool LoadTracks()
    {
        EnsureInitialized();

        return _initialized;
    }

    /// <summary>
    /// Gets a track by TrackNumber from cache, or null if not found
    /// </summary>
    /// <param name="trackNumber">The TrackNumber to look up</param>
    /// <returns>The TrackEntity or null</returns>
    public static TrackEntity? GetByTrackNumber(int trackNumber)
    {
        EnsureInitialized();

        _byTrackNumber.TryGetValue(trackNumber, out var track);

        return track;
    }

    /// <summary>
    /// Gets a track by Id from cache, or null if not found
    /// </summary>
    /// <param name="id">The database Id to look up</param>
    /// <returns>The TrackEntity or null</returns>
    public static TrackEntity? GetById(long id)
    {
        EnsureInitialized();

        _byId.TryGetValue(id, out var track);

        return track;
    }

    /// <summary>
    /// Clears the cache (e.g. after DB changes)
    /// </summary>
    public static void Clear()
    {
        lock (_lock)
        {
            _byTrackNumber.Clear();
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

                    var allTracks = dbFactory.GetRepository<TrackRepository>()?.GetQuery()?.ToList();

                    if (allTracks != null)
                    {
                        foreach (var track in allTracks)
                        {
                            _byTrackNumber[track.TrackNumber] = track;
                            _byId[track.Id] = track;
                        }
                    }

                    _initialized = true;
                }
            }
        }
    }

    #endregion // Methods
}