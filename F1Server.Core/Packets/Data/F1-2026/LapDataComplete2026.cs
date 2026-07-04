using F1Server.Core.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class with all lap data information of all cars - F1 2026
/// </summary>
public class LapDataComplete2026 : ILapDataComplete, ILapDataComplete2026
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public LapDataComplete2026()
    {
        LapData = new LapData2026[ConstData.F12026MaxCars];
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