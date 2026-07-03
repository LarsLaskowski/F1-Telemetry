namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for tyre history
/// </summary>
public interface ITyreStintHistoryDataBase
{
    #region Properties

    /// <summary>
    /// Lap the tyre usage ends on (255 of current tyre)
    /// </summary>
    ushort EndLap { get; set; }

    /// <summary>
    /// Actual tyres used by this driver
    /// </summary>
    ushort TyreActualCompound { get; set; }

    /// <summary>
    /// Visual tyres used by this driver
    /// </summary>
    ushort TyreVisualCompound { get; set; }

    #endregion // Properties
}