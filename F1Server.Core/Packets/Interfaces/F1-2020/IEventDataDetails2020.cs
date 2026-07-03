using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for event details in the event data packet (F1 2020)
/// </summary>
public interface IEventDataDetails2020 : IEventDataDetailsBase
{
    #region Properties

    /// <summary>
    /// Type of penalty
    /// </summary>
    PenaltyType PenaltyType { get; }

    /// <summary>
    /// Type of infringement
    /// </summary>
    InfringementType PenaltyInfringementType { get; }

    /// <summary>
    /// Other vehicle index
    /// </summary>
    ushort PenaltyOtherVehicleIndex { get; }

    /// <summary>
    /// Time gained or time spent doing action in seconds
    /// </summary>
    ushort PenaltyTimeGained { get; }

    /// <summary>
    /// Lap of penalty occured on
    /// </summary>
    ushort PenaltyLapNumber { get; }

    /// <summary>
    /// Number of places gained by this penalty
    /// </summary>
    ushort PenaltyPlacesGained { get; }

    /// <summary>
    /// Top speed achieved in km/h
    /// </summary>
    float TopSpeed { get; }

    #endregion // Properties
}