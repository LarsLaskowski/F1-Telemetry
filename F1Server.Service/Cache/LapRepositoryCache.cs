using System.Collections.Concurrent;

using F1Server.Db.Entity.Tables;

namespace F1Server.Service.Cache;

/// <summary>
/// Caches LapEntity objects for fast lookup by Id or by (SessionId, LapNumber, ParticipantId)
/// </summary>
internal static class LapRepositoryCache
{
    #region Fields

    private static readonly ConcurrentDictionary<long, LapEntity> _byId = new();
    private static readonly ConcurrentDictionary<(ushort LapNumber, long ParticipantId), LapEntity> _byLapNumberParticipant = new();
    private static readonly Lock _lock = new();

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Gets a lap by Id from cache, or null if not found
    /// </summary>
    /// <param name="id">Lap database Id</param>
    /// <returns>The LapEntity or null</returns>
    public static LapEntity? GetById(long id)
    {
        _byId.TryGetValue(id, out var lap);

        return lap;
    }

    /// <summary>
    /// Gets a lap by LapNumber, and ParticipantId from cache, or null if not found
    /// </summary>
    /// <param name="lapNumber">Lap number</param>
    /// <param name="participantId">Participant Id</param>
    /// <returns>The LapEntity or null</returns>
    public static LapEntity? GetByLapNumberParticipant(ushort lapNumber, long participantId)
    {
        _byLapNumberParticipant.TryGetValue((lapNumber, participantId), out var lap);

        return lap;
    }

    /// <summary>
    /// Adds or updates a lap in the cache
    /// </summary>
    /// <param name="lap">The LapEntity to add or update</param>
    public static void AddOrUpdate(LapEntity lap)
    {
        _byId[lap.Id] = lap;
        _byLapNumberParticipant[(lap.LapNumber, lap.ParticipantId)] = lap;
    }

    /// <summary>
    /// Clears the cache (e.g. after DB changes)
    /// </summary>
    public static void Clear()
    {
        lock (_lock)
        {
            _byId.Clear();
            _byLapNumberParticipant.Clear();
        }
    }

    #endregion // Methods
}