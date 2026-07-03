using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Statistics controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    #region Fields

    private readonly F1ServerApplicationData _appData;
    private readonly ILogger<StatisticsController> _logger;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appData">Application data</param>
    /// <param name="logger">Logging interface</param>
    public StatisticsController(F1ServerApplicationData appData, ILogger<StatisticsController> logger)
    {
        _appData = appData;
        _logger = logger;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Get statistics
    /// </summary>
    /// <returns>Statistics</returns>
    [HttpGet]
    public TelemetryStatistics Get()
    {
        var statistics = new TelemetryStatistics();

        using var currentActivity = AppActivity.ApiSource.StartActivity("GetStatistics");

        if (_appData == null)
        {
            _logger?.LogError("Missing application data!");

            currentActivity?.SetStatus(ActivityStatusCode.Error);
        }
        else
        {
            statistics = _appData.Statistics;

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return statistics;
    }

    #endregion // Methods
}