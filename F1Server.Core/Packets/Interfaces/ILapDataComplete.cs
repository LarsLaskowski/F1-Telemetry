namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for all lap data information of all cars
/// </summary>
public interface ILapDataComplete
{
    #region Properties

    /// <summary>
    /// Array of car lap data information
    /// </summary>
    ILapDataBase[] LapData { get; set; }

    #endregion // Properties
}