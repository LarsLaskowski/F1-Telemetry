using System.Collections.Concurrent;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Service.Cache;

/// <summary>
/// Provides a caching mechanism for storing and retrieving the fastest lap data for racing sessions
/// </summary>
public static class FastestLapPerSessionCache
{
    #region Constants

    private const string TimeLiteral = @"ss\.fff";

    private const string LapTimeLiteral = @"mm\:ss\.fff";

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Cached fastest lap data per session id
    /// </summary>
    private static readonly ConcurrentDictionary<long, FastestLapSessionViewData> _fastestLapCache = new();

    /// <summary>
    /// Gates the calculation of a single session so the same session is not calculated twice in parallel
    /// </summary>
    private static readonly ConcurrentDictionary<long, SemaphoreSlim> _sessionLocks = new();

    /// <summary>
    /// Change counter per session id, incremented by every invalidation so a calculation that started before the
    /// invalidation does not store its outdated result
    /// </summary>
    private static readonly ConcurrentDictionary<long, long> _sessionVersions = new();

    /// <summary>
    /// Gates the initial cache warm up so it only runs once
    /// </summary>
    private static readonly SemaphoreSlim _initializationLock = new(1, 1);

    /// <summary>
    /// Is the cache already warmed up?
    /// </summary>
    private static volatile bool _cacheInitialized = false;

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Initializes the cache asynchronously, ensuring it is ready for use
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task InitializeCacheAsync(CancellationToken cancellationToken)
    {
        await EnsureCacheInitialized(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the fastest lap data for a specified session
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which to retrieve the fastest lap data</param>
    /// <returns>
    /// An instance of <see cref="FastestLapSessionViewData"/> containing the fastest lap information for the session.
    /// If no data is available, a new <see cref="FastestLapSessionViewData"/> with the specified <paramref
    /// name="sessionId"/> is returned
    /// </returns>
    public static async Task<FastestLapSessionViewData> GetFastestLapDataForSessionAsync(long sessionId)
    {
        await EnsureCacheInitialized().ConfigureAwait(false);

        if (_fastestLapCache.TryGetValue(sessionId, out var cachedLapData))
        {
            return cachedLapData;
        }

        var fastestLapData = await UpdateCacheForSessionAsync(sessionId).ConfigureAwait(false);

        return fastestLapData ?? new FastestLapSessionViewData
                                 {
                                     SessionId = sessionId
                                 };
    }

    /// <summary>
    /// Drops the cached fastest lap data of a session, so the next request calculates it again. Has to be called
    /// whenever laps of the session were added, changed or removed - otherwise a running session keeps reporting the
    /// fastest lap of the moment its entry was calculated
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session whose cached data is no longer up to date</param>
    public static void InvalidateSession(long sessionId)
    {
        // A calculation that is currently running for this session sees the new version and discards its result
        _sessionVersions.AddOrUpdate(sessionId, 1L, (_, version) => version + 1L);

        _fastestLapCache.TryRemove(sessionId, out _);
    }

    /// <summary>
    /// Removes a session from the cache completely, including its calculation gate. Has to be called when the session
    /// itself was deleted, so no data of a no longer existing session is kept
    /// </summary>
    /// <param name="sessionId">The unique identifier of the deleted session</param>
    public static void RemoveSession(long sessionId)
    {
        InvalidateSession(sessionId);

        _sessionLocks.TryRemove(sessionId, out _);
    }

    /// <summary>
    /// Gets the current change counter of a session
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session</param>
    /// <returns>The change counter of the session, or 0 if the session was never invalidated</returns>
    private static long GetSessionVersion(long sessionId)
    {
        return _sessionVersions.TryGetValue(sessionId, out var version)
                   ? version
                   : 0L;
    }

    /// <summary>
    /// Updates the cache with the fastest lap data for the specified session without blocking other sessions
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which the cache should be updated</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// The cached <see cref="FastestLapSessionViewData"/> of the session or <see langword="null"/> if no data could be
    /// calculated
    /// </returns>
    private static async Task<FastestLapSessionViewData?> UpdateCacheForSessionAsync(long sessionId, CancellationToken cancellationToken = default)
    {
        // Only one calculation per session at a time - requests for other sessions are not blocked
        var sessionLock = _sessionLocks.GetOrAdd(sessionId, _ => new SemaphoreSlim(1, 1));

        await sessionLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (_fastestLapCache.TryGetValue(sessionId, out var cachedLapData))
            {
                return cachedLapData;
            }

            var versionBeforeCalculation = GetSessionVersion(sessionId);

            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                var fastestLapData = await CalculateFastestLapDataAsync(sessionId, dbFactory, cancellationToken).ConfigureAwait(false);

                // An invalidation during the calculation makes the result outdated before it is stored
                if (GetSessionVersion(sessionId) == versionBeforeCalculation)
                {
                    _fastestLapCache[sessionId] = fastestLapData;
                }

                return fastestLapData;
            }
        }
        finally
        {
            sessionLock.Release();
        }
    }

