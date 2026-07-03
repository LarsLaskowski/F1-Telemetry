namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for lap data packets (F1 2023)
/// </summary>
public interface ILapData2023 : ILapDataBase
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
    /// Sector 1 time in milliseonds
    /// </summary>
    ushort Sector1Time { get; }

    /// <summary>
    /// Sector 1 while minutes part
    /// </summary>
    ushort Sector1TimeMinutes { get; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    ushort Sector2Time { get; }

    /// <summary>
    /// Sector 2 whole minutes part
    /// </summary>
    ushort Sector2TimeMinutes { get; }

    /// <summary>
    /// Delta to car in front in milliseconds
    /// </summary>
    ushort DeltaToCarInFront { get; }

    /// <summary>
    /// Delta to race leader in milliseconds
    /// </summary>
    ushort DeltaToRaceLeader { get; }

    /// <summary>
    /// Number of pit stops
    /// </summary>
    ushort NumberPitStops { get; }

    /// <summary>
    /// Accumulated number of warnings
    /// </summary>
    ushort Warnings { get; }

    /// <summary>
    /// Accumulated number of corner cutting warnings
    /// </summary>
    ushort CornerCuttingWarnings { get; }

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