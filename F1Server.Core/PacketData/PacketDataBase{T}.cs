using F1Server.Core.Packets.Data;

namespace F1Server.Core.PacketData;

/// <summary>
/// Base class for every packet data object
/// </summary>
/// <typeparam name="T">Type based of packet type</typeparam>
public abstract class PacketDataBase<T>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="packetData">Packet data</param>
    protected PacketDataBase(PacketHeader packetHeader, T? packetData)
    {
        PacketHeader = packetHeader;
        PacketData = packetData;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Packet header
    /// </summary>
    public PacketHeader PacketHeader { get; private set; }

    /// <summary>
    /// Data of packet
    /// </summary>
    public T? PacketData { get; private set; }

    #endregion // Properties
}