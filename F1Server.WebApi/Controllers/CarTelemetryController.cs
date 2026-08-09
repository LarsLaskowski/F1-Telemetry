using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Service.CarTelemetries;

using Microsoft.AspNetCore.Mvc;

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
    private readonly CarTelemetryService _carTelemetryService;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="carTelemetryService">Car telemetry business logic</param>
    public CarTelemetryController(ILogger<CarTelemetryController> logger, CarTelemetryService carTelemetryService)
    {
        _logger = logger;
        _carTelemetryService = carTelemetryService;
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
        // The activity keeps the name of the local variable it was named after before the business logic moved into
        // the service, so the telemetry name of this action stays unchanged
        using var currentActivity = AppActivity.ApiSource.StartActivity("hasUserTelemetryData");

        _logger?.CheckingUserTelemetryData(sessionId);

        var hasUserTelemetryData = await _carTelemetryService.HasUserTelemetryDataAsync(sessionId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

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
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(TelemetryOfLap));

        _logger?.LoadingTelemetryValuesOfLap(lapId);

        var telemetryData = await _carTelemetryService.GetTelemetryOfLapAsync(lapId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

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
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(TelemetryOfParticipantFastestLap));

        _logger?.LoadingTelemetryValuesOfFastestLap(participantId);

        var telemetryData = await _carTelemetryService.GetTelemetryOfParticipantFastestLapAsync(participantId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.TelemetryValuesLoaded(telemetryData.Count);

        return Ok(telemetryData);
    }

    #endregion // Controller methods
}