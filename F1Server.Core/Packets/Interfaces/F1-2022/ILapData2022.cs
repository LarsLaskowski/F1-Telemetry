namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for lap data packets (F1 2022)
/// </summary>
public interface ILapData2022 : ILapDataBase
{
    #region Properties

    /// <summary>
    /// Last lap time in milliseconds
    /// </summary>
    uint LastLapTime { get; }

    /// <summary>
    /// Current lap time in milliseconds
    /// </summary>
    uint CurrentLapTime { get; }

    /// <summary>
    /// Sector 1 time in milliseconds
    /// </summary>
    ushort Sector1Time { get; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    ushort Sector2Time { get; }

    /// <summary>
    /// Number of pit stops
    /// </summary>
    ushort NumberPitStops { get; }

    /// <summary>
    /// Accumulated number of warnings
    /// </summary>
    ushort Warnings { get; }

    /// <summary>
    /// Number of drive through penalties left to serve
    /// </summary>
    ushort NumberUnservedDriveThroughPens { get; }

    /// <summary>
    /// Number of stop and go penalties left to serve
    /// </summary>
    ushort NumberUnservedStopAndGoPenalties { get; }

    /// <summary>
    /// Pit lane timing active flag
    /// </summary>
    bool IsPitLaneTimerActive { get; }

    /// <summary>
    /// Pit lane time in lan in milliseconds, only if <see cref="IsPitLaneTimerActive"/> is active
    /// </summary>
    ushort PitLaneTimeInLane { get; }

    /// <summary>
    /// Time of actual pit stop in milliseconds
    /// </summary>
    ushort PitStopTimer { get; }

    /// <summary>
    /// Flag if the car should serve a penalty at this stop
    /// </summary>
    bool PitStopShouldServePenalty { get; }

    #endregion // Properties
}