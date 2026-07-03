using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event details of event in F1 2025
/// </summary>
internal class EventDataDetails2025 : EventDataDetails2019, IEventDataDetails2025
{
    #region IEventDataDetails2025

    /// <inheritdoc/>
    public PenaltyType PenaltyType { get; set; }

    /// <inheritdoc/>
    public InfringementType PenaltyInfringementType { get; set; }

    /// <inheritdoc/>
    public ushort PenaltyOtherVehicleIndex { get; set; }

    /// <inheritdoc/>
    public ushort PenaltyTimeGained { get; set; }

    /// <inheritdoc/>
    public ushort PenaltyLapNumber { get; set; }

    /// <inheritdoc/>
    public ushort PenaltyPlacesGained { get; set; }

    /// <inheritdoc/>
    public ResultReason RetirementReason { get; set; }

    /// <inheritdoc/>
    public float TopSpeed { get; set; }

    /// <inheritdoc/>
    public bool IsOverallFastestInSession { get; set; }

    /// <inheritdoc/>
    public bool IsDriverFastestInSession { get; set; }

    /// <inheritdoc/>
    public ushort FastestVehicleIndexInSession { get; set; }

    /// <inheritdoc/>
    public float FastestSpeedInSession { get; set; }

    /// <inheritdoc/>
    public ushort StartLightsNumbers { get; set; }

    /// <inheritdoc/>
    public uint FlashbackFrame { get; set; }

    /// <inheritdoc/>
    public float FlashbackSessionTime { get; set; }

    /// <inheritdoc/>
    public float StopAndGoPenaltyTime { get; set; }

    /// <inheritdoc/>
    public uint ButtonsTriggered { get; set; }

    /// <inheritdoc/>
    public ushort OvertakingVehicleIndex { get; set; }

    /// <inheritdoc/>
    public ushort BeingOvertakenVehicleIndex { get; set; }

    /// <inheritdoc/>
    public bool IsRedFlag { get; set; }

    /// <inheritdoc/>
    public SafetyCarStatus SafetyCarType { get; set; }

    /// <inheritdoc/>
    public SafetyCarEventType SafetyCarEvent { get; set; }

    /// <inheritdoc/>
    public ushort CollisionVehicleIndex1 { get; set; }

    /// <inheritdoc/>
    public ushort CollisionVehicleIndex2 { get; set; }

    /// <inheritdoc/>
    public DrsDisabledReason DrsDisabledReason { get; set; }

    #endregion // IEventDataDetails2025
}