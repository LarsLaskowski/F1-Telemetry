namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for extended complete lap data information in F1 2022
/// </summary>
public interface ILapDataComplete2022
{
    #region Properties

    /// <summary>
    /// Index of personal best car in time trial (255 - invalid)
    /// </summary>
    ushort TimeTrialPersonalBestCarIndex { get; set; }

    /// <summary>
    /// Index of rival car in time trial (255 - invalid)
    /// </summary>
    ushort TimeTrialRivalCarIndex { get; set; }

    #endregion // Properties
}