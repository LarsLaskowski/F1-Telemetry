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
    private readonly ConcurrentDictionary<Type, BaseProcessor> _processors;
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
        _processors = new ConcurrentDictionary<Type, BaseProcessor>();
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
                            PacketTypes.Session => GetOrCreateProcessor(() => new SessionProcessor(_serviceProvider, packetHeader, gameInfo)),
                            PacketTypes.Participants => GetOrCreateProcessor(() => new ParticipantsProcessor(_serviceProvider, packetHeader, gameInfo)),
                            PacketTypes.LapData => GetOrCreateProcessor(() => new LapDataProcessor(_serviceProvider, packetHeader, gameInfo)),
                            PacketTypes.SessionHistory => GetOrCreateProcessor(() => new SessionHistoryProcessor(_serviceProvider, packetHeader, gameInfo)),
                            PacketTypes.CarStatus => GetOrCreateProcessor(() => new CarStatusProcessor(_serviceProvider, packetHeader, gameInfo)),
                            PacketTypes.CarTelemetry => GetOrCreateProcessor(() => new CarTelemetryProcessor(_serviceProvider, packetHeader, gameInfo)),
                            PacketTypes.FinalClassification => GetOrCreateProcessor(() => new FinalClassificationProcessor(_serviceProvider, packetHeader, gameInfo)),
                            PacketTypes.TimeTrial => GetOrCreateProcessor(() => new TimeTrialProcessor(_serviceProvider, packetHeader, gameInfo)),
                            PacketTypes.LapPositions => GetOrCreateProcessor(() => new LapPositionsProcessor(_serviceProvider, packetHeader, gameInfo)),
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
    /// Returns the cached processor of the requested type or creates it once via <paramref name="processorFactory"/>.
    /// The processor cache is cleared whenever the session changes, so every session gets its own processor instances
    /// </summary>
    /// <typeparam name="TProcessor">Type of the processor</typeparam>
    /// <param name="processorFactory">Factory creating the processor when it is not cached yet</param>
    /// <returns>Processor</returns>
    private BaseProcessor GetOrCreateProcessor<TProcessor>(Func<TProcessor> processorFactory)
        where TProcessor : BaseProcessor
    {
        if (_processors.TryGetValue(typeof(TProcessor), out var processor) == false)
        {
            processor = _processors.GetOrAdd(typeof(TProcessor), processorFactory());
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