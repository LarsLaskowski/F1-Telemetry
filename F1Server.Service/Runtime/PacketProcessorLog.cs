using F1Server.Core.Enumerations;

using Microsoft.Extensions.Logging;

namespace F1Server.Service.Runtime;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="PacketProcessor"/>
/// </summary>
internal static partial class PacketProcessorLog
{
    #region Methods

    /// <summary>
    /// Logs an error while reading the packet timestamp
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Error getting packet timestamp!")]
    public static partial void ErrorGettingPacketTimestamp(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs an error while processing a packet
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    /// <param name="packetType">Type of the processed packet</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error processing packet {PacketType}!")]
    public static partial void ErrorProcessingPacket(this ILogger logger, Exception exception, PacketTypes? packetType);

    /// <summary>
    /// Logs an error thrown by a subscriber of the packet received event
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Error in packet received event subscriber!")]
    public static partial void ErrorInPacketReceivedEvent(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs a rejected packet header
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="rejectionCode">Reason for the rejection</param>
    /// <param name="packetLength">Length of the rejected packet</param>
    /// <param name="gameVersion">Game version reported by the packet header</param>
    [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "Rejected packet header: {RejectionCode}, packet length {PacketLength} bytes, reported game version {GameVersion}")]
    public static partial void RejectedPacketHeader(this ILogger logger, HeaderRejectionCode rejectionCode, int packetLength, ushort gameVersion);

    /// <summary>
    /// Logs an error while parsing a packet header
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Error parsing packet header!")]
    public static partial void ErrorParsingPacketHeader(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs an error reported by the packet analyzer
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="packetType">Type of the analyzed packet</param>
    /// <param name="lastError">Error reported by the packet analyzer</param>
    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Error analyzing packet {PacketType}: {LastError}")]
    public static partial void AnalyzerError(this ILogger logger, PacketTypes? packetType, string lastError);

    /// <summary>
    /// Logs an error while analyzing a packet
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    /// <param name="packetType">Type of the analyzed packet</param>
    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "Error analyzing packet {PacketType}!")]
    public static partial void ErrorAnalyzingPacket(this ILogger logger, Exception exception, PacketTypes? packetType);

    /// <summary>
    /// Logs a flashback event without valid event details
    /// </summary>
    /// <param name="logger">Logger instance</param>
    [LoggerMessage(EventId = 8, Level = LogLevel.Warning, Message = "Received flashback event, but no valid event details found!")]
    public static partial void FlashbackEventWithoutDetails(this ILogger logger);

    /// <summary>
    /// Logs an error while checking the game version
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    /// <param name="gameVersion">Checked game version</param>
    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "Error checking game version {GameVersion}!")]
    public static partial void ErrorCheckingGameVersion(this ILogger logger, Exception exception, int gameVersion);

    /// <summary>
    /// Logs an error while removing an invalid session from the database
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    /// <param name="sessionDbId">Database id of the session</param>
    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "Error removing invalid session from database - session: {SessionDbId}!")]
    public static partial void ErrorRemovingInvalidSession(this ILogger logger, Exception exception, long? sessionDbId);

    /// <summary>
    /// Logs the start of a new session
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sessionId">Unique id of the session</param>
    [LoggerMessage(EventId = 11, Level = LogLevel.Information, Message = "New session started with id {SessionId}")]
    public static partial void NewSessionStarted(this ILogger logger, ulong sessionId);

    #endregion // Methods
}