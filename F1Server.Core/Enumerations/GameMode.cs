namespace F1Server.Core.Enumerations;

/// <summary>
/// Game mode enumeration
/// </summary>
public enum GameMode
{
    /// <summary>
    /// No game mode - set for older versions lower than 2022
    /// </summary>
    None = -1,

    /// <summary>
    /// Event mode
    /// </summary>
    EventMode = 0,

    /// <summary>
    /// Grand Prix
    /// </summary>
    GrandPrix = 3,

    /// <summary>
    /// Grand Prix F1 2023
    /// </summary>
    GrandPrix23,

    /// <summary>
    /// Time trial
    /// </summary>
    TimeTrial,

    /// <summary>
    /// Splitscreen
    /// </summary>
    Splitscreen,

    /// <summary>
    /// Online custom
    /// </summary>
    OnlineCustom,

    /// <summary>
    /// Online league
    /// </summary>
    OnlineLeague,

    /// <summary>
    /// Career invitational
    /// </summary>
    CareerInvitational = 11,

    /// <summary>
    /// Championship invitational
    /// </summary>
    ChampionshipInvitational,

    /// <summary>
    /// Championship
    /// </summary>
    Championship,

    /// <summary>
    /// Online championship
    /// </summary>
    OnlineChampionship,

    /// <summary>
    /// Online weekly event
    /// </summary>
    OnlineWeeklyEvent,

    /// <summary>
    /// Story mode
    /// </summary>
    StoryMode = 17,

    /// <summary>
    /// Career 2022
    /// </summary>
    Career22 = 19,

    /// <summary>
    /// Career 2022 online
    /// </summary>
    Career22Online,

    /// <summary>
    /// Career 2023
    /// </summary>
    Career23,

    /// <summary>
    /// Career 2023 online
    /// </summary>
    Career23Online,

    /// <summary>
    /// Driver career 2024
    /// </summary>
    DriverCareer24,

    /// <summary>
    /// Career 2024 online
    /// </summary>
    Career24Online,

    /// <summary>
    /// My team career 2024
    /// </summary>
    MyTeamCareer24,

    /// <summary>
    /// Curated career 2024
    /// </summary>
    CuratedCareer24,

    /// <summary>
    /// My team career 2025
    /// </summary>
    MyTeamCareer25,

    /// <summary>
    /// Driver career 2025
    /// </summary>
    DriverCareer25,

    /// <summary>
    /// Career 2025 online
    /// </summary>
    Career25Online,

    /// <summary>
    /// Challenge career 2025
    /// </summary>
    ChallengeCareer25,

    /// <summary>
    /// Story mode (APXGP)
    /// </summary>
    StoryModeApxgp = 75,

    /// <summary>
    /// Benchmark
    /// </summary>
    Benchmark = 127
}