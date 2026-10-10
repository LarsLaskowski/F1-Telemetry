using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event details of event in F1 2020
/// </summary>
internal class EventDataDetails2020 : EventDataDetails2019, IEventDataDetails2020
{
    #region IEventDataDetails2020

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

    #endregion // IEventDataDetails2020
}