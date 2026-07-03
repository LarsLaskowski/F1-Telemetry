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
    public string EventCode => PacketData != null ? PacketData.EventCode : string.Empty;

    /// <summary>
    /// Is event start?
    /// </summary>
    public bool IsSessionStart => PacketData != null && string.IsNullOrWhiteSpace(PacketData.EventCode) == false && PacketData.EventCode.Equals("SSTA", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Is event end?
    /// </summary>
    public bool IsSessionEnd => PacketData != null && string.IsNullOrWhiteSpace(PacketData.EventCode) == false && PacketData.EventCode.Equals("SEND", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Is flashback event?
    /// </summary>
    public bool IsFlashback => PacketData != null && string.IsNullOrWhiteSpace(PacketData.EventCode) == false && PacketData.EventCode.Equals("FLBK", StringComparison.OrdinalIgnoreCase);

    #endregion // Properties
}