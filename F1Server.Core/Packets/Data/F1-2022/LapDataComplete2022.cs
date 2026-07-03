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
    }

    #endregion // Constructors

    #region ILapDataComplete

    /// <summary>
    /// Array with all lap data information of all cars
    /// </summary>
    public ILapDataBase[] LapData { get; set; }

    #endregion // ILapDataComplete

    #region ILapDataComplete2022

    /// <summary>
    /// Index of personal best car in time trial (255 - invalid)
    /// </summary>
    public ushort TimeTrialPersonalBestCarIndex { get; set; }

    /// <summary>
    /// Index of rival car in time trial (255 - invalid)
    /// </summary>
    public ushort TimeTrialRivalCarIndex { get; set; }

    #endregion // ILapDataComplete2022
}