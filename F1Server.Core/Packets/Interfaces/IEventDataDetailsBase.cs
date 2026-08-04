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
    /// Zero based index of the car in the per-car arrays of the packet. This is the array slot,
    /// not the race number shown on the car
    /// </summary>
    ushort VehicleIndex { get; }

    /// <summary>
    /// Fastest lap in seconds
    /// </summary>
    float FastestLap { get; }

    #endregion // Properties
}