    /// <summary>
    /// Calculates the fastest lap data for a given session, including driver information, lap times, and sector times
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which to calculate the fastest lap data</param>
    /// <param name="dbFactory">The repository factory used to access session and lap data from the database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// A <see cref="FastestLapSessionViewData"/> object containing the fastest lap details for the session. If the
    /// session is invalid or contains no valid laps, the object carries only the session id and otherwise stays empty
    /// </returns>
    private static async Task<FastestLapSessionViewData> CalculateFastestLapDataAsync(long sessionId, RepositoryFactory dbFactory, CancellationToken cancellationToken = default)
    {
        var fastestLapData = new FastestLapSessionViewData
                             {
                                 SessionId = sessionId
                             };

        // Is valid session?
        var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

        var session = sessionQuery == null
                          ? null
                          : await sessionQuery.Include(obj => obj.Track)
                                              .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken)
                                              .ConfigureAwait(false);

        if (session == null)
        {
            return fastestLapData;
        }

        var laps = await LoadValidLapsAsync(session.Id, dbFactory, cancellationToken).ConfigureAwait(false);

        if (laps.Count > 0)
        {
            SetFastestLap(fastestLapData, laps);

            GetFastestSectors(fastestLapData, laps);

            SetHumanFastestLap(fastestLapData, session, laps);
        }

        fastestLapData.ReferenceLapTime = TimeSpan.FromMilliseconds(session.Track.LapReferenceTime).ToString(LapTimeLiteral);
        fastestLapData.ReferenceSector1Time = TimeSpan.FromMilliseconds(session.Track.Sector1ReferenceTime).ToString(TimeLiteral);
        fastestLapData.ReferenceSector2Time = TimeSpan.FromMilliseconds(session.Track.Sector2ReferenceTime).ToString(TimeLiteral);
        fastestLapData.ReferenceSector3Time = TimeSpan.FromMilliseconds(session.Track.Sector3ReferenceTime).ToString(TimeLiteral);

