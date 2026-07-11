using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data packet with lap information (F1 2023)
/// </summary>
public class LapData2023 : ILapData2023
{
    #region ILapData2023

    /// <summary>
    /// Last lap time in milliseconds
    /// </summary>
    public uint LastLapTime { get; set; }

    /// <summary>
    /// Current lap time in milliseconds
    /// </summary>
    public uint CurrentLapTime { get; set; }

    /// <summary>
    /// Sector 1 time in milliseconds
    /// </summary>
    public ushort Sector1Time { get; set; }

    /// <summary>
    /// Sector 1 whole minutes part
    /// </summary>
    public ushort Sector1TimeMinutes { get; set; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    public ushort Sector2Time { get; set; }

    /// <summary>
    /// Sector 2 whole minutes part
    /// </summary>
    public ushort Sector2TimeMinutes { get; set; }

    /// <summary>
    /// Delta to car in front in milliseconds
    /// </summary>
    public ushort DeltaToCarInFront { get; set; }

    /// <summary>
    /// Delta to race leader in milliseconds
    /// </summary>
    public ushort DeltaToRaceLeader { get; set; }

    /// <summary>
    /// Number of pit stops
    /// </summary>
    public ushort NumberPitStops { get; set; }

    /// <summary>
    /// Accumulated number of warnings
    /// </summary>
    public ushort Warnings { get; set; }

    /// <summary>
    /// Accumulated number of corner cutting warnings
    /// </summary>
    public ushort CornerCuttingWarnings { get; set; }

    /// <summary>
    /// Number of drive through penalties left to serve
    /// </summary>
    public ushort NumberUnservedDriveThroughPens { get; set; }

    /// <summary>
    /// Number of stop and go penalties left to serve
    /// </summary>
    public ushort NumberUnservedStopAndGoPenalties { get; set; }

    /// <summary>
    /// Pit lane timing active flag
    /// </summary>
    public bool IsPitLaneTimerActive { get; set; }

    /// <summary>
    /// Pit lane time in lane in milliseconds, only if <see cref="IsPitLaneTimerActive"/> is active
    /// </summary>
    public ushort PitLaneTimeInLane { get; set; }

    /// <summary>
    /// Time of actual pit stop in milliseconds
    /// </summary>
    public ushort PitStopTimer { get; set; }

    /// <summary>
    /// Flag if the car should serve a penalty at this stop
    /// </summary>
    public bool PitStopShouldServePenalty { get; set; }

    #endregion // ILapData2023

    #region ILapDataBase

    /// <summary>
    /// Flag if there is no car available (is nothing from the game)
    /// </summary>
    public bool IsEmpty => GridPosition == 0 && CurrentLapTime == 0;

    /// <summary>
    /// Distance vehicle is around current lap in meters, negative is finish line not crossed yet
    /// </summary>
    public float LapDistance { get; set; }

    /// <summary>
    /// Total distance travelled in session in meters, can be negative like <see cref="LapDistance"/>
    /// </summary>
    public float TotalDistance { get; set; }

    /// <summary>
    /// Delta in seconds for safety car
    /// </summary>
    public float SafetyCarDelta { get; set; }

    /// <summary>
    /// Actual race position
    /// </summary>
    public ushort CarPosition { get; set; }

    /// <summary>
    /// Current lap number
    /// </summary>
    public ushort CurrentLapNumber { get; set; }

    /// <summary>
    /// Current pit status
    /// </summary>
    public PitStatus CurrentPitStatus { get; set; }

    /// <summary>
    /// Current sector
    /// </summary>
    public Sector CurrentSector { get; set; }

    /// <summary>
    /// Is current lap invalid?
    /// </summary>
    public bool IsCurrentLapInvalid { get; set; }

    /// <summary>
    /// Accumulated time penalties in seconds to be added
    /// </summary>
    public ushort TimePenalties { get; set; }

    /// <summary>
    /// Grid start position
    /// </summary>
    public ushort GridPosition { get; set; }

    /// <summary>
    /// Current driver status
    /// </summary>
    public DriverStatus CurrentDriverStatus { get; set; }

    /// <summary>
    /// Current result status
    /// </summary>
    public ResultStatus CurrentResultStatus { get; set; }

    #endregion // ILapDataBase
}