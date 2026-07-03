namespace F1Server.Core.Enumerations;

/// <summary>
/// Ruleset enumeration
/// </summary>
public enum RuleSet
{
    /// <summary>
    /// No ruleset - set in older versions lower than 2022
    /// </summary>
    None = -1,

    /// <summary>
    /// Practice and Qualifying
    /// </summary>
    PracticeQualifying = 0,

    /// <summary>
    /// Race
    /// </summary>
    Race,

    /// <summary>
    /// Time trial
    /// </summary>
    TimeTrial,

    /// <summary>
    /// Time attack
    /// </summary>
    TimeAttack = 4,

    /// <summary>
    /// Checkpoint challenge
    /// </summary>
    CheckpointChallenge = 6,

    /// <summary>
    /// Autocross
    /// </summary>
    Autocross = 8,

    /// <summary>
    /// Drift
    /// </summary>
    Drift,

    /// <summary>
    /// Average speed zone
    /// </summary>
    AverageSpeedZone,

    /// <summary>
    /// Rival duel
    /// </summary>
    RivalDuel,

    /// <summary>
    /// Elimination
    /// </summary>
    Elimination
}