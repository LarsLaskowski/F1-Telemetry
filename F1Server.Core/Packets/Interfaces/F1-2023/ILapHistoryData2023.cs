namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for lap history data in F1 2023
/// </summary>
public interface ILapHistoryData2023
{
    #region Properties

    /// <summary>
    /// Sector 1 whole minute part
    /// </summary>
    ushort Sector1TimeMinutes { get; set; }

    /// <summary>
    /// Sector 2 whole minute part
    /// </summary>
    ushort Sector2TimeMinutes { get; set; }

    /// <summary>
    /// Sector 3 whole minute part
    /// </summary>
    ushort Sector3TimeMinutes { get; set; }

    #endregion // Properties
}