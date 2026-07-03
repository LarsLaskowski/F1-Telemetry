using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class with all lap data information of all cars - F1 2020
/// </summary>
public class LapDataComplete2020 : ILapDataComplete
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public LapDataComplete2020()
    {
        LapData = new LapData2020[22];
    }

    #endregion // Constructors

    #region ILapDataComplete

    /// <summary>
    /// Array with all lap data information of all cars
    /// </summary>
    public ILapDataBase[] LapData { get; set; }

    #endregion // ILapDataComplete
}