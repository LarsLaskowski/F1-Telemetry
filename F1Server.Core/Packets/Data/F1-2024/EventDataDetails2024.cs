using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event details of event in F1 2024
/// </summary>
internal class EventDataDetails2024 : EventDataDetails2019, IEventDataDetails2024
{
    #region IEventDataDetails2024

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
    /// Lap of penalty occurred on
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

    /// <summary>
    /// Overall fastest speed in session
    /// </summary>
    public bool IsOverallFastestInSession { get; set; }

    /// <summary>
    /// Fastest speed for driver in session
    /// </summary>
    public bool IsDriverFastestInSession { get; set; }

    /// <summary>
    /// Fastest id of car in session
    /// </summary>
    public ushort FastestVehicleIndexInSession { get; set; }

    /// <summary>
    /// Speed of the vehicle that is the fastest in session
    /// </summary>
    public float FastestSpeedInSession { get; set; }

    /// <summary>
    /// Number of lights showing
    /// </summary>
    public ushort StartLightsNumbers { get; set; }

    /// <summary>
    /// Frame identifier flashed back
    /// </summary>
    public uint FlashbackFrame { get; set; }

    /// <summary>
    /// Session time flashed back
    /// </summary>
    public float FlashbackSessionTime { get; set; }

    /// <summary>
    /// Bit flags specifying which buttons are being pressed
    /// </summary>
    public uint ButtonsTriggered { get; set; }

    /// <summary>
    /// Vehicle index of the vehicle overtaking
    /// </summary>
    public ushort OvertakingVehicleIndex { get; set; }

    /// <summary>
    /// Vehicle index of vehicle being overtaken
    /// </summary>
    public ushort BeingOvertakenVehicleIndex { get; set; }

    /// <summary>
    /// Red flag shown
    /// </summary>
    public bool IsRedFlag { get; set; }

    /// <summary>
    /// Type of safety car
    /// </summary>
    public SafetyCarStatus SafetyCarType { get; set; }

    /// <summary>
    /// Safety car event type
    /// </summary>
    public SafetyCarEventType SafetyCarEvent { get; set; }

    /// <summary>
    /// Vehicle index of the first vehicle involved in the collision
    /// </summary>
    public ushort CollisionVehicleIndex1 { get; set; }

    /// <summary>
    /// Vehicle index of the second vehicle involved in the collision
    /// </summary>
    public ushort CollisionVehicleIndex2 { get; set; }

    #endregion // IEventDataDetails2024
}