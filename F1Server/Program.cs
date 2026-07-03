using System.Net;
using System.Net.Sockets;

using F1Server.Core;
using F1Server.Core.EventArgs;
using F1Server.Data;
using F1Server.Observability;
using F1Server.Service;
using F1Server.Telemetry;

using Microsoft.Extensions.DependencyInjection;

namespace F1Server;

/// <summary>
/// Klasse für den Start des Projektes
/// </summary>
public static class Program
{
    #region Fields

    private static int _currentGameVersion = 0;
    private static ulong _currentSessionId = 0;
    private static bool _useLogging = false;

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Haupteintrittspunkt
    /// </summary>
    /// <param name="args">Argumente</param>
    public static void Main(string[] args)
    {
        var runInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        var hasDatabase = false;
        var activateWeb = false;

        // Replace Assembly.GetExecutingAssembly() with typeof(Program).Assembly
        Console.WriteLine($"F1 telemetry service starting (version: {typeof(Program).Assembly.GetName().Version})...");
        Console.WriteLine($"System has {Environment.ProcessorCount} processors/cores");

        // Output database environments
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("F1SERVER_DB_HOST")) == false)
        {
            Console.WriteLine("Database information found:");
            Console.WriteLine($"   Database host: {Environment.GetEnvironmentVariable("F1SERVER_DB_HOST")}");

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("F1SERVER_DB_NAME")) == false)
            {
                Console.WriteLine($"   Database name: {Environment.GetEnvironmentVariable("F1SERVER_DB_NAME")}");

                hasDatabase = true;
            }
            else
            {
                Console.WriteLine("   Database name not exists!");
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("F1SERVER_DB_USER")) == false)
            {
                Console.WriteLine($"   Database user: {Environment.GetEnvironmentVariable("F1SERVER_DB_USER")}");
            }
            else
            {
                Console.WriteLine("   Database user not exists!");
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("F1SERVER_DB_PASSWORD")) == false)
            {
                Console.WriteLine("   Database user password is existing");
            }
            else
            {
                Console.WriteLine("   Database user password not exists!");
            }

            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("F1SERVER_WEB")) == false
                && Environment.GetEnvironmentVariable("F1SERVER_WEB")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true)
            {
                Console.WriteLine("   Activating web interface");

                activateWeb = true;
            }
            else
            {
                Console.WriteLine("   No web interface");
            }
        }
        else
        {
            Console.WriteLine("   Database information not exists!");
        }

        var serviceProvider = StartupServiceProvider();

        CheckObservability(serviceProvider);
        CheckTelemetry(serviceProvider);

        var telemetryClient = new TelemetryClient(serviceProvider, hasDatabase, activateWeb);

        telemetryClient.PacketReceived += OnPacketReceived;
        telemetryClient.ConnectionStatusChanged += OnConnectionStatusChanged;
        telemetryClient.ProcessingError += OnProcessingError;
        telemetryClient.StatisticsOutput += OnStatisticsOutput;

        telemetryClient.Startup();

        StartProgram(args, runInDocker, telemetryClient);

        telemetryClient.PacketReceived -= OnPacketReceived;
        telemetryClient.StatisticsOutput -= OnStatisticsOutput;

        Console.WriteLine("Exit!");
    }

    /// <summary>
    /// Initializes and configures a new <see cref="ServiceProvider"/> instance with the required services
    /// </summary>
    /// <returns>A <see cref="ServiceProvider"/> containing the configured services</returns>
    private static ServiceProvider StartupServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new F1ServerApplicationData());
        services.AddSingleton(new ObservabilityConfiguration());
        services.AddSingleton(new TelemetryConfiguration());
        services.AddSingleton(new PacketAnalyzer());
        services.AddOpenTelemetry();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Check wether telemetry is enabled
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> containing the configured services</param>
    private static void CheckObservability(IServiceProvider serviceProvider)
    {
        var otlpTracesEndpoint = Environment.GetEnvironmentVariable("F1SERVER_OTLP_TRACES_URL");
        var otlpMetricsEndpoint = Environment.GetEnvironmentVariable("F1SERVER_OTLP_METRICS_URL");
        var otlpLoggingEndpoint = Environment.GetEnvironmentVariable("F1SERVER_OTLP_LOGGING_URL");

        // If no set, check the old environment variable
        if (string.IsNullOrEmpty(otlpTracesEndpoint))
        {
            otlpTracesEndpoint = Environment.GetEnvironmentVariable("F1SERVER_OTLP_URL");
        }

        var observabilityConfig = serviceProvider.GetRequiredService<Observability.ObservabilityConfiguration>();
        var applicationData = serviceProvider.GetRequiredService<F1ServerApplicationData>();

        var isSucceeded = int.TryParse(Environment.GetEnvironmentVariable("F1SERVER_OTLP_TARGET"), out var telemetryTarget)
                          && (string.IsNullOrEmpty(otlpTracesEndpoint) == false
                              || string.IsNullOrEmpty(otlpMetricsEndpoint) == false
                              || string.IsNullOrEmpty(otlpLoggingEndpoint) == false);

        if (isSucceeded
            && applicationData != null)
        {
            Console.WriteLine("Observability enabled:");
            Console.WriteLine("   Target          : " + (TracingTarget)telemetryTarget);
            Console.WriteLine("   Traces endpoint : " + otlpTracesEndpoint);
            Console.WriteLine("   Metrics endpoint: " + otlpMetricsEndpoint);
            Console.WriteLine("   Logging endpoint: " + otlpLoggingEndpoint);

            observabilityConfig.ConfigureObservability(otlpTracesEndpoint, otlpMetricsEndpoint, otlpLoggingEndpoint, (TracingTarget)telemetryTarget);

            Console.Write("Configuring tracing...");

            applicationData.IsTracingConfigured = observabilityConfig.ConfigureTracing();

            Console.WriteLine(applicationData.IsTracingConfigured ? "successfully!" : "failed!");

            Console.Write("Configuring metrics...");

            applicationData.IsMetricsConfigured = observabilityConfig.ConfigureMetrics(serviceProvider);

            Console.WriteLine(applicationData.IsMetricsConfigured ? "successfully!" : "failed!");

            Console.Write("Configuring logging...");

            applicationData.IsLoggingConfigured = observabilityConfig.ConfigureLogging(serviceProvider);

            Console.WriteLine(applicationData.IsLoggingConfigured ? "successfully!" : "failed!");
        }
        else
        {
            Console.WriteLine("Observability not available!");
        }
    }

    /// <summary>
    /// Configures telemetry settings for the application based on environment variables
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance used to resolve the <see cref="TelemetryConfiguration"/> service</param>
    private static void CheckTelemetry(IServiceProvider serviceProvider)
    {
        var telemetryConfig = serviceProvider.GetRequiredService<TelemetryConfiguration>();

        var telemetryHost = Environment.GetEnvironmentVariable("F1SERVER_TELEMETRY_HOST");
        var telemetryBucket = Environment.GetEnvironmentVariable("F1SERVER_TELEMETRY_BUCKET");
        var telemetryOrg = Environment.GetEnvironmentVariable("F1SERVER_TELEMETRY_ORGANIZATION");
        var telemetryToken = Environment.GetEnvironmentVariable("F1SERVER_TELEMETRY_TOKEN");

        if (string.IsNullOrEmpty(telemetryHost) == false
            && string.IsNullOrEmpty(telemetryBucket) == false
            && string.IsNullOrEmpty(telemetryOrg) == false
            && string.IsNullOrEmpty(telemetryToken) == false)
        {
            Console.Write("Configuring live telemetry...");

            Console.WriteLine(telemetryConfig.SetConfigurationData(telemetryHost, telemetryBucket, telemetryOrg, telemetryToken) ? "successfully!" : "failed!");

            Console.WriteLine($"   Host        : {telemetryHost}");
            Console.WriteLine($"   Bucket      : {telemetryBucket}");
            Console.WriteLine($"   Organization: {telemetryOrg}");
            Console.WriteLine("   Token:      : available");
        }
        else
        {
            Console.WriteLine("Live telemetry not available!");
        }
    }

    /// <summary>
    /// Start application or client or test mode
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <param name="runInDocker">Run in docker?</param>
    /// <param name="telemetryClient">Telemetry client</param>
    private static void StartProgram(string[] args, bool runInDocker, TelemetryClient telemetryClient)
    {
        var hasError = false;

        if (args.Length == 0)
        {
            hasError = StartApplication(telemetryClient, runInDocker);
        }
        else if (args.Length == 1)
        {
            StartClient(args[0]);
        }
        else if (args.Length > 1)
        {
            // Test mode
            StartTest(telemetryClient, args[0], args[1]);
        }
        else
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  Without parameters: running the client, wait receiving data packets from F1 2019 or 202x");
            Console.WriteLine("  Possible parameters:");
            Console.WriteLine("      --test [path]");
            Console.WriteLine("        Read files from folder");
        }

        if (runInDocker)
        {
            if (hasError == false)
            {
                var task = Task.Run(() => Thread.Sleep(Timeout.Infinite));

                task.Wait();
            }
        }
        else
        {
            Console.WriteLine("Press any key to exit!");

            Console.ReadKey();
        }
    }

    /// <summary>
    /// Start the application in container or normal mode
    /// </summary>
    /// <param name="telemetryClient">Telemetry client</param>
    /// <param name="runInDocker">Container mode?</param>
    /// <returns>Error?</returns>
    private static bool StartApplication(TelemetryClient telemetryClient, bool runInDocker)
    {
        var hasError = false;

        if (runInDocker)
        {
            Console.WriteLine("Runs in docker...");

            if (Environment.GetEnvironmentVariable("F1SERVER_RUN_TELEMETRY_LOGGING") == "true")
            {
                Console.WriteLine("Activating telemetry logging");

                telemetryClient.UsePacketLogging = true;

                _useLogging = true;

                if (Directory.Exists("/var/f1-telemetry") == true)
                {
                    telemetryClient.PacketLoggingPath = "/var/f1-telemetry/#Version/#Session/#PacketType";
                    telemetryClient.PacketLoggingProtocolName = "/var/f1-telemetry/#Version/#Session/received-packets.log";

                    Console.WriteLine($"Set logging path to {telemetryClient.PacketLoggingPath}");
                }
            }
            else
            {
                Console.WriteLine("Logging is not enabled!");
            }
        }
        else
        {
            Console.WriteLine("Runs on local machine...");

            telemetryClient.UsePacketLogging = Environment.GetEnvironmentVariable("F1SERVER_RUN_TELEMETRY_LOGGING") == "true";
            telemetryClient.PacketLoggingPath = @"---path to directory---\#Version\#Session\#PacketType";
        }

        var result = telemetryClient.StartReceiving();

        if (result == true)
        {
            Console.WriteLine("Start receiving...");
        }
        else
        {
            Console.WriteLine("Error start receiving");
            Console.WriteLine(telemetryClient.LastError);

            hasError = true;
        }

        return hasError;
    }

    /// <summary>
    /// Start client
    /// </summary>
    /// <param name="parameter">Commandline parameter</param>
    private static void StartClient(string parameter)
    {
#if DEBUG
        if (parameter == "--client")
        {
            var client = new UdpClient();
            var ep = new IPEndPoint(IPAddress.Parse("192.168.0.100"), 20777);

            client.Connect(ep);

            client.Send(new byte[] { 0xe5, 0x07, 0x01, 0x0d, 0x01, 0x03, 0xc7, 0xfe, 0x15, 0xa0, 0x3f, 0x11, 0x5c, 0xb4, 0xed, 0xb4, 0xad, 0x42, 0x4a, 0x07, 0x00, 0x00, 0x00, 0xff, 0x42, 0x55, 0x54, 0x4e, 0x80, 0x00, 0x00, 0x00, 0x40, 0x02, 0x00, 0x00 });

            client.Dispose();
        }
#endif // DEBUG
    }

    /// <summary>
    /// Start test
    /// </summary>
    /// <param name="telemetryClient">Telemetry client</param>
    /// <param name="parameter">Commandline parameter</param>
    /// <param name="sourceDirectory">Source directory</param>
    private static void StartTest(TelemetryClient telemetryClient, string parameter, string sourceDirectory)
    {
        if (parameter.Equals("--test", StringComparison.OrdinalIgnoreCase) && Directory.Exists(sourceDirectory))
        {
            var subDirectories = Directory.EnumerateDirectories(sourceDirectory).ToList();

            if (subDirectories.Count == 0)
            {
                subDirectories = new List<string>
                                 {
                                     sourceDirectory
                                 };
            }

            foreach (var subDirectory in subDirectories)
            {
                AnalyzeFiles(telemetryClient, subDirectory);
            }
        }
    }

    /// <summary>
    /// Analyze files in given directory
    /// </summary>
    /// <param name="telemetryClient">Telemetry client</param>
    /// <param name="directory">Directory</param>
    private static void AnalyzeFiles(TelemetryClient telemetryClient, string directory)
    {
        var files = Directory.GetFiles(directory, "packet-*", SearchOption.AllDirectories)
                             .Select(obj => new
                                            {
                                                fileName = obj,
                                                fInfo = new FileInfo(obj)
                                            })
                             .OrderBy(f => f.fInfo.Name)
                             .ToList();

        if (files.Count > 0)
        {
            foreach (var file in files)
            {
                try
                {
                    Console.WriteLine($"Analyzing file: {file.fInfo.FullName}");

                    var content = File.ReadAllBytes(file.fileName);

                    telemetryClient.ReceiveDataFromFile(content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Exception] Error analyzing file {file.fInfo.Name} => {ex}");
                }
            }
        }
        else
        {
            Console.WriteLine("No files found!");
        }
    }

    /// <summary>
    /// New packet received
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="packetReceivedEventArgs">Event argument</param>
    private static void OnPacketReceived(object? sender, PacketReceivedEventArgs packetReceivedEventArgs)
    {
        if (packetReceivedEventArgs.GameVersion != _currentGameVersion)
        {
            _currentGameVersion = packetReceivedEventArgs.GameVersion;

            Console.WriteLine($"F1 {packetReceivedEventArgs.GameVersion} ({packetReceivedEventArgs.ProductVersion}) started...");
        }

        if (packetReceivedEventArgs.SessionId != _currentSessionId)
        {
            _currentSessionId = packetReceivedEventArgs.SessionId;

            Console.WriteLine($"Session switched to: {_currentSessionId}");
        }

#if DEBUG
        Console.WriteLine($"Packet: {packetReceivedEventArgs.PacketType} - Session timestamp: {packetReceivedEventArgs.Timestamp}");
#endif // DEBUG
    }

    /// <summary>
    /// Output statistics
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="statistics">Statistic data</param>
    private static void OnStatisticsOutput(object? sender, TelemetryStatistics statistics)
    {
        Console.WriteLine("-------------------------- Statistics --------------------------");
        Console.WriteLine($"Total packets                      : {statistics.PacketsReceivedTotal}");
        Console.WriteLine($"Packets received in current session: {statistics.PacketsReceivedCurrentSession}");
        Console.WriteLine($"Packets received in last session   : {statistics.PacketsReceivedLastSession}");
        Console.WriteLine($"Average packet processing time     : {statistics.TotalPacketProcessingTime / statistics.TotalPacketsProcessed:D0} ms");
        Console.WriteLine($"Packages queued for processing     : {statistics.PacketsInQueue}");
        Console.WriteLine($"Packages queued in packet processor: {statistics.PacketsInProcessorQueue}");

        if (_useLogging)
        {
            Console.WriteLine($"Average packet log time            : {statistics.TotalPacketLogTime / statistics.TotalPacketsLogged:D0} ms");
        }
    }

    /// <summary>
    /// Connection status changed
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="connectionStatus">Event argument</param>
    private static void OnConnectionStatusChanged(object? sender, bool connectionStatus)
    {
        if (connectionStatus == false)
        {
            Console.Clear();
            Console.WriteLine("Listening for telemetry data...");
        }
    }

    /// <summary>
    /// Error while processing a received packet
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="processingError">Error text</param>
    private static void OnProcessingError(object? sender, string? processingError)
    {
        if (string.IsNullOrWhiteSpace(processingError) == false)
        {
            Console.WriteLine($"Processing error occurred: {processingError}");
        }
    }

    #endregion // Methods
}