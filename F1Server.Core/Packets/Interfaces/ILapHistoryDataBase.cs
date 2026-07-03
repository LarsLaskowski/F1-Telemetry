namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for lap history data
/// </summary>
public interface ILapHistoryDataBase
{
    #region Properties

    /// <summary>
    /// Lap time in milliseconds
    /// </summary>
    uint LapTime { get; set; }

    /// <summary>
    /// Sector 1 time in milliseconds
    /// </summary>
    ushort Sector1Time { get; set; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    ushort Sector2Time { get; set; }

    /// <summary>
    /// Sector 3 time in milliseconds
    /// </summary>
    ushort Sector3Time { get; set; }

    /// <summary>
    /// Valid lap flag (0x01 - lap valid, 0x02 - sector 1 valid, 0x04 - sector 2 valid, 0x08 - sector 3 valid)
    /// </summary>
    ushort LapValidFlag { get; set; }

    /// <summary>
    /// Is sector 1 time valid?
    /// </summary>
    bool IsValidSector1 { get; }

    /// <summary>
    /// Is sector 2 time valid?
    /// </summary>
    bool IsValidSector2 { get; }

    /// <summary>
    /// Is sector 3 time valid?
    /// </summary>
    bool IsValidSector3 { get; }

    /// <summary>
    /// Is lap time valid?
    /// </summary>
    bool IsLapTimeValid { get; }

    /// <summary>
    /// All sectors and lap time valid?
    /// </summary>
    public bool IsLapTimeCompleteValid => IsValidSector1 && IsValidSector2 && IsValidSector3 && IsLapTimeValid;

    #endregion // Properties
}