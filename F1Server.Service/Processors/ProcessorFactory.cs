using System.Collections.Concurrent;

using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Data;
using F1Server.Data;

namespace F1Server.Service.Processors;

/// <summary>
/// Factory for creating processors
/// </summary>
public sealed class ProcessorFactory : IDisposable
{
    #region Fields

    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, object> _processors;
    private ulong _currentSessionId;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    public ProcessorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _processors = new ConcurrentDictionary<Type, object>();
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Should the frame identifier reset, because of flashback
    /// </summary>
    public bool IsResetFrameIdentifier { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Create a processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    public BaseProcessor? GetProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (packetHeader != null)
        {
            // Every session needs separate processors, because the header information are saved into processor object
            if (packetHeader.UniqueSessionId != _currentSessionId)
            {
                RemoveAllProcessors();

                _currentSessionId = packetHeader.UniqueSessionId;
            }

            processor = packetHeader.PacketType switch
                        {
                            PacketTypes.Session => GetSessionProcessor(packetHeader, gameInfo),
                            PacketTypes.Participants => GetParticipantsProcessor(packetHeader, gameInfo),
                            PacketTypes.LapData => GetLapDataProcessor(packetHeader, gameInfo),
                            PacketTypes.SessionHistory => GetSessionHistoryProcessor(packetHeader, gameInfo),
                            PacketTypes.CarStatus => GetCarStatusProcessor(packetHeader, gameInfo),
                            PacketTypes.CarTelemetry => GetCarTelemetryProcessor(packetHeader, gameInfo),
                            PacketTypes.FinalClassification => GetFinalClassificationProcessor(packetHeader, gameInfo),
                            PacketTypes.TimeTrial => GetTimeTrialProcessor(packetHeader, gameInfo),
                            PacketTypes.LapPositions => GetLapPositionsProcessor(packetHeader, gameInfo),
                            _ => null
                        };

            if (processor != null)
            {
                if (packetHeader.FrameIdentifier > processor.CurrentFrameIdentifier || IsResetFrameIdentifier)
                {
                    processor.LastFrameIdentifier = processor.CurrentFrameIdentifier;

                    if (IsResetFrameIdentifier)
                    {
                        processor.LastFrameIdentifier = packetHeader.FrameIdentifier - 1;

                        IsResetFrameIdentifier = false;
                    }
                }

                processor.CurrentFrameIdentifier = packetHeader.FrameIdentifier;
                processor.SessionTimestampNum = packetHeader.SessionTimeNum;
            }
        }

        return processor;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Create a session processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetSessionProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(SessionProcessor)))
        {
            if (_processors.TryGetValue(typeof(SessionProcessor), out var proc) && proc is SessionProcessor sessionProc)
            {
                processor = sessionProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(SessionProcessor), _serviceProvider, packetHeader, gameInfo) is SessionProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(SessionProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Create a participants processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetParticipantsProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(ParticipantsProcessor)))
        {
            if (_processors.TryGetValue(typeof(ParticipantsProcessor), out var proc) && proc is ParticipantsProcessor participantsProc)
            {
                processor = participantsProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(ParticipantsProcessor), _serviceProvider, packetHeader, gameInfo) is ParticipantsProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(ParticipantsProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Create a participants processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetLapDataProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(LapDataProcessor)))
        {
            if (_processors.TryGetValue(typeof(LapDataProcessor), out var proc) && proc is LapDataProcessor lapDataProc)
            {
                processor = lapDataProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(LapDataProcessor), _serviceProvider, packetHeader, gameInfo) is LapDataProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(LapDataProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Create a session history processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetSessionHistoryProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(SessionHistoryProcessor)))
        {
            if (_processors.TryGetValue(typeof(SessionHistoryProcessor), out var proc) && proc is SessionHistoryProcessor sessionHistoryProc)
            {
                processor = sessionHistoryProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(SessionHistoryProcessor), _serviceProvider, packetHeader, gameInfo) is SessionHistoryProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(SessionHistoryProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Create a car status processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetCarStatusProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(CarStatusProcessor)))
        {
            if (_processors.TryGetValue(typeof(CarStatusProcessor), out var proc) && proc is CarStatusProcessor carStatusProc)
            {
                processor = carStatusProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(CarStatusProcessor), _serviceProvider, packetHeader, gameInfo) is CarStatusProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(CarStatusProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Create a car telemetry processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetCarTelemetryProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(CarTelemetryProcessor)))
        {
            if (_processors.TryGetValue(typeof(CarTelemetryProcessor), out var proc) && proc is CarTelemetryProcessor carTeleProc)
            {
                processor = carTeleProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(CarTelemetryProcessor), _serviceProvider, packetHeader, gameInfo) is CarTelemetryProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(CarTelemetryProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Create a final classification processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetFinalClassificationProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(FinalClassificationProcessor)))
        {
            if (_processors.TryGetValue(typeof(FinalClassificationProcessor), out var proc) && proc is FinalClassificationProcessor finalClassificationProc)
            {
                processor = finalClassificationProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(FinalClassificationProcessor), _serviceProvider, packetHeader, gameInfo) is FinalClassificationProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(FinalClassificationProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Create a time trial processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetTimeTrialProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(TimeTrialProcessor)))
        {
            if (_processors.TryGetValue(typeof(TimeTrialProcessor), out var proc) && proc is TimeTrialProcessor timeTrialProc)
            {
                processor = timeTrialProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(TimeTrialProcessor), _serviceProvider, packetHeader, gameInfo) is TimeTrialProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(TimeTrialProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Create a lap positions processor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="gameInfo">Runtime game information</param>
    /// <returns>Processor</returns>
    private BaseProcessor? GetLapPositionsProcessor(PacketHeader packetHeader, LiveGameData gameInfo)
    {
        BaseProcessor? processor = null;

        if (_processors.ContainsKey(typeof(LapPositionsProcessor)))
        {
            if (_processors.TryGetValue(typeof(LapPositionsProcessor), out var proc) && proc is LapPositionsProcessor lapPositionsProc)
            {
                processor = lapPositionsProc;
            }
        }
        else
        {
            if (Activator.CreateInstance(typeof(LapPositionsProcessor), _serviceProvider, packetHeader, gameInfo) is LapPositionsProcessor proc)
            {
                processor = proc;

                _processors.TryAdd(typeof(LapPositionsProcessor), proc);
            }
        }

        return processor;
    }

    /// <summary>
    /// Remove all processors
    /// </summary>
    private void RemoveAllProcessors()
    {
        // Remove all processors, because the session id was changed?
        _processors?.Clear();
    }

    #endregion // Private methods

    #region IDisposable

    /// <summary>
    /// Dispose method
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Internal dispose method
    /// </summary>
    /// <param name="disposing">Dispose?</param>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            RemoveAllProcessors();
        }
    }

    #endregion // IDisposable
}