        return fastestLapData;
    }

    /// <summary>
    /// Loads the completed and valid laps of a session together with their participant and driver
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session whose laps are loaded</param>
    /// <param name="dbFactory">The repository factory used to access the lap data from the database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The laps of the session, or an empty list if the laps cannot be read</returns>
    private static async Task<List<LapEntity>> LoadValidLapsAsync(long sessionId, RepositoryFactory dbFactory, CancellationToken cancellationToken)
    {
        var lapQuery = dbFactory.GetRepository<LapRepository>()?.GetQuery();

        var laps = lapQuery == null
                       ? null
                       : await lapQuery.Where(l => l.SessionId == sessionId
                                                   && l.LapTime > 0
                                                   && l.Sector1Time > 0
                                                   && l.Sector2Time > 0
                                                   && l.Sector3Time > 0
                                                   && l.DbIsCompleted == 1
                                                   && l.DbIsInvalidLapTime == 0)
                                       .Include(l => l.Participant)
                                       .ThenInclude(p => p.Driver)
                                       .ToListAsync(cancellationToken)
                                       .ConfigureAwait(false);

        return laps ?? [];
    }

    /// <summary>
    /// Sets the fastest lap of the session and marks the sectors of that lap that are the fastest ones of the session
    /// </summary>
    /// <param name="fastestLapData">Data structure the fastest lap is stored into</param>
    /// <param name="laps">List of all laps from session</param>
    private static void SetFastestLap(FastestLapSessionViewData fastestLapData, List<LapEntity> laps)
    {
        var lapData = laps.MinBy(l => l.LapTime);

        if (lapData == null)
        {
            return;
        }

        fastestLapData.FastestLapDriver = lapData.Participant.Driver.Name;
        fastestLapData.FastestLapDriverId = lapData.Participant.DriverId;
        fastestLapData.FastestLap = TimeSpan.FromMilliseconds(lapData.LapTime).ToString(LapTimeLiteral);
        fastestLapData.IsFastestLapDriverHuman = lapData.Participant.IsHumanControlled;
        fastestLapData.FastestLapSector1 = TimeSpan.FromMilliseconds(lapData.Sector1Time).ToString(TimeLiteral);
        fastestLapData.FastestLapSector2 = TimeSpan.FromMilliseconds(lapData.Sector2Time).ToString(TimeLiteral);
        fastestLapData.FastestLapSector3 = TimeSpan.FromMilliseconds(lapData.Sector3Time).ToString(TimeLiteral);

        fastestLapData.IsFastestLapSector1 = laps.Min(l => l.Sector1Time) == lapData.Sector1Time;
        fastestLapData.IsFastestLapSector2 = laps.Min(l => l.Sector2Time) == lapData.Sector2Time;
        fastestLapData.IsFastestLapSector3 = laps.Min(l => l.Sector3Time) == lapData.Sector3Time;
    }

    /// <summary>
    /// Sets the fastest lap of the human controlled participants and its difference to the reference times of the track
    /// </summary>
    /// <param name="fastestLapData">Data structure the fastest human lap is stored into</param>
    /// <param name="session">Session the laps belong to, carrying the reference times of its track</param>
    /// <param name="laps">List of all laps from session</param>
    private static void SetHumanFastestLap(FastestLapSessionViewData fastestLapData, SessionEntity session, List<LapEntity> laps)
    {
        var fastestLapByHuman = laps.Where(l => l.Participant.IsHumanControlled)
                                    .MinBy(l => l.LapTime);

        if (fastestLapByHuman == null)
        {
            return;
        }

        fastestLapData.HumanPlayersFastestLap = TimeSpan.FromMilliseconds(fastestLapByHuman.LapTime).ToString(LapTimeLiteral);
        fastestLapData.ReferenceDifferenceHumanLapTime = TimeSpan.FromMilliseconds(session.Track.LapReferenceTime - fastestLapByHuman.LapTime).ToString(TimeLiteral);

        fastestLapData.ReferenceDifferenceHumanSector1Time = TimeSpan.FromMilliseconds(session.Track.Sector1ReferenceTime - fastestLapByHuman.Sector1Time).ToString(TimeLiteral);
        fastestLapData.ReferenceDifferenceHumanSector2Time = TimeSpan.FromMilliseconds(session.Track.Sector2ReferenceTime - fastestLapByHuman.Sector2Time).ToString(TimeLiteral);
        fastestLapData.ReferenceDifferenceHumanSector3Time = TimeSpan.FromMilliseconds(session.Track.Sector3ReferenceTime - fastestLapByHuman.Sector3Time).ToString(TimeLiteral);
    }

    /// <summary>
    /// Ensures that the cache is initialized with the fastest lap data for all sessions. A warm up that is aborted by a
    /// cancellation request leaves the cache uninitialized, so the next caller warms it up again
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed</param>
    /// <returns>A task that represents the asynchronous operation of initializing the cache</returns>
    private static async Task EnsureCacheInitialized(CancellationToken cancellationToken = default)
    {
        if (_cacheInitialized)
        {
            return;
        }

        // Asynchronous gate - concurrent callers wait without blocking a thread pool thread
        await _initializationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            if (_cacheInitialized)
            {
                return;
            }

            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

                List<long> sessionIds = [];

                if (sessionQuery != null)
                {
                    sessionIds = await sessionQuery.Select(s => s.Id)
                                                   .ToListAsync(cancellationToken)
                                                   .ConfigureAwait(false);
                }

                foreach (var sessionId in sessionIds)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // The warm up was aborted before all sessions were calculated, so the cache stays uninitialized
                        // and is warmed up again by the next caller
                        return;
                    }

                    var versionBeforeCalculation = GetSessionVersion(sessionId);

                    var fastestLapData = await CalculateFastestLapDataAsync(sessionId, dbFactory, cancellationToken).ConfigureAwait(false);

                    if (GetSessionVersion(sessionId) == versionBeforeCalculation)
                    {
                        // Update or add the fastest lap data for the session
                        _fastestLapCache[sessionId] = fastestLapData;
                    }
                }
            }

            // Set cache initialized flag
            _cacheInitialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    /// <summary>
    /// Get fastest sector times from laps
    /// </summary>
    /// <param name="fastestLapData">Data structur store fastest sector times into</param>
    /// <param name="laps">List of all laps from session</param>
    private static void GetFastestSectors(FastestLapSessionViewData fastestLapData, List<LapEntity> laps)
    {
        var theoreticalLapTime = 0U;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFastestSectors));

        // Fastest sector 1
        GetFastestSector1(fastestLapData, laps, ref theoreticalLapTime);

        // Fastest sector 2
        GetFastestSector2(fastestLapData, laps, ref theoreticalLapTime);

        // Fastest sector 3
        GetFastestSector3(fastestLapData, laps, ref theoreticalLapTime);

        fastestLapData.TheoreticalFastestLap = TimeSpan.FromMilliseconds(theoreticalLapTime).ToString(LapTimeLiteral);
    }

    /// <summary>
    /// Get fastest sector 1
    /// </summary>
    /// <param name="fastestLapData">Data structure store fastest sector times into</param>
    /// <param name="laps">List of all laps from session</param>
    /// <param name="theoreticalLapTime">Theoretical fastest lap time</param>
    private static void GetFastestSector1(FastestLapSessionViewData fastestLapData, List<LapEntity> laps, ref uint theoreticalLapTime)
    {
        var lapData = laps.MinBy(s => s.Sector1Time);

        if (lapData != null)
        {
            fastestLapData.FastestSector1Driver = lapData.Participant.Driver.Name;
            fastestLapData.FastestSector1DriverId = lapData.Participant.DriverId;
            fastestLapData.FastestSector1 = TimeSpan.FromMilliseconds(lapData.Sector1Time).ToString(TimeLiteral);
            fastestLapData.IsFastestSector1DriverHuman = lapData.Participant.IsHumanControlled;

            theoreticalLapTime = lapData.Sector1Time;

            if (fastestLapData.IsFastestLapDriverHuman == false)
            {
                if (laps.Exists(l => l.Participant.DbIsHumanControlled == 1))
                {
                    var humanFastestSector1 = laps.Where(l => l.Participant.DbIsHumanControlled == 1).Min(s => s.Sector1Time);

                    fastestLapData.FastestHumanSector1 = TimeSpan.FromMilliseconds(humanFastestSector1).ToString(TimeLiteral);
                }
            }
            else
            {
                fastestLapData.FastestHumanSector1 = fastestLapData.FastestSector1;
            }
        }
    }

    /// <summary>
    /// Get fastest sector 2
    /// </summary>
    /// <param name="fastestLapData">Data structure store fastest sector times into</param>
    /// <param name="laps">List of all laps from session</param>
    /// <param name="theoreticalLapTime">Theoretical fastest lap time</param>
    private static void GetFastestSector2(FastestLapSessionViewData fastestLapData, List<LapEntity> laps, ref uint theoreticalLapTime)
    {
        var lapData = laps.MinBy(s => s.Sector2Time);

        if (lapData != null)
        {
            fastestLapData.FastestSector2Driver = lapData.Participant.Driver.Name;
            fastestLapData.FastestSector2DriverId = lapData.Participant.DriverId;
            fastestLapData.FastestSector2 = TimeSpan.FromMilliseconds(lapData.Sector2Time).ToString(TimeLiteral);
            fastestLapData.IsFastestSector2DriverHuman = lapData.Participant.IsHumanControlled;

            theoreticalLapTime += lapData.Sector2Time;

            if (fastestLapData.IsFastestLapDriverHuman == false)
            {
                if (laps.Exists(l => l.Participant.DbIsHumanControlled == 1))
                {
                    var humanFastestSector2 = laps.Where(l => l.Participant.DbIsHumanControlled == 1).Min(s => s.Sector2Time);

                    fastestLapData.FastestHumanSector2 = TimeSpan.FromMilliseconds(humanFastestSector2).ToString(TimeLiteral);
                }
            }
            else
            {
                fastestLapData.FastestHumanSector2 = fastestLapData.FastestSector2;
            }
        }
    }

    /// <summary>
    /// Get fastest sector 3
    /// </summary>
    /// <param name="fastestLapData">Data structure store fastest sector times into</param>
    /// <param name="laps">List of all laps from session</param>
    /// <param name="theoreticalLapTime">Theoretical fastest lap time</param>
    private static void GetFastestSector3(FastestLapSessionViewData fastestLapData, List<LapEntity> laps, ref uint theoreticalLapTime)
    {
        var lapData = laps.MinBy(s => s.Sector3Time);

        if (lapData != null)
        {
            fastestLapData.FastestSector3Driver = lapData.Participant.Driver.Name;
            fastestLapData.FastestSector3DriverId = lapData.Participant.DriverId;
            fastestLapData.FastestSector3 = TimeSpan.FromMilliseconds(lapData.Sector3Time).ToString(TimeLiteral);
            fastestLapData.IsFastestSector3DriverHuman = lapData.Participant.IsHumanControlled;

            theoreticalLapTime += lapData.Sector3Time;

            if (fastestLapData.IsFastestLapDriverHuman == false)
            {
                if (laps.Exists(l => l.Participant.DbIsHumanControlled == 1))
                {
                    var humanFastestSector3 = laps.Where(l => l.Participant.DbIsHumanControlled == 1).Min(s => s.Sector3Time);

                    fastestLapData.FastestHumanSector3 = TimeSpan.FromMilliseconds(humanFastestSector3).ToString(TimeLiteral);
                }
            }
            else
            {
                fastestLapData.FastestHumanSector3 = fastestLapData.FastestSector3;
            }
        }
    }

    #endregion // Methods
}