namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for lap data packets (F1 2019)
/// </summary>
public interface ILapData2019 : ILapDataBase
{
    #region Properties

    /// <summary>
    /// Last lap time in seconds
    /// </summary>
    float LastLapTime { get; }

    /// <summary>
    /// Current lap time in seconds
    /// </summary>
    float CurrentLapTime { get; }

    /// <summary>
    /// Best lap time in session in seconds
    /// </summary>
    float BestLapTime { get; }

    /// <summary>
    /// Sector 1 time in seconds
    /// </summary>
    float Sector1Time { get; }

    /// <summary>
    /// Sector 2 time in seconds
    /// </summary>
    float Sector2Time { get; }

    #endregion // Properties
}