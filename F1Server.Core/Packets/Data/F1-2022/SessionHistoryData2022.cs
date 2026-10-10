using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session history lap data (F1 2022)
/// </summary>
public class SessionHistoryData2022 : ISessionHistoryDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionHistoryData2022()
    {
        LapHistory = new SessionHistoryLapData2022[100];
        TyreStintHistory = new SessionHistoryTyreStintData2022[8];

        for (int lapHistory = 0; lapHistory < LapHistory.Length; ++lapHistory)
        {
            LapHistory[lapHistory] = new SessionHistoryLapData2022();
        }

        for (int tyreStintHistory = 0; tyreStintHistory < TyreStintHistory.Length; ++tyreStintHistory)
        {
            TyreStintHistory[tyreStintHistory] = new SessionHistoryTyreStintData2022();
        }
    }

    #endregion // Constructors

    #region ISessionHistoryDataBase

    /// <inheritdoc/>
    public ushort CarIndex { get; set; }

    /// <inheritdoc/>
    public ushort NumberOfLaps { get; set; }

    /// <inheritdoc/>
    public ushort NumberOfTyreStints { get; set; }

    /// <inheritdoc/>
    public ushort BestLapNumber { get; set; }

    /// <inheritdoc/>
    public ushort BestSector1LapNumber { get; set; }

    /// <inheritdoc/>
    public ushort BestSector2LapNumber { get; set; }

    /// <inheritdoc/>
    public ushort BestSector3LapNumber { get; set; }

    /// <inheritdoc/>
    public ILapHistoryDataBase[] LapHistory { get; }

    /// <inheritdoc/>
    public ITyreStintHistoryDataBase[] TyreStintHistory { get; }

    #endregion // ISessionHistoryDataBase
}