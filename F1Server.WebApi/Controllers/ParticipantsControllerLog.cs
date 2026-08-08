namespace F1Server.WebApi.Controllers;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="ParticipantsController"/>
/// </summary>
internal static partial class ParticipantsControllerLog
{
    #region Methods

    /// <summary>
    /// Logs the start of the participant loading for a session
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Load participants for session {SessionId}...")]
    public static partial void LoadingParticipants(this ILogger logger, long? sessionId);

    /// <summary>
    /// Logs the number of loaded participants for a session
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="participants">Number of loaded participants</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Loaded {Participants} participants for session {SessionId}.")]
    public static partial void ParticipantsLoaded(this ILogger logger, int participants, long? sessionId);

    #endregion // Methods
}