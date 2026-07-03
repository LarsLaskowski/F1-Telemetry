using System.Collections.Concurrent;

using F1Server.Db.Entity.Tables;
using F1Server.Service.Cache.Keys;

namespace F1Server.Service.Cache;

/// <summary>
/// Caches <see cref="ParticipantEntity"/> objects for fast lookup by SessionId and DriverId
/// </summary>
internal static class ParticipantsRepositoryCache
{
    #region Fields

    /// <summary>
    /// Cache for participants by session and driver
    /// </summary>
    private static readonly ConcurrentDictionary<ParticipantCacheKey, ParticipantEntity> _bySessionAndDriver = new();

    /// <summary>
    /// Cache for participants by database id
    /// </summary>
    private static readonly ConcurrentDictionary<long, ParticipantEntity> _byId = new();

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Gets a participant by session and driver id
    /// </summary>
    /// <param name="sessionId">Session id</param>
    /// <param name="driverId">Driver id</param>
    /// <returns>The <see cref="ParticipantEntity"/> or null if not found</returns>
    public static ParticipantEntity? GetBySessionAndDriverId(long sessionId, long driverId)
    {
        _bySessionAndDriver.TryGetValue(new ParticipantCacheKey(sessionId, driverId), out var participant);

        return participant;
    }

    /// <summary>
    /// Gets a participant by database id
    /// </summary>
    /// <param name="id">Database id</param>
    /// <returns>The <see cref="ParticipantEntity"/> or null if not found</returns>
    public static ParticipantEntity? GetById(long id)
    {
        _byId.TryGetValue(id, out var participant);

        return participant;
    }

    /// <summary>
    /// Adds or updates a participant in the cache
    /// </summary>
    /// <param name="participant">The participant entity to add or update</param>
    public static void AddOrUpdate(ParticipantEntity participant)
    {
        var key = new ParticipantCacheKey(participant.SessionId, participant.DriverId);

        _bySessionAndDriver[key] = participant;
        _byId[participant.Id] = participant;
    }

    /// <summary>
    /// Clears all stored session and driver mappings, as well as all stored identifiers
    /// </summary>
    public static void Clear()
    {
        _bySessionAndDriver.Clear();
        _byId.Clear();
    }

    /// <summary>
    /// Removes all entries from the internal collections that do not belong to the specified session
    /// </summary>
    /// <param name="sessionId">The session ID used to determine which entries to retain</param>
    public static void Clear(long sessionId)
    {
        if (_bySessionAndDriver.IsEmpty == false)
        {
            var oldEntries = _bySessionAndDriver.Where(kvp => kvp.Key.SessionId != sessionId)
                                                .Select(kvp => kvp.Key)
                                                .ToList();

            foreach (var key in oldEntries)
            {
                _bySessionAndDriver.TryRemove(key, out _);
            }
        }

        if (_byId.IsEmpty == false)
        {
            var oldEntries = _byId.Where(k => k.Value.SessionId != sessionId)
                                  .Select(kvp => kvp.Key)
                                  .ToList();

            foreach (var key in oldEntries)
            {
                _byId.TryRemove(key, out _);
            }
        }
    }

    #endregion // Methods
}