namespace F1Server.WebApi.Controllers;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="FastestLapController"/>
/// </summary>
internal static partial class FastestLapControllerLog
{
    #region Methods

    /// <summary>
    /// Logs the start of the fastest lap loading for a track
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="trackId">Id of the track</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Fastest laps of track {TrackId}...")]
    public static partial void LoadingFastestLapsOfTrack(this ILogger logger, long? trackId);

    /// <summary>
    /// Logs the number of loaded fastest laps
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="fastestLaps">Number of loaded fastest laps</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Fastest laps of track loaded ({FastestLaps})")]
    public static partial void FastestLapsOfTrackLoaded(this ILogger logger, int fastestLaps);

    #endregion // Methods
}