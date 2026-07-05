namespace F1Server.Core.Enumerations;

/// <summary>
/// Enumeration of types in event data packet
/// </summary>
public enum EventType
{
    /// <summary>
    /// Unknown event type
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Fastest lap achieved
    /// </summary>
    FastestLap,

    /// <summary>
    /// Driver retired
    /// </summary>
    Retirement,

    /// <summary>
    /// Team mate in pit
    /// </summary>
    TeamMateInPit,

    /// <summary>
    /// Race winner
    /// </summary>
    RaceWinner,

    /// <summary>
    /// Penalty has been issued (since 2020)
    /// </summary>
    Penalty,

    /// <summary>
    /// Speed trap triggered (since 2020)
    /// </summary>
    SpeedTrap,

    /// <summary>
    /// Start lights (since 2021)
    /// </summary>
    StartLights,

    /// <summary>
    /// Drive through penalty served (since 2021)
    /// </summary>
    DriveThroughPenaltyServed,

    /// <summary>
    /// Stop and go penalty served (since 2021)
    /// </summary>
    StopAndGoPenaltyServed,

    /// <summary>
    /// Flashback activated (since 2021)
    /// </summary>
    Flashback,

    /// <summary>
    /// Buttons triggered (since 2021)
    /// </summary>
    Buttons,

    /// <summary>
    /// Red flag shown (since 2023)
    /// </summary>
    RedFlag,

    /// <summary>
    /// Overtake occurred (since 2023)
    /// </summary>
    Overtake,

    /// <summary>
    /// Safety car deployed (since 2023)
    /// </summary>
    SafetyCar,

    /// <summary>
    /// Collision occurred (since 2023)
    /// </summary>
    Collision,

    /// <summary>
    /// DRS disabled (since 2023)
    /// </summary>
    DrsDisabled
}