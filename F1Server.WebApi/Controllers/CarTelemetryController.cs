using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Controller receiving car telemetry data
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CarTelemetryController : ControllerBase
{
    #region Fields

    private readonly ILogger<CarTelemetryController> _logger;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    public CarTelemetryController(ILogger<CarTelemetryController> logger)
    {
        _logger = logger;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Are telemetry data for human driver in this session?
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>User telemetry data available?</returns>
    [Route("HasUserTelemetry/{sessionId}")]
    [HttpGet]
    public async Task<bool> HasUserTelemetryData(long sessionId)
    {
        var hasUserTelemetryData = false;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(hasUserTelemetryData));

        _logger?.CheckingUserTelemetryData(sessionId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var telemetryQuery = dbFactory.GetRepository<CarTelemetryRepository>()?.GetQuery();
            var lapQuery = dbFactory.GetRepository<LapRepository>()?.GetQuery();

            if (telemetryQuery != null && lapQuery != null)
            {
                hasUserTelemetryData = await telemetryQuery.AnyAsync(t => lapQuery.Any(l => l.Id == t.LapNumberId
                                                                                            && l.Participant.SessionId == sessionId
                                                                                            && l.Participant.DbIsHumanControlled == 1))
                                                           .ConfigureAwait(false);
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.UserTelemetryDataChecked(hasUserTelemetryData);

        return hasUserTelemetryData;
    }

    /// <summary>
    /// Reading telemetry data of given lap
    /// </summary>
    /// <param name="lapId">Id of lap</param>
    /// <returns>Telemetry data</returns>
    [Route("TelemetryByLap/{lapId}")]
    [HttpGet]
    public async Task<IActionResult> TelemetryOfLap(long lapId)
    {
        List<TelemetryViewData>? telemetryData = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(TelemetryOfLap));

        _logger?.LoadingTelemetryValuesOfLap(lapId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var telemetryQuery = dbFactory.GetRepository<CarTelemetryRepository>()?.GetQuery();

            if (telemetryQuery != null)
            {
                telemetryData = await telemetryQuery.Where(t => t.LapNumberId == lapId)
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

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.TelemetryValuesLoaded(telemetryData?.Count ?? 0);

        return Ok(telemetryData);
    }

    /// <summary>
    /// Reading telemetry data of fastest lap for given participant and session
    /// </summary>
    /// <param name="participantId">Id of participant</param>
    /// <returns>Telemetry data</returns>
    [Route("TelemetryByParticipantFastestLap/{participantId}")]
    [HttpGet]
    public async Task<IActionResult> TelemetryOfParticipantFastestLap(long participantId)
    {
        var telemetryData = new List<TelemetryViewData>();

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(TelemetryOfParticipantFastestLap));

        _logger?.LoadingTelemetryValuesOfFastestLap(participantId);

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
                    telemetryData = await telemetryQuery.Where(t => t.LapNumberId == fastestLap.Id)
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
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.TelemetryValuesLoaded(telemetryData?.Count ?? 0);

        return Ok(telemetryData);
    }

    #endregion // Controller methods
}