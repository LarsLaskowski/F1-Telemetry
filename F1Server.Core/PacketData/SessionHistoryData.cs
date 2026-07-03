using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Data class with session history data
/// </summary>
public class SessionHistoryData : PacketDataBase<ISessionHistoryDataBase>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Packet header</param>
    /// <param name="sessionHistoryData">Session history data</param>
    public SessionHistoryData(PacketHeader packetHeader, ISessionHistoryDataBase sessionHistoryData)
        : base(packetHeader, sessionHistoryData)
    {
    }

    #endregion // Constructors
}