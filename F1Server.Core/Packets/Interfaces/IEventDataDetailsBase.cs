using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for event details in the event data packet
/// </summary>
public interface IEventDataDetailsBase
{
    #region Properties

    /// <summary>
    /// Type of event
    /// </summary>
    EventType EventType { get; }

    /// <summary>
    /// Car number
    /// </summary>
    ushort VehicleIndex { get; }

    /// <summary>
    /// Fastest lap in seconds
    /// </summary>
    float FastestLap { get; }

    #endregion // Properties
}