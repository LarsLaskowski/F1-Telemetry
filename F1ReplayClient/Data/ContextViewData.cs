using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

using F1ReplayClient.Data.Base;

using F1Server.Core.Enumerations;

namespace F1ReplayClient.Data;

/// <summary>
/// Data context class for MainWindow.xaml
/// </summary>
internal class ContextViewData : NotifyPropertyBase, IDataErrorInfo
{
    #region Fields

    private bool _isStartable;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public ContextViewData()
    {
        TrackName = string.Empty;
        SessionType = string.Empty;
        FormulaType = string.Empty;
        Weather = string.Empty;
        Host = string.Empty;
        FilesFolder = string.Empty;
        Status = string.Empty;
        TimeEstimated = string.Empty;
        TimeDuration = string.Empty;
        RunFirstVisibility = Visibility.Collapsed;
        IsRunning = false;
        IsStartable = false;
        IsSortable = false;
        SendData = true;

        PacketViewTypes = new();

        EventCodes = new ObservableCollection<string>();
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Valid server?
    /// </summary>
    public bool IsValidHost { get; private set; }

    /// <summary>
    /// Current track name
    /// </summary>
    public string TrackName
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(TrackName));
        }
    }

    /// <summary>
    /// Type of session
    /// </summary>
    public string SessionType
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(SessionType));
        }
    }

    /// <summary>
    /// Typ of formula
    /// </summary>
    public string FormulaType
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(FormulaType));
        }
    }

    /// <summary>
    /// Total laps
    /// </summary>
    public int TotalLaps
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(TotalLaps));
        }
    }

    /// <summary>
    /// Session id
    /// </summary>
    public ulong SessionId
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(SessionId));
        }
    }

    /// <summary>
    /// Weather condition
    /// </summary>
    public string Weather
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(Weather));
        }
    }

    /// <summary>
    /// AI difficulty
    /// </summary>
    public short AiDifficulty
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(AiDifficulty));
        }
    }

    /// <summary>
    /// Number of each packet type
    /// </summary>
    public PacketViewTypes PacketViewTypes
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(PacketViewTypes));
        }
    }

    /// <summary>
    /// Hostname with port
    /// </summary>
    public string Host
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(Host));
        }
    }

    /// <summary>
    /// Data folder
    /// </summary>
    public string FilesFolder
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(FilesFolder));
        }
    }

    /// <summary>
    /// Status output
    /// </summary>
    public string Status
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(Status));
        }
    }

    /// <summary>
    /// Current number of processed files
    /// </summary>
    public int ProcessedFiles
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(ProcessedFiles));
        }
    }

    /// <summary>
    /// Total number of files
    /// </summary>
    public int TotalFiles
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(TotalFiles));
        }
    }

    /// <summary>
    /// Fields are modifyable?
    /// </summary>
    public bool IsEditable
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(IsEditable));
        }
    }

    /// <summary>
    /// Start available?
    /// </summary>
    public bool IsStartable
    {
        get => _isStartable && IsRunning == false;
        set
        {
            _isStartable = value;

            RaisePropertyChange(nameof(IsStartable));
        }
    }

    /// <summary>
    /// Order packets available?
    /// </summary>
    public bool IsSortable
    {
        get => _isStartable && IsRunning == false && field;
        set
        {
            field = value;

            RaisePropertyChange(nameof(IsSortable));
        }
    }

    /// <summary>
    /// Running?
    /// </summary>
    public bool IsRunning
    {
        get;
        set
        {
            if (value && RunFirstVisibility == Visibility.Collapsed)
            {
                RunFirstVisibility = Visibility.Visible;
            }

            if (value && field == false)
            {
                PacketViewTypes = new PacketViewTypes();
            }

            field = value;

            RaisePropertyChange(nameof(IsRunning));
            RaisePropertyChange(nameof(IsStartable));
            RaisePropertyChange(nameof(IsSortable));
            RaisePropertyChange(nameof(RunFirstVisibility));
        }
    }

    /// <summary>
    /// Visibility of output
    /// </summary>
    public Visibility RunFirstVisibility { get; private set; }

    /// <summary>
    /// Game version
    /// </summary>
    public int GameVersion
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(GameVersion));
        }
    }

    /// <summary>
    /// Send anything
    /// </summary>
    public bool SendData
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(SendData));
        }
    }

    /// <summary>
    /// Ignore car packets (Motion, CarDamage, CarStatus, CarTelemetry, CarSetups)
    /// </summary>
    public bool IgnoreCarPackets
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(IgnoreCarPackets));
        }
    }

    /// <summary>
    /// Break after sending session packet
    /// </summary>
    public bool BreakAtSessionPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtSessionPacket));
        }
    }

    /// <summary>
    /// Break after sending participants packet
    /// </summary>
    public bool BreakAtParticipantsPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtParticipantsPacket));
        }
    }

    /// <summary>
    /// Break after sending lap data packet
    /// </summary>
    public bool BreakAtLapDataPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtLapDataPacket));
        }
    }

    /// <summary>
    /// Break after sending final classification packet
    /// </summary>
    public bool BreakAtFinalClassificationPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtFinalClassificationPacket));
        }
    }

    /// <summary>
    /// Break after sending time trial packet
    /// </summary>
    public bool BreakAtTimeTrialPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtTimeTrialPacket));
        }
    }

    /// <summary>
    /// Break after sending time trial packet
    /// </summary>
    public bool BreakAtLapPositionsPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtLapPositionsPacket));
        }
    }

    /// <summary>
    /// Break after sending session history packet
    /// </summary>
    public bool BreakAtSessionHistoryPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtSessionHistoryPacket));
        }
    }

    /// <summary>
    /// Break after sending car telemetry packet
    /// </summary>
    public bool BreakAtCarTelemetryPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtCarTelemetryPacket));
        }
    }

    /// <summary>
    /// Break after sending car telemetry 2 packet
    /// </summary>
    public bool BreakAtCarTelemetry2Packet
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtCarTelemetry2Packet));
        }
    }

    /// <summary>
    /// Break after sending car status packet
    /// </summary>
    public bool BreakAtCarStatusPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtCarStatusPacket));
        }
    }

    /// <summary>
    /// Current packet type
    /// </summary>
    public PacketTypes CurrentPacketType
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(CurrentPacketType));
        }
    }

    /// <summary>
    /// Processing packets paused?
    /// </summary>
    public bool IsPaused
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(IsPaused));
        }
    }

    /// <summary>
    /// Break after sending event packet
    /// </summary>
    public bool BreakAtEventPacket
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(BreakAtEventPacket));
        }
    }

    /// <summary>
    /// Event codes in session
    /// </summary>
    public ObservableCollection<string> EventCodes
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(EventCodes));
        }
    }

    /// <summary>
    /// Estimated processing time of all packets in current session directory
    /// </summary>
    public string TimeEstimated
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(TimeEstimated));
        }
    }

    /// <summary>
    /// Current processing time
    /// </summary>
    public string TimeDuration
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(TimeDuration));
        }
    }

    #endregion // Properties

    #region IDataErrorInfo

    /// <summary>
    /// Returns error text
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Check content
    /// </summary>
    /// <param name="columnName">Name of field</param>
    /// <returns>Error</returns>
    public string this[string columnName]
    {
        get
        {
            return CheckFieldContent(columnName);
        }
    }

    /// <summary>
    /// Check content of fields
    /// </summary>
    /// <param name="fieldName">Field name</param>
    /// <returns>Error text</returns>
    private string CheckFieldContent(string fieldName)
    {
        var strError = string.Empty;

        switch (fieldName)
        {
            case nameof(Host):
                {
                    strError = CheckHostName();
                }
                break;
        }

        return strError;
    }

    /// <summary>
    /// Check the hostname
    /// </summary>
    /// <returns>Error text</returns>
    private string CheckHostName()
    {
        var strError = string.Empty;

        if (string.IsNullOrWhiteSpace(Host) == false)
        {
            if (Host.Contains(':'))
            {
                var split = Host.Split(':');

                if (split.Length == 2)
                {
                    if (int.TryParse(split[1], out var port) && port > 0)
                    {
                        IsValidHost = true;
                    }
                    else
                    {
                        strError = "Invalid port";
                    }
                }
                else
                {
                    strError = "Less or more colons";
                }
            }
            else
            {
                strError = "No port";
            }
        }
        else
        {
            strError = "No host";

            IsValidHost = false;
        }

        return strError;
    }

    #endregion // IDataErrorInfo
}