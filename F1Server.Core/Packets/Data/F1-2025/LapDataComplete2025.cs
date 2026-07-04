using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class with all lap data information of all cars - F1 2025
/// </summary>
public class LapDataComplete2025 : ILapDataComplete, ILapDataComplete2025
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public LapDataComplete2025()
    {
        LapData = new LapData2025[22];
    }

    #endregion // Constructors

    #region ILapDataComplete

    /// <inheritdoc/>
    public ILapDataBase[] LapData { get; set; }

    #endregion // ILapDataComplete

    #region ILapDataComplete2023

    /// <inheritdoc/>
    public ushort TimeTrialPersonalBestCarIndex { get; set; }

    /// <inheritdoc/>
    public ushort TimeTrialRivalCarIndex { get; set; }

    #endregion // ILapDataComplete2023
}