namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for one car session history data
/// </summary>
public interface ISessionHistoryDataBase
{
    #region Properties

    /// <summary>
    /// Index of car
    /// </summary>
    ushort CarIndex { get; set; }

    /// <summary>
    /// Number of laps in the data (including partial data of current lap)
    /// </summary>
    ushort NumberOfLaps { get; set; }

    /// <summary>
    /// Number of tyre stints in the data
    /// </summary>
    ushort NumberOfTyreStints { get; set; }

    /// <summary>
    /// Number of best lap time achieved on
    /// </summary>
    ushort BestLapNumber { get; set; }

    /// <summary>
    /// Number of lap with best sector 1 time
    /// </summary>
    ushort BestSector1LapNumber { get; set; }

    /// <summary>
    /// Number of lap with best sector 2 time
    /// </summary>
    ushort BestSector2LapNumber { get; set; }

    /// <summary>
    /// Number of lap with best sector 3 time
    /// </summary>
    ushort BestSector3LapNumber { get; set; }

    /// <summary>
    /// Array (max 100) lap data
    /// </summary>
    ILapHistoryDataBase[] LapHistory { get; set; }

    /// <summary>
    /// History data of tyre stints
    /// </summary>
    ITyreStintHistoryDataBase[] TyreStintHistory { get; set; }

    #endregion // Properties
}