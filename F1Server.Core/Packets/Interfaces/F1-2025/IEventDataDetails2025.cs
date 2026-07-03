using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for event details in the event data packet (F1 2025)
/// </summary>
public interface IEventDataDetails2025 : IEventDataDetailsBase
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
    /// Reason of retirement
    /// </summary>
    ResultReason RetirementReason { get; }

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
    /// Gets or sets the duration, in seconds, of the stop-and-go penalty
    /// </summary>
    float StopAndGoPenaltyTime { get; set; }

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

    /// <summary>
    /// Type of safety car
    /// </summary>
    SafetyCarStatus SafetyCarType { get; }

    /// <summary>
    /// Safety car event type
    /// </summary>
    SafetyCarEventType SafetyCarEvent { get; }

    /// <summary>
    /// Vehicle index of the first vehicle involved in the collision
    /// </summary>
    ushort CollisionVehicleIndex1 { get; }

    /// <summary>
    /// Vehicle index of the second vehicle involved in the collision
    /// </summary>
    ushort CollisionVehicleIndex2 { get; }

    /// <summary>
    /// Reason why DRS is disabled
    /// </summary>
    DrsDisabledReason DrsDisabledReason { get; }

    #endregion // Properties
}