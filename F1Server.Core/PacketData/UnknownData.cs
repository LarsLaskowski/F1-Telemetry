using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Data class for unknown or not implemented packets
/// </summary>
public class UnknownData : PacketDataBase<IUnknownData>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    public UnknownData(PacketHeader packetHeader)
        : base(packetHeader, null)
    {
    }

    #endregion // Constructors
}