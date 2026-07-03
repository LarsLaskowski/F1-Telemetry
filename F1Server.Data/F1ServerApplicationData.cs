using F1Server.Core.Interfaces;

using Microsoft.Extensions.Logging;

namespace F1Server.Data;

/// <summary>
/// Application data
/// </summary>
public sealed class F1ServerApplicationData
{
    #region Properties

    /// <summary>
    /// Statistics
    /// </summary>
    public TelemetryStatistics Statistics { get; } = new TelemetryStatistics();

    /// <summary>
    /// Gets or sets the telemetry writer used to record and send telemetry data
    /// </summary>
    public ITelemetryWriter? TelemetryWriter { get; set; }

    /// <summary>
    /// Gets or sets the application metrics used to monitor and track performance data
    /// </summary>
    public IAppMetrics? AppMetrics { get; set; }

    /// <summary>
    /// Logger factory for creating loggers
    /// </summary>
    public ILoggerFactory? LoggerFactory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether tracing is configured
    /// </summary>
    public bool IsTracingConfigured { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether metrics are configured
    /// </summary>
    public bool IsMetricsConfigured { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether logging is configured
    /// </summary>
    public bool IsLoggingConfigured { get; set; }

    /// <summary>
    /// Gets or sets the logger used to record diagnostic and operational messages
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Is a live recording session running?
    /// </summary>
    public bool IsLiveSession { get; set; }

    /// <summary>
    /// Id of live session
    /// </summary>
    public long LiveSessionId { get; set; }

    /// <summary>
    /// Live session data
    /// </summary>
    public ILiveSessionData LiveSessionData { get; set; }

    /// <summary>
    /// Receiving data?
    /// </summary>
    public bool IsReceiving { get; set; }

    /// <summary>
    /// Active session?
    /// </summary>
    public bool IsActiveSession { get; set; }

    #endregion // Properties
}