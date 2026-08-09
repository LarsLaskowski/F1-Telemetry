namespace F1Server.WebApi.Controllers;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="GamesController"/>
/// </summary>
internal static partial class GamesControllerLog
{
    #region Methods

    /// <summary>
    /// Logs the start of the game loading
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Games loading...")]
    public static partial void LoadingGames(this ILogger logger);

    /// <summary>
    /// Logs the number of loaded games
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="loadedGames">Number of loaded games</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Games loaded ({LoadedGames}).")]
    public static partial void GamesLoaded(this ILogger logger, int loadedGames);

    #endregion // Methods
}