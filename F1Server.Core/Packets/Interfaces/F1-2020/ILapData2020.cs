namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for lap data packets (F1 2020)
/// </summary>
public interface ILapData2020 : ILapDataBase
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
    /// Sector 1 time in milliseconds
    /// </summary>
    ushort Sector1Time { get; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    ushort Sector2Time { get; }

    /// <summary>
    /// Best lap time in session in seconds
    /// </summary>
    float BestLapTime { get; }

    /// <summary>
    /// Number of lap achieved best lap time
    /// </summary>
    ushort BestLapNumber { get; }

    /// <summary>
    /// Sector 1 time of best lap in milliseconds
    /// </summary>
    ushort BestLapSector1Time { get; }

    /// <summary>
    /// Sector 2 time of best lap in milliseconds
    /// </summary>
    ushort BestLapSector2Time { get; }

    /// <summary>
    /// Sector 3 time of best lap in milliseconds
    /// </summary>
    ushort BestLapSector3Time { get; }

    /// <summary>
    /// Best Sector 1 time overall in milliseconds
    /// </summary>
    ushort BestOverallSector1Time { get; }

    /// <summary>
    /// Number of lap achieved best sector 1 time
    /// </summary>
    ushort BestOverallSector1LapNumber { get; }

    /// <summary>
    /// Best Sector 2 time overall in milliseconds
    /// </summary>
    ushort BestOverallSector2Time { get; }

    /// <summary>
    /// Number of lap achieved best sector 2 time
    /// </summary>
    ushort BestOverallSector2LapNumber { get; }

    /// <summary>
    /// Best Sector 3 time overall in milliseconds
    /// </summary>
    ushort BestOverallSector3Time { get; }

    /// <summary>
    /// Number of lap achieved best sector 3 time
    /// </summary>
    ushort BestOverallSector3LapNumber { get; }

    #endregion // Properties
}