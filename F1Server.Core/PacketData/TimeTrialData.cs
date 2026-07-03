using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Data class with time trial data
/// </summary>
public class TimeTrialData : PacketDataBase<ITimeTrialDataBase>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="timeTrialData">Time trial data</param>
    public TimeTrialData(PacketHeader packetHeader, ITimeTrialDataBase timeTrialData)
        : base(packetHeader, timeTrialData)
    {
    }

    #endregion // Constructors
}