using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data packet with lap information (F1 2021)
/// </summary>
public class LapData2021 : ILapData2021
{
    #region ILapData2021

    /// <inheritdoc/>
    public uint LastLapTime { get; set; }

    /// <inheritdoc/>
    public uint CurrentLapTime { get; set; }

    /// <inheritdoc/>
    public ushort Sector1Time { get; set; }

    /// <inheritdoc/>
    public ushort Sector2Time { get; set; }

    /// <inheritdoc/>
    public ushort Warnings { get; set; }

    /// <inheritdoc/>
    public ushort NumberUnservedDriveThroughPens { get; set; }

    /// <inheritdoc/>
    public ushort NumberUnservedStopAndGoPenalties { get; set; }

    /// <inheritdoc/>
    public ushort NumberPitStops { get; set; }

    /// <inheritdoc/>
    public bool IsPitLaneTimerActive { get; set; }

    /// <inheritdoc/>
    public ushort PitLaneTimeInLane { get; set; }

    /// <inheritdoc/>
    public ushort PitStopTimer { get; set; }

    /// <inheritdoc/>
    public bool PitStopShouldServePenalty { get; set; }

    #endregion // ILapData2021

    #region ILapDataBase

    /// <inheritdoc/>
    public bool IsEmpty => GridPosition == 0 && CurrentLapTime == 0;

    /// <inheritdoc/>
    public float LapDistance { get; set; }

    /// <inheritdoc/>
    public float TotalDistance { get; set; }

    /// <inheritdoc/>
    public float SafetyCarDelta { get; set; }

    /// <inheritdoc/>
    public ushort CarPosition { get; set; }

    /// <inheritdoc/>
    public ushort CurrentLapNumber { get; set; }

    /// <inheritdoc/>
    public PitStatus CurrentPitStatus { get; set; }

    /// <inheritdoc/>
    public Sector CurrentSector { get; set; }

    /// <inheritdoc/>
    public bool IsCurrentLapInvalid { get; set; }

    /// <inheritdoc/>
    public ushort TimePenalties { get; set; }

    /// <inheritdoc/>
    public ushort GridPosition { get; set; }

    /// <inheritdoc/>
    public DriverStatus CurrentDriverStatus { get; set; }

    /// <inheritdoc/>
    public ResultStatus CurrentResultStatus { get; set; }

    #endregion // ILapDataBase
}