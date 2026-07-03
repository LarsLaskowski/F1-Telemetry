using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data packet with lap information (F1 2025)
/// </summary>
public class LapData2025 : ILapData2023, ILapData2025
{
    #region IPacketLapData2023

    /// <inheritdoc/>
    public bool IsEmpty => GridPosition == 0 && CurrentLapTime == 0;

    /// <inheritdoc/>
    public uint LastLapTime { get; set; }

    /// <inheritdoc/>
    public uint CurrentLapTime { get; set; }

    /// <inheritdoc/>
    public ushort Sector1Time { get; set; }

    /// <inheritdoc/>
    public ushort Sector1TimeMinutes { get; set; }

    /// <inheritdoc/>
    public ushort Sector2Time { get; set; }

    /// <inheritdoc/>
    public ushort Sector2TimeMinutes { get; set; }

    /// <inheritdoc/>
    public ushort DeltaToCarInFront { get; set; }

    /// <inheritdoc/>
    public ushort DeltaToRaceLeader { get; set; }

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
    public ushort NumberPitStops { get; set; }

    /// <inheritdoc/>
    public Sector CurrentSector { get; set; }

    /// <inheritdoc/>
    public bool IsCurrentLapInvalid { get; set; }

    /// <inheritdoc/>
    public ushort TimePenalties { get; set; }

    /// <inheritdoc/>
    public ushort Warnings { get; set; }

    /// <inheritdoc/>
    public ushort CornerCuttingWarnings { get; set; }

    /// <inheritdoc/>
    public ushort NumberUnservedDriveThroughPens { get; set; }

    /// <inheritdoc/>
    public ushort NumberUnservedStopAndGoPenalties { get; set; }

    /// <inheritdoc/>
    public ushort GridPosition { get; set; }

    /// <inheritdoc/>
    public DriverStatus CurrentDriverStatus { get; set; }

    /// <inheritdoc/>
    public ResultStatus CurrentResultStatus { get; set; }

    /// <inheritdoc/>
    public bool IsPitLaneTimerActive { get; set; }

    /// <inheritdoc/>
    public ushort PitLaneTimeInLane { get; set; }

    /// <inheritdoc/>
    public ushort PitStopTimer { get; set; }

    /// <inheritdoc/>
    public bool PitStopShouldServePenalty { get; set; }

    #endregion // IPacketLapData2023

    #region IPacketLapData2025

    /// <inheritdoc/>
    public ushort DeltaToCarInFrontMinutes { get; set; }

    /// <inheritdoc/>
    public ushort DeltaToRaceLeaderMinutes { get; set; }

    /// <inheritdoc/>
    public float SpeedTrapFastestSpeed { get; set; }

    /// <inheritdoc/>
    public ushort SpeedTrapFastestLap { get; set; }

    #endregion // IPacketLapData2025
}