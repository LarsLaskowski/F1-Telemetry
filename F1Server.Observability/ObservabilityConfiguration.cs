using F1Server.Core.Interfaces;
using F1Server.Core.Observability;
using F1Server.Data;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace F1Server.Observability;

/// <summary>
/// Represents the configuration settings for writing metrics, traces and logs
/// </summary>
public sealed class ObservabilityConfiguration : IDisposable
{
    #region Const fields

    private const string ServiceName = "F1-Telemetry";
    private const string ServiceVersion = "1.0";

    #endregion // Const fields

    #region Fields

    private TracerProvider? _traceProvider;
    private MeterProvider? _meterProvider;
    private string _instanceName;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// OpenTelemetry endpoint for traces
    /// </summary>
    public string? OpenTelemetryTracesEndpoint { get; set; }

    /// <summary>
    /// OpenTelemetry endpoint for logging
    /// </summary>
    public string? OpenTelemetryLoggingEndpoint { get; set; }

    /// <summary>
    /// OpenTelemetry endpoint for metrics
    /// </summary>
    public string? OpenTelemetryMetricsEndpoint { get; set; }

    /// <summary>
    /// Target framework receiving traces and metrics
    /// </summary>
    public TracingTarget OpenTelemetryTarget { get; set; }

    /// <summary>
    /// Gets the application metrics used to monitor and analyze the performance and behavior of the application
    /// </summary>
    public IAppMetrics AppMetrics { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Enable OpenTelemetry
    /// </summary>
    /// <param name="tracesEndpoint">Target endpoint for traces</param>
    /// <param name="metricsEndpoint">Target endpoint for metrics</param>
    /// <param name="loggingEndpoint">Target endpoint for logging</param>
    /// <param name="telemetryTarget">Target</param>
    public void ConfigureObservability(string? tracesEndpoint, string? metricsEndpoint, string? loggingEndpoint, TracingTarget telemetryTarget)
    {
        OpenTelemetryTracesEndpoint = tracesEndpoint;
        OpenTelemetryMetricsEndpoint = metricsEndpoint;
        OpenTelemetryLoggingEndpoint = loggingEndpoint;
        OpenTelemetryTarget = telemetryTarget;

        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) ?? false;

        _instanceName = isDevelopment ? "Development" : "Production";
    }

    /// <summary>
    /// Configures tracing for the application using OpenTelemetry
    /// </summary>
    /// <returns>Status</returns>
    public bool ConfigureTracing()
    {
        var isConfigured = false;

        if (string.IsNullOrEmpty(OpenTelemetryTracesEndpoint) == false)
        {
            // Replace the problematic line with the correct method call
            var otlpProvider = Sdk.CreateTracerProviderBuilder()
                                  .AddSource(ServiceName)
                                  .AddSource("F1-Telemetry-WebAPI")
                                  .ConfigureResource(r => r.AddService(ServiceName, serviceVersion: ServiceVersion, serviceInstanceId: _instanceName))
                                  .AddEntityFrameworkCoreInstrumentation();

            if (OpenTelemetryTarget is TracingTarget.OpenTelemetry)
            {
                otlpProvider.AddOtlpExporter(o =>
                                             {
                                                 o.Endpoint = new Uri(OpenTelemetryTracesEndpoint);
                                                 o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                             });
            }
            else
            {
                otlpProvider.AddConsoleExporter();
            }

            _traceProvider = otlpProvider.Build();

            isConfigured = true;
        }

        return isConfigured;
    }

    /// <summary>
    /// Activates and configures metrics for the F1-Telemetry server, enabling Prometheus-based monitoring
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> containing the configured services</param>
    /// <returns>Status</returns>
    public bool ConfigureMetrics(IServiceProvider serviceProvider)
    {
        var isConfigured = false;

        var applicationData = serviceProvider?.GetRequiredService<F1ServerApplicationData>();

        if (string.IsNullOrEmpty(OpenTelemetryMetricsEndpoint) == false)
        {
            var otlpProvider = Sdk.CreateMeterProviderBuilder()
                                  .ConfigureResource(r => r.AddService(ServiceName, serviceVersion: ServiceVersion, serviceInstanceId: _instanceName))
                                  .AddMeter(MetricsName.F1MeterName)
                                  .AddRuntimeInstrumentation()
                                  .AddProcessInstrumentation()
                                  .AddEventCountersInstrumentation(o =>
                                                                   {
                                                                       o.AddEventSources("Microsoft.Data.SqlClient.EventSource");
                                                                       o.AddEventSources("Microsoft.EntityFrameworkCore");
                                                                   })
                                  .AddOtlpExporter(o =>
                                                   {
                                                       o.Endpoint = new Uri(OpenTelemetryMetricsEndpoint);
                                                       o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                                   });

            _meterProvider = otlpProvider.Build();

            AppMetrics = new AppMetrics();

            applicationData?.AppMetrics = AppMetrics;

            isConfigured = true;
        }

        return isConfigured;
    }

    /// <summary>
    /// Configures OpenTelemetry logging for the application
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> containing the configured services</param>
    /// <returns>Status</returns>
    public bool ConfigureLogging(IServiceProvider serviceProvider)
    {
        var isConfigured = false;

        var applicationData = serviceProvider?.GetRequiredService<F1ServerApplicationData>();

        if (string.IsNullOrEmpty(OpenTelemetryLoggingEndpoint) == false
            && applicationData != null)
        {
            applicationData.LoggerFactory = LoggerFactory.Create(builder =>
                                                                 {
                                                                     builder.AddOpenTelemetry(options =>
                                                                                              {
                                                                                                  options.SetResourceBuilder(ResourceBuilder.CreateEmpty()
                                                                                                                                            .AddService(ServiceName, serviceVersion: ServiceVersion, serviceInstanceId: _instanceName));

                                                                                                  options.IncludeFormattedMessage = true;
                                                                                                  options.IncludeScopes = true;
                                                                                                  options.ParseStateValues = true;

                                                                                                  options.AddOtlpExporter(otlpOptions =>
                                                                                                                          {
                                                                                                                              otlpOptions.Endpoint = new Uri(OpenTelemetryLoggingEndpoint);
                                                                                                                              otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                                                                                                          });
                                                                                              });

                                                                     builder.AddConsole();
                                                                 });

            applicationData.Logger = applicationData.LoggerFactory.CreateLogger<ITelemetryClient>();

            applicationData.Logger.LogInformation("OpenTelemetry logging configured successfully.");

            isConfigured = true;
        }

        return isConfigured;
    }

    #endregion // Methods

    #region IDisposable

    /// <summary>
    /// Releases the resources used by the current instance of the class
    /// </summary>
    public void Dispose()
    {
        _traceProvider?.ForceFlush();
        _traceProvider?.Shutdown();
        _traceProvider?.Dispose();
        _traceProvider = null;

        _meterProvider?.ForceFlush();
        _meterProvider?.Shutdown();
        _meterProvider?.Dispose();
        _meterProvider = null;
    }

    #endregion // IDisposable
}