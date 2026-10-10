using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event details of event in F1 2023
/// </summary>
internal class EventDataDetails2023 : EventDataDetails2019, IEventDataDetails2023
{
    #region IEventDataDetails2023

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
    public uint ButtonsTriggered { get; set; }

    /// <inheritdoc/>
    public ushort OvertakingVehicleIndex { get; set; }

    /// <inheritdoc/>
    public ushort BeingOvertakenVehicleIndex { get; set; }

    /// <inheritdoc/>
    public bool IsRedFlag { get; set; }

    #endregion // IEventDataDetails2023
}