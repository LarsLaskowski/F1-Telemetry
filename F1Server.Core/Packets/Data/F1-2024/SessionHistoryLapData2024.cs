using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Lap history data from session (F1 2024)
/// </summary>
public class SessionHistoryLapData2024 : ILapHistoryDataBase, ILapHistoryData2024
{
    #region ILapHistoryDataBase

    /// <summary>
    /// Lap time in milliseconds
    /// </summary>
    public uint LapTime { get; set; }

    /// <summary>
    /// Sector 1 time in milliseconds
    /// </summary>
    public ushort Sector1Time { get; set; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    public ushort Sector2Time { get; set; }

    /// <summary>
    /// Sector 3 time in milliseconds
    /// </summary>
    public ushort Sector3Time { get; set; }

    /// <summary>
    /// Valid lap flag (0x01 - lap valid, 0x02 - sector 1 valid, 0x04 - sector 2 valid, 0x08 - sector 3 valid)
    /// </summary>
    public ushort LapValidFlag { get; set; }

    /// <summary>
    /// Is sector 1 time valid?
    /// </summary>
    public bool IsValidSector1 => (LapValidFlag & 0x02) == 0x02;

    /// <summary>
    /// Is sector 2 time valid?
    /// </summary>
    public bool IsValidSector2 => (LapValidFlag & 0x04) == 0x04;

    /// <summary>
    /// Is sector 3 time valid?
    /// </summary>
    public bool IsValidSector3 => (LapValidFlag & 0x08) == 0x08;

    /// <summary>
    /// Is lap time valid?
    /// </summary>
    public bool IsLapTimeValid => (LapValidFlag & 0x01) == 0x01;

    #endregion // ILapHistoryDataBase

    #region ILapHistoryData2024

    /// <summary>
    /// Sector 1 whole minute part
    /// </summary>
    public ushort Sector1TimeMinutes { get; set; }

    /// <summary>
    /// Sector 2 whole minute part
    /// </summary>
    public ushort Sector2TimeMinutes { get; set; }

    /// <summary>
    /// Sector 3 whole minute part
    /// </summary>
    public ushort Sector3TimeMinutes { get; set; }

    #endregion // ILapHistoryData2024
}