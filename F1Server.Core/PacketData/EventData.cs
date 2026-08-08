using F1Server.Core.Data;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Event data class
/// </summary>
public class EventData : PacketDataBase<IEventDataBase>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="eventData">Event data</param>
    public EventData(PacketHeader packetHeader, IEventDataBase eventData)
        : base(packetHeader, eventData)
    {
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Event code
    /// </summary>
    public string EventCode => PacketData is not null ? PacketData.EventCode : string.Empty;

    /// <summary>
    /// Is event start?
    /// </summary>
    public bool IsSessionStart => PacketData is not null && string.IsNullOrWhiteSpace(PacketData.EventCode) == false && PacketData.EventCode.Equals(EventCodes.SessionStart, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Is event end?
    /// </summary>
    public bool IsSessionEnd => PacketData is not null && string.IsNullOrWhiteSpace(PacketData.EventCode) == false && PacketData.EventCode.Equals(EventCodes.SessionEnd, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Is flashback event?
    /// </summary>
    public bool IsFlashback => PacketData is not null && string.IsNullOrWhiteSpace(PacketData.EventCode) == false && PacketData.EventCode.Equals(EventCodes.Flashback, StringComparison.OrdinalIgnoreCase);

    #endregion // Properties
}