using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Data class with final classification data of all cars in one session
/// </summary>
public class FinalClassificationData : PacketDataBase<IFinalClassificationData>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="finalClassificationData">Final classification data</param>
    public FinalClassificationData(PacketHeader packetHeader, IFinalClassificationData finalClassificationData)
        : base(packetHeader, finalClassificationData)
    {
    }

    #endregion // Constructors
}