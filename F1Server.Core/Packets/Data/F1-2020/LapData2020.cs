using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data packet with lap information (F1 2020)
/// </summary>
public class LapData2020 : ILapData2020
{
    #region ILapData2020

    /// <summary>
    /// Last lap time
    /// </summary>
    public float LastLapTime { get; set; }

    /// <summary>
    /// Current lap time
    /// </summary>
    public float CurrentLapTime { get; set; }

    /// <summary>
    /// Sector 1 time in milliseconds
    /// </summary>
    public ushort Sector1Time { get; set; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    public ushort Sector2Time { get; set; }

    /// <summary>
    /// Best lap time in session in seconds
    /// </summary>
    public float BestLapTime { get; set; }

    /// <summary>
    /// Number of lap achieved best lap time
    /// </summary>
    public ushort BestLapNumber { get; set; }

    /// <summary>
    /// Sector 1 time of best lap in milliseconds
    /// </summary>
    public ushort BestLapSector1Time { get; set; }

    /// <summary>
    /// Sector 2 time of best lap in milliseconds
    /// </summary>
    public ushort BestLapSector2Time { get; set; }

    /// <summary>
    /// Sector 3 time of best lap in milliseconds
    /// </summary>
    public ushort BestLapSector3Time { get; set; }

    /// <summary>
    /// Best Sector 1 time overall in milliseconds
    /// </summary>
    public ushort BestOverallSector1Time { get; set; }

    /// <summary>
    /// Number of lap achieved best sector 1 time
    /// </summary>
    public ushort BestOverallSector1LapNumber { get; set; }

    /// <summary>
    /// Best Sector 2 time overall in milliseconds
    /// </summary>
    public ushort BestOverallSector2Time { get; set; }

    /// <summary>
    /// Number of lap achieved best sector 2 time
    /// </summary>
    public ushort BestOverallSector2LapNumber { get; set; }

    /// <summary>
    /// Best Sector 3 time overall in milliseconds
    /// </summary>
    public ushort BestOverallSector3Time { get; set; }

    /// <summary>
    /// Number of lap achieved best sector 3 time
    /// </summary>
    public ushort BestOverallSector3LapNumber { get; set; }

    #endregion // ILapData2020

    #region ILapDataBase

    /// <summary>
    /// Flag if there is no car available (is nothing from the game)
    /// </summary>
    public bool IsEmpty => CurrentLapNumber == 0 && CarPosition == 0 && GridPosition == 0 && TotalDistance <= 0.0;

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