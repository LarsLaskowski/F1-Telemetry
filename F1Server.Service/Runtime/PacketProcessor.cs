using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.EventArgs;
using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;
using F1Server.Service.Processors;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleToAttribute("F1Server.Tests")]

namespace F1Server.Service.Runtime;

/// <summary>
/// Process received packets
/// </summary>
internal class PacketProcessor : IDisposable
{
    #region Fields

    private readonly PacketAnalyzer _packetAnalyzer;
    private readonly ProcessorFactory _processorFactory;
    private readonly ConcurrentQueue<ReceivedPacketData> _queuedPackets;
    private readonly Lock _lockObj = new();
    private readonly F1ServerApplicationData? _appData;
    private bool _waitingForFirstSessionPacket;
    private ulong _currentSessionIdentifier;
    private bool _isNewSession;
    private bool _queuePacketsNow;

    /// <summary>
    /// Number of packets currently held in <see cref="_queuedPackets"/>, maintained to avoid <see cref="ConcurrentQueue{T}.Count"/> in the packet path
    /// </summary>
    private long _queuedPacketCount;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> containing the configured services</param>
    /// <param name="useDatabase">Flag using a database</param>
    public PacketProcessor(IServiceProvider serviceProvider, bool useDatabase)
    {
        UseDatabase = useDatabase;

        _packetAnalyzer = serviceProvider.GetRequiredService<PacketAnalyzer>();
        _appData = serviceProvider.GetRequiredService<F1ServerApplicationData>();

        _processorFactory = new ProcessorFactory(serviceProvider);
        _queuedPackets = new ConcurrentQueue<ReceivedPacketData>();

        Logger = _appData.Logger;
    }

    #endregion // Constructors

    #region Events

    /// <summary>
    /// Event raised when a received packet indicates a changed game version or a changed session identifier
    /// </summary>
    public event EventHandler<PacketReceivedEventArgs> PacketReceived;

    #endregion // Events

    #region Properties

    /// <summary>
    /// Gets the logger instance used for logging messages and events
    /// </summary>
    public ILogger? Logger { get; }

    /// <summary>
    /// Current session runtime information
    /// </summary>
    public SessionRuntimeData? Session { get; private set; }

    /// <summary>
    /// Flag using a database or not
    /// </summary>
    public bool UseDatabase { get; }

    /// <summary>
    /// Last error
    /// </summary>
    public string? LastError { get; private set; }

    /// <summary>
    /// Number of current game
    /// </summary>
    public int CurrentGame { get; private set; }

    /// <summary>
    /// Runtime game information
    /// </summary>
    public LiveGameData CurrentGameInfo { get; private set; }

