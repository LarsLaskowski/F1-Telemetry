namespace F1Server.WebApi.Controllers;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="FinalClassificationController"/>
/// </summary>
internal static partial class FinalClassificationControllerLog
{
    #region Methods

    /// <summary>
    /// Logs the start of the final classification loading for a session
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Get final classification for session {SessionId}...")]
    public static partial void LoadingFinalClassification(this ILogger logger, long? sessionId);

    /// <summary>
    /// Logs an error while loading the final classifications
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Exception while loading final classifications!")]
    public static partial void ErrorLoadingFinalClassifications(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs the number of loaded final classifications
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="count">Number of loaded final classifications</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Final classification for session loaded: {Count}")]
    public static partial void FinalClassificationLoaded(this ILogger logger, int count);

    #endregion // Methods
}