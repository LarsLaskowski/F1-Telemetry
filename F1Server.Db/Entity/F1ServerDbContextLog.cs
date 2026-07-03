using F1Server.Db.Enumerations;

using Microsoft.Extensions.Logging;

namespace F1Server.Db.Entity;

/// <summary>
/// Source-generated, high performance logger extension methods used by <see cref="F1ServerDbContext"/>
/// </summary>
internal static partial class F1ServerDbContextLog
{
    #region Methods

    /// <summary>
    /// Logs that an unsupported database server type was configured
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="dbServerType">The unsupported database server type</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Unsupported database server type: {DbServerType}")]
    public static partial void UnsupportedDatabaseServerType(this ILogger logger, SqlServerType dbServerType);

    /// <summary>
    /// Logs the MariaDB configuration
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="serverName">Name of the database server</param>
    /// <param name="database">Name of the database</param>
    /// <param name="user">Database user</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Configuring MariaDB with server: {ServerName}, database: {Database}, user: {User}")]
    public static partial void ConfiguringMariaDb(this ILogger logger, string serverName, string database, string user);

    /// <summary>
    /// Logs an error while connecting to a MariaDB database
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Error connecting to MariaDB database!")]
    public static partial void ErrorConnectingMariaDb(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs an error while parsing the database server version
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="dbServerVersion">The database server version that could not be parsed</param>
    [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "Error parsing database server version: {DbServerVersion}")]
    public static partial void ErrorParsingServerVersion(this ILogger logger, string dbServerVersion);

    /// <summary>
    /// Logs the Microsoft SQL Server configuration
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="server">Name of the database server</param>
    /// <param name="database">Name of the database</param>
    /// <param name="user">Database user</param>
    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Configuring Microsoft SQL Server with server: {Server}, database: {Database}, user: {User}")]
    public static partial void ConfiguringMicrosoftSql(this ILogger logger, string server, string database, string user);

    /// <summary>
    /// Logs an error while connecting to a Microsoft SQL Server database
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Error connecting to Microsoft SQL Server database!")]
    public static partial void ErrorConnectingMicrosoftSql(this ILogger logger, Exception exception);

    /// <summary>
    /// Logs the PostgreSQL configuration
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="serverName">Name of the database server</param>
    /// <param name="database">Name of the database</param>
    /// <param name="user">Database user</param>
    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Configuring PostgreSQL with server: {ServerName}, database: {Database}, user: {User}")]
    public static partial void ConfiguringPostgreSql(this ILogger logger, string serverName, string database, string user);

    /// <summary>
    /// Logs an error while connecting to a PostgreSQL database
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="exception">Exception that occurred</param>
    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "Error connecting to PostgreSQL database!")]
    public static partial void ErrorConnectingPostgreSql(this ILogger logger, Exception exception);

    #endregion // Methods
}