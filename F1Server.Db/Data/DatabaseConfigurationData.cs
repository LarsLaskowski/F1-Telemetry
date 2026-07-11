using F1Server.Core.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace F1Server.Db.Data;

/// <summary>
/// Data class with options configuring the database connection and schema
/// </summary>
internal class DatabaseConfigurationData
{
    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="DbContextOptionsBuilder"/> used to configure the database context
    /// </summary>
    public DbContextOptionsBuilder OptionsBuilder { get; set; }

    /// <summary>
    /// Gets or sets the name of the database to connect to
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// Gets or sets the hostname or IP address of the database server
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    /// Gets or sets the username used to authenticate with the database server
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets or sets the password used to authenticate with the database server
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to trust the server's SSL certificate without validation
    /// </summary>
    public bool TrustServerCertificate { get; set; }

    /// <summary>
    /// Gets or sets the logger used for logging database-related events and messages
    /// </summary>
    public ILogger? Logger { get; set; }

    /// <summary>
    /// Gets or sets the metrics collector used for collecting and reporting database-related metrics
    /// </summary>
    public IAppMetrics? AppMetrics { get; set; } = null;

    #endregion // Properties
}