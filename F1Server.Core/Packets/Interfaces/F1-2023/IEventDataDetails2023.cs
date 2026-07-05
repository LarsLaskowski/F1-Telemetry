using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for event details in the event data packet (F1 2023)
/// </summary>
public interface IEventDataDetails2023 : IEventDataDetailsBase
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
    /// Lap of penalty occurred on
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

    /// <summary>
    /// Overall fastest speed in session
    /// </summary>
    bool IsOverallFastestInSession { get; }

    /// <summary>
    /// Fastest speed for driver in session
    /// </summary>
    bool IsDriverFastestInSession { get; }

    /// <summary>
    /// Fastest id of car in session
    /// </summary>
    ushort FastestVehicleIndexInSession { get; }

    /// <summary>
    /// Speed of the vehicle that is the fastest in session
    /// </summary>
    float FastestSpeedInSession { get; }

    /// <summary>
    /// Number of lights showing
    /// </summary>
    ushort StartLightsNumbers { get; }

    /// <summary>
    /// Frame identifier flashed back
    /// </summary>
    uint FlashbackFrame { get; }

    /// <summary>
    /// Session time flashed back
    /// </summary>
    float FlashbackSessionTime { get; }

    /// <summary>
    /// Bit flags specifying which buttons are being pressed
    /// </summary>
    uint ButtonsTriggered { get; }

    /// <summary>
    /// Vehicle index of the vehicle overtaking
    /// </summary>
    ushort OvertakingVehicleIndex { get; }

    /// <summary>
    /// Vehicle index of vehicle being overtaken
    /// </summary>
    ushort BeingOvertakenVehicleIndex { get; }

    /// <summary>
    /// Red flag shown
    /// </summary>
    bool IsRedFlag { get; }

    #endregion // Properties
}