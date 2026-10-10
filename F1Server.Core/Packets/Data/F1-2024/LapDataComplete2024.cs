using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class with all lap data information of all cars - F1 2024
/// </summary>
public class LapDataComplete2024 : ILapDataComplete, ILapDataComplete2024
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public LapDataComplete2024()
    {
        LapData = new LapData2024[22];

        for (int lapData = 0; lapData < LapData.Length; ++lapData)
        {
            LapData[lapData] = new LapData2024();
        }
    }

    #endregion // Constructors

    #region ILapDataComplete

    /// <inheritdoc/>
    public ILapDataBase[] LapData { get; }

    #endregion // ILapDataComplete

    #region ILapDataComplete2023

    /// <inheritdoc/>
    public ushort TimeTrialPersonalBestCarIndex { get; set; }

    /// <inheritdoc/>
    public ushort TimeTrialRivalCarIndex { get; set; }

    #endregion // ILapDataComplete2023
}