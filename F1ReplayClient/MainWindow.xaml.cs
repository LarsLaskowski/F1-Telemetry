using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using F1ReplayClient.Data;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Shared;

namespace F1ReplayClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IDisposable
{
    #region Fields

    private const string AppName = "F1ReplayClient";

    private readonly PacketAnalyzer _packetAnalyzer = new();
    private readonly ContextViewData _viewData = new();

    private CancellationTokenSource? _cts;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        DataContext = _viewData;

        _viewData.IsEditable = true;

        if (Environment.GetCommandLineArgs().Length > 0)
        {
            var cmdArgs = Environment.GetCommandLineArgs();

            var hostArg = Array.Find(cmdArgs, arg => arg.StartsWith("--host="));

            if (string.IsNullOrWhiteSpace(hostArg) == false)
            {
                _viewData.Host = hostArg["--host=".Length..];
            }

            _viewData.Status = "Ready";
        }
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Valid folder?
    /// </summary>
    public bool IsValidFolder { get; private set; }

    /// <summary>
    /// Files to analyze
    /// </summary>
    public int FilesToAnalyze { get; private set; }

    /// <summary>
    /// Startable?
    /// </summary>
    public bool IsStartable => IsValidFolder && IsRunning == false;

    /// <summary>
    /// Is running?
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Session info from folder found?
    /// </summary>
    public bool HasSessionInfo { get; private set; }

    /// <summary>
    /// List of current files in selected folder
    /// </summary>
    internal List<FileData> CurrentFolderFiles { get; private set; } = new();

    #endregion // Properties

    #region View methods

    /// <summary>
    /// Select folder
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="routedEventArgs">Event arguments</param>
    private void OnButtonClick(object sender, RoutedEventArgs routedEventArgs)
    {
        var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
                     {
                         Multiselect = false,
                         ShowNewFolderButton = false,
                         SelectedPath = _viewData.FilesFolder,
                         Description = "Select a path...",
                         UseDescriptionForTitle = true
                     };

        if (dialog.ShowDialog() == true)
        {
            IsValidFolder = false;
            HasSessionInfo = false;

            var folderPath = dialog.SelectedPath;

            _viewData.SessionId = 0;
            _viewData.SessionType = string.Empty;
            _viewData.FormulaType = string.Empty;
            _viewData.TrackName = string.Empty;
            _viewData.TotalLaps = 0;
            _viewData.Weather = string.Empty;
            _viewData.AiDifficulty = 0;
            _viewData.IsStartable = false;
            _viewData.IsSortable = false;

            _viewData.FilesFolder = folderPath;

            _viewData.Status = "Analyzing path...";

            CurrentFolderFiles.Clear();

            Task.Run(() => AnalyzeFolder(folderPath));
        }
    }

