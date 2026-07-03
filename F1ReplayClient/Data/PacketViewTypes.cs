using F1ReplayClient.Data.Base;

namespace F1ReplayClient.Data;

/// <summary>
/// Packet view types
/// </summary>
internal class PacketViewTypes : NotifyPropertyBase
{
    #region Properties

    /// <summary>
    /// Unknown packet type
    /// </summary>
    public int Unknown
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(Unknown));
        }
    }

    /// <summary>
    /// Motion packet
    /// </summary>
    public int Motion
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(Motion));
        }
    }

    /// <summary>
    /// Session packet
    /// </summary>
    public int Session
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(Session));
        }
    }

    /// <summary>
    /// Lap data packet
    /// </summary>
    public int LapData
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(LapData));
        }
    }

    /// <summary>
    /// Event packet
    /// </summary>
    public int Event
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(Event));
        }
    }

    /// <summary>
    /// Participants packet
    /// </summary>
    public int Participants
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(Participants));
        }
    }

    /// <summary>
    /// Car setups packet
    /// </summary>
    public int CarSetups
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(CarSetups));
        }
    }

    /// <summary>
    /// Car telemetry packet
    /// </summary>
    public int CarTelemetry
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(CarTelemetry));
        }
    }

    /// <summary>
    /// Car telemetry 2 packet
    /// </summary>
    public int CarTelemetry2
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(CarTelemetry2));
        }
    }

    /// <summary>
    /// Car status packet
    /// </summary>
    public int CarStatus
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(CarStatus));
        }
    }

    /// <summary>
    /// Final classification packet
    /// </summary>
    public int FinalClassification
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(FinalClassification));
        }
    }

    /// <summary>
    /// Lobby info packet
    /// </summary>
    public int LobbyInfo
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(LobbyInfo));
        }
    }

    /// <summary>
    /// Car damage packet
    /// </summary>
    public int CarDamage
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(CarDamage));
        }
    }

    /// <summary>
    /// Session history packet
    /// </summary>
    public int SessionHistory
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(SessionHistory));
        }
    }

    /// <summary>
    /// Tyre sets packet
    /// </summary>
    public int TyreSets
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(TyreSets));
        }
    }

    /// <summary>
    /// Motion extended packet
    /// </summary>
    public int MotionEx
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(MotionEx));
        }
    }

    /// <summary>
    /// Time trial packet
    /// </summary>
    public int TimeTrial
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(TimeTrial));
        }
    }

    /// <summary>
    /// Lap positions packet
    /// </summary>
    public int LapPositions
    {
        get;
        set
        {
            field = value;

            RaisePropertyChange(nameof(LapPositions));
        }
    }

    #endregion // Properties
}