using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Car status data
/// </summary>
public class CarStatus : PacketDataBase<ICarStatusBase>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="statusData">Car status data</param>
    public CarStatus(PacketHeader packetHeader, ICarStatusBase statusData)
        : base(packetHeader, statusData)
    {
    }

    #endregion // Constructors
}