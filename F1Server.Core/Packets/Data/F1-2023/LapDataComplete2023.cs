using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class with all lap data information of all cars - F1 2023
/// </summary>
public class LapDataComplete2023 : ILapDataComplete, ILapDataComplete2023
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public LapDataComplete2023()
    {
        LapData = new LapData2023[22];

        for (int lapData = 0; lapData < LapData.Length; ++lapData)
        {
            LapData[lapData] = new LapData2023();
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