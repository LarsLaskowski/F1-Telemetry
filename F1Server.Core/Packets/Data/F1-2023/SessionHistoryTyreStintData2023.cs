using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Tyre stint history data of session (F1 2023)
/// </summary>
internal class SessionHistoryTyreStintData2023 : ITyreStintHistoryDataBase
{
    #region ITyreStintHistoryDataBase

    /// <summary>
    /// Lap the tyre usage ends on (255 of current tyre)
    /// </summary>
    public ushort EndLap { get; set; }

    /// <summary>
    /// Actual tyres used by this driver
    /// </summary>
    public ushort TyreActualCompound { get; set; }

    /// <summary>
    /// Visual tyres used by this driver
    /// </summary>
    public ushort TyreVisualCompound { get; set; }

    #endregion // ITyreStintHistoryDataBase
}