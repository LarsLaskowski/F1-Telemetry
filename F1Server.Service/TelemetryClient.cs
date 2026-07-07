using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.EventArgs;
using F1Server.Core.Interfaces;
using F1Server.Core.Observability;
using F1Server.Core.Packets.Data;
using F1Server.Data;
using F1Server.Data.Events;
using F1Server.Db.Entity;
using F1Server.Service.Cache;
using F1Server.Service.Runtime;
using F1Server.Telemetry;
using F1Server.WebApi;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace F1Server.Service;

/// <summary>
/// Base client to receive packets from F1 20xx game
/// </summary>
public sealed class TelemetryClient : ITelemetryClient, IDisposable
{
    #region Fields

    private readonly IServiceProvider _serviceProvider;
    private readonly bool _useDatabase;
    private readonly int _port = 20777;
    private readonly ConcurrentQueue<ReceivedPacketData> _packetQueue;
    private readonly ConcurrentQueue<ReceivedPacketData> _logPacketQueue;
    private readonly System.Timers.Timer _timeoutTimer;
    private readonly System.Timers.Timer _statisticsTimer;
    private readonly F1ServerApplicationData _applicationData;
    private readonly PacketProcessor _packetProcessor;
    private readonly WebHosting _webHosting;
    private UdpClient? _udpClient;
    private TcpListener? _tcpServer;
    private IPEndPoint? _ipEndpoint;
    private CancellationTokenSource _cts;
    private CancellationTokenSource _ctsLog;
    private bool _isQueueWorkerRunning;
    private bool _isLoggingQueueRunning;
    private bool _statisticsTimerRunning;
    private long _queuedPackets;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> containing the configured services</param>
    /// <param name="useDatabase">Database information seems to exists</param>
    /// <param name="useWebHosting">Activating web interface</param>
    /// <param name="port">Optionaler Port</param>
    public TelemetryClient(IServiceProvider serviceProvider, bool useDatabase, bool useWebHosting, int port = 0)
    {
        using var currentActity = AppActivity.SrvSource.StartActivity("TelemetryClient-Create");

        if (port != 0)
        {
            _port = port;
        }

        _serviceProvider = serviceProvider;
        _useDatabase = useDatabase;

        _applicationData = serviceProvider.GetRequiredService<F1ServerApplicationData>();

        _applicationData.TelemetryWriter = new TelemetryWriter(serviceProvider);

        _applicationData.Statistics.SessionChanged += OnStatisticsSessionChanged;

        Logger = _applicationData.Logger;

        _packetProcessor = new PacketProcessor(serviceProvider, useDatabase);

        _packetProcessor.PacketReceived += OnPacketReceived;

        _packetQueue = new ConcurrentQueue<ReceivedPacketData>();
        _logPacketQueue = new ConcurrentQueue<ReceivedPacketData>();

        _timeoutTimer = new System.Timers.Timer(ConstData.TimeoutInMs)
                        {
                            AutoReset = true
                        };

        _timeoutTimer.Elapsed += OnTimerElapsed;

        _statisticsTimer = new System.Timers.Timer(ConstData.StatisticTimeoutInMs)
                           {
                               AutoReset = true
                           };

        _statisticsTimer.Elapsed += OnStatisticTimerElapsed;

        if (useWebHosting)
        {
            _webHosting = new WebHosting();
        }

        currentActity?.SetStatus(ActivityStatusCode.Ok, "TelemetryClient created");
    }

    #endregion // Constructors

    #region Events

    /// <summary>
    /// Event, if a packet received
    /// </summary>
    public event EventHandler<PacketReceivedEventArgs> PacketReceived;

    /// <summary>
    /// Event, if connection changed
    /// </summary>
    public event EventHandler<bool> ConnectionStatusChanged;

    /// <summary>
    /// Event, if something went wrong while processing packet
    /// </summary>
    public event EventHandler<string?> ProcessingError;

