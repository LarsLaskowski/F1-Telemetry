using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Basic event data details implementation
/// </summary>
internal class EventDataDetails2019 : IEventDataDetailsBase
{
    #region IEventDataDetailsBase

    /// <inheritdoc/>
    public EventType EventType { get; set; }

    /// <inheritdoc/>
    public ushort VehicleIndex { get; set; }

    /// <inheritdoc/>
    public float FastestLap { get; set; }

    #endregion // IEventDataDetailsBase
}