using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Lap history data from session (F1 2025)
/// </summary>
public class SessionHistoryLapData2025 : ILapHistoryDataBase, ILapHistoryData2025
{
    #region ILapHistoryDataBase

    /// <inheritdoc/>
    public uint LapTime { get; set; }

    /// <inheritdoc/>
    public ushort Sector1Time { get; set; }

    /// <inheritdoc/>
    public ushort Sector2Time { get; set; }

    /// <inheritdoc/>
    public ushort Sector3Time { get; set; }

    /// <inheritdoc/>
    public ushort LapValidFlag { get; set; }

    /// <inheritdoc/>
    public bool IsValidSector1 => (LapValidFlag & 0x02) == 0x02;

    /// <inheritdoc/>
    public bool IsValidSector2 => (LapValidFlag & 0x04) == 0x04;

    /// <inheritdoc/>
    public bool IsValidSector3 => (LapValidFlag & 0x08) == 0x08;

    /// <inheritdoc/>
    public bool IsLapTimeValid => (LapValidFlag & 0x01) == 0x01;

    #endregion // ILapHistoryDataBase

    #region ILapHistoryData2025

    /// <inheritdoc/>
    public ushort Sector1TimeMinutes { get; set; }

    /// <inheritdoc/>
    public ushort Sector2TimeMinutes { get; set; }

    /// <inheritdoc/>
    public ushort Sector3TimeMinutes { get; set; }

    #endregion // ILapHistoryData2025
}