    /// <summary>
    /// Event for current statistics
    /// </summary>
    public event EventHandler<TelemetryStatistics> StatisticsOutput;

    #endregion // Events

    #region Properties

    /// <summary>
    /// Gets the logger instance used for logging messages and events
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Flag activating packet logging
    /// </summary>
    public bool UsePacketLogging { get; set; }

    /// <summary>
    /// Path for packet logging
    /// </summary>
    public string PacketLoggingPath { get; set; }

    /// <summary>
    /// Protocol file name for packet logging
    /// </summary>
    public string PacketLoggingProtocolName { get; set; }

    /// <summary>
    /// Is connected?
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    /// Last error
    /// </summary>
    public string LastError { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Startup method to initialize the client
    /// </summary>
    public void Startup()
    {
        using var currentActity = AppActivity.SrvSource.StartActivity("TelemetryClient-Startup");

        if (_useDatabase)
        {
            try
            {
                using (var dbFactory = RepositoryFactory.InitDatabase(_serviceProvider))
                {
                    dbFactory.InitDatabase();
                }
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();

                currentActity?.AddException(ex);

                Logger?.LogError(ex, "Error initializing database connection!");
            }

            if (string.IsNullOrEmpty(LastError))
            {
                try
                {
                    DriverRepositoryCache.LoadDrivers();
                    TeamRepositoryCache.LoadTeams();
                    TrackRepositoryCache.LoadTracks();
                    NationalityRepositoryCache.LoadNationalities();
                }
                catch (Exception ex)
                {
                    LastError = ex.ToString();

                    currentActity?.AddException(ex);

                    Logger?.LogError(ex, "Error loading cache data from database!");
                }
            }
        }

        currentActity?.SetStatus(string.IsNullOrEmpty(LastError) ? ActivityStatusCode.Ok : ActivityStatusCode.Error, "TelemetryClient started");
    }

    /// <summary>
    /// Start receiving packets
    /// </summary>
    /// <returns>Status</returns>
    public bool StartReceiving()
    {
        var retValue = true;

        var currentActivity = AppActivity.SrvSource.StartActivity("StartReceiving");

        try
        {
            _cts = new CancellationTokenSource();
            _ctsLog = new CancellationTokenSource();
            _udpClient = new UdpClient(_port);
            _ipEndpoint = new IPEndPoint(IPAddress.Any, _port);
            _tcpServer = new TcpListener(IPAddress.Any, _port + 1);

            currentActivity?.AddTag("f1.receiving_port", _port);
            currentActivity?.AddTag("f1.tcp_receiving_port", _port + 1);

            _tcpServer.Start();

            CheckPacketProcessingTaskIsRunning();

            if (_webHosting?.IsRunning == false)
            {
                _webHosting.StartWebHosting(_serviceProvider);
            }

            // Start receiving first data
            _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);

            using (ExecutionContext.SuppressFlow())
            {
                Task.Run(() => ReceiveReplayPackets(_cts.Token), _cts.Token);
            }

            _applicationData.IsReceiving = true;

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            LastError = ex.ToString();

            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());

            Logger?.LogError(ex, "Error starting receiving packets!");

            retValue = false;
        }

        currentActivity?.Stop();
        currentActivity?.Dispose();

