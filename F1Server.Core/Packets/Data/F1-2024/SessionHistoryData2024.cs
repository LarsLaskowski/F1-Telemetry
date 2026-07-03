using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session history lap data (F1 2024)
/// </summary>
public class SessionHistoryData2024 : ISessionHistoryDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionHistoryData2024()
    {
        LapHistory = new SessionHistoryLapData2024[100];
        TyreStintHistory = new SessionHistoryTyreStintData2024[8];
    }

    #endregion // Constructors

    #region ISessionHistoryDataBase

    /// <summary>
    /// Index of car
    /// </summary>
    public ushort CarIndex { get; set; }

    /// <summary>
    /// Number of laps in the data (including partial data of current lap)
    /// </summary>
    public ushort NumberOfLaps { get; set; }

    /// <summary>
    /// Number of tyre stints in the data
    /// </summary>
    public ushort NumberOfTyreStints { get; set; }

    /// <summary>
    /// Number of best lap time achieved on
    /// </summary>
    public ushort BestLapNumber { get; set; }

    /// <summary>
    /// Number of lap with best sector 1 time
    /// </summary>
    public ushort BestSector1LapNumber { get; set; }

    /// <summary>
    /// Number if lap with best sector 2 time
    /// </summary>
    public ushort BestSector2LapNumber { get; set; }

    /// <summary>
    /// Number of lap with best sector 3 time
    /// </summary>
    public ushort BestSector3LapNumber { get; set; }

    /// <summary>
    /// Array (max 100) lap data
    /// </summary>
    public ILapHistoryDataBase[] LapHistory { get; set; }

    /// <summary>
    /// History data of tyre stints
    /// </summary>
    public ITyreStintHistoryDataBase[] TyreStintHistory { get; set; }

    #endregion // ISessionHistoryDataBase
}