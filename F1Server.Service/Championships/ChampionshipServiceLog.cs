using Microsoft.Extensions.Logging;

namespace F1Server.Service.Championships;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="ChampionshipService"/>
/// </summary>
internal static partial class ChampionshipServiceLog
{
    #region Methods

    /// <summary>
    /// Logs an unknown track number while creating a championship
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="track">Unknown track number</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "[CreateChampionship] Unknown track number {track}!")]
    public static partial void UnknownTrackNumber(this ILogger logger, long track);

    #endregion // Methods
}