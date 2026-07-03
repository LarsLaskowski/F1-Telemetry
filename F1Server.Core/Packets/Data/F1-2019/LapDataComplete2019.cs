using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class with all lap data information of all cars - F1 2019
/// </summary>
public class LapDataComplete2019 : ILapDataComplete
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public LapDataComplete2019()
    {
        LapData = new LapData2019[20];
    }

    #endregion // Constructors

    #region ILapDataComplete

    /// <summary>
    /// Array with all lap data information of all cars
    /// </summary>
    public ILapDataBase[] LapData { get; set; }

    #endregion // ILapDataComplete
}