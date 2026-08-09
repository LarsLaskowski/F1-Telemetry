using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Service.CarTelemetries;

/// <summary>
/// Business logic of the car telemetry - reads the recorded telemetry values of a lap and checks whether a session
/// carries telemetry of a human controlled participant
/// </summary>
public class CarTelemetryService
{
    #region Methods

    /// <summary>
    /// Checks whether telemetry data of a human controlled participant was recorded for a session
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Status whether the session carries telemetry data of a human controlled participant</returns>
    public async Task<bool> HasUserTelemetryDataAsync(long sessionId)
    {
        var hasUserTelemetryData = false;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var telemetryQuery = dbFactory.GetRepository<CarTelemetryRepository>()?.GetQuery();

            if (telemetryQuery != null)
            {
                hasUserTelemetryData = await telemetryQuery.AnyAsync(t => t.Lap.SessionId == sessionId
                                                                          && t.Lap.Participant.DbIsHumanControlled == 1)
                                                           .ConfigureAwait(false);
            }
        }

        return hasUserTelemetryData;
    }

    /// <summary>
    /// Loads the telemetry values of a lap ordered by the number of their packet
    /// </summary>
    /// <param name="lapId">Database id of the lap</param>
    /// <returns>Telemetry values of the lap, or <see langword="null"/> when the telemetry cannot be read</returns>
    public async Task<List<TelemetryViewData>?> GetTelemetryOfLapAsync(long lapId)
    {
        List<TelemetryViewData>? telemetryData = null;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var telemetryQuery = dbFactory.GetRepository<CarTelemetryRepository>()?.GetQuery();

            if (telemetryQuery != null)
            {
                telemetryData = await LoadTelemetryOfLapAsync(telemetryQuery, lapId).ConfigureAwait(false);
            }
        }

        return telemetryData;
    }

    /// <summary>
    /// Loads the telemetry values of the fastest completed lap of a participant
    /// </summary>
    /// <param name="participantId">Database id of the participant</param>
    /// <returns>Telemetry values of the fastest lap of the participant</returns>
    public async Task<List<TelemetryViewData>> GetTelemetryOfParticipantFastestLapAsync(long participantId)
    {
        var telemetryData = new List<TelemetryViewData>();

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var lapQuery = dbFactory.GetRepository<LapRepository>()?.GetQuery();

            var laps = lapQuery == null
                           ? null
                           : await lapQuery.Where(l => l.ParticipantId == participantId && l.DbIsCompleted == 1)
                                           .ToListAsync()
                                           .ConfigureAwait(false);

            if (laps?.Count > 0)
            {
                var fastestLap = laps.MinBy(l => l.LapTime);

                var telemetryQuery = dbFactory.GetRepository<CarTelemetryRepository>()?.GetQuery();

                if (fastestLap != null && telemetryQuery != null)
                {
                    telemetryData = await LoadTelemetryOfLapAsync(telemetryQuery, fastestLap.Id).ConfigureAwait(false);
                }
            }
        }

        return telemetryData;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Loads the telemetry values of a lap ordered by the number of their packet
    /// </summary>
    /// <param name="telemetryQuery">Query of the stored telemetry rows</param>
    /// <param name="lapId">Database id of the lap</param>
    /// <returns>Telemetry values of the lap</returns>
    private static async Task<List<TelemetryViewData>> LoadTelemetryOfLapAsync(IQueryable<CarTelemetryEntity> telemetryQuery, long lapId)
    {
        return await telemetryQuery.Where(t => t.LapNumberId == lapId)
                                   .OrderBy(t => t.PacketNumber)
                                   .Select(t => new TelemetryViewData
                                                {
                                                    PacketId = t.PacketNumber,
                                                    Distance = t.LapDistance,
                                                    Speed = t.Speed,
                                                    Brake = t.Brake,
                                                    Throttle = t.Throttle,
                                                    Gear = t.Gear,
                                                    EngineRPM = t.EngineRPM
                                                })
                                   .ToListAsync()
                                   .ConfigureAwait(false);
    }

    #endregion // Private methods
}