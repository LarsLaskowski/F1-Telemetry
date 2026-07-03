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
    }

    #endregion // Constructors

    #region ILapDataComplete

    /// <summary>
    /// Array with all lap data information of all cars
    /// </summary>
    public ILapDataBase[] LapData { get; set; }

    #endregion // ILapDataComplete

    #region ILapDataComplete2024

    /// <summary>
    /// Index of personal best car in time trial (255 - invalid)
    /// </summary>
    public ushort TimeTrialPersonalBestCarIndex { get; set; }

    /// <summary>
    /// Index of rival car in time trial (255 - invalid)
    /// </summary>
    public ushort TimeTrialRivalCarIndex { get; set; }

    #endregion // ILapDataComplete2024
}