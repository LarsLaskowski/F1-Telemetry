using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class with all lap data information of all cars - F1 2022
/// </summary>
public class LapDataComplete2022 : ILapDataComplete, ILapDataComplete2022
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public LapDataComplete2022()
    {
        LapData = new LapData2022[22];

        for (int lapData = 0; lapData < LapData.Length; ++lapData)
        {
            LapData[lapData] = new LapData2022();
        }
    }

    #endregion // Constructors

    #region ILapDataComplete

    /// <inheritdoc/>
    public ILapDataBase[] LapData { get; }

    #endregion // ILapDataComplete

    #region ILapDataComplete2022

    /// <inheritdoc/>
    public ushort TimeTrialPersonalBestCarIndex { get; set; }

    /// <inheritdoc/>
    public ushort TimeTrialRivalCarIndex { get; set; }

    #endregion // ILapDataComplete2022
}