    /// <summary>
    /// Start processing
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="routedEventArgs">Argument</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Ignore here")]
    private void OnStartClick(object sender, RoutedEventArgs routedEventArgs)
    {
        if (string.IsNullOrWhiteSpace(_viewData.Host) == false)
        {
            if (_viewData.Host.Contains(':'))
            {
                var hostData = _viewData.Host.Split(':');

                if (int.TryParse(hostData[1], out var port))
                {
                    var folderPath = _viewData.FilesFolder;

                    if (string.IsNullOrWhiteSpace(folderPath) == false)
                    {
                        IsRunning = true;

                        _viewData.IsEditable = false;

                        _cts?.Cancel();
                        _cts?.Dispose();

                        _cts = null;

                        _cts = new CancellationTokenSource();

                        _viewData.ProcessedFiles = 0;
                        _viewData.TotalFiles = FilesToAnalyze;

                        Task.Run(() => Replay(hostData[0], port, _cts.Token), _cts.Token);
                    }
                    else
                    {
                        MessageBox.Show("No folder path!", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid port!", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid host information!", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("No host information!", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Sort packets
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="routedEventArgs">Argument</param>
    private void OnSortClick(object sender, RoutedEventArgs routedEventArgs)
    {
        var folderPath = _viewData.FilesFolder;

        if (string.IsNullOrWhiteSpace(folderPath) == false)
        {
            IsRunning = true;

            _viewData.IsEditable = false;

            _cts?.Cancel();
            _cts?.Dispose();

            _cts = null;

            _cts = new CancellationTokenSource();

            _viewData.ProcessedFiles = 0;
            _viewData.TotalFiles = FilesToAnalyze;

            Task.Run(() => SortPackets(_cts.Token), _cts.Token);
        }
        else
        {
            MessageBox.Show("No folder path!", AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Continue packet processing
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="routedEventArgs">Argument</param>
    private void OnContinueClick(object sender, RoutedEventArgs routedEventArgs)
    {
        _viewData.IsPaused = false;
    }

    /// <summary>
    /// Stop processing packets
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="routedEventArgs">Argument</param>
    private void OnStopClick(object sender, RoutedEventArgs routedEventArgs)
    {
        _cts?.Cancel();
    }

    #endregion // View methods

    #region Methods

    /// <summary>
    /// Analyse folder content
    /// </summary>
    /// <param name="folder">folder</param>
    private void AnalyzeFolder(string folder)
    {
        _viewData.IsEditable = false;

        if (string.IsNullOrWhiteSpace(folder) == false && Directory.Exists(folder))
        {
            CurrentFolderFiles = Directory.EnumerateFiles(folder, "packet-*", SearchOption.AllDirectories)
                                          .Select(obj => new FileData
                                                         {
                                                             FileName = obj,
                                                             FileInfo = new FileInfo(obj)
                                                         })
                                          .OrderBy(f => f.FileInfo.Name)
                                          .ToList();

            if (CurrentFolderFiles.Any())
            {
                var gameVersion = 0;

                IsValidFolder = true;

                _viewData.Status = $"Files found: {CurrentFolderFiles.Count}";

                FilesToAnalyze = CurrentFolderFiles.Count;

                if (HasSessionInfo == false)
                {
                    gameVersion = FileFunctions.GetGameVersionFromFile(CurrentFolderFiles[0].FileName);

                    var sessionDetector = new DetectSessionData(CurrentFolderFiles.Select(f => f.FileName).ToList(), gameVersion);

                    if (sessionDetector.DetectSession() && sessionDetector.SessionData != null)
                    {
                        _viewData.TrackName = sessionDetector.SessionData.TrackName;
                        _viewData.SessionType = sessionDetector.SessionData.SessionType.ToString();
                        _viewData.FormulaType = sessionDetector.SessionData.FormulaType.ToString();
                        _viewData.TotalLaps = sessionDetector.SessionData.TotalLaps;
                        _viewData.SessionId = sessionDetector.SessionData.SessionId;
                        _viewData.Weather = sessionDetector.SessionData.Weather.ToString();
                        _viewData.AiDifficulty = sessionDetector.SessionData.AiDifficulty;
                    }
                }

                _viewData.GameVersion = gameVersion;
            }
        }

        if (CurrentFolderFiles.Count == 0)
        {
            IsValidFolder = false;

            _viewData.Status = "No files found!";

            FilesToAnalyze = 0;
        }

        _viewData.IsStartable = IsValidFolder && _viewData.IsValidHost;
        _viewData.IsSortable = _viewData.GameVersion >= 2023;
        _viewData.IsEditable = true;
    }

    /// <summary>
    /// Replay content of folder
    /// </summary>
    /// <param name="hostName">Hostname</param>
    /// <param name="port">Port</param>
    /// <param name="cancelToken">Cancellation token</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Ignore here")]
    private void Replay(string hostName, int port, CancellationToken cancelToken)
    {
        _viewData.IsRunning = true;

        Application.Current.Dispatcher.Invoke(_viewData.EventCodes.Clear);

        if (string.IsNullOrWhiteSpace(hostName) == false && port > 0 && CurrentFolderFiles.Count > 0)
        {
            var runWatch = new Stopwatch();
            var timeWatch = new Stopwatch();
            var processingWatch = new Stopwatch();
            var processedFiles = 0;
            var sessionFrequency = 0;
            var processingDuration = 0.0D;
            var lastFileDuration = 0.0D;
            var packetType = PacketTypes.Unknown;

            runWatch.Start();
            processingWatch.Start();

            _viewData.ProcessedFiles = 0;

            var buffer = ArrayPool<byte>.Shared.Rent(4096);
            var lengthPrefix = new byte[sizeof(int)];

            TcpClient? tcpClient = null;
            NetworkStream? netStream = null;

            foreach (var file in CurrentFolderFiles)
            {
                var skipPacket = false;

                timeWatch.Restart();

                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }

                while (_viewData.IsPaused)
                {
                    cancelToken.WaitHandle.WaitOne(25);
                }

                using (var fs = new FileStream(file.FileName, FileMode.Open, FileAccess.Read))
                {
                    var fileLength = (int)file.FileInfo.Length;

                    try
                    {
                        fs.ReadExactly(buffer, 0, fileLength);

                        var packet = new ReceivedPacketData();

                        packet.SetRawData(buffer.AsSpan(0, fileLength).ToArray());

                        packetType = DeterminePacketType(packet, out var eventCode);

                        if (packetType == PacketTypes.Session)
                        {
                            sessionFrequency++;
                        }

                        if (string.IsNullOrWhiteSpace(eventCode) == false
                            && eventCode != "BUTN")
                        {
                            Application.Current.Dispatcher.Invoke(() => _viewData.EventCodes.Insert(0, eventCode));
                        }

                        PacketTypeToView(packetType);

                        if (_viewData.IgnoreCarPackets
                            && (packetType == PacketTypes.CarSetups
                                || packetType == PacketTypes.CarDamage
                                || packetType == PacketTypes.Motion
                                || packetType == PacketTypes.TyreSets
                                || packetType == PacketTypes.MotionEx))
                        {
                            skipPacket = true;
                        }

                        // send data
                        if (_viewData.SendData && skipPacket == false)
                        {
                            if (tcpClient == null || tcpClient.Connected == false)
                            {
                                netStream?.Dispose();
                                tcpClient?.Dispose();

                                tcpClient = new TcpClient(hostName, port);
                                netStream = tcpClient.GetStream();
                            }

                            // Prefix every packet with its length so the reused connection can be framed on the server side
                            BinaryPrimitives.WriteInt32LittleEndian(lengthPrefix, fileLength);

                            netStream!.Write(lengthPrefix);
                            netStream.Write(buffer.AsSpan(0, fileLength));

                            cancelToken.WaitHandle.WaitOne(2);
                        }
                    }
                    catch
                    {
                        // Ignore exceptions in this step, but drop a broken connection so the next packet reconnects
                        netStream?.Dispose();
                        tcpClient?.Dispose();

                        netStream = null;
                        tcpClient = null;
                    }
                    finally
                    {
                        fs.Close();
                    }
                }

                ++processedFiles;

                _viewData.ProcessedFiles = processedFiles;

                timeWatch.Stop();

                if (skipPacket)
                {
                    processingDuration += lastFileDuration;
                }
                else
                {
                    processingDuration += timeWatch.Elapsed.TotalMilliseconds;

                    lastFileDuration = timeWatch.Elapsed.TotalMilliseconds;
                }

                _viewData.TimeEstimated = TimeSpan.FromMilliseconds(CurrentFolderFiles.Count * (processingDuration / processedFiles)).ToString(@"hh\:mm\:ss\.fff");

                // Break after special packets?
                if ((packetType == PacketTypes.Event && _viewData.BreakAtEventPacket)
                    || (packetType == PacketTypes.Session && _viewData.BreakAtSessionPacket)
                    || (packetType == PacketTypes.LapData && _viewData.BreakAtLapDataPacket)
                    || (packetType == PacketTypes.Participants && _viewData.BreakAtParticipantsPacket)
                    || (packetType == PacketTypes.FinalClassification && _viewData.BreakAtFinalClassificationPacket)
                    || (packetType == PacketTypes.SessionHistory && _viewData.BreakAtSessionHistoryPacket)
                    || (packetType == PacketTypes.CarTelemetry && _viewData.BreakAtCarTelemetryPacket)
                    || (packetType == PacketTypes.CarTelemetry2 && _viewData.BreakAtCarTelemetry2Packet)
                    || (packetType == PacketTypes.CarStatus && _viewData.BreakAtCarStatusPacket)
                    || (packetType == PacketTypes.TimeTrial && _viewData.BreakAtTimeTrialPacket)
                    || (packetType == PacketTypes.LapPositions && _viewData.BreakAtLapPositionsPacket))
                {
                    _viewData.CurrentPacketType = packetType;

                    _viewData.IsPaused = true;
                }
                else
                {
                    _viewData.CurrentPacketType = PacketTypes.Unknown;
                }

                _viewData.TimeDuration = TimeSpan.FromMilliseconds(processingWatch.Elapsed.TotalMilliseconds).ToString(@"hh\:mm\:ss\.fff");
            }

            netStream?.Dispose();
            tcpClient?.Dispose();

            ArrayPool<byte>.Shared.Return(buffer);
        }

        IsRunning = false;

        _viewData.IsRunning = false;
        _viewData.IsPaused = false;
        _viewData.IsEditable = true;
    }

    /// <summary>
    /// Order packets
    /// </summary>
    /// <param name="cancelToken">Cancellation token</param>
    private void SortPackets(CancellationToken cancelToken)
    {
        _viewData.IsRunning = true;

        if (CurrentFolderFiles.Count > 0)
        {
            var options = new ParallelOptions()
                          {
                              MaxDegreeOfParallelism = Environment.ProcessorCount
                          };
            var processedFiles = 0;

            _viewData.ProcessedFiles = 0;

            Parallel.ForEach(CurrentFolderFiles,
                             options,
                             (file, state) =>
                             {
                                 if (cancelToken.IsCancellationRequested)
                                 {
                                     state.Break();
                                 }

                                 var buffer = ArrayPool<byte>.Shared.Rent(30);

                                 using (var fs = new FileStream(file.FileName, FileMode.Open, FileAccess.Read))
                                 {
                                     try
                                     {
                                         fs.ReadExactly(buffer, 0, 30);

                                         ref var memRef = ref MemoryMarshal.GetReference(buffer.AsSpan());

                                         file.SessionTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref memRef, 15));
                                         file.OverallFrameIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref memRef, 23));
                                     }
                                     catch
                                     {
                                         // Ignore exceptions in this step
                                     }
                                     finally
                                     {
                                         fs.Close();

                                         ArrayPool<byte>.Shared.Return(buffer);
                                     }
                                 }

                                 var updatedProcessedFiles = Interlocked.Increment(ref processedFiles);

                                 _viewData.ProcessedFiles = updatedProcessedFiles;
                             });

            CurrentFolderFiles = CurrentFolderFiles.OrderBy(f => f.SessionTime).ToList();
        }

        IsRunning = false;

        _viewData.IsRunning = false;
        _viewData.IsPaused = false;
        _viewData.IsEditable = true;
    }

    /// <summary>
    /// Determine type of packet
    /// </summary>
    /// <param name="packetData">Data from file</param>
    /// <param name="eventCode">Event code</param>
    /// <returns>Packet type</returns>
    private PacketTypes DeterminePacketType(ReceivedPacketData packetData, out string eventCode)
    {
        var packetType = PacketTypes.Unknown;

        eventCode = string.Empty;

        if (packetData?.PacketHeader != null && _packetAnalyzer != null)
        {
            packetType = packetData.PacketHeader.PacketType;

            if (packetType == PacketTypes.Event)
            {
                var packetContent = _packetAnalyzer.GetEventData(packetData.PacketHeader, packetData.PacketRawData);

                if (packetContent is EventData eventData)
                {
                    eventCode = eventData.EventCode;
                }
            }
        }

        return packetType;
    }

    /// <summary>
    /// Count the packet
    /// </summary>
    /// <param name="packetType">Type of packet</param>
    private void PacketTypeToView(PacketTypes packetType)
    {
        switch (packetType)
        {
            case PacketTypes.Motion:
                _viewData.PacketViewTypes.Motion++;
                break;

            case PacketTypes.Session:
                _viewData.PacketViewTypes.Session++;
                break;

            case PacketTypes.LapData:
                _viewData.PacketViewTypes.LapData++;
                break;

            case PacketTypes.Event:
                _viewData.PacketViewTypes.Event++;
                break;

            case PacketTypes.Participants:
                _viewData.PacketViewTypes.Participants++;
                break;

            case PacketTypes.CarSetups:
                _viewData.PacketViewTypes.CarSetups++;
                break;

            case PacketTypes.CarTelemetry:
                _viewData.PacketViewTypes.CarTelemetry++;
                break;

            case PacketTypes.CarTelemetry2:
                _viewData.PacketViewTypes.CarTelemetry2++;
                break;

            case PacketTypes.CarStatus:
                _viewData.PacketViewTypes.CarStatus++;
                break;

            case PacketTypes.CarDamage:
                _viewData.PacketViewTypes.CarDamage++;
                break;

            case PacketTypes.FinalClassification:
                _viewData.PacketViewTypes.FinalClassification++;
                break;

            case PacketTypes.LobbyInfo:
                _viewData.PacketViewTypes.LobbyInfo++;
                break;

            case PacketTypes.SessionHistory:
                _viewData.PacketViewTypes.SessionHistory++;
                break;

            case PacketTypes.TyreSets:
                _viewData.PacketViewTypes.TyreSets++;
                break;

            case PacketTypes.MotionEx:
                _viewData.PacketViewTypes.MotionEx++;
                break;

            case PacketTypes.TimeTrial:
                _viewData.PacketViewTypes.TimeTrial++;
                break;

            case PacketTypes.LapPositions:
                _viewData.PacketViewTypes.LapPositions++;
                break;

            default:
                _viewData.PacketViewTypes.Unknown++;
                break;
        }
    }

    #endregion // Methods

    #region IDisposable

    /// <summary>
    /// Dispose method
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal dispose method
    /// </summary>
    /// <param name="disposing">Dispose now?</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cts?.Cancel();

            _cts?.Dispose();

            _cts = null;
        }
    }

    #endregion // IDisposable
}