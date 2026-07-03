using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Data class with lap positions
/// </summary>
public class LapPositions : PacketDataBase<ILapPositionsBase>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="lapPositions">Lap positions</param>
    public LapPositions(PacketHeader packetHeader, ILapPositionsBase lapPositions)
        : base(packetHeader, lapPositions)
    {
    }

    #endregion // Constructors
}