    /// <summary>
    /// Number of queued packets
    /// </summary>
    public long QueuedPackets => Interlocked.Read(ref _queuedPacketCount);

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Gets the time stamp of this packet
    /// </summary>
    /// <param name="receivedPacketData">Received packet</param>
    /// <returns>Timestamp</returns>
    public uint GetPacketTimestamp(ReceivedPacketData receivedPacketData)
    {
        uint packetTimestamp = 0;

        try
        {
            if (receivedPacketData.PacketHeader != null)
            {
                packetTimestamp = receivedPacketData.PacketHeader.SessionTimeNum;
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error getting packet timestamp!");
        }

        return packetTimestamp;
    }

    /// <summary>
    /// Analyze data packet
    /// </summary>
    /// <param name="receivedPacketData">Received data</param>
    /// <returns>Status</returns>
    public bool ProcessPacket(ReceivedPacketData receivedPacketData)
    {
        var isProcessed = false;

        // Lock to be sure that all following packtets have the correct game information
        lock (_lockObj)
        {
            try
            {
                LastError = string.Empty;

                if (receivedPacketData.PacketHeader != null)
                {
                    var raisePacketEvent = false;

                    if (receivedPacketData.PacketHeader.UniqueSessionId != _currentSessionIdentifier)
                    {
                        _isNewSession = true;

                        _appData?.IsActiveSession = false;

                        _currentSessionIdentifier = receivedPacketData.PacketHeader.UniqueSessionId;

                        raisePacketEvent = true;
                    }

                    if (CurrentGame != receivedPacketData.PacketHeader.GameVersion)
                    {
                        CurrentGame = receivedPacketData.PacketHeader.GameVersion;

                        CheckGameVersion(CurrentGame, receivedPacketData.PacketHeader.MajorGameVersion, receivedPacketData.PacketHeader.MinorGameVersion);

                        raisePacketEvent = true;
                    }

                    // Raise the event only on game version or session changes to avoid a Task and event argument allocation per packet
                    if (raisePacketEvent)
                    {
                        RaisePacketReceived(receivedPacketData.PacketHeader);
                    }

                    //// The final classification packet should not be the final packet, normally in F1 2021++ a bulk of session history packets will be send after this packet.
                    //// Processing files is slightly different, so the bulk session history can saved before this packet
                    if (_queuePacketsNow == true && receivedPacketData.PacketHeader.PacketType == PacketTypes.FinalClassification)
                    {
                        _queuePacketsNow = false;
                    }

                    isProcessed = InternalProcessPackets(receivedPacketData);
                }
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();

                Logger?.LogError(ex, "Error processing packet {PacketType}!", receivedPacketData.PacketHeader?.PacketType);
            }
        }

        return isProcessed;
    }

    /// <summary>
    /// Returns the packet processor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <returns>Processor</returns>
    internal BaseProcessor? GetProcessor(PacketHeader packetHeader)
    {
        return _processorFactory.GetProcessor(packetHeader, CurrentGameInfo);
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Raises the <see cref="PacketReceived"/> event synchronously so subscriber exceptions are observed and logged
    /// </summary>
    /// <param name="packetHeader">Header of received packet</param>
    private void RaisePacketReceived(PacketHeader packetHeader)
    {
        try
        {
            PacketReceived?.Invoke(this, new PacketReceivedEventArgs(packetHeader));
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in packet received event subscriber!");
        }
    }

    /// <summary>
    /// Internal method for processing received packets
    /// </summary>
    /// <param name="receivedPacketData">Received packet data</param>
    /// <returns>Status</returns>
    private bool InternalProcessPackets(ReceivedPacketData receivedPacketData)
    {
        var retValue = false;

        // No more queuing and we have queued packets? Process this queue!
        if (_queuePacketsNow == false && Interlocked.Read(ref _queuedPacketCount) > 0)
        {
            retValue = ProcessQueuedPackets(receivedPacketData);
        }
        else
        {
            if (_queuePacketsNow == true)
            {
                _queuedPackets?.Enqueue(receivedPacketData);

                Interlocked.Increment(ref _queuedPacketCount);
            }
            else
            {
                var isFinished = Session?.CurrentSession?.IsFinished;

                retValue = AnalyzePacket(receivedPacketData);

                if (Session?.CurrentSession?.IsFinished == true && isFinished == false && Session.GameVersion >= 2021)
                {
                    _queuePacketsNow = true;
                }
            }
        }

        return retValue;
    }

    /// <summary>
    /// Process queued packets
    /// </summary>
    /// <param name="receivedPacketData">Received packet data</param>
    /// <returns>Status</returns>
    private bool ProcessQueuedPackets(ReceivedPacketData receivedPacketData)
    {
        // Don't forget to analyze first the actually received packet, this is the final classification packet
        bool retValue = AnalyzePacket(receivedPacketData);

        while (_queuedPackets.IsEmpty == false)
        {
            if (_queuedPackets.TryDequeue(out var queuedPacket))
            {
                Interlocked.Decrement(ref _queuedPacketCount);

                if (queuedPacket.PacketHeader != null)
                {
                    AnalyzePacket(queuedPacket);
                }
            }
        }

        return retValue;
    }

    /// <summary>
    /// Analyze the received packet
    /// </summary>
    /// <param name="receivedPacketData">Raw packet data</param>
    /// <returns>Status</returns>
    private bool AnalyzePacket(ReceivedPacketData receivedPacketData)
    {
        var retValue = false;

        using var currentActivity = AppActivity.SrvSource.StartActivity("AnalyzePacket");

        try
        {
            var packetData = _packetAnalyzer.GetPacketData(receivedPacketData);

            // Do something with the packet
            if (packetData != null)
            {
                retValue = ProcessPacketInternal(receivedPacketData, packetData);
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error analyzing packet {PacketType}!", receivedPacketData.PacketHeader?.PacketType);

            LastError = ex.ToString();

            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
            currentActivity?.AddException(ex);
        }

        return retValue;
    }

    /// <summary>
    /// Process received packet data
    /// </summary>
    /// <param name="packetData">Packet data</param>
    /// <param name="packetContent">Content of packet</param>
    /// <returns>Status</returns>
    private bool ProcessPacketInternal(ReceivedPacketData packetData, object? packetContent)
    {
        var isProcessed = true;

        // Pre-process event packets, because the indicate a new or finished session
        var sessionIsFinished = PreProcessEvents(packetContent as EventData, packetData.PacketHeader?.PacketType);

        if (packetData.PacketHeader?.PacketType == PacketTypes.FinalClassification && Session != null)
        {
            Session.FinalClassificationReceived = true;
        }

        // Process only packets, if a database is available
        if (UseDatabase && packetData.PacketHeader != null)
        {
            // Waiting for a new session?
            CheckNewSession(packetData.PacketHeader.PacketType, packetContent as SessionData);

            if (Session != null && (Session.IsRecordable == true || Session.CurrentSession == null))
            {
                isProcessed = ProcessPacket(packetData.PacketHeader!, packetContent);
            }

            if (sessionIsFinished && Session != null)
            {
                Session.FinishSession();
            }
        }

        return isProcessed;
    }

    /// <summary>
    /// Process packet
    /// </summary>
    /// <param name="packetHeader">Header of received packet</param>
    /// <param name="packetContent">Content of received packet</param>
    /// <returns>Is processed?</returns>
    private bool ProcessPacket(PacketHeader packetHeader, object? packetContent)
    {
        var isProcessed = true;

        try
        {
            var processor = GetProcessor(packetHeader);

            if (processor != null)
            {
                isProcessed = processor.Process(packetContent, Session);

                if (isProcessed == false && string.IsNullOrWhiteSpace(processor.LastException) == false)
                {
                    LastError = processor.LastException;

                    _appData?.AppMetrics?.ProcessingErrors.Add(1, new KeyValuePair<string, object?>("LastError", processor.LastException));
                }

                if (Session?.IsInvalidSession == true)
                {
                    RemoveInvalidSessionFromDatabase();

                    Session.IsInvalidSession = false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error processing packet {PacketType}!", packetHeader.PacketType);

            LastError = ex.ToString();
        }

        if (Session != null && _appData?.LiveSessionId == 0 && Session.CurrentSession != null && Session.CurrentSession.DbId != 0)
        {
            _appData.LiveSessionId = Session.CurrentSession.DbId;
            _appData.LiveSessionData = Session.CurrentSession;
        }

        return isProcessed;
    }

    /// <summary>
    /// Process event packets first
    /// </summary>
    /// <param name="eventData">Event data</param>
    /// <param name="packetType">Current packet type</param>
    /// <returns>Session finished?</returns>
    private bool PreProcessEvents(EventData? eventData, PacketTypes? packetType)
    {
        var sessionIsFinished = false;
        var isFinalClassificationPacket = IsFinalClassificationPacket(packetType);
        var isOtherPacket = IsOtherPacket(packetType);

        // Receiving other packets without SSTA event?
        if ((eventData?.IsSessionStart == true || isOtherPacket)
            && _appData?.IsActiveSession == false
            && _isNewSession
            && _waitingForFirstSessionPacket == false)
        {
            _waitingForFirstSessionPacket = true;

            // A new session resets the last session, because some packets arrive after session is finished
            Session = null;

            using var currentActivity = AppActivity.SrvSource.StartActivity("SessionStart");

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        // Receiving a final classification packet without SEND event?
        if ((eventData?.IsSessionEnd == true || isFinalClassificationPacket) && _appData?.IsActiveSession == true)
        {
            sessionIsFinished = true;

            _appData.IsLiveSession = false;
            _appData.IsActiveSession = false;

            using var currentActivity = AppActivity.SrvSource.StartActivity("SessionEnd");

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        UpdateFlashbacks(eventData);

        return sessionIsFinished;
    }

    /// <summary>
    /// Is other packettype, no event, no final classification?
    /// </summary>
    /// <param name="packetType">Packet type</param>
    /// <returns>Status</returns>
    private bool IsOtherPacket(PacketTypes? packetType)
    {
        var isOther = false;

        if (packetType != null)
        {
            isOther = packetType != PacketTypes.Event && packetType != PacketTypes.FinalClassification;
        }

        return isOther;
    }

    /// <summary>
    /// Is other packettype, no event, no final classification?
    /// </summary>
    /// <param name="packetType">Packet type</param>
    /// <returns>Status</returns>
    private bool IsFinalClassificationPacket(PacketTypes? packetType)
    {
        var isFinal = false;

        if (packetType != null)
        {
            isFinal = packetType == PacketTypes.FinalClassification;
        }

        return isFinal;
    }

    /// <summary>
    /// Update usage of flashbacks
    /// </summary>
    /// <param name="eventData">Event data</param>
    private void UpdateFlashbacks(EventData? eventData)
    {
        if (eventData?.IsFlashback == true && Session != null)
        {
            Session.FlashbacksUsed++;

            if (eventData.PacketData?.EventDetails is IEventDataDetails2021 eventDetails21)
            {
                Session.LastFlashbackFrame = eventDetails21.FlashbackFrame;

                _processorFactory.IsResetFrameIdentifier = true;
            }
            else if (eventData.PacketData?.EventDetails is IEventDataDetails2022 eventDetails22)
            {
                Session.LastFlashbackFrame = eventDetails22.FlashbackFrame;

                _processorFactory.IsResetFrameIdentifier = true;
            }
            else if (eventData.PacketData?.EventDetails is IEventDataDetails2023 eventDetails23)
            {
                Session.LastFlashbackFrame = eventDetails23.FlashbackFrame;

                _processorFactory.IsResetFrameIdentifier = true;
            }
            else if (eventData.PacketData?.EventDetails is IEventDataDetails2024 eventDetails24)
            {
                Session.LastFlashbackFrame = eventDetails24.FlashbackFrame;

                _processorFactory.IsResetFrameIdentifier = true;
            }
            else if (eventData.PacketData?.EventDetails is IEventDataDetails2025 eventDetails25)
            {
                Session.LastFlashbackFrame = eventDetails25.FlashbackFrame;

                _processorFactory.IsResetFrameIdentifier = true;
            }
            else
            {
                Logger?.LogWarning("Received flashback event, but no valid event details found!");
            }
        }
    }

    /// <summary>
    /// Check current game version
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    /// <param name="majorVersion">Major game version</param>
    /// <param name="minorVersion">Minor game version</param>
    private void CheckGameVersion(int gameVersion, int majorVersion, int minorVersion)
    {
        if (UseDatabase)
        {
            using var currentActivity = AppActivity.SrvSource.StartActivity(nameof(CheckGameVersion));

            currentActivity?.AddTag("f1.game_name", $"F1 {gameVersion}");
            currentActivity?.AddTag("f1.game_version", $"{majorVersion}.{minorVersion}");

            try
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    var isValid = false;
                    var game = dbFactory.GetRepository<GameVersionRepository>()?.GetQuery()?.FirstOrDefault(g => g.Version == gameVersion);

                    if (game != null)
                    {
                        isValid = UpdateGameData(gameVersion, majorVersion, minorVersion, dbFactory, game);
                    }
                    else
                    {
                        game = new GameVersionEntity
                               {
                                   Version = gameVersion,
                                   Name = $"F1 {gameVersion}",
                                   MajorVersion = majorVersion,
                                   MinorVersion = minorVersion,
                                   LastUsed = DateTime.UtcNow
                               };

                        if (dbFactory.GetRepository<GameVersionRepository>()?.Add(game) == true)
                        {
                            isValid = true;
                        }
                    }

                    if (isValid && (CurrentGameInfo == null || CurrentGameInfo.GameVersion != gameVersion))
                    {
                        CurrentGameInfo = new LiveGameData
                                          {
                                              DbId = game.Id,
                                              GameVersion = game.Version,
                                              LastTimeUsed = game.LastUsed,
                                              MajorVersion = game.MajorVersion,
                                              MinorVersion = game.MinorVersion,
                                              Name = game.Name
                                          };
                    }
                }

                currentActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                currentActivity?.AddException(ex);

                Logger?.LogError(ex, "Error checking game version {GameVersion}!", gameVersion);
            }
        }
    }

    /// <summary>
    /// Update game data
    /// </summary>
    /// <param name="gameVersion">Game version</param>
    /// <param name="majorVersion">Major version</param>
    /// <param name="minorVersion">Minor version</param>
    /// <param name="dbFactory">DbFactory object</param>
    /// <param name="game">Game entity</param>
    /// <returns>Valid?</returns>
    private bool UpdateGameData(int gameVersion, int majorVersion, int minorVersion, RepositoryFactory dbFactory, GameVersionEntity game)
    {
        return dbFactory.GetRepository<GameVersionRepository>()?.Refresh(g => g.Version == gameVersion,
                                                                         (obj) =>
                                                                         {
                                                                             obj.MajorVersion = majorVersion > game.MajorVersion ? majorVersion : game.MajorVersion;
                                                                             obj.MinorVersion = minorVersion > game.MinorVersion ? minorVersion : game.MinorVersion;
                                                                             obj.LastUsed = DateTime.UtcNow;
                                                                         }) == true;
    }

    /// <summary>
    /// Remove invalid session from database
    /// </summary>
    private void RemoveInvalidSessionFromDatabase()
    {
        if (UseDatabase && Session != null)
        {
            try
            {
                using (var dbFactory = RepositoryFactory.CreateInstance())
                {
                    var isRemoved = dbFactory.GetRepository<SessionRepository>()?.Remove(s => s.Id == Session.SessionDbId);

                    // Removed?
                    if (isRemoved != null && isRemoved.Value)
                    {
                        Session = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error removing invalid session from database - session: {SessionDbId}!", Session?.SessionDbId);
            }

            if (_appData != null)
            {
                _appData.LiveSessionId = 0;
                _appData.IsLiveSession = false;
            }
        }
    }

    /// <summary>
    /// Is new session?
    /// </summary>
    /// <param name="packetType">Type of received packet</param>
    /// <param name="sessionData">Session data</param>
    private void CheckNewSession(PacketTypes packetType, SessionData? sessionData)
    {
        if (_waitingForFirstSessionPacket == true
            && packetType == PacketTypes.Session
            && sessionData?.IsRecordable == true)
        {
            Session = new SessionRuntimeData(CurrentGame, sessionData.SessionId, sessionData.SessionType);

            using var currentActivity = AppActivity.SrvSource.StartActivity("NewSession");

            currentActivity?.AddTag("f1.session_id", sessionData.PacketHeader.UniqueSessionId);

            Logger?.LogInformation("New session started with id {SessionId}", sessionData.PacketHeader.UniqueSessionId);

            _waitingForFirstSessionPacket = false;
            _isNewSession = false;

            if (_appData != null)
            {
                _appData.IsLiveSession = true;
                _appData.LiveSessionId = 0;
                _appData.IsActiveSession = true;
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
    }

    #endregion // Private methods

    #region IDisposable

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal dispose method
    /// </summary>
    /// <param name="disposing">Dispose flag</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _processorFactory?.Dispose();
        }
    }

    #endregion // IDisposable
}