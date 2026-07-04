using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data packet with lap information (F1 2021)
/// </summary>
public class LapData2021 : ILapData2021
{
    #region ILapDataBase

    /// <summary>
    /// Flag if there is no car available (is nothing from the game)
    /// </summary>
    public bool IsEmpty => GridPosition == 0 && CurrentLapTime == 0;

    #endregion // ILapDataBase

    #region ILapData2021

    /// <summary>
    /// Last lap time in milliseconds
    /// </summary>
    public uint LastLapTime { get; set; }

    /// <summary>
    /// Current lap time in milliseconds
    /// </summary>
    public uint CurrentLapTime { get; set; }

    /// <summary>
    /// Sector 1 time in milliseonds
    /// </summary>
    public ushort Sector1Time { get; set; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    public ushort Sector2Time { get; set; }

    #endregion // ILapData2021

    #region ILapDataBase

    /// <summary>
    /// Distance vehicle is around current lap in meters, negativ is finish line not crossed yet
    /// </summary>
    public float LapDistance { get; set; }

    /// <summary>
    /// Total distance travelled in session in meters, can be negativ like <see cref="LapDistance"/>
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

    #endregion // ILapDataBase

    #region ILapData2021

    /// <summary>
    /// Number of pit stops
    /// </summary>
    public ushort NumberPitStops { get; set; }

    #endregion // ILapData2021

    #region ILapDataBase

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

    #endregion // ILapDataBase

    #region ILapData2021

    /// <summary>
    /// Accumulated number of warnings
    /// </summary>
    public ushort Warnings { get; set; }

    /// <summary>
    /// Number of drive through penalties left to serve
    /// </summary>
    public ushort NumberUnservedDriveThroughPens { get; set; }

    /// <summary>
    /// Number of stop and go penalties left to serve
    /// </summary>
    public ushort NumberUnservedStopAndGoPenalties { get; set; }

    #endregion // ILapData2021

    #region ILapDataBase

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

    #region ILapData2021

    /// <summary>
    /// Pit lane timing active flag
    /// </summary>
    public bool IsPitLaneTimerActive { get; set; }

    /// <summary>
    /// Pit lane time in lan in milliseconds, only if <see cref="IsPitLaneTimerActive"/> is active
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

    #endregion // ILapData2021
}