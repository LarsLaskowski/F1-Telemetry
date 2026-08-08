namespace F1Server.WebApi.Controllers;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="ChampionshipController"/>
/// </summary>
internal static partial class ChampionshipControllerLog
{
    #region Methods

    /// <summary>
    /// Logs the start of the championship loading
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Championschips loading...")]
    public static partial void LoadingChampionships(this ILogger logger);

    /// <summary>
    /// Logs the number of loaded championships
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="championshipsLoaded">Number of loaded championships</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Championship loaded ({ChampionshipsLoaded}).")]
    public static partial void ChampionshipsLoaded(this ILogger logger, int championshipsLoaded);

    /// <summary>
    /// Logs the start of the championship creation
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Create championship...")]
    public static partial void CreatingChampionship(this ILogger logger);

    /// <summary>
    /// Logs the start of the check for an active championship
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Exists an active championship for this session {SessionId}...")]
    public static partial void CheckingActiveChampionship(this ILogger logger, long sessionId);

    /// <summary>
    /// Logs the result of the check for an active championship
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="activeChampionship">Id of the active championship</param>
    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Active championship exists: {ActiveChampionship}...")]
    public static partial void ActiveChampionshipChecked(this ILogger logger, long activeChampionship);

    /// <summary>
    /// Logs the start of the check whether a session is active in a championship
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    /// <param name="championshipId">Id of the championship</param>
    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Checking session {SessionId} is active in championship {ChampionshipId}...")]
    public static partial void CheckingSessionActiveInChampionship(this ILogger logger, long sessionId, long championshipId);

    /// <summary>
    /// Logs the result of the check whether a session is active in a championship
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionIsActive">Session active in the championship?</param>
    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Active session in championship exists: {SessionIsActive}...")]
    public static partial void SessionActiveInChampionshipChecked(this ILogger logger, bool sessionIsActive);

    /// <summary>
    /// Logs that a session is added to a championship
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 8, Level = LogLevel.Information, Message = "Adding session {SessionId} to championship...")]
    public static partial void AddingSessionToChampionship(this ILogger logger, long sessionId);

    /// <summary>
    /// Logs an unknown track number while creating a championship
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="track">Unknown track number</param>
    [LoggerMessage(EventId = 9, Level = LogLevel.Warning, Message = "[CreateChampionship] Unknown track number {track}!")]
    public static partial void UnknownTrackNumber(this ILogger logger, long track);

    #endregion // Methods
}