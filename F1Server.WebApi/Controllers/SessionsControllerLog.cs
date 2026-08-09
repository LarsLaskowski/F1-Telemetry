namespace F1Server.WebApi.Controllers;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="SessionsController"/>
/// </summary>
internal static partial class SessionsControllerLog
{
    #region Methods

    /// <summary>
    /// Logs the start of the session loading
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Sessions loading...")]
    public static partial void LoadingSessions(this ILogger logger);

    /// <summary>
    /// Logs an error while loading the sessions
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Exception while loading sessions!")]
    public static partial void ErrorLoadingSessions(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs the paging result of the session loading
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="pageIndex">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sessions">Number of found sessions</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Sessions loaded for page {PageIndex} - page size: {PageSize} - total sessions: {Sessions}")]
    public static partial void SessionsLoaded(this ILogger logger, int pageIndex, int pageSize, int? sessions);

    /// <summary>
    /// Logs the start of the session count determination
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Sessions count...")]
    public static partial void CountingSessions(this ILogger logger);

    /// <summary>
    /// Logs the number of found sessions
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessions">Number of found sessions</param>
    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Sessions found ({Sessions}).")]
    public static partial void SessionsCounted(this ILogger logger, int sessions);

    /// <summary>
    /// Logs the start of the last finished session loading
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Get last finished session...")]
    public static partial void LoadingLastFinishedSession(this ILogger logger);

    /// <summary>
    /// Logs an error while reading the last finished session
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "Exception while reading last finished session!")]
    public static partial void ErrorLoadingLastFinishedSession(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs the id of the last finished session
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="lastFinishedSession">Id of the last finished session</param>
    [LoggerMessage(EventId = 8, Level = LogLevel.Information, Message = "Last finished session: {LastFinishedSession}.")]
    public static partial void LastFinishedSessionLoaded(this ILogger logger, long lastFinishedSession);

    /// <summary>
    /// Logs the start of the loading of a single session
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 9, Level = LogLevel.Information, Message = "Session loading ({SessionId})...")]
    public static partial void LoadingSession(this ILogger logger, long sessionId);

    /// <summary>
    /// Logs a missing session id
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 10, Level = LogLevel.Warning, Message = "Session id is null or zero!")]
    public static partial void SessionIdNullOrZero(this ILogger logger);

    /// <summary>
    /// Logs whether a single session was loaded
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionLoaded">Session loaded?</param>
    [LoggerMessage(EventId = 11, Level = LogLevel.Information, Message = "Session loaded ({SessionLoaded}).")]
    public static partial void SessionLoaded(this ILogger logger, bool sessionLoaded);

    /// <summary>
    /// Logs the start of the session loading for a track
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="trackId">Id of the track</param>
    [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "Load session for track {TrackId}...")]
    public static partial void LoadingSessionsOfTrack(this ILogger logger, long? trackId);

    /// <summary>
    /// Logs an error while loading the sessions of a track
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    /// <param name="trackId">Id of the track</param>
    [LoggerMessage(EventId = 13, Level = LogLevel.Error, Message = "Exception while loading sessions for track {TrackId}!")]
    public static partial void ErrorLoadingSessionsOfTrack(this ILogger logger, Exception exception, long? trackId);

    /// <summary>
    /// Logs the start of the fastest lap loading for a session
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 14, Level = LogLevel.Information, Message = "Load fastest lap of session {SessionId}...")]
    public static partial void LoadingFastestLapOfSession(this ILogger logger, long? sessionId);

    /// <summary>
    /// Logs the start of the session time table loading
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 15, Level = LogLevel.Information, Message = "Loading session time table ({SessionId})...")]
    public static partial void LoadingSessionTimeTable(this ILogger logger, long? sessionId);

    /// <summary>
    /// Logs an error while loading the session time table
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 16, Level = LogLevel.Error, Message = "Exception while loading session time table!")]
    public static partial void ErrorLoadingSessionTimeTable(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs the start of the session deletion
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 17, Level = LogLevel.Information, Message = "Delete session {SessionId}...")]
    public static partial void DeletingSession(this ILogger logger, long sessionId);

    /// <summary>
    /// Logs the result of the session deletion
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="deleted">Session deleted?</param>
    [LoggerMessage(EventId = 18, Level = LogLevel.Information, Message = "Session deleted: {Deleted}")]
    public static partial void SessionDeleted(this ILogger logger, bool deleted);

    #endregion // Methods
}