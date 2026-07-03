using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Data class with lap data of all cars
/// </summary>
public class LapData : PacketDataBase<ILapDataComplete>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="lapData">Lap data</param>
    public LapData(PacketHeader packetHeader, ILapDataComplete lapData)
        : base(packetHeader, lapData)
    {
    }

    #endregion // Constructors
}