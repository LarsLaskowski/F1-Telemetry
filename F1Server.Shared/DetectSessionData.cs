using System.Buffers;

using F1Server.Core;
using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.PacketData;
using F1Server.Shared.Data;

namespace F1Server.Shared;

/// <summary>
/// Class to detect session information from F1 packet files
/// </summary>
public class DetectSessionData
{
    #region Fields

    private readonly PacketAnalyzer _packetAnalyzer = new();
    private readonly List<string> _files;
    private bool _hasSessionInfo;

    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DetectSessionData"/> class
    /// </summary>
    /// <param name="files">List of files</param>
    /// <param name="gameVersion">Game version</param>
    public DetectSessionData(List<string> files, int gameVersion)
    {
        _files = files;
        GameVersion = gameVersion;
    }

    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Session data
    /// </summary>
    public SessionDataInfo? SessionData { get; private set; }

    /// <summary>
    /// Game version
    /// </summary>
    public int GameVersion { get; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Detects whether a session is present based on the provided files
    /// </summary>
    /// <returns><see langword="true"/> if session information is detected; otherwise, <see langword="false"/></returns>
    public bool DetectSession()
    {
        var isSessionDetected = false;

        if (_files.Count > 0)
        {
            var options = new ParallelOptions()
                          {
                              MaxDegreeOfParallelism = Environment.ProcessorCount - 1
                          };

            Parallel.ForEach(_files,
                             options,
                             (file, state) =>
                             {
                                 CheckPossibleSessionPacket(file);

                                 if (_hasSessionInfo)
                                 {
                                     isSessionDetected = true;

                                     state.Break();
                                 }
                             });
        }

        return isSessionDetected;
    }

    /// <summary>
    /// Check wether this packet is a session packet
    /// </summary>
    /// <param name="file">File name</param>
    private void CheckPossibleSessionPacket(string file)
    {
        var fInfo = new FileInfo(file);

        if (fInfo.Length > 30 && _hasSessionInfo == false && IsSessionFileSize(fInfo.Length))
        {
            _hasSessionInfo = AnalyzePossibleSessionPacket(file, out var sessionData);

            if (_hasSessionInfo)
            {
                SessionData = sessionData;
            }
        }
    }

    /// <summary>
    /// Check file size
    /// </summary>
    /// <param name="fileSize">File size</param>
    /// <returns>Is a possible session file?</returns>
    private bool IsSessionFileSize(long fileSize)
    {
        return GameVersion switch
               {
                   2018 => fileSize == 147,
                   2019 => fileSize == ConstData.F12019SessionSize + ConstData.F12019HeaderSize,
                   2020 => fileSize == ConstData.F12020SessionSize + ConstData.F12019HeaderSize,
                   2021 => fileSize == ConstData.F12021SessionSize + ConstData.F12019HeaderSize,
                   2022 => fileSize == ConstData.F12022SessionSize + ConstData.F12019HeaderSize,
                   2023 => fileSize == ConstData.F12023SessionSize + ConstData.F12023HeaderSize,
                   2024 => fileSize == ConstData.F12024SessionSize + ConstData.F12024HeaderSize,
                   2025 => fileSize == ConstData.F12025SessionSize + ConstData.F12025HeaderSize,
                   2026 => fileSize == ConstData.F12026SessionSize + ConstData.F12026HeaderSize,
                   _ => false
               };
    }

    /// <summary>
    /// Check a possible session packet
    /// </summary>
    /// <param name="fileName">Name of file</param>
    /// <param name="sessionDataContent">Session data</param>
    /// <returns>Valid session packet</returns>
    private bool AnalyzePossibleSessionPacket(string fileName, out SessionDataInfo sessionDataContent)
    {
        var isSessionFile = false;

        sessionDataContent = new SessionDataInfo();

        using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            var buffer = ArrayPool<byte>.Shared.Rent((int)new FileInfo(fileName).Length);

            try
            {
                var bytesRead = fs.Read(buffer, 0, buffer.Length);

                if (buffer.Length > 0
                    && bytesRead > 0)
                {
                    var packetData = new ReceivedPacketData();

                    packetData.SetRawData(buffer);

                    if (packetData.PacketHeader?.PacketType == PacketTypes.Session)
                    {
                        var packetContent = _packetAnalyzer.GetSessionData(packetData.PacketHeader, packetData.PacketRawData);

                        if (packetContent is SessionData sessionData && sessionData.PacketData != null)
                        {
                            sessionDataContent.TrackName = sessionData.PacketData.TrackName;
                            sessionDataContent.SessionType = sessionData.PacketData.SessionType;
                            sessionDataContent.FormulaType = sessionData.PacketData.FormulaType;
                            sessionDataContent.TotalLaps = sessionData.PacketData.TotalLaps;
                            sessionDataContent.SessionId = sessionData.PacketHeader.UniqueSessionId;
                            sessionDataContent.Weather = sessionData.PacketData.Weather;

                            isSessionFile = true;
                        }
                    }
                }
            }
            catch
            {
                // Ignore any exceptions
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        return isSessionFile;
    }

    #endregion // Methods
}