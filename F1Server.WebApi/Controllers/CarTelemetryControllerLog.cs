namespace F1Server.WebApi.Controllers;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="CarTelemetryController"/>
/// </summary>
internal static partial class CarTelemetryControllerLog
{
    #region Methods

    /// <summary>
    /// Logs the start of the check for available user telemetry data
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Id of the session</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Exists user telemetry data for session {SessionId}...")]
    public static partial void CheckingUserTelemetryData(this ILogger logger, long sessionId);

    /// <summary>
    /// Logs the result of the check for available user telemetry data
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="hasUserTelemetryData">User telemetry data available?</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "User telemetry data exists: {HasUserTelemetryData}...")]
    public static partial void UserTelemetryDataChecked(this ILogger logger, bool hasUserTelemetryData);

    /// <summary>
    /// Logs the start of the telemetry value loading for a lap
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="lapId">Id of the lap</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Telemetry values for lap: {LapId}...")]
    public static partial void LoadingTelemetryValuesOfLap(this ILogger logger, long lapId);

    /// <summary>
    /// Logs the start of the telemetry value loading for the fastest lap of a participant
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="participantId">Id of the participant</param>
    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Telemetry values for participant of fastest lap: {ParticipantId}...")]
    public static partial void LoadingTelemetryValuesOfFastestLap(this ILogger logger, long participantId);

    /// <summary>
    /// Logs the number of loaded telemetry values
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="loadedValues">Number of loaded telemetry values</param>
    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Telemetry values loaded: ({LoadedValues})")]
    public static partial void TelemetryValuesLoaded(this ILogger logger, int loadedValues);

    #endregion // Methods
}