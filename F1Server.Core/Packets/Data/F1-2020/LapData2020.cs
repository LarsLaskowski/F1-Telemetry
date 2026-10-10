using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data packet with lap information (F1 2020)
/// </summary>
public class LapData2020 : ILapData2020
{
    #region ILapData2020

    /// <inheritdoc/>
    public float LastLapTime { get; set; }

    /// <inheritdoc/>
    public float CurrentLapTime { get; set; }

    /// <inheritdoc/>
    public ushort Sector1Time { get; set; }

    /// <inheritdoc/>
    public ushort Sector2Time { get; set; }

    /// <inheritdoc/>
    public float BestLapTime { get; set; }

    /// <inheritdoc/>
    public ushort BestLapNumber { get; set; }

    /// <inheritdoc/>
    public ushort BestLapSector1Time { get; set; }

    /// <inheritdoc/>
    public ushort BestLapSector2Time { get; set; }

    /// <inheritdoc/>
    public ushort BestLapSector3Time { get; set; }

    /// <inheritdoc/>
    public ushort BestOverallSector1Time { get; set; }

    /// <inheritdoc/>
    public ushort BestOverallSector1LapNumber { get; set; }

    /// <inheritdoc/>
    public ushort BestOverallSector2Time { get; set; }

    /// <inheritdoc/>
    public ushort BestOverallSector2LapNumber { get; set; }

    /// <inheritdoc/>
    public ushort BestOverallSector3Time { get; set; }

    /// <inheritdoc/>
    public ushort BestOverallSector3LapNumber { get; set; }

    #endregion // ILapData2020

    #region ILapDataBase

    /// <inheritdoc/>
    public bool IsEmpty => CurrentLapNumber == 0 && CarPosition == 0 && GridPosition == 0 && TotalDistance <= 0.0;

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