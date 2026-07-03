using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event details of event in F1 2020
/// </summary>
internal class EventDataDetails2020 : EventDataDetails2019, IEventDataDetails2020
{
    #region IEventDataDetails2020

    /// <summary>
    /// Type of penalty
    /// </summary>
    public PenaltyType PenaltyType { get; set; }

    /// <summary>
    /// Type of infringement
    /// </summary>
    public InfringementType PenaltyInfringementType { get; set; }

    /// <summary>
    /// Other vehicle index
    /// </summary>
    public ushort PenaltyOtherVehicleIndex { get; set; }

    /// <summary>
    /// Time gained or time spent doing action in seconds
    /// </summary>
    public ushort PenaltyTimeGained { get; set; }

    /// <summary>
    /// Lap of penalty occured on
    /// </summary>
    public ushort PenaltyLapNumber { get; set; }

    /// <summary>
    /// Number of places gained by this penalty
    /// </summary>
    public ushort PenaltyPlacesGained { get; set; }

    /// <summary>
    /// Top speed achieved in km/h
    /// </summary>
    public float TopSpeed { get; set; }

    #endregion // IEventDataDetails2020
}