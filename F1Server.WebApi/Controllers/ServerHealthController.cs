using System.Diagnostics;

using F1Server.Data;
using F1Server.Db.Entity;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Health controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ServerHealthController : ControllerBase
{
    #region Fields

    private readonly F1ServerApplicationData _appData;
    private readonly ILogger<HealthController> _logger;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appData">Application data</param>
    /// <param name="logger">Logging interface</param>
    public ServerHealthController(F1ServerApplicationData appData, ILogger<HealthController> logger)
    {
        _logger = logger;
        _appData = appData;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Get action
    /// </summary>
    /// <returns>Healthy state</returns>
    [HttpGet]
    public IActionResult Get()
    {
        // Check database
        var isHealthy = CheckDatabase();

        // Check application data
        if (isHealthy)
        {
            isHealthy = CheckServerFeatureAvailability();
        }

        return isHealthy ? Ok() : BadRequest();
    }

    #endregion // Controller methods

    #region Methods

    /// <summary>
    /// Checks whether the database is initialized and ready for use
    /// </summary>
    /// <returns><see langword="true"/> if the database is initialized; otherwise, <see langword="false"/></returns>
    private bool CheckDatabase()
    {
        var isOk = false;

        if (RepositoryFactory.IsInitialized)
        {
            isOk = true;
        }
        else
        {
            _logger?.LogError("Database is not initialized!");
        }

        return isOk;
    }

    /// <summary>
    /// Checks whether the application data is receiving input and meets the required conditions for telemetry, metrics, and tracing
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the application data is receiving input and meets the required conditions;  otherwise,
    /// <see langword="false"/>
    /// </returns>
    private bool CheckServerFeatureAvailability()
    {
        var isTelemetryWriterReady = false;
        var isMetricsInitialized = false;
        var isReceiving = false;
        var isTracingEnabled = false;
        var isLoggingEnabled = false;

        if (_appData?.IsReceiving == true)
        {
            isReceiving = true;

            isTelemetryWriterReady = _appData.TelemetryWriter?.IsReady == true;

            if (isTelemetryWriterReady == false)
            {
                _logger?.LogWarning("Telemetry writer is not ready to receive data!");
            }

            isMetricsInitialized = CheckMetrics();
            isTracingEnabled = CheckTracing();
            isLoggingEnabled = CheckLogging();
        }
        else
        {
            _logger?.LogError("Server is not receiving data!");
        }

        return isReceiving && isTelemetryWriterReady && isMetricsInitialized && isTracingEnabled && isLoggingEnabled;
    }

    /// <summary>
    /// Determines whether application metrics are enabled and properly initialized
    /// </summary>
    /// <returns><see langword="true"/> if metrics are enabled and initialized; otherwise, <see langword="false"/></returns>
    private bool CheckMetrics()
    {
        var isMetricsEnabled = false;

        if (_appData != null)
        {
            if (_appData.IsMetricsConfigured)
            {
                isMetricsEnabled = _appData.AppMetrics != null;

                if (isMetricsEnabled == false)
                {
                    _logger?.LogWarning("Application metrics are not initialized!");
                }
            }
            else
            {
                // If metrics are not configured, we assume they are initialized for the sake of this check
                isMetricsEnabled = true;
            }
        }
        else
        {
            _logger?.LogError("Application data is not available for metrics check!");
        }

        return isMetricsEnabled;
    }

    /// <summary>
    /// Checks whether tracing is enabled in the application data
    /// </summary>
    /// <returns><see langword="true"/> if tracing is enabled; otherwise, <see langword="false"/></returns>
    private bool CheckTracing()
    {
        var isTracingEnabled = false;

        if (_appData != null)
        {
            if (_appData.IsTracingConfigured)
            {
                var activitySource = new ActivitySource("F1-Telemetry");

                isTracingEnabled = activitySource.HasListeners();

                if (isTracingEnabled == false)
                {
                    _logger?.LogWarning("No listeners are attached to the activity source.");
                }
            }
            else
            {
                // If tracing is not configured, we assume it is enabled for the sake of this check
                isTracingEnabled = true;
            }
        }

        return isTracingEnabled;
    }

    /// <summary>
    /// Determines whether logging is configured and enabled for the application
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if logging is configured and the logger factory is available;  otherwise, <see
    /// langword="false"/>
    /// </returns>
    private bool CheckLogging()
    {
        var isLoggingConfigured = false;

        if (_appData != null)
        {
            if (_appData.IsLoggingConfigured)
            {
                isLoggingConfigured = _appData.LoggerFactory != null;

                if (isLoggingConfigured == false)
                {
                    _logger.LogWarning("No logger factory is configured for logging.");
                }
            }
            else
            {
                // If logging is not configured, we assume it is enabled for the sake of this check
                isLoggingConfigured = true;
            }
        }

        return isLoggingConfigured;
    }

    #endregion // Methods
}