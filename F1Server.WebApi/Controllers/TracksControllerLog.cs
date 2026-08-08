namespace F1Server.WebApi.Controllers;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="TracksController"/>
/// </summary>
internal static partial class TracksControllerLog
{
    #region Methods

    /// <summary>
    /// Logs the start of the track loading
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Tracks loading...")]
    public static partial void LoadingTracks(this ILogger logger);

    /// <summary>
    /// Logs the number of loaded tracks
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="tracksLoaded">Number of loaded tracks</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Tracks loaded ({TracksLoaded}).")]
    public static partial void TracksLoaded(this ILogger logger, int? tracksLoaded);

    /// <summary>
    /// Logs the start of the loading of a single track
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="trackId">Id of the track</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Track loading ({TrackId})...")]
    public static partial void LoadingTrack(this ILogger logger, long trackId);

    /// <summary>
    /// Logs whether a single track was loaded
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="trackLoaded">Track loaded?</param>
    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Track loaded ({TrackLoaded}).")]
    public static partial void TrackLoaded(this ILogger logger, bool trackLoaded);

    #endregion // Methods
}