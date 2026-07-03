using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Participants data for active session
/// </summary>
public class Participants : PacketDataBase<IParticipantsBase>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="packetData">Packet data</param>
    public Participants(PacketHeader packetHeader, IParticipantsBase packetData)
        : base(packetHeader, packetData)
    {
    }

    #endregion // Constructors
}