namespace F1Server.Db.Enumerations;

/// <summary>
/// Type of used database server
/// </summary>
public enum SqlServerType
{
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// MySQL/MariaDB
    /// </summary>
    MariaDb,

    /// <summary>
    /// Microsoft SQL-Server (Azure, LocalDb, other editions)
    /// </summary>
    MsSqlServer,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSql,

    /// <summary>
    /// In-Memory
    /// </summary>
    InMemory = 99
}