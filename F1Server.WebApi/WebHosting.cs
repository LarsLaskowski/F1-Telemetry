using F1Server.Core.Observability;
using F1Server.Data;
using F1Server.Observability;
using F1Server.WebApi.Cache;
using F1Server.WebApi.Core;
using F1Server.WebApi.Hubs;

using Microsoft.Extensions.Caching.Hybrid;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace F1Server.WebApi;

/// <summary>
/// Class providing web hosting with angular
/// </summary>
public class WebHosting : IDisposable
{
    #region Fields

    private CancellationTokenSource? _cts;
    private WebApplication? _webApplication;

    #endregion // Fields

    #region Properties

    /// <summary>
    /// Web hosting is running?
    /// </summary>
    public bool IsRunning { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Start web hosting
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> containing the configured services</param>
    public void StartWebHosting(IServiceProvider serviceProvider)
    {
        if (IsRunning == false)
        {
            var builder = WebApplication.CreateBuilder();

            var applicationData = serviceProvider.GetRequiredService<F1ServerApplicationData>();

            builder.Services.AddSingleton<F1ServerApplicationData>(applicationData);
            builder.Services.AddSingleton<TimerManager>();

            SetObservability(serviceProvider, builder);

            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.AddHybridCache(options =>
                                            {
                                                options.DefaultEntryOptions = new HybridCacheEntryOptions
                                                                              {
                                                                                  Expiration = TimeSpan.FromMinutes(5),
                                                                                  LocalCacheExpiration = TimeSpan.FromMinutes(5)
                                                                              };
                                            });

            builder.Services.AddCors(options => options.AddPolicy("F1ServerAllowSpecification",
                                                                  build => build.AllowAnyMethod()
                                                                                .AllowAnyHeader()
                                                                                .SetIsOriginAllowed(_ => true)
                                                                                .AllowCredentials()));

            builder.Services.AddControllersWithViews().AddApplicationPart(typeof(WebHosting).Assembly);
            builder.Services.AddEndpointsApiExplorer();

            _webApplication = builder.Build();

            if (_webApplication.Environment.IsDevelopment() == false)
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _webApplication.UseHsts();

                _webApplication.UseHttpsRedirection();
            }
            else
            {
                _webApplication.UseSwagger();

                _webApplication.UseSwaggerUI(options =>
                                             {
                                                 options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                                                 options.RoutePrefix = "swagger";
                                             });

                _webApplication.UseDeveloperExceptionPage();

                _webApplication.Urls.Add("http://+:4812");
            }

            _webApplication.UseStaticFiles();
            _webApplication.UseDefaultFiles();
            _webApplication.UseRouting();
            _webApplication.UseStatusCodePages();

            _webApplication.UseCors("F1ServerAllowSpecification");

            _webApplication.UseResponseCaching();

            _webApplication.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");

            _webApplication.MapHub<LiveSessionHub>("/live");

            _webApplication.MapFallbackToFile("index.html");

            _cts = new CancellationTokenSource();

            _webApplication.StartAsync(_cts.Token);

            IsRunning = true;

            StartupCache(_cts.Token).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Stop web hosting
    /// </summary>
    /// <returns>Status</returns>
    public bool StopWebHosting()
    {
        if (IsRunning)
        {
            _webApplication?.StopAsync().Wait();

            _cts?.Cancel();
            _cts?.Dispose();

            _cts = null;

            IsRunning = false;
        }

        return IsRunning;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Set observability capabilities
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> containing the configured services</param>
    /// <param name="builder">Builder object</param>
    private void SetObservability(IServiceProvider serviceProvider, WebApplicationBuilder builder)
    {
        var isDevelopment = builder.Environment.IsDevelopment();
        var instanceName = isDevelopment ? "Development" : "Production";

        var otl = builder.Services.AddOpenTelemetry()
                                  .ConfigureResource(c => c.AddService("F1-Telemetry",
                                                                       serviceVersion: "1.0",
                                                                       serviceInstanceId: instanceName));

        var observabilityConfiguration = serviceProvider?.GetRequiredService<Observability.ObservabilityConfiguration>();

        if (observabilityConfiguration != null)
        {
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService("F1-Telemetry", serviceVersion: "1.0", serviceInstanceId: instanceName);

            ActivateTracing(observabilityConfiguration, otl, resourceBuilder);
            ActivateLogging(observabilityConfiguration, builder, resourceBuilder);
            ActivateMetrics(observabilityConfiguration, otl, resourceBuilder);
        }
    }

    /// <summary>
    /// Configures and activates OpenTelemetry tracing for the application
    /// </summary>
    /// <param name="observabilityConfiguration">The configuration object containing observability settings</param>
    /// <param name="otl">The <see cref="OpenTelemetry.OpenTelemetryBuilder"/> used to configure OpenTelemetry tracing</param>
    /// <param name="resourceBuilder">The <see cref="ResourceBuilder"/> used to define the resource attributes for tracing</param>
    private void ActivateTracing(ObservabilityConfiguration observabilityConfiguration, OpenTelemetry.OpenTelemetryBuilder? otl, ResourceBuilder resourceBuilder)
    {
        if (string.IsNullOrWhiteSpace(observabilityConfiguration.OpenTelemetryTracesEndpoint) == false)
        {
            otl?.WithTracing(t =>
                             {
                                 t.SetResourceBuilder(resourceBuilder);
                                 t.AddSource("F1-Telemetry-WebAPI");
                                 t.AddSource("F1-Telemetry");
                                 t.SetSampler(new AlwaysOnSampler());
                                 t.AddAspNetCoreInstrumentation(options =>
                                                                {
                                                                    options.Filter = filter =>
                                                                                     {
                                                                                         var framework = false;
                                                                                         var swagger = false;

                                                                                         framework = filter.Request?.Path.Value != null && filter.Request.Path.Value.Contains("_framework", StringComparison.OrdinalIgnoreCase);
                                                                                         swagger = filter.Request?.Path.Value != null && filter.Request.Path.Value.Contains("swagger", StringComparison.OrdinalIgnoreCase);

                                                                                         return framework == false && swagger == false;
                                                                                     };
                                                                });

                                 t.AddHttpClientInstrumentation();

                                 if (observabilityConfiguration.OpenTelemetryTarget == TracingTarget.OpenTelemetry)
                                 {
                                     t.AddOtlpExporter(o =>
                                                       {
                                                           o.Endpoint = new Uri(observabilityConfiguration.OpenTelemetryTracesEndpoint);
                                                           o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                                       });
                                 }

                                 if (observabilityConfiguration.OpenTelemetryTarget is TracingTarget.Console)
                                 {
                                     t.AddConsoleExporter();
                                 }
                             });
        }
    }

    /// <summary>
    /// Configures logging for the application using OpenTelemetry and other logging providers
    /// </summary>
    /// <param name="observabilityConfiguration">The configuration object containing observability settings</param>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application's logging services</param>
    /// <param name="resourceBuilder">The <see cref="ResourceBuilder"/> used to define the resource attributes for logging</param>
    private void ActivateLogging(ObservabilityConfiguration observabilityConfiguration, WebApplicationBuilder builder, ResourceBuilder resourceBuilder)
    {
        if (string.IsNullOrEmpty(observabilityConfiguration.OpenTelemetryLoggingEndpoint) == false)
        {
            builder.Logging.AddOpenTelemetry(options =>
                                             {
                                                 options.SetResourceBuilder(resourceBuilder);

                                                 options.IncludeFormattedMessage = true;
                                                 options.IncludeScopes = true;
                                                 options.ParseStateValues = true;

                                                 options.AddOtlpExporter(options =>
                                                                         {
                                                                             options.Endpoint = new Uri(observabilityConfiguration.OpenTelemetryLoggingEndpoint);
                                                                             options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                                                         });
                                             });

            builder.Logging.SetMinimumLevel(LogLevel.Information);
        }
    }

    /// <summary>
    /// Configures and activates OpenTelemetry metrics for the application
    /// </summary>
    /// <param name="observabilityConfiguration">The configuration object containing observability settings</param>
    /// <param name="otl">An optional <see cref="OpenTelemetry.OpenTelemetryBuilder"/> used to configure OpenTelemetry metrics</param>
    /// <param name="resourceBuilder">The <see cref="ResourceBuilder"/> used to define the resource attributes for metrics</param>
    private void ActivateMetrics(ObservabilityConfiguration observabilityConfiguration, OpenTelemetryBuilder? otl, ResourceBuilder resourceBuilder)
    {
        if (string.IsNullOrEmpty(observabilityConfiguration.OpenTelemetryMetricsEndpoint) == false)
        {
            otl?.WithMetrics(m =>
                             {
                                 m.SetResourceBuilder(resourceBuilder);
                                 m.AddAspNetCoreInstrumentation();
                                 m.AddHttpClientInstrumentation();
                                 m.AddMeter(MetricsName.F1MeterName);
                                 m.AddMeter("Microsoft.AspNetCore.Diagnostics");
                                 m.AddMeter("Microsoft.AspNetCore.Routing");
                                 m.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                                 m.AddOtlpExporter(o =>
                                                   {
                                                       o.Endpoint = new Uri(observabilityConfiguration.OpenTelemetryMetricsEndpoint);
                                                       o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                                                   });
                             });
        }
    }

    #endregion // Private methods

    #region IDisposable

    /// <summary>
    /// Internal dispose method
    /// </summary>
    /// <param name="disposing">Dispose flag</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cts?.Dispose();

            ValueTask? retVal = _webApplication?.DisposeAsync();

            _ = retVal;
        }
    }

    /// <summary>
    /// Initializes the fastest lap per session cache asynchronously
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. If the operation is canceled, the task will be terminated</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task StartupCache(CancellationToken cancellationToken)
    {
        await FastestLapPerSessionCache.InitializeCacheAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    #endregion // IDisposable
}