        return retValue;
    }

    /// <summary>
    /// Receive data from a file
    /// </summary>
    /// <param name="recvData">File content</param>
    /// <returns>Result</returns>
    public bool ReceiveDataFromFile(byte[] recvData)
    {
        var retValue = false;

        // Es darf ansonsten nichts weiter laufen
        if (IsConnected == false
            && UsePacketLogging == false
            && recvData?.Length > 0
            && _udpClient == null)
        {
            var packetData = new ReceivedPacketData();

            packetData.SetRawData(recvData);

            if (_packetProcessor.ProcessPacket(packetData) == false)
            {
                ProcessingError?.Invoke(this, _packetProcessor.LastError);
            }
        }

        return retValue;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Callback method for every single packet
    /// </summary>
    /// <param name="result">Async result</param>
    private void ReceiveCallback(IAsyncResult result)
    {
        Activity? currentActivity = null;

        using (ExecutionContext.SuppressFlow())
        {
            currentActivity = AppActivity.SrvSource.StartActivity("PacketReceived", ActivityKind.Server, null);
        }

        if (_udpClient == null || _ipEndpoint == null)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, "UDP client or endpoint is not initialized!");

            currentActivity?.Stop();
            currentActivity?.Dispose();

            return;
        }

        if (IsConnected == false)
        {
            IsConnected = true;

            ConnectionStatusChanged?.Invoke(this, true);
        }

        _applicationData.Statistics.PacketsReceivedTotal++;
        _applicationData.Statistics.PacketsReceivedCurrentSession++;

        _applicationData.AppMetrics?.PacketsReceived.Add(1);

        _timeoutTimer.Stop();
        _timeoutTimer.Start();

        if (_statisticsTimerRunning == false)
        {
            _statisticsTimerRunning = true;

            _statisticsTimer.Start();
        }

        try
        {
            byte[] recvData = _udpClient.EndReceive(result, ref _ipEndpoint);

            currentActivity?.SetTag("ReceivedBytes", recvData.Length);

            Interlocked.Increment(ref _queuedPackets);

            CheckPacketProcessingTaskIsRunning();

            var packetData = new ReceivedPacketData();

            packetData.SetRawData(recvData);

            if (Logger?.IsEnabled(LogLevel.Trace) == true)
            {
                Logger.LogTrace("Received packet with length: {Length}", recvData.Length);
            }

            EnqueueLoggingPacket(ref packetData);

            _applicationData.Statistics.PacketsInQueue = _queuedPackets;

            // Enqueue the received data for processing
            _packetQueue.Enqueue(packetData);

            _applicationData.AppMetrics?.PacketsInQueue.Record(_packetQueue.Count);

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
            currentActivity?.AddException(ex);

            Logger?.LogError(ex, "Error receiving UDP packet!");
        }

        currentActivity?.Stop();
        currentActivity?.Dispose();

        // Start receiving new data
        _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
    }

    /// <summary>
    /// Timeout timer elapsed
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="elapsedEventArgs">Event argument</param>
    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs elapsedEventArgs)
    {
        IsConnected = false;

        ConnectionStatusChanged?.Invoke(this, false);

        // Stop the timer, is started automatically if a connection is created
        _timeoutTimer.Stop();
    }

    /// <summary>
    /// Timeout statistic timer elapsed
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="elapsedEventArgs">Event argument</param>
    private void OnStatisticTimerElapsed(object? sender, System.Timers.ElapsedEventArgs elapsedEventArgs)
    {
        StatisticsOutput?.Invoke(this, _applicationData.Statistics);

        // No connection and no packets? Stop the timer
        if (IsConnected == false && _applicationData.Statistics.PacketsInQueue == 0)
        {
            _statisticsTimer.Stop();

            _statisticsTimerRunning = false;
        }
    }

    /// <summary>
    /// Packet received event method
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="packetReceivedEventArgs">Event argument</param>
    private void OnPacketReceived(object? sender, PacketReceivedEventArgs packetReceivedEventArgs)
    {
        PacketReceived?.Invoke(this, packetReceivedEventArgs);
    }

    /// <summary>
    /// Event, if a session changed
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sessionChangedEventArgs">Eventdata</param>
    private void OnStatisticsSessionChanged(object sender, SessionChangedEventArgs sessionChangedEventArgs)
    {
        if (sessionChangedEventArgs.LastSessionId > 0)
        {
            Task.Run(() => WriteLastSessionStatistics(sessionChangedEventArgs.LastSessionId, sessionChangedEventArgs.LastSessionGameVersion, sessionChangedEventArgs.LastSessionMetrics));
        }
        else
        {
            Task.Run(() => WriteNewSessionStarts(sessionChangedEventArgs.CurrentSessionId, sessionChangedEventArgs.CurrentSessionGameVersion));
        }
    }

    /// <summary>
    /// Process all packets from queue
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private void ProcessCurrentPackets(CancellationToken cancellationToken)
    {
        var runWatch = new Stopwatch();

        ulong lastSessionId = 0;

        while (_packetQueue.TryDequeue(out var packetData))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            Interlocked.Decrement(ref _queuedPackets);

            _applicationData.AppMetrics?.PacketsInQueue.Record(_packetQueue.Count);

            if (Logger?.IsEnabled(LogLevel.Trace) == true)
            {
                Logger.LogTrace("Processing packet with id: {PacketId}, type: {PacketType}, session id: {SessionId}", packetData.PacketNumber, packetData.PacketHeader?.PacketType, packetData.PacketHeader?.UniqueSessionId);
            }

            var currentActivity = AppActivity.SrvSource.StartActivity("ProcessCurrentPacket", ActivityKind.Internal, null);

            currentActivity?.AddTag("f1.packet_id", packetData.PacketNumber);
            currentActivity?.AddTag("f1.session_id", packetData.PacketHeader?.UniqueSessionId);
            currentActivity?.AddTag("f1.packet_type", packetData.PacketHeader?.PacketType);

            currentActivity?.SetStatus(ActivityStatusCode.Ok);

            // Fix session id if it is under circumstances not set
            if (packetData.PacketHeader?.UniqueSessionId == 0)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, "Packet without session id");

                packetData.PacketHeader.UniqueSessionId = lastSessionId;

                Logger?.LogWarning("Packet (type: {PacketType}) without session id, using last session id: {SessionId}", packetData.PacketHeader?.PacketType, lastSessionId);
            }

            lastSessionId = packetData.PacketHeader?.UniqueSessionId ?? 0;

            _applicationData.Statistics.CheckChangeSession(packetData.PacketHeader?.UniqueSessionId, packetData.PacketHeader?.GameVersion);

            _applicationData.Statistics.PacketsInQueue = _queuedPackets;

            runWatch.Restart();

            if (_packetProcessor.ProcessPacket(packetData) == false)
            {
                _applicationData.Statistics.CurrentSessionMetrics.UnsuccessfullyProcessed++;

                if (string.IsNullOrWhiteSpace(_packetProcessor.LastError) == false)
                {
                    ProcessingError?.Invoke(this, _packetProcessor.LastError);

                    _applicationData.Statistics.CurrentSessionMetrics.Errors++;

                    currentActivity?.SetStatus(ActivityStatusCode.Error, "Packet processed with error");
                    currentActivity?.AddTag("f1.packet_process_error", _packetProcessor.LastError);

                    Logger?.LogError("Error processing packet: {Error}", _packetProcessor.LastError);
                }
            }

            runWatch.Stop();

            _applicationData.Statistics.PacketsInProcessorQueue = _packetProcessor.QueuedPackets;

            _applicationData.Statistics.TotalPacketsProcessed++;
            _applicationData.Statistics.TotalPacketProcessingTime += runWatch.Elapsed.TotalMilliseconds;

            _applicationData.AppMetrics?.RecordProcessedPacket(packetData.PacketHeader?.PacketType, runWatch.Elapsed.TotalMilliseconds);

            _applicationData.Statistics.CurrentSessionMetrics.UpdatePacketStatistics(packetData.PacketHeader?.PacketType, runWatch.Elapsed.TotalMilliseconds);

            currentActivity?.Stop();
            currentActivity?.Dispose();
        }
    }

    /// <summary>
    /// Process current log packets
    /// </summary>
    /// <param name="packetAnalyzer">Packet analyzer</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private void ProcessCurrentLoggingPackets(PacketAnalyzer? packetAnalyzer, CancellationToken cancellationToken)
    {
        var runWatch = new Stopwatch();

        while (packetAnalyzer != null && _logPacketQueue.TryDequeue(out var packetData))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            _applicationData.Statistics.TotalPacketsLogged++;

            runWatch.Restart();

            if (packetData.PacketHeader?.UniqueSessionId > 0)
            {
                var pathToLog = PacketLoggingPath;

                pathToLog = ReplaceFilePathTokens(packetData.PacketHeader, pathToLog);

                if (string.IsNullOrWhiteSpace(pathToLog) == false)
                {
                    using (var writeActivity = AppActivity.SrvSource.StartActivity("WriteToFile", ActivityKind.Internal, null))
                    {
                        writeActivity?.AddTag("f1.packet_id", packetData.PacketNumber);
                        writeActivity?.AddTag("f1.session_id", packetData.PacketHeader.UniqueSessionId);
                        writeActivity?.AddTag("f1.packet_type", packetData.PacketHeader.PacketType);

                        try
                        {
                            CheckAndCreateLoggingPath(pathToLog);

                            File.WriteAllBytes($"{pathToLog}/packet-{packetData.PacketHeader.UniqueSessionId}-{packetData.PacketNumber:D8}", packetData.PacketRawData[..packetData.PacketLength].ToArray());

                            WriteLogProtocol(packetData);

                            writeActivity?.SetStatus(ActivityStatusCode.Ok);
                        }
                        catch (Exception ex)
                        {
                            LastError = ex.ToString();

                            writeActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                            writeActivity?.AddException(ex);

                            Logger?.LogError(ex, "Error writing packet data to file!");
                        }
                    }
                }
            }

            runWatch.Stop();

            _applicationData.AppMetrics?.PacketLoggingTime.Record(runWatch.Elapsed.TotalMilliseconds);

            _applicationData.Statistics.TotalPacketLogTime += runWatch.Elapsed.TotalMilliseconds;
        }
    }

    /// <summary>
    /// Write protocol for logging
    /// </summary>
    /// <param name="packetData">Received packet data</param>
    private void WriteLogProtocol(ReceivedPacketData packetData)
    {
        var protocolFile = ReplaceFilePathTokens(packetData.PacketHeader, PacketLoggingProtocolName);

        if (string.IsNullOrEmpty(protocolFile) == false)
        {
            using (var fileStream = new FileStream(protocolFile, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using var writer = new StreamWriter(fileStream);

                writer.WriteLine($"Timestamp: {packetData.Timestamp:yyyy-MM-dd HH:mm:ss.fff} UTC # Number: {packetData.PacketNumber:D10} | Type: {packetData.PacketHeader?.PacketType, -20}| Length: {packetData.PacketLength}");
            }
        }
    }

    /// <summary>
    /// Check logging directory and create it if neccessary
    /// </summary>
    /// <param name="pathToLog">Logging path</param>
    private void CheckAndCreateLoggingPath(string pathToLog)
    {
        if (Directory.Exists(pathToLog) == false)
        {
            Directory.CreateDirectory(pathToLog);
        }
    }

    /// <summary>
    /// Replace token in file path
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="pathToLog">Logging path</param>
    /// <returns>Replaced logging path</returns>
    private string ReplaceFilePathTokens(PacketHeader? packetHeader, string pathToLog)
    {
        if (packetHeader != null && string.IsNullOrEmpty(pathToLog) == false)
        {
            pathToLog = ReplaceFilePathTokens(packetHeader.GameVersion, packetHeader.UniqueSessionId, pathToLog);

            if (pathToLog.Contains("#packettype", StringComparison.OrdinalIgnoreCase))
            {
                var packetSubDir = packetHeader.PacketType switch
                                   {
                                       PacketTypes.LapData => "lapdata",
                                       PacketTypes.Motion => "motion",
                                       PacketTypes.Session => "session",
                                       PacketTypes.CarSetups => "car-setups",
                                       PacketTypes.CarDamage => "car-damage",
                                       PacketTypes.CarStatus => "car-status",
                                       PacketTypes.CarTelemetry => "car-telemetry",
                                       PacketTypes.Event => "event",
                                       PacketTypes.FinalClassification => "finalclassification",
                                       PacketTypes.LobbyInfo => "lobby",
                                       PacketTypes.Participants => "participants",
                                       PacketTypes.SessionHistory => "session-history",
                                       PacketTypes.TyreSets => "tyresets",
                                       PacketTypes.MotionEx => "motionex",
                                       PacketTypes.TimeTrial => "timetrial",
                                       PacketTypes.LapPositions => "lappositions",
                                       _ => "not-specified"
                                   };

                pathToLog = pathToLog.Replace("#packettype", packetSubDir, StringComparison.OrdinalIgnoreCase);
            }
        }

        return pathToLog;
    }

    /// <summary>
    /// Replace file path tokens
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    /// <param name="sessionId">Session id</param>
    /// <param name="pathToLog">Logging path</param>
    /// <returns>Replaced path</returns>
    private string ReplaceFilePathTokens(ushort gameVersion, ulong sessionId, string pathToLog)
    {
        if (string.IsNullOrEmpty(pathToLog) == false)
        {
            // #version is replaced with game version (2020, 2021, ...)
            pathToLog = pathToLog.Replace("#version", $"{gameVersion}", StringComparison.OrdinalIgnoreCase);

            // #session is replaced with current session id
            pathToLog = pathToLog.Replace("#session", $"{sessionId}", StringComparison.OrdinalIgnoreCase);
        }

        return pathToLog;
    }

    /// <summary>
    /// Check whether the task is running to process all incoming packets
    /// </summary>
    private void CheckPacketProcessingTaskIsRunning()
    {
        if (_isQueueWorkerRunning == false)
        {
            _isQueueWorkerRunning = true;

            _applicationData.Statistics.PacketsInQueue = _queuedPackets;

            using (ExecutionContext.SuppressFlow())
            {
                Task.Run(() => ProcessPacketQueue(_cts.Token), _cts.Token);
            }
        }
    }

    /// <summary>
    /// Check whether the task is running to process logging packets
    /// </summary>
    /// <param name="packetData">Packet data</param>
    private void EnqueueLoggingPacket(ref ReceivedPacketData packetData)
    {
        // Is logging active?
        if (UsePacketLogging && string.IsNullOrWhiteSpace(PacketLoggingPath) == false)
        {
            // Enqueue the packet for logging
            _logPacketQueue.Enqueue(packetData);

            _applicationData.AppMetrics?.PacketsLogged.Add(1);

            // Check whether the task is running to process all incoming logging packets
            if (_isLoggingQueueRunning == false)
            {
                _isLoggingQueueRunning = true;

                Task.Run(() => ProcessPacketLoggingQueue(_ctsLog.Token), _ctsLog.Token);
            }
        }
    }

    #endregion // Private methods

    #region Task methods

    /// <summary>
    /// Task working on the packet queue
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private void ProcessPacketQueue(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                ProcessCurrentPackets(cancellationToken);

                if (_packetQueue?.Count == 0)
                {
                    _applicationData.Statistics.PacketsInQueue = 0;
                }
            }
            catch (Exception ex)
            {
                using var currentActivity = AppActivity.SrvSource.StartActivity("ProcessPacketQueueError", ActivityKind.Internal, null);

                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                Logger?.LogError(ex, "Error processing packet queue!");
            }

            cancellationToken.WaitHandle.WaitOne(100);
        }

        _isQueueWorkerRunning = false;
    }

    /// <summary>
    /// Receives packets from ReplayClient
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task ReceiveReplayPackets(CancellationToken cancellationToken)
    {
        var recvBuf = new byte[4096];

        while (cancellationToken.IsCancellationRequested == false && _tcpServer != null)
        {
            TcpClient tcpClient = await _tcpServer.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);

            if (tcpClient != null)
            {
                using var stream = tcpClient.GetStream();

                _applicationData.Statistics.PacketsReceivedTotal++;
                _applicationData.Statistics.PacketsReceivedCurrentSession++;

                var recvBytes = await stream.ReadAsync(recvBuf, cancellationToken).ConfigureAwait(true);

                if (recvBytes > 0)
                {
                    using var currentActivity = AppActivity.SrvSource.StartActivity("ReceiveReplayPacket", ActivityKind.Server);

                    currentActivity?.SetTag("ReceivedBytes", recvBytes);

                    var packetData = new ReceivedPacketData();

                    packetData.SetRawData(recvBuf.AsSpan(0, recvBytes).ToArray());

                    if (Logger?.IsEnabled(LogLevel.Trace) == true)
                    {
                        Logger.LogTrace("Received TCP packet with length: {Length}", recvBytes);
                    }

                    EnqueueLoggingPacket(ref packetData);

                    Interlocked.Increment(ref _queuedPackets);

                    _applicationData.Statistics.PacketsInQueue = _queuedPackets;

                    // Put the received data in our queue
                    _packetQueue.Enqueue(packetData);

                    _applicationData.AppMetrics?.PacketsReceived.Add(1);
                    _applicationData.AppMetrics?.PacketsInQueue.Record(_packetQueue.Count);

                    currentActivity?.SetStatus(ActivityStatusCode.Ok);
                }

                tcpClient.Close();
                tcpClient.Dispose();
            }
        }

        _tcpServer?.Stop();

        _tcpServer?.Dispose();

        _tcpServer = null;
    }

    /// <summary>
    /// Process all packets to log
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private void ProcessPacketLoggingQueue(CancellationToken cancellationToken)
    {
        var packetAnalyzer = new PacketAnalyzer();

        while (cancellationToken.IsCancellationRequested == false)
        {
            try
            {
                ProcessCurrentLoggingPackets(packetAnalyzer, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error processing logging packet queue!");
            }

            cancellationToken.WaitHandle.WaitOne(100);
        }

        _isLoggingQueueRunning = false;
    }

    /// <summary>
    /// Write new session starts to protocol
    /// </summary>
    /// <param name="newSessionId">New session id</param>
    /// <param name="newGameVersion">New game version</param>
    private void WriteNewSessionStarts(ulong newSessionId, ushort newGameVersion)
    {
        var protocolFile = ReplaceFilePathTokens(newGameVersion, newSessionId, PacketLoggingProtocolName);

        if (string.IsNullOrEmpty(protocolFile) == false)
        {
            using (var fileStream = new FileStream(protocolFile, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using var writer = new StreamWriter(fileStream);

                writer.WriteLine($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} UTC # Session starts   ================================================================================");
            }
        }
    }

    /// <summary>
    /// Write last session statistics to protocol
    /// </summary>
    /// <param name="lastSessionId">Last session id</param>
    /// <param name="lastGameVersion">Last game version</param>
    /// <param name="lastSessionMetrics">Last session metrics</param>
    private void WriteLastSessionStatistics(ulong lastSessionId, ushort lastGameVersion, PacketsPerSessionMetrics lastSessionMetrics)
    {
        var protocolFile = ReplaceFilePathTokens(lastGameVersion, lastSessionId, PacketLoggingProtocolName);

        if (string.IsNullOrEmpty(protocolFile) == false)
        {
            using (var fileStream = new FileStream(protocolFile, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using var writer = new StreamWriter(fileStream);

                if (lastSessionMetrics != null)
                {
                    writer.WriteLine("------------------------------------------------------------------------------------------------------");

                    writer.WriteLine($"Total packets received               : {lastSessionMetrics.TotalPacketsReceived, -6}");
                    writer.WriteLine($"Errors occurred                      : {lastSessionMetrics.Errors, -6}");
                    writer.WriteLine($"Session packets received             : {lastSessionMetrics.Session.Received, -6} (avg: {lastSessionMetrics.Session.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Lap packets received                 : {lastSessionMetrics.LapData.Received, -6} (avg: {lastSessionMetrics.LapData.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Event packets received               : {lastSessionMetrics.Event.Received, -6} (avg: {lastSessionMetrics.Event.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Participants packets received        : {lastSessionMetrics.Participants.Received, -6} (avg: {lastSessionMetrics.Participants.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Car telemetry packets received       : {lastSessionMetrics.CarTelemetry.Received, -6} (avg: {lastSessionMetrics.CarTelemetry.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Car status packets received          : {lastSessionMetrics.CarStatus.Received, -6} (avg: {lastSessionMetrics.CarStatus.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Car setup packets received           : {lastSessionMetrics.CarSetups.Received, -6} (avg: {lastSessionMetrics.CarSetups.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Car damage packets received          : {lastSessionMetrics.CarDamage.Received, -6} (avg: {lastSessionMetrics.CarDamage.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Motion packets received              : {lastSessionMetrics.Motion.Received, -6} (avg: {lastSessionMetrics.Motion.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"MotionEx packets received            : {lastSessionMetrics.MotionEx.Received, -6} (avg: {lastSessionMetrics.MotionEx.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"TyreSets packets received            : {lastSessionMetrics.TyreSets.Received, -6} (avg: {lastSessionMetrics.TyreSets.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Lap positions packets received       : {lastSessionMetrics.LapPositions.Received, -6} (avg: {lastSessionMetrics.LapPositions.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Time trial packets received          : {lastSessionMetrics.TimeTrial.Received, -6} (avg: {lastSessionMetrics.TimeTrial.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Lobby info packets received          : {lastSessionMetrics.LobbyInfo.Received, -6} (avg: {lastSessionMetrics.LobbyInfo.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Session history packets received     : {lastSessionMetrics.SessionHistory.Received, -6} (avg: {lastSessionMetrics.SessionHistory.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Car telemetry 2 packets received     : {lastSessionMetrics.CarTelemetry2.Received, -6} (avg: {lastSessionMetrics.CarTelemetry2.AvgProcessingTime:F3}ms)");
                    writer.WriteLine($"Final classification packets received: {lastSessionMetrics.FinalClassification.Received, -6} (avg: {lastSessionMetrics.FinalClassification.AvgProcessingTime:F3}ms)");

                    writer.WriteLine("------------------------------------------------------------------------------------------------------");
                }

                writer.WriteLine($"Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} UTC # Session finished ================================================================================");
            }
        }
    }

    #endregion // Task methods

    #region IDisposable

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        if (_applicationData != null)
        {
            if (_applicationData.TelemetryWriter is TelemetryWriter telemetryWriter)
            {
                telemetryWriter.Dispose();
            }

            _applicationData.TelemetryWriter = null;

            _applicationData.Statistics.SessionChanged -= OnStatisticsSessionChanged;

            _applicationData.LoggerFactory?.Dispose();
            _applicationData.LoggerFactory = null;
        }

        _timeoutTimer?.Dispose();
        _statisticsTimer?.Dispose();

        _packetProcessor?.Dispose();

        _tcpServer?.Stop();
        _tcpServer?.Dispose();
        _tcpServer = null;

        _cts?.Cancel();
        _cts?.Dispose();

        _ctsLog?.Cancel();
        _ctsLog?.Dispose();

        _webHosting?.StopWebHosting();
        _webHosting?.Dispose();

        _udpClient?.Dispose();
        _udpClient = null;
    }

    #endregion // IDisposable
}