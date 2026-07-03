using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;

namespace F1Server.WebApi.Cache;

/// <summary>
/// Provides a caching mechanism for storing and retrieving the fastest lap data for racing sessions
/// </summary>
internal static class FastestLapPerSessionCache
{
    #region Constants

    private const string TimeLiteral = @"ss\.fff";

    #endregion // Constants

    #region Fields

    private static readonly Dictionary<long, FastestLapSessionViewData> _fastestLapCache = new();
    private static readonly object _cacheLock = new();
    private static bool _cacheInitialized = false;

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
        await EnsureCacheInitialized().ConfigureAwait(true);

        lock (_cacheLock)
        {
            if (_fastestLapCache.ContainsKey(sessionId) == false)
            {
                UpdateCacheForSession(sessionId);
            }

            if (_fastestLapCache.TryGetValue(sessionId, out var fastestLapData))
            {
                return fastestLapData;
            }
        }

        return new FastestLapSessionViewData
               {
                   SessionId = sessionId
               };
    }

    /// <summary>
    /// Updates the cache with the fastest lap data for the specified session
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which the cache should be updated</param>
    private static void UpdateCacheForSession(long sessionId)
    {
        lock (_cacheLock)
        {
            using var dbFactory = RepositoryFactory.CreateInstance();

            var fastestLapData = CalculateFastestLapDataAsync(sessionId, dbFactory).GetAwaiter().GetResult();

            if (fastestLapData != null)
            {
                _fastestLapCache[sessionId] = fastestLapData;
            }
        }
    }

    /// <summary>
    /// Calculates the fastest lap data for a given session, including driver information, lap times, and sector times
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which to calculate the fastest lap data</param>
    /// <param name="dbFactory">The repository factory used to access session and lap data from the database</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// A <see cref="FastestLapSessionViewData"/> object containing the fastest lap details for the session,  or <see
    /// langword="null"/> if the session is invalid or contains no valid laps
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

        if (session != null)
        {
            // Get laps of session
            var lapQuery = dbFactory.GetRepository<LapRepository>()?.GetQuery();

            List<LapEntity> laps = [];

            if (lapQuery != null)
            {
                laps = await lapQuery.Where(l => l.SessionId == session.Id
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
            }

            if (laps.Count > 0)
            {
                var lapData = laps.MinBy(l => l.LapTime);

                if (lapData != null)
                {
                    fastestLapData.FastestLapDriver = lapData.Participant.Driver.Name;
                    fastestLapData.FastestLapDriverId = lapData.Participant.DriverId;
                    fastestLapData.FastestLap = TimeSpan.FromMilliseconds(lapData.LapTime).ToString(@"mm\:ss\.fff");
                    fastestLapData.IsFastestLapDriverHuman = lapData.Participant.IsHumanControlled;
                    fastestLapData.FastestLapSector1 = TimeSpan.FromMilliseconds(lapData.Sector1Time).ToString(TimeLiteral);
                    fastestLapData.FastestLapSector2 = TimeSpan.FromMilliseconds(lapData.Sector2Time).ToString(TimeLiteral);
                    fastestLapData.FastestLapSector3 = TimeSpan.FromMilliseconds(lapData.Sector3Time).ToString(TimeLiteral);

                    fastestLapData.IsFastestLapSector1 = laps.Min(l => l.Sector1Time) == lapData.Sector1Time;
                    fastestLapData.IsFastestLapSector2 = laps.Min(l => l.Sector2Time) == lapData.Sector2Time;
                    fastestLapData.IsFastestLapSector3 = laps.Min(l => l.Sector3Time) == lapData.Sector3Time;
                }

                GetFastestSectors(fastestLapData, laps);

                var humanLaps = laps.Where(l => l.Participant.IsHumanControlled).ToList();

                if (humanLaps.Count > 0)
                {
                    var fastestLapByHuman = humanLaps.MinBy(l => l.LapTime);

                    if (fastestLapByHuman != null)
                    {
                        fastestLapData.HumanPlayersFastestLap = TimeSpan.FromMilliseconds(fastestLapByHuman.LapTime).ToString(@"mm\:ss\.fff");
                        fastestLapData.ReferenceDifferenceHumanLapTime = TimeSpan.FromMilliseconds(session.Track.LapReferenceTime - fastestLapByHuman.LapTime).ToString(TimeLiteral);

                        fastestLapData.ReferenceDifferenceHumanSector1Time = TimeSpan.FromMilliseconds(session.Track.Sector1ReferenceTime - fastestLapByHuman.Sector1Time).ToString(TimeLiteral);
                        fastestLapData.ReferenceDifferenceHumanSector2Time = TimeSpan.FromMilliseconds(session.Track.Sector2ReferenceTime - fastestLapByHuman.Sector2Time).ToString(TimeLiteral);
                        fastestLapData.ReferenceDifferenceHumanSector3Time = TimeSpan.FromMilliseconds(session.Track.Sector3ReferenceTime - fastestLapByHuman.Sector3Time).ToString(TimeLiteral);
                    }
                }
            }

            fastestLapData.ReferenceLapTime = TimeSpan.FromMilliseconds(session.Track.LapReferenceTime).ToString(@"mm\:ss\.fff");
            fastestLapData.ReferenceSector1Time = TimeSpan.FromMilliseconds(session.Track.Sector1ReferenceTime).ToString(TimeLiteral);
            fastestLapData.ReferenceSector2Time = TimeSpan.FromMilliseconds(session.Track.Sector2ReferenceTime).ToString(TimeLiteral);
            fastestLapData.ReferenceSector3Time = TimeSpan.FromMilliseconds(session.Track.Sector3ReferenceTime).ToString(TimeLiteral);
        }

        return fastestLapData;
    }

    /// <summary>
    /// Ensures that the cache is initialized with the fastest lap data for all sessions
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed</param>
    /// <returns>A task that represents the asynchronous operation of initializing the cache</returns>
    private static async Task EnsureCacheInitialized(CancellationToken cancellationToken = default)
    {
        if (_cacheInitialized == false)
        {
            using var dbFactory = RepositoryFactory.CreateInstance();

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
                var fastestLapData = await CalculateFastestLapDataAsync(sessionId, dbFactory, cancellationToken).ConfigureAwait(true);

                if (fastestLapData != null)
                {
                    lock (_cacheLock)
                    {
                        // Update or add the fastest lap data for the session
                        _fastestLapCache[sessionId] = fastestLapData;
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    // If cancellation is requested, exit the loop
                    break;
                }
            }

            lock (_cacheLock)
            {
                // Set cache initialized flag
                _cacheInitialized = true;
            }
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

        fastestLapData.TheoreticalFastestLap = TimeSpan.FromMilliseconds(theoreticalLapTime).ToString(@"mm\:ss\.fff");
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