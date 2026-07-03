using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session history lap data (F1 2025)
/// </summary>
public class SessionHistoryData2025 : ISessionHistoryDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionHistoryData2025()
    {
        LapHistory = new SessionHistoryLapData2025[100];
        TyreStintHistory = new SessionHistoryTyreStintData2025[8];
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
    public ILapHistoryDataBase[] LapHistory { get; set; }

    /// <inheritdoc/>
    public ITyreStintHistoryDataBase[] TyreStintHistory { get; set; }

    #endregion // ISessionHistoryDataBase
}