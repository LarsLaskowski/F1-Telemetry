namespace F1Server.Core.Enumerations;

/// <summary>
/// Enumeration of packet types
/// </summary>
public enum PacketTypes
{
    /// <summary>
    /// Unknown/Not set
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Motion data of players car - only while player is in control
    /// </summary>
    Motion = 1,

    /// <summary>
    /// Session data
    /// </summary>
    Session,

    /// <summary>
    /// Lap times of cars in the session
    /// </summary>
    LapData,

    /// <summary>
    /// Events
    /// </summary>
    Event,

    /// <summary>
    /// Participants in the session
    /// </summary>
    Participants,

    /// <summary>
    /// Car setups
    /// </summary>
    CarSetups,

    /// <summary>
    /// Car telemetry data
    /// </summary>
    CarTelemetry,

    /// <summary>
    /// Status data of all cars
    /// </summary>
    CarStatus,

    /// <summary>
    /// Final classification at the end of the race (new in F1 2020)
    /// </summary>
    FinalClassification,

    /// <summary>
    /// Information about players in a multiplayer lobby (new in F1 2020)
    /// </summary>
    LobbyInfo,

    /// <summary>
    /// Damage status of all cars (new in F1 2021)
    /// </summary>
    CarDamage,

    /// <summary>
    /// Lap and tyre data for session (new in F1 2021)
    /// </summary>
    SessionHistory,

    /// <summary>
    /// Extended tyre set data
    /// </summary>
    TyreSets,

    /// <summary>
    /// Extended motion data for player car
    /// </summary>
    MotionEx,

    /// <summary>
    /// Time trial specific data (new in F1 2024)
    /// </summary>
    TimeTrial,

    /// <summary>
    /// Lap positions of all cars at the start of each lap (new in F1 2025)
    /// </summary>
    LapPositions,

    /// <summary>
    /// Additional car telemetry data - active aero, overtake mode (new in F1 2026)
    /// </summary>
    CarTelemetry2
}