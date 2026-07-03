using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Basic event data details implementation
/// </summary>
internal class EventDataDetails2019 : IEventDataDetailsBase
{
    #region IEventDataDetailsBase

    /// <summary>
    /// Type of event
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// Car number
    /// </summary>
    public ushort VehicleIndex { get; set; }

    /// <summary>
    /// Fastest lap in seconds
    /// </summary>
    public float FastestLap { get; set; }

    #endregion // IEventDataDetailsBase
}