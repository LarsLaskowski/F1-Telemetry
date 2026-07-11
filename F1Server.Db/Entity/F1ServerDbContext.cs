using F1Server.Core.Data;
using F1Server.Core.Exceptions;
using F1Server.Core.Interfaces;
using F1Server.Data;
using F1Server.Db.Data;
using F1Server.Db.Entity.Tables;
using F1Server.Db.Enumerations;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MySqlConnector;

using Npgsql;

namespace F1Server.Db.Entity;

/// <summary>
/// Accessing the database
/// </summary>
public sealed class F1ServerDbContext : DbContext
{
    #region Constants

    /// <summary>
    /// Default host name used when no database host is configured
    /// </summary>
    private const string Localhost = "localhost";

    #endregion // Constants

    #region Fields

    /// <summary>
    /// Single stateless command interceptor instance shared by all context instances
    /// </summary>
    private static readonly CommandInterceptor _commandInterceptor = new CommandInterceptor();

    /// <summary>
    /// Indicates whether the database configuration has already been logged (0 = not yet, 1 = logged)
    /// </summary>
    private static int _configurationLogged;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="F1ServerDbContext"/> class
    /// </summary>
    public F1ServerDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="F1ServerDbContext"/> class from pre-built options.
    /// This constructor is used by the context pool behind <see cref="RepositoryFactory"/>
    /// </summary>
    /// <param name="options">Pre-built context options</param>
    public F1ServerDbContext(DbContextOptions<F1ServerDbContext> options)
        : base(options)
    {
        var applicationData = ResolveApplicationData(RepositoryFactory.ServiceProvider);

        AppMetrics = applicationData?.AppMetrics;
        Logger = applicationData?.Logger;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Version information from used database server
    /// </summary>
    public static string? DbServerVersion { get; private set; }

    /// <summary>
    /// Used sql server type
    /// </summary>
    public static SqlServerType DbServerType { get; private set; }

    /// <summary>
    /// Connection string
    /// </summary>
    public static string ConnectionString { get; private set; }

    /// <summary>
    /// Access to application metrics for tracking performance and errors
    /// </summary>
    public IAppMetrics? AppMetrics { get; }

    /// <summary>
    /// Gets the logger instance used for logging messages and events
    /// </summary>
    public ILogger? Logger { get; }

    /// <summary>
    /// Last error
    /// </summary>
    public string? LastError
    {
        get;
        set
        {
            field = value;

            if (string.IsNullOrWhiteSpace(value) == false)
            {
                AppMetrics?.DbErrorCount.Add(1, new KeyValuePair<string, object?>("LastError", field));
            }
        }
    }

    #endregion // Properties

    #region Static methods

    /// <summary>
    /// Builds the context options shared by all pooled context instances. The database configuration
    /// is read from the environment variables once and reused for every pooled context
    /// </summary>
    /// <param name="serviceProvider">Service provider used to resolve logging and metrics; may be null</param>
    /// <returns>Configured context options</returns>
    internal static DbContextOptions<F1ServerDbContext> BuildOptions(IServiceProvider? serviceProvider)
    {
        var applicationData = ResolveApplicationData(serviceProvider);
        var optionsBuilder = new DbContextOptionsBuilder<F1ServerDbContext>();

        ConfigureOptions(optionsBuilder, applicationData?.Logger, applicationData?.AppMetrics);

        return optionsBuilder.Options;
    }

    #endregion // Static methods

    #region Private methods

    /// <summary>
    /// Determines in a thread-safe way whether the database configuration should be logged.
    /// Returns true only for the first call within the process lifetime
    /// </summary>
    /// <returns>True if the configuration has not been logged yet, otherwise false</returns>
    private static bool ShouldLogConfiguration()
    {
        return Interlocked.CompareExchange(ref _configurationLogged, 1, 0) == 0;
    }

    /// <summary>
    /// Resolves the shared application data from the given service provider
    /// </summary>
    /// <param name="serviceProvider">Service provider; may be null</param>
    /// <returns>Application data or null if it cannot be resolved</returns>
    private static F1ServerApplicationData? ResolveApplicationData(IServiceProvider? serviceProvider)
    {
        try
        {
            return serviceProvider?.GetRequiredService<F1ServerApplicationData>();
        }
        catch
        {
            // Ignore exceptions in this step, as it may not be critical for the context initialization
            return null;
        }
    }

    /// <summary>
    /// Detects the configured database server type and configures the matching provider options
    /// </summary>
    /// <param name="optionsBuilder">Options builder</param>
    /// <param name="logger">Logger used for configuration messages</param>
    /// <param name="appMetrics">Application metrics used for error counting</param>
    /// <exception cref="NotSupportedException">Thrown when the detected server type is not supported</exception>
    private static void ConfigureOptions(DbContextOptionsBuilder optionsBuilder, ILogger? logger, IAppMetrics? appMetrics)
    {
        DbServerType = DetectServerType();

        if (DbServerType != SqlServerType.Unknown)
        {
            var database = Environment.GetEnvironmentVariable("F1SERVER_DB_NAME");
            var server = Environment.GetEnvironmentVariable("F1SERVER_DB_HOST");
            var userId = Environment.GetEnvironmentVariable("F1SERVER_DB_USER");
            var passwd = Environment.GetEnvironmentVariable("F1SERVER_DB_PASSWORD");

            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = "f1telemetry";
            }

            if (string.IsNullOrWhiteSpace(database))
            {
                database = "f1telemetry";
            }

            var dbConfigOptions = new DatabaseConfigurationData
                                  {
                                      OptionsBuilder = optionsBuilder,
                                      Database = database,
                                      Server = server ?? Localhost,
                                      User = userId,
                                      Password = passwd ?? string.Empty,
                                      Logger = logger,
                                      AppMetrics = appMetrics
                                  };

            if (DbServerType == SqlServerType.MariaDb)
            {
                ConfigureMariaDb(dbConfigOptions);
            }
            else if (DbServerType == SqlServerType.MsSqlServer)
            {
                var trustServerCertificate = Environment.GetEnvironmentVariable("F1SERVER_DB_MSSQL_TRUST_SERVER_CERTIFICATE");

                dbConfigOptions.TrustServerCertificate = bool.TryParse(trustServerCertificate, out var parsedTrustServerCertificate) == false || parsedTrustServerCertificate;

                ConfigureMicrosoftSql(dbConfigOptions);
            }
            else if (DbServerType == SqlServerType.PostgreSql)
            {
                ConfigurePostgreSql(dbConfigOptions);
            }
            else if (DbServerType == SqlServerType.InMemory)
            {
                ConfigureInMemory(dbConfigOptions);
            }
            else
            {
                logger?.UnsupportedDatabaseServerType(DbServerType);

                throw new NotSupportedException($"The {DbServerType} is not supported!");
            }
        }

        optionsBuilder.AddInterceptors(_commandInterceptor);
    }

    /// <summary>
    /// Detect sql server type mode
    /// </summary>
    /// <returns>Detected sql server type</returns>
    /// <exception cref="DbException">Unknown mode</exception>
    private static SqlServerType DetectServerType()
    {
        var serverType = Environment.GetEnvironmentVariable("F1SERVER_DATABASE_TYPE");

        // Not set? Set default to MariaDb
        if (string.IsNullOrWhiteSpace(serverType))
        {
            serverType = "1";
        }

        return serverType switch
               {
                   "1" => SqlServerType.MariaDb,
                   "2" => SqlServerType.MsSqlServer,
                   "3" => SqlServerType.PostgreSql,
                   "99" => SqlServerType.InMemory,
                   _ => throw new DbException($"Unknown sql server type or not specified - detected value: {serverType}"),
               };
    }

    /// <summary>
    /// Configure MariaDB connection
    /// </summary>
    /// <param name="databaseConfiguration">Database configuration data</param>
    private static void ConfigureMariaDb(DatabaseConfigurationData databaseConfiguration)
    {
        var serverName = databaseConfiguration.Server;
        uint serverPort = 3306;
        ServerVersion? dbServerVersion = null;

        if (string.IsNullOrWhiteSpace(serverName) == false && serverName.Contains(':'))
        {
            var serverSplit = serverName.Split(':');

            serverName = serverSplit[0];

            if (uint.TryParse(serverSplit[1], out serverPort) == false)
            {
                serverPort = 3306;
            }
        }

        if (databaseConfiguration.Logger is not null && ShouldLogConfiguration())
        {
            databaseConfiguration.Logger.ConfiguringMariaDb(serverName, databaseConfiguration.Database, databaseConfiguration.User);
        }

        var connectionStringBuilder = new MySqlConnectionStringBuilder
                                      {
                                          ApplicationName = "F1Server",
                                          Database = databaseConfiguration.Database,
                                          Port = serverPort,
                                          Server = serverName,
                                          UserID = databaseConfiguration.User,
                                          Password = databaseConfiguration.Password,
                                          UseCompression = true
                                      };

        ConnectionString = connectionStringBuilder.ConnectionString;

        if (string.IsNullOrEmpty(DbServerVersion))
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    DbServerVersion = connection.ServerVersion;

                    dbServerVersion = ServerVersion.Parse(connection.ServerVersion);
                }
                catch (Exception ex)
                {
                    RecordConfigurationError(databaseConfiguration.AppMetrics, ex);

                    databaseConfiguration.Logger?.ErrorConnectingMariaDb(ex);
                }
            }
        }
        else
        {
            if (ServerVersion.TryParse(DbServerVersion, out dbServerVersion) == false)
            {
                databaseConfiguration.Logger?.ErrorParsingServerVersion(DbServerVersion);
            }
        }

        databaseConfiguration.OptionsBuilder.UseMySql(ConnectionString,
                                                      dbServerVersion,
                                                      contextOptions =>
                                                      {
                                                          contextOptions.MigrationsAssembly("F1Server.Db.MySqlMigrations");
                                                          contextOptions.EnableRetryOnFailure();
                                                      });
    }

    /// <summary>
    /// Configure Microsoft SQL Server connection
    /// </summary>
    /// <param name="databaseConfiguration">Database configuration data</param>
    private static void ConfigureMicrosoftSql(DatabaseConfigurationData databaseConfiguration)
    {
        if (databaseConfiguration.Logger is not null && ShouldLogConfiguration())
        {
            databaseConfiguration.Logger.ConfiguringMicrosoftSql(databaseConfiguration.Server, databaseConfiguration.Database, databaseConfiguration.User);
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder
                                      {
                                          ApplicationName = "F1Server",
                                          DataSource = databaseConfiguration.Server,
                                          InitialCatalog = databaseConfiguration.Database,
                                          UserID = databaseConfiguration.User,
                                          Password = databaseConfiguration.Password,
                                          MultipleActiveResultSets = false,
                                          IntegratedSecurity = false,
                                          TrustServerCertificate = databaseConfiguration.TrustServerCertificate
                                      };

        ConnectionString = connectionStringBuilder.ConnectionString;

        // Gets the server version only if not set
        if (string.IsNullOrEmpty(DbServerVersion))
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    DbServerVersion = connection.ServerVersion;
                }
                catch (Exception ex)
                {
                    databaseConfiguration.Logger?.ErrorConnectingMicrosoftSql(ex);

                    RecordConfigurationError(databaseConfiguration.AppMetrics, ex);
                }
            }
        }

        databaseConfiguration.OptionsBuilder.UseSqlServer(ConnectionString,
                                                          contextOptions =>
                                                          {
                                                              contextOptions.MigrationsAssembly("F1Server.Db.MsSqlMigrations");
                                                              contextOptions.EnableRetryOnFailure();
                                                          });
    }

    /// <summary>
    /// Configure PostgreSQL connection
    /// </summary>
    /// <param name="databaseConfiguration">Database configuration data</param>
    private static void ConfigurePostgreSql(DatabaseConfigurationData databaseConfiguration)
    {
        var serverName = "localhost";
        uint serverPort = 5432;

        if (string.IsNullOrWhiteSpace(databaseConfiguration.Server) == false && databaseConfiguration.Server.Contains(':'))
        {
            var serverSplit = databaseConfiguration.Server.Split(':');

            serverName = serverSplit[0];

            if (uint.TryParse(serverSplit[1], out serverPort) == false)
            {
                serverPort = 5432;
            }
        }

        if (databaseConfiguration.Logger is not null && ShouldLogConfiguration())
        {
            databaseConfiguration.Logger.ConfiguringPostgreSql(serverName, databaseConfiguration.Database, databaseConfiguration.User);
        }

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
                                      {
                                          ApplicationName = "F1Server",
                                          Database = databaseConfiguration.Database,
                                          Host = serverName,
                                          Port = (int)serverPort,
                                          Username = databaseConfiguration.User,
                                          Password = databaseConfiguration.Password,
                                      };

        ConnectionString = connectionStringBuilder.ConnectionString;

        // Gets the server version only if not set
        if (string.IsNullOrEmpty(DbServerVersion))
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    var sql = "SELECT version()";

                    using var cmd = new NpgsqlCommand(sql, connection);

                    DbServerVersion = cmd.ExecuteScalar()?.ToString();
                }
                catch (Exception ex)
                {
                    RecordConfigurationError(databaseConfiguration.AppMetrics, ex);

                    databaseConfiguration.Logger?.ErrorConnectingPostgreSql(ex);
                }
            }
        }

        databaseConfiguration.OptionsBuilder.UseNpgsql(ConnectionString,
                                                       contextOptions =>
                                                       {
                                                           contextOptions.MigrationsAssembly("F1Server.Db.PostgreSqlMigrations");
                                                           contextOptions.EnableRetryOnFailure();
                                                       });
    }

    /// <summary>
    /// Configure InMemory connection for testing only
    /// </summary>
    /// <param name="databaseConfiguration">Database configuration data</param>
    private static void ConfigureInMemory(DatabaseConfigurationData databaseConfiguration)
    {
        var testAssembly = "F1Server.Tests, ";

        if (Array.Exists(AppDomain.CurrentDomain.GetAssemblies(), a => a.FullName?.StartsWith(testAssembly, StringComparison.OrdinalIgnoreCase) == true))
        {
            databaseConfiguration.OptionsBuilder.UseInMemoryDatabase("F1TelemetryTest")
                                                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        }
        else
        {
            throw new InvalidOperationException("InMemory database can only used in unit tests!");
        }
    }

    /// <summary>
    /// Records a failed connection attempt during configuration in the application metrics
    /// </summary>
    /// <param name="appMetrics">Application metrics; may be null</param>
    /// <param name="exception">Exception that occurred</param>
    private static void RecordConfigurationError(IAppMetrics? appMetrics, Exception exception)
    {
        appMetrics?.DbErrorCount.Add(1, new KeyValuePair<string, object?>("LastError", exception.ToString()));
    }

    /// <summary>
    /// Insert tracks into database
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    private void SeedTracks(ModelBuilder modelBuilder)
    {
        try
        {
            modelBuilder?.Entity<TrackEntity>()
                        .HasData(new TrackEntity
                                 {
                                     Id = 1,
                                     TrackNumber = 0,
                                     Name = SeedNames.Melbourne,
                                     LapReferenceTime = 76086,
                                     Sector1ReferenceTime = 26213,
                                     Sector2ReferenceTime = 17547,
                                     Sector3ReferenceTime = 32326
                                 },
                                 new TrackEntity
                                 {
                                     Id = 2,
                                     TrackNumber = 1,
                                     Name = SeedNames.PaulRicard,
                                     LapReferenceTime = 88384,
                                     Sector1ReferenceTime = 21725,
                                     Sector2ReferenceTime = 27336,
                                     Sector3ReferenceTime = 39323
                                 },
                                 new TrackEntity
                                 {
                                     Id = 3,
                                     TrackNumber = 2,
                                     Name = SeedNames.Shanghai,
                                     LapReferenceTime = 90098,
                                     Sector1ReferenceTime = 23633,
                                     Sector2ReferenceTime = 27041,
                                     Sector3ReferenceTime = 39424
                                 },
                                 new TrackEntity
                                 {
                                     Id = 4,
                                     TrackNumber = 3,
                                     Name = SeedNames.SakhirBahrain,
                                     LapReferenceTime = 87550,
                                     Sector1ReferenceTime = 28228,
                                     Sector2ReferenceTime = 37605,
                                     Sector3ReferenceTime = 21717
                                 },
                                 new TrackEntity
                                 {
                                     Id = 5,
                                     TrackNumber = 4,
                                     Name = SeedNames.Catalunya,
                                     LapReferenceTime = 70490,
                                     Sector1ReferenceTime = 21268,
                                     Sector2ReferenceTime = 28388,
                                     Sector3ReferenceTime = 20834
                                 },
                                 new TrackEntity
                                 {
                                     Id = 6,
                                     TrackNumber = 5,
                                     Name = SeedNames.Monaco,
                                     LapReferenceTime = 69897,
                                     Sector1ReferenceTime = 18286,
                                     Sector2ReferenceTime = 33560,
                                     Sector3ReferenceTime = 18051
                                 },
                                 new TrackEntity
                                 {
                                     Id = 7,
                                     TrackNumber = 6,
                                     Name = SeedNames.Montreal,
                                     LapReferenceTime = 68744,
                                     Sector1ReferenceTime = 19103,
                                     Sector2ReferenceTime = 21748,
                                     Sector3ReferenceTime = 27893
                                 },
                                 new TrackEntity
                                 {
                                     Id = 8,
                                     TrackNumber = 7,
                                     Name = SeedNames.Silverstone,
                                     LapReferenceTime = 84532,
                                     Sector1ReferenceTime = 26940,
                                     Sector2ReferenceTime = 34630,
                                     Sector3ReferenceTime = 22962
                                 },
                                 new TrackEntity
                                 {
                                     Id = 9,
                                     TrackNumber = 8,
                                     Name = SeedNames.Hockenheim,
                                     LapReferenceTime = 70963,
                                     Sector1ReferenceTime = 15279,
                                     Sector2ReferenceTime = 34159,
                                     Sector3ReferenceTime = 21525
                                 },
                                 new TrackEntity
                                 {
                                     Id = 10,
                                     TrackNumber = 9,
                                     Name = SeedNames.Hungaroring,
                                     LapReferenceTime = 74551,
                                     Sector1ReferenceTime = 26892,
                                     Sector2ReferenceTime = 26319,
                                     Sector3ReferenceTime = 21340
                                 },
                                 new TrackEntity
                                 {
                                     Id = 11,
                                     TrackNumber = 10,
                                     Name = SeedNames.Spa,
                                     LapReferenceTime = 100331,
                                     Sector1ReferenceTime = 29592,
                                     Sector2ReferenceTime = 42794,
                                     Sector3ReferenceTime = 27945
                                 },
                                 new TrackEntity
                                 {
                                     Id = 12,
                                     TrackNumber = 11,
                                     Name = SeedNames.Monza,
                                     LapReferenceTime = 78401,
                                     Sector1ReferenceTime = 26003,
                                     Sector2ReferenceTime = 26278,
                                     Sector3ReferenceTime = 26120
                                 },
                                 new TrackEntity
                                 {
                                     Id = 13,
                                     TrackNumber = 12,
                                     Name = SeedNames.Singapore,
                                     LapReferenceTime = 94904,
                                     Sector1ReferenceTime = 26392,
                                     Sector2ReferenceTime = 36524,
                                     Sector3ReferenceTime = 31988
                                 },
                                 new TrackEntity
                                 {
                                     Id = 14,
                                     TrackNumber = 13,
                                     Name = SeedNames.Suzuka,
                                     LapReferenceTime = 86051,
                                     Sector1ReferenceTime = 30889,
                                     Sector2ReferenceTime = 38756,
                                     Sector3ReferenceTime = 16406
                                 },
                                 new TrackEntity
                                 {
                                     Id = 15,
                                     TrackNumber = 14,
                                     Name = SeedNames.AbuDhabi,
                                     LapReferenceTime = 81379,
                                     Sector1ReferenceTime = 17113,
                                     Sector2ReferenceTime = 35001,
                                     Sector3ReferenceTime = 29265
                                 },
                                 new TrackEntity
                                 {
                                     Id = 16,
                                     TrackNumber = 15,
                                     Name = SeedNames.Texas,
                                     LapReferenceTime = 91376,
                                     Sector1ReferenceTime = 25031,
                                     Sector2ReferenceTime = 36621,
                                     Sector3ReferenceTime = 29724
                                 },
                                 new TrackEntity
                                 {
                                     Id = 17,
                                     TrackNumber = 16,
                                     Name = SeedNames.Brazil,
                                     LapReferenceTime = 67039,
                                     Sector1ReferenceTime = 16654,
                                     Sector2ReferenceTime = 33806,
                                     Sector3ReferenceTime = 16579
                                 },
                                 new TrackEntity
                                 {
                                     Id = 18,
                                     TrackNumber = 17,
                                     Name = SeedNames.Austria,
                                     LapReferenceTime = 62994,
                                     Sector1ReferenceTime = 15716,
                                     Sector2ReferenceTime = 28174,
                                     Sector3ReferenceTime = 19054
                                 },
                                 new TrackEntity
                                 {
                                     Id = 19,
                                     TrackNumber = 18,
                                     Name = SeedNames.Sochi,
                                     LapReferenceTime = 89867,
                                     Sector1ReferenceTime = 32606,
                                     Sector2ReferenceTime = 31066,
                                     Sector3ReferenceTime = 26195
                                 },
                                 new TrackEntity
                                 {
                                     Id = 20,
                                     TrackNumber = 19,
                                     Name = SeedNames.Mexico,
                                     LapReferenceTime = 75181,
                                     Sector1ReferenceTime = 28221,
                                     Sector2ReferenceTime = 27944,
                                     Sector3ReferenceTime = 19016
                                 },
                                 new TrackEntity
                                 {
                                     Id = 21,
                                     TrackNumber = 20,
                                     Name = SeedNames.BakuAzerbaijan,
                                     LapReferenceTime = 99352,
                                     Sector1ReferenceTime = 35288,
                                     Sector2ReferenceTime = 40019,
                                     Sector3ReferenceTime = 24045
                                 },
                                 new TrackEntity
                                 {
                                     Id = 22,
                                     TrackNumber = 21,
                                     Name = SeedNames.SakhirShort,
                                     LapReferenceTime = 53252,
                                     Sector1ReferenceTime = 18566,
                                     Sector2ReferenceTime = 18474,
                                     Sector3ReferenceTime = 16212
                                 },
                                 new TrackEntity
                                 {
                                     Id = 23,
                                     TrackNumber = 22,
                                     Name = SeedNames.SilverstoneShort,
                                     LapReferenceTime = 51812,
                                     Sector1ReferenceTime = 10960,
                                     Sector2ReferenceTime = 16373,
                                     Sector3ReferenceTime = 24479
                                 },
                                 new TrackEntity
                                 {
                                     Id = 24,
                                     TrackNumber = 23,
                                     Name = SeedNames.TexasShort,
                                     LapReferenceTime = 30000,
                                     Sector1ReferenceTime = 10000,
                                     Sector2ReferenceTime = 10000,
                                     Sector3ReferenceTime = 10000
                                 },
                                 new TrackEntity
                                 {
                                     Id = 25,
                                     TrackNumber = 24,
                                     Name = SeedNames.SuzukaShort,
                                     LapReferenceTime = 30000,
                                     Sector1ReferenceTime = 10000,
                                     Sector2ReferenceTime = 10000,
                                     Sector3ReferenceTime = 10000
                                 },
                                 new TrackEntity
                                 {
                                     Id = 26,
                                     TrackNumber = 25,
                                     Name = SeedNames.Hanoi,
                                     LapReferenceTime = 93454,
                                     Sector1ReferenceTime = 25342,
                                     Sector2ReferenceTime = 40367,
                                     Sector3ReferenceTime = 27745
                                 },
                                 new TrackEntity
                                 {
                                     Id = 27,
                                     TrackNumber = 26,
                                     Name = SeedNames.Zandvoort,
                                     LapReferenceTime = 67834,
                                     Sector1ReferenceTime = 23711,
                                     Sector2ReferenceTime = 23428,
                                     Sector3ReferenceTime = 20695
                                 },
                                 new TrackEntity
                                 {
                                     Id = 28,
                                     TrackNumber = 27,
                                     Name = SeedNames.Imola,
                                     LapReferenceTime = 73311,
                                     Sector1ReferenceTime = 23564,
                                     Sector2ReferenceTime = 25323,
                                     Sector3ReferenceTime = 24515
                                 },
                                 new TrackEntity
                                 {
                                     Id = 29,
                                     TrackNumber = 28,
                                     Name = SeedNames.Portimao,
                                     LapReferenceTime = 75588,
                                     Sector1ReferenceTime = 21567,
                                     Sector2ReferenceTime = 29255,
                                     Sector3ReferenceTime = 24766
                                 },
                                 new TrackEntity
                                 {
                                     Id = 30,
                                     TrackNumber = 29,
                                     Name = SeedNames.Jeddah,
                                     LapReferenceTime = 85870,
                                     Sector1ReferenceTime = 31244,
                                     Sector2ReferenceTime = 27721,
                                     Sector3ReferenceTime = 26905
                                 },
                                 new TrackEntity
                                 {
                                     Id = 31,
                                     TrackNumber = 30,
                                     Name = SeedNames.Miami,
                                     LapReferenceTime = 85890,
                                     Sector1ReferenceTime = 29419,
                                     Sector2ReferenceTime = 31444,
                                     Sector3ReferenceTime = 25027
                                 },
                                 new TrackEntity
                                 {
                                     Id = 32,
                                     TrackNumber = 31,
                                     Name = SeedNames.LasVegas,
                                     LapReferenceTime = 90406,
                                     Sector1ReferenceTime = 43317,
                                     Sector2ReferenceTime = 25530,
                                     Sector3ReferenceTime = 21559
                                 },
                                 new TrackEntity
                                 {
                                     Id = 33,
                                     TrackNumber = 32,
                                     Name = SeedNames.Losail,
                                     LapReferenceTime = 79850,
                                     Sector1ReferenceTime = 26190,
                                     Sector2ReferenceTime = 26501,
                                     Sector3ReferenceTime = 27159
                                 },
                                 new TrackEntity
                                 {
                                     Id = 34,
                                     TrackNumber = 39,
                                     Name = SeedNames.SilverstoneReverse,
                                     LapReferenceTime = 0,
                                     Sector1ReferenceTime = 0,
                                     Sector2ReferenceTime = 0,
                                     Sector3ReferenceTime = 0
                                 },
                                 new TrackEntity
                                 {
                                     Id = 35,
                                     TrackNumber = 40,
                                     Name = SeedNames.AustriaReverse,
                                     LapReferenceTime = 0,
                                     Sector1ReferenceTime = 0,
                                     Sector2ReferenceTime = 0,
                                     Sector3ReferenceTime = 0
                                 },
                                 new TrackEntity
                                 {
                                     Id = 36,
                                     TrackNumber = 41,
                                     Name = SeedNames.ZandvoortReverse,
                                     LapReferenceTime = 0,
                                     Sector1ReferenceTime = 0,
                                     Sector2ReferenceTime = 0,
                                     Sector3ReferenceTime = 0
                                 },
                                 new TrackEntity
                                 {
                                     Id = 37,
                                     TrackNumber = 42,
                                     Name = SeedNames.Madrid,
                                     LapReferenceTime = 0,
                                     Sector1ReferenceTime = 0,
                                     Sector2ReferenceTime = 0,
                                     Sector3ReferenceTime = 0
                                 });
        }
        catch
        {
            // Ignore exceptions in this step
        }
    }

    /// <summary>
    /// Insert default drivers into database
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    private void SeedDrivers(ModelBuilder modelBuilder)
    {
        try
        {
            modelBuilder?.Entity<DriverEntity>()
                        .HasData(new DriverEntity
                                 {
                                     Id = 1,
                                     DriverGameId = 0,
                                     Name = SeedNames.CarlosSainz
                                 },
                                 new DriverEntity
                                 {
                                     Id = 2,
                                     DriverGameId = 1,
                                     Name = SeedNames.DaniilKvyat
                                 },
                                 new DriverEntity
                                 {
                                     Id = 3,
                                     DriverGameId = 2,
                                     Name = SeedNames.DanielRicciardo
                                 },
                                 new DriverEntity
                                 {
                                     Id = 4,
                                     DriverGameId = 3,
                                     Name = SeedNames.FernandoAlonso
                                 },
                                 new DriverEntity
                                 {
                                     Id = 5,
                                     DriverGameId = 4,
                                     Name = SeedNames.FelipeMassa
                                 },
                                 new DriverEntity
                                 {
                                     Id = 6,
                                     DriverGameId = 6,
                                     Name = SeedNames.KimiRaikkonen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 7,
                                     DriverGameId = 7,
                                     Name = SeedNames.LewisHamilton
                                 },
                                 new DriverEntity
                                 {
                                     Id = 8,
                                     DriverGameId = 9,
                                     Name = SeedNames.MaxVerstappen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 9,
                                     DriverGameId = 10,
                                     Name = SeedNames.NicoHulkenberg
                                 },
                                 new DriverEntity
                                 {
                                     Id = 10,
                                     DriverGameId = 11,
                                     Name = SeedNames.KevinMagnussen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 11,
                                     DriverGameId = 12,
                                     Name = SeedNames.RomainGrosjean
                                 },
                                 new DriverEntity
                                 {
                                     Id = 12,
                                     DriverGameId = 13,
                                     Name = SeedNames.SebastianVettel
                                 },
                                 new DriverEntity
                                 {
                                     Id = 13,
                                     DriverGameId = 14,
                                     Name = SeedNames.SergioPerez
                                 },
                                 new DriverEntity
                                 {
                                     Id = 14,
                                     DriverGameId = 15,
                                     Name = SeedNames.ValtteriBottas
                                 },
                                 new DriverEntity
                                 {
                                     Id = 15,
                                     DriverGameId = 17,
                                     Name = SeedNames.EstebanOcon
                                 },
                                 new DriverEntity
                                 {
                                     Id = 16,
                                     DriverGameId = 19,
                                     Name = SeedNames.LanceStroll
                                 },
                                 new DriverEntity
                                 {
                                     Id = 17,
                                     DriverGameId = 20,
                                     Name = SeedNames.ArronBarnes
                                 },
                                 new DriverEntity
                                 {
                                     Id = 18,
                                     DriverGameId = 21,
                                     Name = SeedNames.MartinGiles
                                 },
                                 new DriverEntity
                                 {
                                     Id = 19,
                                     DriverGameId = 22,
                                     Name = SeedNames.AlexMurray
                                 },
                                 new DriverEntity
                                 {
                                     Id = 20,
                                     DriverGameId = 23,
                                     Name = SeedNames.LucasRoth
                                 },
                                 new DriverEntity
                                 {
                                     Id = 21,
                                     DriverGameId = 24,
                                     Name = SeedNames.IgorCorreia
                                 },
                                 new DriverEntity
                                 {
                                     Id = 22,
                                     DriverGameId = 25,
                                     Name = SeedNames.SophieLevasseur
                                 },
                                 new DriverEntity
                                 {
                                     Id = 23,
                                     DriverGameId = 26,
                                     Name = SeedNames.JonasSchiffer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 24,
                                     DriverGameId = 27,
                                     Name = SeedNames.AlainForest
                                 },
                                 new DriverEntity
                                 {
                                     Id = 25,
                                     DriverGameId = 28,
                                     Name = SeedNames.JayLetourneau
                                 },
                                 new DriverEntity
                                 {
                                     Id = 26,
                                     DriverGameId = 29,
                                     Name = SeedNames.EstoSaari
                                 },
                                 new DriverEntity
                                 {
                                     Id = 27,
                                     DriverGameId = 30,
                                     Name = SeedNames.YasarAtiyeh
                                 },
                                 new DriverEntity
                                 {
                                     Id = 28,
                                     DriverGameId = 31,
                                     Name = SeedNames.CallistoCalabresi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 29,
                                     DriverGameId = 32,
                                     Name = SeedNames.NaotaIzumi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 30,
                                     DriverGameId = 33,
                                     Name = SeedNames.HowardClarke
                                 },
                                 new DriverEntity
                                 {
                                     Id = 31,
                                     DriverGameId = 34,
                                     Name = SeedNames.WileimKaufmann
                                 },
                                 new DriverEntity
                                 {
                                     Id = 32,
                                     DriverGameId = 35,
                                     Name = SeedNames.MarieLaursen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 33,
                                     DriverGameId = 36,
                                     Name = SeedNames.FlavioNieves
                                 },
                                 new DriverEntity
                                 {
                                     Id = 34,
                                     DriverGameId = 37,
                                     Name = SeedNames.PeterBelousov
                                 },
                                 new DriverEntity
                                 {
                                     Id = 35,
                                     DriverGameId = 38,
                                     Name = SeedNames.KlimekMichalski
                                 },
                                 new DriverEntity
                                 {
                                     Id = 36,
                                     DriverGameId = 39,
                                     Name = SeedNames.SantiagoMoreno
                                 },
                                 new DriverEntity
                                 {
                                     Id = 37,
                                     DriverGameId = 40,
                                     Name = SeedNames.BenjaminCoppens
                                 },
                                 new DriverEntity
                                 {
                                     Id = 38,
                                     DriverGameId = 41,
                                     Name = SeedNames.NoahVisser
                                 },
                                 new DriverEntity
                                 {
                                     Id = 39,
                                     DriverGameId = 42,
                                     Name = SeedNames.GertWaldmuller
                                 },
                                 new DriverEntity
                                 {
                                     Id = 40,
                                     DriverGameId = 43,
                                     Name = SeedNames.JulianQuesada
                                 },
                                 new DriverEntity
                                 {
                                     Id = 41,
                                     DriverGameId = 44,
                                     Name = SeedNames.DanielJones
                                 },
                                 new DriverEntity
                                 {
                                     Id = 42,
                                     DriverGameId = 45,
                                     Name = SeedNames.ArtemMarkelov
                                 },
                                 new DriverEntity
                                 {
                                     Id = 43,
                                     DriverGameId = 46,
                                     Name = SeedNames.TadasukeMakino
                                 },
                                 new DriverEntity
                                 {
                                     Id = 44,
                                     DriverGameId = 47,
                                     Name = SeedNames.SeanGelael
                                 },
                                 new DriverEntity
                                 {
                                     Id = 45,
                                     DriverGameId = 48,
                                     Name = SeedNames.NyckDeVries
                                 },
                                 new DriverEntity
                                 {
                                     Id = 46,
                                     DriverGameId = 49,
                                     Name = SeedNames.JackAitken
                                 },
                                 new DriverEntity
                                 {
                                     Id = 47,
                                     DriverGameId = 50,
                                     Name = SeedNames.GeorgeRussell
                                 },
                                 new DriverEntity
                                 {
                                     Id = 48,
                                     DriverGameId = 51,
                                     Name = SeedNames.MaximilianGunther
                                 },
                                 new DriverEntity
                                 {
                                     Id = 49,
                                     DriverGameId = 52,
                                     Name = SeedNames.NireiFukuzumi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 50,
                                     DriverGameId = 53,
                                     Name = SeedNames.LucaGhiotto
                                 },
                                 new DriverEntity
                                 {
                                     Id = 51,
                                     DriverGameId = 54,
                                     Name = SeedNames.LandoNorris
                                 },
                                 new DriverEntity
                                 {
                                     Id = 52,
                                     DriverGameId = 55,
                                     Name = SeedNames.SergioSetteCamara
                                 },
                                 new DriverEntity
                                 {
                                     Id = 53,
                                     DriverGameId = 56,
                                     Name = SeedNames.LouisDeletraz
                                 },
                                 new DriverEntity
                                 {
                                     Id = 54,
                                     DriverGameId = 57,
                                     Name = SeedNames.AntonioFuoco
                                 },
                                 new DriverEntity
                                 {
                                     Id = 55,
                                     DriverGameId = 58,
                                     Name = SeedNames.CharlesLeclerc
                                 },
                                 new DriverEntity
                                 {
                                     Id = 56,
                                     DriverGameId = 59,
                                     Name = SeedNames.PierreGasly
                                 },
                                 new DriverEntity
                                 {
                                     Id = 57,
                                     DriverGameId = 62,
                                     Name = SeedNames.AlexanderAlbon
                                 },
                                 new DriverEntity
                                 {
                                     Id = 58,
                                     DriverGameId = 63,
                                     Name = SeedNames.NicholasLatifi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 59,
                                     DriverGameId = 64,
                                     Name = SeedNames.DorianBoccolacci
                                 },
                                 new DriverEntity
                                 {
                                     Id = 60,
                                     DriverGameId = 65,
                                     Name = SeedNames.NikoKari
                                 },
                                 new DriverEntity
                                 {
                                     Id = 61,
                                     DriverGameId = 66,
                                     Name = SeedNames.RobertoMerhi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 62,
                                     DriverGameId = 67,
                                     Name = SeedNames.ArjunMaini
                                 },
                                 new DriverEntity
                                 {
                                     Id = 63,
                                     DriverGameId = 68,
                                     Name = SeedNames.AlessioLorandi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 64,
                                     DriverGameId = 69,
                                     Name = SeedNames.RubenMeijer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 65,
                                     DriverGameId = 70,
                                     Name = SeedNames.RashidNair
                                 },
                                 new DriverEntity
                                 {
                                     Id = 66,
                                     DriverGameId = 71,
                                     Name = SeedNames.JackTremblay
                                 },
                                 new DriverEntity
                                 {
                                     Id = 67,
                                     DriverGameId = 72,
                                     Name = SeedNames.DevonButler
                                 },
                                 new DriverEntity
                                 {
                                     Id = 68,
                                     DriverGameId = 73,
                                     Name = SeedNames.LukasWeber
                                 },
                                 new DriverEntity
                                 {
                                     Id = 69,
                                     DriverGameId = 74,
                                     Name = SeedNames.AntonioGiovinazzi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 70,
                                     DriverGameId = 75,
                                     Name = SeedNames.RobertKubica
                                 },
                                 new DriverEntity
                                 {
                                     Id = 71,
                                     DriverGameId = 76,
                                     Name = SeedNames.AlainProst
                                 },
                                 new DriverEntity
                                 {
                                     Id = 72,
                                     DriverGameId = 77,
                                     Name = SeedNames.AyrtonSenna
                                 },
                                 new DriverEntity
                                 {
                                     Id = 73,
                                     DriverGameId = 78,
                                     Name = SeedNames.NobaharuMatsushita
                                 },
                                 new DriverEntity
                                 {
                                     Id = 74,
                                     DriverGameId = 79,
                                     Name = SeedNames.NikitaMazepin
                                 },
                                 new DriverEntity
                                 {
                                     Id = 75,
                                     DriverGameId = 80,
                                     Name = SeedNames.GuanyaZhou
                                 },
                                 new DriverEntity
                                 {
                                     Id = 76,
                                     DriverGameId = 81,
                                     Name = SeedNames.MickSchumacher
                                 },
                                 new DriverEntity
                                 {
                                     Id = 77,
                                     DriverGameId = 82,
                                     Name = SeedNames.CallumIlott
                                 },
                                 new DriverEntity
                                 {
                                     Id = 78,
                                     DriverGameId = 83,
                                     Name = SeedNames.JuanManuelCorrea
                                 },
                                 new DriverEntity
                                 {
                                     Id = 79,
                                     DriverGameId = 84,
                                     Name = SeedNames.JordanKing
                                 },
                                 new DriverEntity
                                 {
                                     Id = 80,
                                     DriverGameId = 85,
                                     Name = SeedNames.MahaveerRaghunathan
                                 },
                                 new DriverEntity
                                 {
                                     Id = 81,
                                     DriverGameId = 86,
                                     Name = SeedNames.TatianaCalderon
                                 },
                                 new DriverEntity
                                 {
                                     Id = 82,
                                     DriverGameId = 87,
                                     Name = SeedNames.AnthoineHubert
                                 },
                                 new DriverEntity
                                 {
                                     Id = 83,
                                     DriverGameId = 88,
                                     Name = SeedNames.GuilianoAlesi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 84,
                                     DriverGameId = 89,
                                     Name = SeedNames.RalphBoschung
                                 },
                                 new DriverEntity
                                 {
                                     Id = 85,
                                     DriverGameId = 90,
                                     Name = SeedNames.MichaelSchumacher
                                 },
                                 new DriverEntity
                                 {
                                     Id = 86,
                                     DriverGameId = 91,
                                     Name = SeedNames.DanTicktum
                                 },
                                 new DriverEntity
                                 {
                                     Id = 87,
                                     DriverGameId = 92,
                                     Name = SeedNames.MarcusArmstrong
                                 },
                                 new DriverEntity
                                 {
                                     Id = 88,
                                     DriverGameId = 93,
                                     Name = SeedNames.ChristianLundgaard
                                 },
                                 new DriverEntity
                                 {
                                     Id = 89,
                                     DriverGameId = 94,
                                     Name = SeedNames.YukiTsunoda
                                 },
                                 new DriverEntity
                                 {
                                     Id = 90,
                                     DriverGameId = 95,
                                     Name = SeedNames.JehanDaruvala
                                 },
                                 new DriverEntity
                                 {
                                     Id = 91,
                                     DriverGameId = 96,
                                     Name = SeedNames.GulhermeSamaia
                                 },
                                 new DriverEntity
                                 {
                                     Id = 92,
                                     DriverGameId = 97,
                                     Name = SeedNames.PedroPiquet
                                 },
                                 new DriverEntity
                                 {
                                     Id = 93,
                                     DriverGameId = 98,
                                     Name = SeedNames.FelipeDrugovich
                                 },
                                 new DriverEntity
                                 {
                                     Id = 94,
                                     DriverGameId = 99,
                                     Name = SeedNames.RobertSchwartzman
                                 },
                                 new DriverEntity
                                 {
                                     Id = 95,
                                     DriverGameId = 100,
                                     Name = SeedNames.RoyNissany
                                 },
                                 new DriverEntity
                                 {
                                     Id = 96,
                                     DriverGameId = 101,
                                     Name = SeedNames.MarinoSato
                                 },
                                 new DriverEntity
                                 {
                                     Id = 97,
                                     DriverGameId = 102,
                                     Name = SeedNames.AidanJackson
                                 },
                                 new DriverEntity
                                 {
                                     Id = 98,
                                     DriverGameId = 103,
                                     Name = SeedNames.CasperAkkerman
                                 },
                                 new DriverEntity
                                 {
                                     Id = 99,
                                     DriverGameId = 109,
                                     Name = SeedNames.JensonButton
                                 },
                                 new DriverEntity
                                 {
                                     Id = 100,
                                     DriverGameId = 110,
                                     Name = SeedNames.DavidCoulthard
                                 },
                                 new DriverEntity
                                 {
                                     Id = 101,
                                     DriverGameId = 111,
                                     Name = SeedNames.NicoRosberg
                                 },
                                 new DriverEntity
                                 {
                                     Id = 102,
                                     DriverGameId = 112,
                                     Name = SeedNames.OscarPiastri
                                 },
                                 new DriverEntity
                                 {
                                     Id = 103,
                                     DriverGameId = 113,
                                     Name = SeedNames.LiamLawson
                                 },
                                 new DriverEntity
                                 {
                                     Id = 104,
                                     DriverGameId = 114,
                                     Name = SeedNames.JuriVips
                                 },
                                 new DriverEntity
                                 {
                                     Id = 105,
                                     DriverGameId = 115,
                                     Name = SeedNames.TheoPourchaire
                                 },
                                 new DriverEntity
                                 {
                                     Id = 106,
                                     DriverGameId = 116,
                                     Name = SeedNames.RichardVerschoor
                                 },
                                 new DriverEntity
                                 {
                                     Id = 107,
                                     DriverGameId = 117,
                                     Name = SeedNames.LirimZendeli
                                 },
                                 new DriverEntity
                                 {
                                     Id = 108,
                                     DriverGameId = 118,
                                     Name = SeedNames.DavidBeckmann
                                 },
                                 new DriverEntity
                                 {
                                     Id = 109,
                                     DriverGameId = 119,
                                     Name = SeedNames.GianlucaPetecof
                                 },
                                 new DriverEntity
                                 {
                                     Id = 110,
                                     DriverGameId = 120,
                                     Name = SeedNames.MatteoNannini
                                 },
                                 new DriverEntity
                                 {
                                     Id = 111,
                                     DriverGameId = 121,
                                     Name = SeedNames.AlessioDeledda
                                 },
                                 new DriverEntity
                                 {
                                     Id = 112,
                                     DriverGameId = 122,
                                     Name = SeedNames.BentViscaal
                                 },
                                 new DriverEntity
                                 {
                                     Id = 113,
                                     DriverGameId = 123,
                                     Name = SeedNames.EnzoFittipaldi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 114,
                                     DriverGameId = 125,
                                     Name = SeedNames.MarkWebber
                                 },
                                 new DriverEntity
                                 {
                                     Id = 115,
                                     DriverGameId = 126,
                                     Name = SeedNames.JacquesVilleneuve
                                 },
                                 new DriverEntity
                                 {
                                     Id = 116,
                                     DriverGameId = 127,
                                     Name = SeedNames.JakeHughes
                                 },
                                 new DriverEntity
                                 {
                                     Id = 117,
                                     DriverGameId = 128,
                                     Name = SeedNames.FrederikVesti
                                 },
                                 new DriverEntity
                                 {
                                     Id = 118,
                                     DriverGameId = 129,
                                     Name = SeedNames.OlliCaldwell
                                 },
                                 new DriverEntity
                                 {
                                     Id = 119,
                                     DriverGameId = 130,
                                     Name = SeedNames.LoganSargeant
                                 },
                                 new DriverEntity
                                 {
                                     Id = 120,
                                     DriverGameId = 131,
                                     Name = SeedNames.CemBolukbasi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 121,
                                     DriverGameId = 132,
                                     Name = SeedNames.AyumuIwasa
                                 },
                                 new DriverEntity
                                 {
                                     Id = 122,
                                     DriverGameId = 133,
                                     Name = SeedNames.ClementNovalak
                                 },
                                 new DriverEntity
                                 {
                                     Id = 123,
                                     DriverGameId = 134,
                                     Name = SeedNames.DennisHauger
                                 },
                                 new DriverEntity
                                 {
                                     Id = 124,
                                     DriverGameId = 135,
                                     Name = SeedNames.CalanWilliams
                                 },
                                 new DriverEntity
                                 {
                                     Id = 125,
                                     DriverGameId = 136,
                                     Name = SeedNames.JackDoohan
                                 },
                                 new DriverEntity
                                 {
                                     Id = 126,
                                     DriverGameId = 137,
                                     Name = SeedNames.AmauryCordeel
                                 },
                                 new DriverEntity
                                 {
                                     Id = 127,
                                     DriverGameId = 138,
                                     Name = SeedNames.MikaHakkinen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 128,
                                     DriverGameId = 139,
                                     Name = SeedNames.CallieMayer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 129,
                                     DriverGameId = 140,
                                     Name = SeedNames.NoahBell
                                 },
                                 new DriverEntity
                                 {
                                     Id = 130,
                                     DriverGameId = 141,
                                     Name = SeedNames.JakeHughes
                                 },
                                 new DriverEntity
                                 {
                                     Id = 131,
                                     DriverGameId = 142,
                                     Name = SeedNames.FrederikVesti
                                 },
                                 new DriverEntity
                                 {
                                     Id = 132,
                                     DriverGameId = 143,
                                     Name = SeedNames.OlliCaldwell
                                 },
                                 new DriverEntity
                                 {
                                     Id = 133,
                                     DriverGameId = 144,
                                     Name = SeedNames.LoganSargeant
                                 },
                                 new DriverEntity
                                 {
                                     Id = 134,
                                     DriverGameId = 145,
                                     Name = SeedNames.CemBolukbasi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 135,
                                     DriverGameId = 146,
                                     Name = SeedNames.AyumuIwasa
                                 },
                                 new DriverEntity
                                 {
                                     Id = 136,
                                     DriverGameId = 147,
                                     Name = SeedNames.ClementNovalak
                                 },
                                 new DriverEntity
                                 {
                                     Id = 137,
                                     DriverGameId = 148,
                                     Name = SeedNames.JackDoohan
                                 },
                                 new DriverEntity
                                 {
                                     Id = 138,
                                     DriverGameId = 149,
                                     Name = SeedNames.AmauryCordeel
                                 },
                                 new DriverEntity
                                 {
                                     Id = 139,
                                     DriverGameId = 150,
                                     Name = SeedNames.DennisHauger
                                 },
                                 new DriverEntity
                                 {
                                     Id = 140,
                                     DriverGameId = 151,
                                     Name = SeedNames.CalanWilliams
                                 },
                                 new DriverEntity
                                 {
                                     Id = 141,
                                     DriverGameId = 152,
                                     Name = SeedNames.JamieChadwick
                                 },
                                 new DriverEntity
                                 {
                                     Id = 142,
                                     DriverGameId = 153,
                                     Name = SeedNames.KamuiKobayashi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 143,
                                     DriverGameId = 154,
                                     Name = SeedNames.PastorMaldonado
                                 },
                                 new DriverEntity
                                 {
                                     Id = 144,
                                     DriverGameId = 155,
                                     Name = SeedNames.MikaHakkinen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 145,
                                     DriverGameId = 156,
                                     Name = SeedNames.NigelMansell
                                 },
                                 new DriverEntity
                                 {
                                     Id = 146,
                                     DriverGameId = 157,
                                     Name = SeedNames.ZaneMaloney
                                 },
                                 new DriverEntity
                                 {
                                     Id = 147,
                                     DriverGameId = 158,
                                     Name = SeedNames.VictorMartins
                                 },
                                 new DriverEntity
                                 {
                                     Id = 148,
                                     DriverGameId = 159,
                                     Name = SeedNames.OliverBearman
                                 },
                                 new DriverEntity
                                 {
                                     Id = 149,
                                     DriverGameId = 160,
                                     Name = SeedNames.JakCrawford
                                 },
                                 new DriverEntity
                                 {
                                     Id = 150,
                                     DriverGameId = 161,
                                     Name = SeedNames.IsackHadjar
                                 },
                                 new DriverEntity
                                 {
                                     Id = 151,
                                     DriverGameId = 162,
                                     Name = SeedNames.ArthurLeclerc
                                 },
                                 new DriverEntity
                                 {
                                     Id = 152,
                                     DriverGameId = 163,
                                     Name = SeedNames.BradBenavides
                                 },
                                 new DriverEntity
                                 {
                                     Id = 153,
                                     DriverGameId = 164,
                                     Name = SeedNames.RomanStanek
                                 },
                                 new DriverEntity
                                 {
                                     Id = 154,
                                     DriverGameId = 165,
                                     Name = SeedNames.KushMaini
                                 },
                                 new DriverEntity
                                 {
                                     Id = 155,
                                     DriverGameId = 166,
                                     Name = SeedNames.JamesHunt
                                 },
                                 new DriverEntity
                                 {
                                     Id = 156,
                                     DriverGameId = 167,
                                     Name = SeedNames.JuanPabloMontoya
                                 },
                                 new DriverEntity
                                 {
                                     Id = 157,
                                     DriverGameId = 168,
                                     Name = SeedNames.BrendonLeigh
                                 },
                                 new DriverEntity
                                 {
                                     Id = 158,
                                     DriverGameId = 169,
                                     Name = SeedNames.DavidTonizza
                                 },
                                 new DriverEntity
                                 {
                                     Id = 159,
                                     DriverGameId = 170,
                                     Name = SeedNames.JarnoOpmeer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 160,
                                     DriverGameId = 171,
                                     Name = SeedNames.LucasBlakeley
                                 },
                                 new DriverEntity
                                 {
                                     Id = 161,
                                     DriverGameId = 20250,
                                     Name = SeedNames.CarlosSainz
                                 },
                                 new DriverEntity
                                 {
                                     Id = 162,
                                     DriverGameId = 20252,
                                     Name = SeedNames.DanielRicciardo
                                 },
                                 new DriverEntity
                                 {
                                     Id = 163,
                                     DriverGameId = 20253,
                                     Name = SeedNames.FernandoAlonso
                                 },
                                 new DriverEntity
                                 {
                                     Id = 164,
                                     DriverGameId = 20254,
                                     Name = SeedNames.FelipeMassa
                                 },
                                 new DriverEntity
                                 {
                                     Id = 165,
                                     DriverGameId = 20257,
                                     Name = SeedNames.LewisHamilton
                                 },
                                 new DriverEntity
                                 {
                                     Id = 166,
                                     DriverGameId = 20259,
                                     Name = SeedNames.MaxVerstappen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 167,
                                     DriverGameId = 202510,
                                     Name = SeedNames.NicoHulkenberg
                                 },
                                 new DriverEntity
                                 {
                                     Id = 168,
                                     DriverGameId = 202511,
                                     Name = SeedNames.KevinMagnussen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 169,
                                     DriverGameId = 202514,
                                     Name = SeedNames.SergioPerez
                                 },
                                 new DriverEntity
                                 {
                                     Id = 170,
                                     DriverGameId = 202515,
                                     Name = SeedNames.ValtteriBottas
                                 },
                                 new DriverEntity
                                 {
                                     Id = 171,
                                     DriverGameId = 202517,
                                     Name = SeedNames.EstebanOcon
                                 },
                                 new DriverEntity
                                 {
                                     Id = 172,
                                     DriverGameId = 202519,
                                     Name = SeedNames.LanceStroll
                                 },
                                 new DriverEntity
                                 {
                                     Id = 173,
                                     DriverGameId = 202520,
                                     Name = SeedNames.ArronBarnes
                                 },
                                 new DriverEntity
                                 {
                                     Id = 174,
                                     DriverGameId = 202521,
                                     Name = SeedNames.MartinGiles
                                 },
                                 new DriverEntity
                                 {
                                     Id = 175,
                                     DriverGameId = 202522,
                                     Name = SeedNames.AlexMurray
                                 },
                                 new DriverEntity
                                 {
                                     Id = 176,
                                     DriverGameId = 202523,
                                     Name = SeedNames.LucasRoth
                                 },
                                 new DriverEntity
                                 {
                                     Id = 177,
                                     DriverGameId = 202524,
                                     Name = SeedNames.IgorCorreia
                                 },
                                 new DriverEntity
                                 {
                                     Id = 178,
                                     DriverGameId = 202525,
                                     Name = SeedNames.SophieLevasseur
                                 },
                                 new DriverEntity
                                 {
                                     Id = 179,
                                     DriverGameId = 202526,
                                     Name = SeedNames.JonasSchiffer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 180,
                                     DriverGameId = 202527,
                                     Name = SeedNames.AlainForest
                                 },
                                 new DriverEntity
                                 {
                                     Id = 181,
                                     DriverGameId = 202528,
                                     Name = SeedNames.JayLetourneau
                                 },
                                 new DriverEntity
                                 {
                                     Id = 182,
                                     DriverGameId = 202529,
                                     Name = SeedNames.EstoSaari
                                 },
                                 new DriverEntity
                                 {
                                     Id = 183,
                                     DriverGameId = 202530,
                                     Name = SeedNames.YasarAtiyeh
                                 },
                                 new DriverEntity
                                 {
                                     Id = 184,
                                     DriverGameId = 202531,
                                     Name = SeedNames.CallistoCalabresi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 185,
                                     DriverGameId = 202532,
                                     Name = SeedNames.NaotaIzumi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 186,
                                     DriverGameId = 202533,
                                     Name = SeedNames.HowardClarke
                                 },
                                 new DriverEntity
                                 {
                                     Id = 187,
                                     DriverGameId = 202534,
                                     Name = SeedNames.LarsKaufmann
                                 },
                                 new DriverEntity
                                 {
                                     Id = 188,
                                     DriverGameId = 202535,
                                     Name = SeedNames.MarieLaursen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 189,
                                     DriverGameId = 202536,
                                     Name = SeedNames.FlavioNieves
                                 },
                                 new DriverEntity
                                 {
                                     Id = 190,
                                     DriverGameId = 202538,
                                     Name = SeedNames.KlimekMichalski
                                 },
                                 new DriverEntity
                                 {
                                     Id = 191,
                                     DriverGameId = 202539,
                                     Name = SeedNames.SantiagoMoreno
                                 },
                                 new DriverEntity
                                 {
                                     Id = 192,
                                     DriverGameId = 202540,
                                     Name = SeedNames.BenjaminCoppens
                                 },
                                 new DriverEntity
                                 {
                                     Id = 193,
                                     DriverGameId = 202541,
                                     Name = SeedNames.NoahVisser
                                 },
                                 new DriverEntity
                                 {
                                     Id = 194,
                                     DriverGameId = 202550,
                                     Name = SeedNames.GeorgeRussell
                                 },
                                 new DriverEntity
                                 {
                                     Id = 195,
                                     DriverGameId = 202554,
                                     Name = SeedNames.LandoNorris
                                 },
                                 new DriverEntity
                                 {
                                     Id = 196,
                                     DriverGameId = 202558,
                                     Name = SeedNames.CharlesLeclerc
                                 },
                                 new DriverEntity
                                 {
                                     Id = 197,
                                     DriverGameId = 202559,
                                     Name = SeedNames.PierreGasly
                                 },
                                 new DriverEntity
                                 {
                                     Id = 198,
                                     DriverGameId = 202562,
                                     Name = SeedNames.AlexanderAlbon
                                 },
                                 new DriverEntity
                                 {
                                     Id = 199,
                                     DriverGameId = 202570,
                                     Name = SeedNames.RashidNair
                                 },
                                 new DriverEntity
                                 {
                                     Id = 200,
                                     DriverGameId = 202571,
                                     Name = SeedNames.JackTremblay
                                 },
                                 new DriverEntity
                                 {
                                     Id = 201,
                                     DriverGameId = 202577,
                                     Name = SeedNames.AyrtonSenna
                                 },
                                 new DriverEntity
                                 {
                                     Id = 202,
                                     DriverGameId = 202580,
                                     Name = SeedNames.GuanyaZhou
                                 },
                                 new DriverEntity
                                 {
                                     Id = 203,
                                     DriverGameId = 202583,
                                     Name = SeedNames.JuanManuelCorrea
                                 },
                                 new DriverEntity
                                 {
                                     Id = 204,
                                     DriverGameId = 202590,
                                     Name = SeedNames.MichaelSchumacher
                                 },
                                 new DriverEntity
                                 {
                                     Id = 205,
                                     DriverGameId = 202594,
                                     Name = SeedNames.YukiTsunoda
                                 },
                                 new DriverEntity
                                 {
                                     Id = 206,
                                     DriverGameId = 2025102,
                                     Name = SeedNames.AidanJackson
                                 },
                                 new DriverEntity
                                 {
                                     Id = 207,
                                     DriverGameId = 2025109,
                                     Name = SeedNames.JensonButton
                                 },
                                 new DriverEntity
                                 {
                                     Id = 208,
                                     DriverGameId = 2025110,
                                     Name = SeedNames.DavidCoulthard
                                 },
                                 new DriverEntity
                                 {
                                     Id = 209,
                                     DriverGameId = 2025112,
                                     Name = SeedNames.OscarPiastri
                                 },
                                 new DriverEntity
                                 {
                                     Id = 210,
                                     DriverGameId = 2025113,
                                     Name = SeedNames.LiamLawson
                                 },
                                 new DriverEntity
                                 {
                                     Id = 211,
                                     DriverGameId = 2025116,
                                     Name = SeedNames.RichardVerschoor
                                 },
                                 new DriverEntity
                                 {
                                     Id = 212,
                                     DriverGameId = 2025123,
                                     Name = SeedNames.EnzoFittipaldi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 213,
                                     DriverGameId = 2025125,
                                     Name = SeedNames.MarkWebber
                                 },
                                 new DriverEntity
                                 {
                                     Id = 214,
                                     DriverGameId = 2025126,
                                     Name = SeedNames.JacquesVilleneuve
                                 },
                                 new DriverEntity
                                 {
                                     Id = 215,
                                     DriverGameId = 2025127,
                                     Name = SeedNames.CallieMayer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 216,
                                     DriverGameId = 2025132,
                                     Name = SeedNames.LoganSargeant
                                 },
                                 new DriverEntity
                                 {
                                     Id = 217,
                                     DriverGameId = 2025136,
                                     Name = SeedNames.JackDoohan
                                 },
                                 new DriverEntity
                                 {
                                     Id = 218,
                                     DriverGameId = 2025137,
                                     Name = SeedNames.AmauryCordeel
                                 },
                                 new DriverEntity
                                 {
                                     Id = 219,
                                     DriverGameId = 2025138,
                                     Name = SeedNames.DennisHauger
                                 },
                                 new DriverEntity
                                 {
                                     Id = 220,
                                     DriverGameId = 2025145,
                                     Name = SeedNames.ZaneMaloney
                                 },
                                 new DriverEntity
                                 {
                                     Id = 221,
                                     DriverGameId = 2025146,
                                     Name = SeedNames.VictorMartins
                                 },
                                 new DriverEntity
                                 {
                                     Id = 222,
                                     DriverGameId = 2025147,
                                     Name = SeedNames.OliverBearman
                                 },
                                 new DriverEntity
                                 {
                                     Id = 223,
                                     DriverGameId = 2025148,
                                     Name = SeedNames.JakCrawford
                                 },
                                 new DriverEntity
                                 {
                                     Id = 224,
                                     DriverGameId = 2025149,
                                     Name = SeedNames.IsackHadjar
                                 },
                                 new DriverEntity
                                 {
                                     Id = 225,
                                     DriverGameId = 2025152,
                                     Name = SeedNames.RomanStanek
                                 },
                                 new DriverEntity
                                 {
                                     Id = 226,
                                     DriverGameId = 2025153,
                                     Name = SeedNames.KushMaini
                                 },
                                 new DriverEntity
                                 {
                                     Id = 227,
                                     DriverGameId = 2025156,
                                     Name = SeedNames.BrendonLeigh
                                 },
                                 new DriverEntity
                                 {
                                     Id = 228,
                                     DriverGameId = 2025157,
                                     Name = SeedNames.DavidTonizza
                                 },
                                 new DriverEntity
                                 {
                                     Id = 229,
                                     DriverGameId = 2025158,
                                     Name = SeedNames.JarnoOpmeer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 230,
                                     DriverGameId = 2025159,
                                     Name = SeedNames.LucasBlakeley
                                 },
                                 new DriverEntity
                                 {
                                     Id = 231,
                                     DriverGameId = 2025160,
                                     Name = SeedNames.PaulAron
                                 },
                                 new DriverEntity
                                 {
                                     Id = 232,
                                     DriverGameId = 2025161,
                                     Name = SeedNames.GabrielBortoleto
                                 },
                                 new DriverEntity
                                 {
                                     Id = 233,
                                     DriverGameId = 2025162,
                                     Name = SeedNames.FrancoColapinto
                                 },
                                 new DriverEntity
                                 {
                                     Id = 234,
                                     DriverGameId = 2025163,
                                     Name = SeedNames.TaylorBarnard
                                 },
                                 new DriverEntity
                                 {
                                     Id = 235,
                                     DriverGameId = 2025164,
                                     Name = SeedNames.JoshuaDurksen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 236,
                                     DriverGameId = 2025165,
                                     Name = SeedNames.AndreaKimiAntonelli
                                 },
                                 new DriverEntity
                                 {
                                     Id = 237,
                                     DriverGameId = 2025166,
                                     Name = SeedNames.Ritomomiyata
                                 },
                                 new DriverEntity
                                 {
                                     Id = 238,
                                     DriverGameId = 2025167,
                                     Name = SeedNames.RafaelVillagomez
                                 },
                                 new DriverEntity
                                 {
                                     Id = 239,
                                     DriverGameId = 2025168,
                                     Name = SeedNames.ZakOSullivan
                                 },
                                 new DriverEntity
                                 {
                                     Id = 240,
                                     DriverGameId = 2025169,
                                     Name = SeedNames.PepeMarti
                                 },
                                 new DriverEntity
                                 {
                                     Id = 241,
                                     DriverGameId = 2025170,
                                     Name = SeedNames.SonnyHayes
                                 },
                                 new DriverEntity
                                 {
                                     Id = 242,
                                     DriverGameId = 2025171,
                                     Name = SeedNames.JoshuaPearce
                                 },
                                 new DriverEntity
                                 {
                                     Id = 243,
                                     DriverGameId = 2025172,
                                     Name = SeedNames.CallumVoisin
                                 },
                                 new DriverEntity
                                 {
                                     Id = 244,
                                     DriverGameId = 2025173,
                                     Name = SeedNames.MatiasZagazeta
                                 },
                                 new DriverEntity
                                 {
                                     Id = 245,
                                     DriverGameId = 2025174,
                                     Name = SeedNames.NikolaTsolov
                                 },
                                 new DriverEntity
                                 {
                                     Id = 246,
                                     DriverGameId = 2025175,
                                     Name = SeedNames.TimTramnitz
                                 },
                                 new DriverEntity
                                 {
                                     Id = 247,
                                     DriverGameId = 2025185,
                                     Name = SeedNames.LucaCortez
                                 },
                                 new DriverEntity
                                 {
                                     Id = 248,
                                     DriverGameId = 20260,
                                     Name = SeedNames.CarlosSainz
                                 },
                                 new DriverEntity
                                 {
                                     Id = 249,
                                     DriverGameId = 20262,
                                     Name = SeedNames.DanielRicciardo
                                 },
                                 new DriverEntity
                                 {
                                     Id = 250,
                                     DriverGameId = 20263,
                                     Name = SeedNames.FernandoAlonso
                                 },
                                 new DriverEntity
                                 {
                                     Id = 251,
                                     DriverGameId = 20264,
                                     Name = SeedNames.FelipeMassa
                                 },
                                 new DriverEntity
                                 {
                                     Id = 252,
                                     DriverGameId = 20267,
                                     Name = SeedNames.LewisHamilton
                                 },
                                 new DriverEntity
                                 {
                                     Id = 253,
                                     DriverGameId = 20269,
                                     Name = SeedNames.MaxVerstappen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 254,
                                     DriverGameId = 202610,
                                     Name = SeedNames.NicoHulkenberg
                                 },
                                 new DriverEntity
                                 {
                                     Id = 255,
                                     DriverGameId = 202611,
                                     Name = SeedNames.KevinMagnussen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 256,
                                     DriverGameId = 202614,
                                     Name = SeedNames.SergioPerez
                                 },
                                 new DriverEntity
                                 {
                                     Id = 257,
                                     DriverGameId = 202615,
                                     Name = SeedNames.ValtteriBottas
                                 },
                                 new DriverEntity
                                 {
                                     Id = 258,
                                     DriverGameId = 202617,
                                     Name = SeedNames.EstebanOcon
                                 },
                                 new DriverEntity
                                 {
                                     Id = 259,
                                     DriverGameId = 202619,
                                     Name = SeedNames.LanceStroll
                                 },
                                 new DriverEntity
                                 {
                                     Id = 260,
                                     DriverGameId = 202620,
                                     Name = SeedNames.ArronBarnes
                                 },
                                 new DriverEntity
                                 {
                                     Id = 261,
                                     DriverGameId = 202621,
                                     Name = SeedNames.MartinGiles
                                 },
                                 new DriverEntity
                                 {
                                     Id = 262,
                                     DriverGameId = 202622,
                                     Name = SeedNames.AlexMurray
                                 },
                                 new DriverEntity
                                 {
                                     Id = 263,
                                     DriverGameId = 202623,
                                     Name = SeedNames.LucasRoth
                                 },
                                 new DriverEntity
                                 {
                                     Id = 264,
                                     DriverGameId = 202624,
                                     Name = SeedNames.IgorCorreia
                                 },
                                 new DriverEntity
                                 {
                                     Id = 265,
                                     DriverGameId = 202625,
                                     Name = SeedNames.SophieLevasseur
                                 },
                                 new DriverEntity
                                 {
                                     Id = 266,
                                     DriverGameId = 202626,
                                     Name = SeedNames.JonasSchiffer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 267,
                                     DriverGameId = 202627,
                                     Name = SeedNames.AlainForest
                                 },
                                 new DriverEntity
                                 {
                                     Id = 268,
                                     DriverGameId = 202628,
                                     Name = SeedNames.JayLetourneau
                                 },
                                 new DriverEntity
                                 {
                                     Id = 269,
                                     DriverGameId = 202629,
                                     Name = SeedNames.EstoSaari
                                 },
                                 new DriverEntity
                                 {
                                     Id = 270,
                                     DriverGameId = 202630,
                                     Name = SeedNames.YasarAtiyeh
                                 },
                                 new DriverEntity
                                 {
                                     Id = 271,
                                     DriverGameId = 202631,
                                     Name = SeedNames.CallistoCalabresi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 272,
                                     DriverGameId = 202632,
                                     Name = SeedNames.NaotaIzumi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 273,
                                     DriverGameId = 202633,
                                     Name = SeedNames.HowardClarke
                                 },
                                 new DriverEntity
                                 {
                                     Id = 274,
                                     DriverGameId = 202634,
                                     Name = SeedNames.LarsKaufmann
                                 },
                                 new DriverEntity
                                 {
                                     Id = 275,
                                     DriverGameId = 202635,
                                     Name = SeedNames.MarieLaursen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 276,
                                     DriverGameId = 202636,
                                     Name = SeedNames.FlavioNieves
                                 },
                                 new DriverEntity
                                 {
                                     Id = 277,
                                     DriverGameId = 202638,
                                     Name = SeedNames.KlimekMichalski
                                 },
                                 new DriverEntity
                                 {
                                     Id = 278,
                                     DriverGameId = 202639,
                                     Name = SeedNames.SantiagoMoreno
                                 },
                                 new DriverEntity
                                 {
                                     Id = 279,
                                     DriverGameId = 202640,
                                     Name = SeedNames.BenjaminCoppens
                                 },
                                 new DriverEntity
                                 {
                                     Id = 280,
                                     DriverGameId = 202641,
                                     Name = SeedNames.NoahVisser
                                 },
                                 new DriverEntity
                                 {
                                     Id = 281,
                                     DriverGameId = 202650,
                                     Name = SeedNames.GeorgeRussell
                                 },
                                 new DriverEntity
                                 {
                                     Id = 282,
                                     DriverGameId = 202654,
                                     Name = SeedNames.LandoNorris
                                 },
                                 new DriverEntity
                                 {
                                     Id = 283,
                                     DriverGameId = 202658,
                                     Name = SeedNames.CharlesLeclerc
                                 },
                                 new DriverEntity
                                 {
                                     Id = 284,
                                     DriverGameId = 202659,
                                     Name = SeedNames.PierreGasly
                                 },
                                 new DriverEntity
                                 {
                                     Id = 285,
                                     DriverGameId = 202662,
                                     Name = SeedNames.AlexanderAlbon
                                 },
                                 new DriverEntity
                                 {
                                     Id = 286,
                                     DriverGameId = 202670,
                                     Name = SeedNames.RashidNair
                                 },
                                 new DriverEntity
                                 {
                                     Id = 287,
                                     DriverGameId = 202671,
                                     Name = SeedNames.JackTremblay
                                 },
                                 new DriverEntity
                                 {
                                     Id = 288,
                                     DriverGameId = 202677,
                                     Name = SeedNames.AyrtonSenna
                                 },
                                 new DriverEntity
                                 {
                                     Id = 289,
                                     DriverGameId = 202680,
                                     Name = SeedNames.GuanyaZhou
                                 },
                                 new DriverEntity
                                 {
                                     Id = 290,
                                     DriverGameId = 202683,
                                     Name = SeedNames.JuanManuelCorrea
                                 },
                                 new DriverEntity
                                 {
                                     Id = 291,
                                     DriverGameId = 202690,
                                     Name = SeedNames.MichaelSchumacher
                                 },
                                 new DriverEntity
                                 {
                                     Id = 292,
                                     DriverGameId = 202694,
                                     Name = SeedNames.YukiTsunoda
                                 },
                                 new DriverEntity
                                 {
                                     Id = 293,
                                     DriverGameId = 2026102,
                                     Name = SeedNames.AidanJackson
                                 },
                                 new DriverEntity
                                 {
                                     Id = 294,
                                     DriverGameId = 2026109,
                                     Name = SeedNames.JensonButton
                                 },
                                 new DriverEntity
                                 {
                                     Id = 295,
                                     DriverGameId = 2026110,
                                     Name = SeedNames.DavidCoulthard
                                 },
                                 new DriverEntity
                                 {
                                     Id = 296,
                                     DriverGameId = 2026112,
                                     Name = SeedNames.OscarPiastri
                                 },
                                 new DriverEntity
                                 {
                                     Id = 297,
                                     DriverGameId = 2026113,
                                     Name = SeedNames.LiamLawson
                                 },
                                 new DriverEntity
                                 {
                                     Id = 298,
                                     DriverGameId = 2026116,
                                     Name = SeedNames.RichardVerschoor
                                 },
                                 new DriverEntity
                                 {
                                     Id = 299,
                                     DriverGameId = 2026123,
                                     Name = SeedNames.EnzoFittipaldi
                                 },
                                 new DriverEntity
                                 {
                                     Id = 300,
                                     DriverGameId = 2026125,
                                     Name = SeedNames.MarkWebber
                                 },
                                 new DriverEntity
                                 {
                                     Id = 301,
                                     DriverGameId = 2026126,
                                     Name = SeedNames.JacquesVilleneuve
                                 },
                                 new DriverEntity
                                 {
                                     Id = 302,
                                     DriverGameId = 2026127,
                                     Name = SeedNames.CallieMayer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 303,
                                     DriverGameId = 2026132,
                                     Name = SeedNames.LoganSargeant
                                 },
                                 new DriverEntity
                                 {
                                     Id = 304,
                                     DriverGameId = 2026136,
                                     Name = SeedNames.JackDoohan
                                 },
                                 new DriverEntity
                                 {
                                     Id = 305,
                                     DriverGameId = 2026137,
                                     Name = SeedNames.AmauryCordeel
                                 },
                                 new DriverEntity
                                 {
                                     Id = 306,
                                     DriverGameId = 2026138,
                                     Name = SeedNames.DennisHauger
                                 },
                                 new DriverEntity
                                 {
                                     Id = 307,
                                     DriverGameId = 2026145,
                                     Name = SeedNames.ZaneMaloney
                                 },
                                 new DriverEntity
                                 {
                                     Id = 308,
                                     DriverGameId = 2026146,
                                     Name = SeedNames.VictorMartins
                                 },
                                 new DriverEntity
                                 {
                                     Id = 309,
                                     DriverGameId = 2026147,
                                     Name = SeedNames.OliverBearman
                                 },
                                 new DriverEntity
                                 {
                                     Id = 310,
                                     DriverGameId = 2026148,
                                     Name = SeedNames.JakCrawford
                                 },
                                 new DriverEntity
                                 {
                                     Id = 311,
                                     DriverGameId = 2026149,
                                     Name = SeedNames.IsackHadjar
                                 },
                                 new DriverEntity
                                 {
                                     Id = 312,
                                     DriverGameId = 2026152,
                                     Name = SeedNames.RomanStanek
                                 },
                                 new DriverEntity
                                 {
                                     Id = 313,
                                     DriverGameId = 2026153,
                                     Name = SeedNames.KushMaini
                                 },
                                 new DriverEntity
                                 {
                                     Id = 314,
                                     DriverGameId = 2026156,
                                     Name = SeedNames.BrendonLeigh
                                 },
                                 new DriverEntity
                                 {
                                     Id = 315,
                                     DriverGameId = 2026157,
                                     Name = SeedNames.DavidTonizza
                                 },
                                 new DriverEntity
                                 {
                                     Id = 316,
                                     DriverGameId = 2026158,
                                     Name = SeedNames.JarnoOpmeer
                                 },
                                 new DriverEntity
                                 {
                                     Id = 317,
                                     DriverGameId = 2026159,
                                     Name = SeedNames.LucasBlakeley
                                 },
                                 new DriverEntity
                                 {
                                     Id = 318,
                                     DriverGameId = 2026160,
                                     Name = SeedNames.PaulAron
                                 },
                                 new DriverEntity
                                 {
                                     Id = 319,
                                     DriverGameId = 2026161,
                                     Name = SeedNames.GabrielBortoleto
                                 },
                                 new DriverEntity
                                 {
                                     Id = 320,
                                     DriverGameId = 2026162,
                                     Name = SeedNames.FrancoColapinto
                                 },
                                 new DriverEntity
                                 {
                                     Id = 321,
                                     DriverGameId = 2026163,
                                     Name = SeedNames.TaylorBarnard
                                 },
                                 new DriverEntity
                                 {
                                     Id = 322,
                                     DriverGameId = 2026164,
                                     Name = SeedNames.JoshuaDurksen
                                 },
                                 new DriverEntity
                                 {
                                     Id = 323,
                                     DriverGameId = 2026165,
                                     Name = SeedNames.AndreaKimiAntonelli
                                 },
                                 new DriverEntity
                                 {
                                     Id = 324,
                                     DriverGameId = 2026166,
                                     Name = SeedNames.Ritomomiyata
                                 },
                                 new DriverEntity
                                 {
                                     Id = 325,
                                     DriverGameId = 2026167,
                                     Name = SeedNames.RafaelVillagomez
                                 },
                                 new DriverEntity
                                 {
                                     Id = 326,
                                     DriverGameId = 2026168,
                                     Name = SeedNames.ZakOSullivan
                                 },
                                 new DriverEntity
                                 {
                                     Id = 327,
                                     DriverGameId = 2026169,
                                     Name = SeedNames.PepeMarti
                                 },
                                 new DriverEntity
                                 {
                                     Id = 328,
                                     DriverGameId = 2026170,
                                     Name = SeedNames.SonnyHayes
                                 },
                                 new DriverEntity
                                 {
                                     Id = 329,
                                     DriverGameId = 2026171,
                                     Name = SeedNames.JoshuaPearce
                                 },
                                 new DriverEntity
                                 {
                                     Id = 330,
                                     DriverGameId = 2026172,
                                     Name = SeedNames.CallumVoisin
                                 },
                                 new DriverEntity
                                 {
                                     Id = 331,
                                     DriverGameId = 2026173,
                                     Name = SeedNames.MatiasZagazeta
                                 },
                                 new DriverEntity
                                 {
                                     Id = 332,
                                     DriverGameId = 2026174,
                                     Name = SeedNames.NikolaTsolov
                                 },
                                 new DriverEntity
                                 {
                                     Id = 333,
                                     DriverGameId = 2026175,
                                     Name = SeedNames.TimTramnitz
                                 },
                                 new DriverEntity
                                 {
                                     Id = 334,
                                     DriverGameId = 2026185,
                                     Name = SeedNames.LucaCortez
                                 },
                                 new DriverEntity
                                 {
                                     Id = 335,
                                     DriverGameId = 2026186,
                                     Name = SeedNames.LukeBrowning
                                 },
                                 new DriverEntity
                                 {
                                     Id = 336,
                                     DriverGameId = 2026187,
                                     Name = SeedNames.CianShields
                                 },
                                 new DriverEntity
                                 {
                                     Id = 337,
                                     DriverGameId = 2026188,
                                     Name = SeedNames.ArvidLindblad
                                 },
                                 new DriverEntity
                                 {
                                     Id = 338,
                                     DriverGameId = 2026189,
                                     Name = SeedNames.DinoBeganovic
                                 },
                                 new DriverEntity
                                 {
                                     Id = 339,
                                     DriverGameId = 2026190,
                                     Name = SeedNames.LeonardoFornaroli
                                 },
                                 new DriverEntity
                                 {
                                     Id = 340,
                                     DriverGameId = 2026191,
                                     Name = SeedNames.OliverGoethe
                                 },
                                 new DriverEntity
                                 {
                                     Id = 341,
                                     DriverGameId = 2026192,
                                     Name = SeedNames.GabrieleMini
                                 },
                                 new DriverEntity
                                 {
                                     Id = 342,
                                     DriverGameId = 2026193,
                                     Name = SeedNames.SebastianMontoya
                                 },
                                 new DriverEntity
                                 {
                                     Id = 343,
                                     DriverGameId = 2026194,
                                     Name = SeedNames.AlexanderDunne
                                 },
                                 new DriverEntity
                                 {
                                     Id = 344,
                                     DriverGameId = 2026195,
                                     Name = SeedNames.MaxEsterson
                                 },
                                 new DriverEntity
                                 {
                                     Id = 345,
                                     DriverGameId = 2026196,
                                     Name = SeedNames.SamiMeguetounif
                                 },
                                 new DriverEntity
                                 {
                                     Id = 346,
                                     DriverGameId = 2026197,
                                     Name = SeedNames.JohnBennett
                                 },
                                 new DriverEntity
                                 {
                                     Id = 1000,
                                     DriverGameId = 255,
                                     Name = string.Empty,
                                     IsHumanDriver = true
                                 });
        }
        catch
        {
            // Ignore exceptions in this step
        }
    }

    /// <summary>
    /// Insert nationalities into database
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    private void SeedNationalities(ModelBuilder modelBuilder)
    {
        try
        {
            modelBuilder?.Entity<NationalityEntity>()
                        .HasData(new NationalityEntity
                                 {
                                     Id = 1,
                                     NationalityGameId = 0,
                                     Name = SeedNames.Unknown
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 2,
                                     NationalityGameId = 1,
                                     Name = SeedNames.American
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 3,
                                     NationalityGameId = 2,
                                     Name = SeedNames.Argentinean
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 4,
                                     NationalityGameId = 3,
                                     Name = SeedNames.Australian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 5,
                                     NationalityGameId = 4,
                                     Name = SeedNames.Austrian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 6,
                                     NationalityGameId = 5,
                                     Name = SeedNames.Azerbaijani
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 7,
                                     NationalityGameId = 6,
                                     Name = SeedNames.Bahraini
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 8,
                                     NationalityGameId = 7,
                                     Name = SeedNames.Belgian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 9,
                                     NationalityGameId = 8,
                                     Name = SeedNames.Bolivian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 10,
                                     NationalityGameId = 9,
                                     Name = SeedNames.Brazilian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 11,
                                     NationalityGameId = 10,
                                     Name = SeedNames.British
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 12,
                                     NationalityGameId = 11,
                                     Name = SeedNames.Bulgarian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 13,
                                     NationalityGameId = 12,
                                     Name = SeedNames.Cameroonian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 14,
                                     NationalityGameId = 13,
                                     Name = SeedNames.Canadian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 15,
                                     NationalityGameId = 14,
                                     Name = SeedNames.Chilean
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 16,
                                     NationalityGameId = 15,
                                     Name = SeedNames.Chinese
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 17,
                                     NationalityGameId = 16,
                                     Name = SeedNames.Colombian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 18,
                                     NationalityGameId = 17,
                                     Name = SeedNames.CostaRican
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 19,
                                     NationalityGameId = 18,
                                     Name = SeedNames.Croatian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 20,
                                     NationalityGameId = 19,
                                     Name = SeedNames.Cypriot
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 21,
                                     NationalityGameId = 20,
                                     Name = SeedNames.Czech
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 22,
                                     NationalityGameId = 21,
                                     Name = SeedNames.Danish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 23,
                                     NationalityGameId = 22,
                                     Name = SeedNames.Dutch
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 24,
                                     NationalityGameId = 23,
                                     Name = SeedNames.Ecuadorian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 25,
                                     NationalityGameId = 24,
                                     Name = SeedNames.English
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 26,
                                     NationalityGameId = 25,
                                     Name = SeedNames.Emirian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 27,
                                     NationalityGameId = 26,
                                     Name = SeedNames.Estonian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 28,
                                     NationalityGameId = 27,
                                     Name = SeedNames.Finnish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 29,
                                     NationalityGameId = 28,
                                     Name = SeedNames.French
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 30,
                                     NationalityGameId = 29,
                                     Name = SeedNames.German
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 31,
                                     NationalityGameId = 30,
                                     Name = SeedNames.Ghanaian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 32,
                                     NationalityGameId = 31,
                                     Name = SeedNames.Greek
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 33,
                                     NationalityGameId = 32,
                                     Name = SeedNames.Guatemalan
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 34,
                                     NationalityGameId = 33,
                                     Name = SeedNames.Honduran
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 35,
                                     NationalityGameId = 34,
                                     Name = SeedNames.HongKonger
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 36,
                                     NationalityGameId = 35,
                                     Name = SeedNames.Hungarian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 37,
                                     NationalityGameId = 36,
                                     Name = SeedNames.Icelander
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 38,
                                     NationalityGameId = 37,
                                     Name = SeedNames.Indian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 39,
                                     NationalityGameId = 38,
                                     Name = SeedNames.Indonesian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 40,
                                     NationalityGameId = 39,
                                     Name = SeedNames.Irish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 41,
                                     NationalityGameId = 40,
                                     Name = SeedNames.Israeli
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 42,
                                     NationalityGameId = 41,
                                     Name = SeedNames.Italian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 43,
                                     NationalityGameId = 42,
                                     Name = SeedNames.Jamaican
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 44,
                                     NationalityGameId = 43,
                                     Name = SeedNames.Japanese
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 45,
                                     NationalityGameId = 44,
                                     Name = SeedNames.Jordanian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 46,
                                     NationalityGameId = 45,
                                     Name = SeedNames.Kuwaiti
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 47,
                                     NationalityGameId = 46,
                                     Name = SeedNames.Latvian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 48,
                                     NationalityGameId = 47,
                                     Name = SeedNames.Lebanese
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 49,
                                     NationalityGameId = 48,
                                     Name = SeedNames.Lithuanian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 50,
                                     NationalityGameId = 49,
                                     Name = SeedNames.Luxembourger
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 51,
                                     NationalityGameId = 50,
                                     Name = SeedNames.Malaysian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 52,
                                     NationalityGameId = 51,
                                     Name = SeedNames.Maltese
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 53,
                                     NationalityGameId = 52,
                                     Name = SeedNames.Mexican
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 54,
                                     NationalityGameId = 53,
                                     Name = SeedNames.Monegasque
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 55,
                                     NationalityGameId = 54,
                                     Name = SeedNames.NewZealander
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 56,
                                     NationalityGameId = 55,
                                     Name = SeedNames.Nicaraguan
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 57,
                                     NationalityGameId = 56,
                                     Name = SeedNames.NorthernIrish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 58,
                                     NationalityGameId = 57,
                                     Name = SeedNames.Norwegian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 59,
                                     NationalityGameId = 58,
                                     Name = SeedNames.Omani
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 60,
                                     NationalityGameId = 59,
                                     Name = SeedNames.Pakistani
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 61,
                                     NationalityGameId = 60,
                                     Name = SeedNames.Panamanian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 62,
                                     NationalityGameId = 61,
                                     Name = SeedNames.Paraguayan
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 63,
                                     NationalityGameId = 62,
                                     Name = SeedNames.Peruvian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 64,
                                     NationalityGameId = 63,
                                     Name = SeedNames.Polish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 65,
                                     NationalityGameId = 64,
                                     Name = SeedNames.Portuguese
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 66,
                                     NationalityGameId = 65,
                                     Name = SeedNames.Qatari
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 67,
                                     NationalityGameId = 66,
                                     Name = SeedNames.Romanian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 68,
                                     NationalityGameId = 67,
                                     Name = SeedNames.Russian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 69,
                                     NationalityGameId = 68,
                                     Name = SeedNames.Salvadoran
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 70,
                                     NationalityGameId = 69,
                                     Name = SeedNames.Saudi
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 71,
                                     NationalityGameId = 70,
                                     Name = SeedNames.Scottish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 72,
                                     NationalityGameId = 71,
                                     Name = SeedNames.Serbian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 73,
                                     NationalityGameId = 72,
                                     Name = SeedNames.Singaporean
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 74,
                                     NationalityGameId = 73,
                                     Name = SeedNames.Slovakian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 75,
                                     NationalityGameId = 74,
                                     Name = SeedNames.Slovenian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 76,
                                     NationalityGameId = 75,
                                     Name = SeedNames.SouthKorean
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 77,
                                     NationalityGameId = 76,
                                     Name = SeedNames.SouthAfrican
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 78,
                                     NationalityGameId = 77,
                                     Name = SeedNames.Spanish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 79,
                                     NationalityGameId = 78,
                                     Name = SeedNames.Swedish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 80,
                                     NationalityGameId = 79,
                                     Name = SeedNames.Swiss
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 81,
                                     NationalityGameId = 80,
                                     Name = SeedNames.Thai
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 82,
                                     NationalityGameId = 81,
                                     Name = SeedNames.Turkish
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 83,
                                     NationalityGameId = 82,
                                     Name = SeedNames.Uruguayan
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 84,
                                     NationalityGameId = 83,
                                     Name = SeedNames.Ukrainian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 85,
                                     NationalityGameId = 84,
                                     Name = SeedNames.Venezuelan
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 86,
                                     NationalityGameId = 85,
                                     Name = SeedNames.Barbadian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 87,
                                     NationalityGameId = 86,
                                     Name = SeedNames.Welsh
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 88,
                                     NationalityGameId = 87,
                                     Name = SeedNames.Vietnamese
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 89,
                                     NationalityGameId = 88,
                                     Name = SeedNames.Algerian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 90,
                                     NationalityGameId = 89,
                                     Name = SeedNames.Bosnian
                                 },
                                 new NationalityEntity
                                 {
                                     Id = 91,
                                     NationalityGameId = 90,
                                     Name = SeedNames.Filipino
                                 });
        }
        catch
        {
            // Ignore exceptions in this step
        }
    }

    /// <summary>
    /// Insert teams into database
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    private void SeedTeams(ModelBuilder modelBuilder)
    {
        try
        {
            modelBuilder?.Entity<TeamEntity>()
                        .HasData(new TeamEntity
                                 {
                                     Id = 1,
                                     TeamGameId = 20190,
                                     Name = SeedNames.Mercedes
                                 },
                                 new TeamEntity
                                 {
                                     Id = 2,
                                     TeamGameId = 20191,
                                     Name = SeedNames.Ferrari
                                 },
                                 new TeamEntity
                                 {
                                     Id = 3,
                                     TeamGameId = 20192,
                                     Name = SeedNames.RedBullRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 4,
                                     TeamGameId = 20193,
                                     Name = SeedNames.Williams
                                 },
                                 new TeamEntity
                                 {
                                     Id = 5,
                                     TeamGameId = 20194,
                                     Name = SeedNames.RacingPoint
                                 },
                                 new TeamEntity
                                 {
                                     Id = 6,
                                     TeamGameId = 20195,
                                     Name = SeedNames.Renault
                                 },
                                 new TeamEntity
                                 {
                                     Id = 7,
                                     TeamGameId = 20196,
                                     Name = SeedNames.ToroRosso
                                 },
                                 new TeamEntity
                                 {
                                     Id = 8,
                                     TeamGameId = 20197,
                                     Name = SeedNames.Haas
                                 },
                                 new TeamEntity
                                 {
                                     Id = 9,
                                     TeamGameId = 20198,
                                     Name = SeedNames.McLaren
                                 },
                                 new TeamEntity
                                 {
                                     Id = 10,
                                     TeamGameId = 20199,
                                     Name = SeedNames.AlfaRomeo
                                 },
                                 new TeamEntity
                                 {
                                     Id = 11,
                                     TeamGameId = 201910,
                                     Name = SeedNames.McLaren1988
                                 },
                                 new TeamEntity
                                 {
                                     Id = 12,
                                     TeamGameId = 201911,
                                     Name = SeedNames.McLaren1991
                                 },
                                 new TeamEntity
                                 {
                                     Id = 13,
                                     TeamGameId = 201912,
                                     Name = SeedNames.Williams1992
                                 },
                                 new TeamEntity
                                 {
                                     Id = 14,
                                     TeamGameId = 201913,
                                     Name = SeedNames.Ferrari1995
                                 },
                                 new TeamEntity
                                 {
                                     Id = 15,
                                     TeamGameId = 201914,
                                     Name = SeedNames.Williams1996
                                 },
                                 new TeamEntity
                                 {
                                     Id = 16,
                                     TeamGameId = 201915,
                                     Name = SeedNames.McLaren1998
                                 },
                                 new TeamEntity
                                 {
                                     Id = 17,
                                     TeamGameId = 201916,
                                     Name = SeedNames.Ferrari2002
                                 },
                                 new TeamEntity
                                 {
                                     Id = 18,
                                     TeamGameId = 201917,
                                     Name = SeedNames.Ferrari2004
                                 },
                                 new TeamEntity
                                 {
                                     Id = 19,
                                     TeamGameId = 201918,
                                     Name = SeedNames.Renault2006
                                 },
                                 new TeamEntity
                                 {
                                     Id = 20,
                                     TeamGameId = 201919,
                                     Name = SeedNames.Ferrari2007
                                 },
                                 new TeamEntity
                                 {
                                     Id = 21,
                                     TeamGameId = 201921,
                                     Name = SeedNames.RedBull2010
                                 },
                                 new TeamEntity
                                 {
                                     Id = 22,
                                     TeamGameId = 201922,
                                     Name = SeedNames.Ferrari1976
                                 },
                                 new TeamEntity
                                 {
                                     Id = 23,
                                     TeamGameId = 201923,
                                     Name = SeedNames.ARTGrandPrix
                                 },
                                 new TeamEntity
                                 {
                                     Id = 24,
                                     TeamGameId = 201924,
                                     Name = SeedNames.CamposVexatecRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 25,
                                     TeamGameId = 201925,
                                     Name = SeedNames.Carlin
                                 },
                                 new TeamEntity
                                 {
                                     Id = 26,
                                     TeamGameId = 201926,
                                     Name = SeedNames.CharouzRacingSystem
                                 },
                                 new TeamEntity
                                 {
                                     Id = 27,
                                     TeamGameId = 201927,
                                     Name = SeedNames.DAMS
                                 },
                                 new TeamEntity
                                 {
                                     Id = 28,
                                     TeamGameId = 201928,
                                     Name = SeedNames.RussianTime
                                 },
                                 new TeamEntity
                                 {
                                     Id = 29,
                                     TeamGameId = 201929,
                                     Name = SeedNames.MPMotorsport
                                 },
                                 new TeamEntity
                                 {
                                     Id = 30,
                                     TeamGameId = 201930,
                                     Name = SeedNames.Pertamina
                                 },
                                 new TeamEntity
                                 {
                                     Id = 31,
                                     TeamGameId = 201931,
                                     Name = SeedNames.McLaren1990
                                 },
                                 new TeamEntity
                                 {
                                     Id = 32,
                                     TeamGameId = 201932,
                                     Name = SeedNames.Trident
                                 },
                                 new TeamEntity
                                 {
                                     Id = 33,
                                     TeamGameId = 201933,
                                     Name = SeedNames.BWTArden
                                 },
                                 new TeamEntity
                                 {
                                     Id = 34,
                                     TeamGameId = 201934,
                                     Name = SeedNames.McLaren1976
                                 },
                                 new TeamEntity
                                 {
                                     Id = 35,
                                     TeamGameId = 201935,
                                     Name = SeedNames.Lotus1972
                                 },
                                 new TeamEntity
                                 {
                                     Id = 36,
                                     TeamGameId = 201936,
                                     Name = SeedNames.Ferrari1979
                                 },
                                 new TeamEntity
                                 {
                                     Id = 37,
                                     TeamGameId = 201937,
                                     Name = SeedNames.McLaren1982
                                 },
                                 new TeamEntity
                                 {
                                     Id = 38,
                                     TeamGameId = 201938,
                                     Name = SeedNames.Williams2003
                                 },
                                 new TeamEntity
                                 {
                                     Id = 39,
                                     TeamGameId = 201939,
                                     Name = SeedNames.Brawn2009
                                 },
                                 new TeamEntity
                                 {
                                     Id = 40,
                                     TeamGameId = 201940,
                                     Name = SeedNames.Lotus1978
                                 },
                                 new TeamEntity
                                 {
                                     Id = 41,
                                     TeamGameId = 201942,
                                     Name = SeedNames.ArtGP19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 42,
                                     TeamGameId = 201943,
                                     Name = SeedNames.Campos19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 43,
                                     TeamGameId = 201944,
                                     Name = SeedNames.Carlin19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 44,
                                     TeamGameId = 201945,
                                     Name = SeedNames.SauberJuniorCharouz19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 45,
                                     TeamGameId = 201946,
                                     Name = SeedNames.Dams19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 46,
                                     TeamGameId = 201947,
                                     Name = SeedNames.UniVirtuosi19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 47,
                                     TeamGameId = 201948,
                                     Name = SeedNames.MPMotorsport19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 48,
                                     TeamGameId = 201949,
                                     Name = SeedNames.Prema19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 49,
                                     TeamGameId = 201950,
                                     Name = SeedNames.Trident19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 50,
                                     TeamGameId = 201951,
                                     Name = SeedNames.Arden19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 51,
                                     TeamGameId = 201963,
                                     Name = SeedNames.Ferrari1990
                                 },
                                 new TeamEntity
                                 {
                                     Id = 52,
                                     TeamGameId = 201964,
                                     Name = SeedNames.McLaren2010
                                 },
                                 new TeamEntity
                                 {
                                     Id = 53,
                                     TeamGameId = 201965,
                                     Name = SeedNames.Ferrari2010
                                 },
                                 new TeamEntity
                                 {
                                     Id = 54,
                                     TeamGameId = 20200,
                                     Name = SeedNames.Mercedes
                                 },
                                 new TeamEntity
                                 {
                                     Id = 55,
                                     TeamGameId = 20201,
                                     Name = SeedNames.Ferrari
                                 },
                                 new TeamEntity
                                 {
                                     Id = 56,
                                     TeamGameId = 20202,
                                     Name = SeedNames.RedBullRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 57,
                                     TeamGameId = 20203,
                                     Name = SeedNames.Williams
                                 },
                                 new TeamEntity
                                 {
                                     Id = 58,
                                     TeamGameId = 20204,
                                     Name = SeedNames.RacingPoint
                                 },
                                 new TeamEntity
                                 {
                                     Id = 59,
                                     TeamGameId = 20205,
                                     Name = SeedNames.Renault
                                 },
                                 new TeamEntity
                                 {
                                     Id = 60,
                                     TeamGameId = 20206,
                                     Name = SeedNames.AlphaTauri
                                 },
                                 new TeamEntity
                                 {
                                     Id = 61,
                                     TeamGameId = 20207,
                                     Name = SeedNames.Haas
                                 },
                                 new TeamEntity
                                 {
                                     Id = 62,
                                     TeamGameId = 20208,
                                     Name = SeedNames.McLaren
                                 },
                                 new TeamEntity
                                 {
                                     Id = 63,
                                     TeamGameId = 20209,
                                     Name = SeedNames.AlfaRomeo
                                 },
                                 new TeamEntity
                                 {
                                     Id = 64,
                                     TeamGameId = 202010,
                                     Name = SeedNames.McLaren1988
                                 },
                                 new TeamEntity
                                 {
                                     Id = 65,
                                     TeamGameId = 202011,
                                     Name = SeedNames.McLaren1991
                                 },
                                 new TeamEntity
                                 {
                                     Id = 66,
                                     TeamGameId = 202012,
                                     Name = SeedNames.Williams1992
                                 },
                                 new TeamEntity
                                 {
                                     Id = 67,
                                     TeamGameId = 202013,
                                     Name = SeedNames.Ferrari1995
                                 },
                                 new TeamEntity
                                 {
                                     Id = 68,
                                     TeamGameId = 202014,
                                     Name = SeedNames.Williams1996
                                 },
                                 new TeamEntity
                                 {
                                     Id = 69,
                                     TeamGameId = 202015,
                                     Name = SeedNames.McLaren1998
                                 },
                                 new TeamEntity
                                 {
                                     Id = 70,
                                     TeamGameId = 202016,
                                     Name = SeedNames.Ferrari2002
                                 },
                                 new TeamEntity
                                 {
                                     Id = 71,
                                     TeamGameId = 202017,
                                     Name = SeedNames.Ferrari2004
                                 },
                                 new TeamEntity
                                 {
                                     Id = 72,
                                     TeamGameId = 202018,
                                     Name = SeedNames.Renault2006
                                 },
                                 new TeamEntity
                                 {
                                     Id = 73,
                                     TeamGameId = 202019,
                                     Name = SeedNames.Ferrari2007
                                 },
                                 new TeamEntity
                                 {
                                     Id = 74,
                                     TeamGameId = 202020,
                                     Name = SeedNames.McLaren2008
                                 },
                                 new TeamEntity
                                 {
                                     Id = 75,
                                     TeamGameId = 202021,
                                     Name = SeedNames.RedBull2010
                                 },
                                 new TeamEntity
                                 {
                                     Id = 76,
                                     TeamGameId = 202022,
                                     Name = SeedNames.Ferrari1976
                                 },
                                 new TeamEntity
                                 {
                                     Id = 77,
                                     TeamGameId = 202023,
                                     Name = SeedNames.ARTGrandPrix
                                 },
                                 new TeamEntity
                                 {
                                     Id = 78,
                                     TeamGameId = 202024,
                                     Name = SeedNames.CamposVexatecRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 79,
                                     TeamGameId = 202025,
                                     Name = SeedNames.Carlin
                                 },
                                 new TeamEntity
                                 {
                                     Id = 80,
                                     TeamGameId = 202026,
                                     Name = SeedNames.CharouzRacingSystem
                                 },
                                 new TeamEntity
                                 {
                                     Id = 81,
                                     TeamGameId = 202027,
                                     Name = SeedNames.DAMS
                                 },
                                 new TeamEntity
                                 {
                                     Id = 82,
                                     TeamGameId = 202028,
                                     Name = SeedNames.RussianTime
                                 },
                                 new TeamEntity
                                 {
                                     Id = 83,
                                     TeamGameId = 202029,
                                     Name = SeedNames.MPMotorsport
                                 },
                                 new TeamEntity
                                 {
                                     Id = 84,
                                     TeamGameId = 202030,
                                     Name = SeedNames.Pertamina
                                 },
                                 new TeamEntity
                                 {
                                     Id = 85,
                                     TeamGameId = 202031,
                                     Name = SeedNames.McLaren1990
                                 },
                                 new TeamEntity
                                 {
                                     Id = 86,
                                     TeamGameId = 202032,
                                     Name = SeedNames.Trident
                                 },
                                 new TeamEntity
                                 {
                                     Id = 87,
                                     TeamGameId = 202033,
                                     Name = SeedNames.BWTArden
                                 },
                                 new TeamEntity
                                 {
                                     Id = 88,
                                     TeamGameId = 202034,
                                     Name = SeedNames.McLaren1976
                                 },
                                 new TeamEntity
                                 {
                                     Id = 89,
                                     TeamGameId = 202035,
                                     Name = SeedNames.Lotus1972
                                 },
                                 new TeamEntity
                                 {
                                     Id = 90,
                                     TeamGameId = 202036,
                                     Name = SeedNames.Ferrari1979
                                 },
                                 new TeamEntity
                                 {
                                     Id = 91,
                                     TeamGameId = 202037,
                                     Name = SeedNames.McLaren1982
                                 },
                                 new TeamEntity
                                 {
                                     Id = 92,
                                     TeamGameId = 202038,
                                     Name = SeedNames.Williams2003
                                 },
                                 new TeamEntity
                                 {
                                     Id = 93,
                                     TeamGameId = 202039,
                                     Name = SeedNames.Brawn2009
                                 },
                                 new TeamEntity
                                 {
                                     Id = 94,
                                     TeamGameId = 202040,
                                     Name = SeedNames.Lotus1978
                                 },
                                 new TeamEntity
                                 {
                                     Id = 95,
                                     TeamGameId = 202041,
                                     Name = SeedNames.F1Generic
                                 },
                                 new TeamEntity
                                 {
                                     Id = 96,
                                     TeamGameId = 202042,
                                     Name = SeedNames.ArtGP19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 97,
                                     TeamGameId = 202043,
                                     Name = SeedNames.Campos19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 98,
                                     TeamGameId = 202044,
                                     Name = SeedNames.Carlin19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 99,
                                     TeamGameId = 202045,
                                     Name = SeedNames.SauberJuniorCharouz19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 100,
                                     TeamGameId = 202046,
                                     Name = SeedNames.Dams19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 101,
                                     TeamGameId = 202047,
                                     Name = SeedNames.UniVirtuosi19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 102,
                                     TeamGameId = 202048,
                                     Name = SeedNames.MPMotorsport19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 103,
                                     TeamGameId = 202049,
                                     Name = SeedNames.Prema19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 104,
                                     TeamGameId = 202050,
                                     Name = SeedNames.Trident19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 105,
                                     TeamGameId = 202051,
                                     Name = SeedNames.Arden19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 106,
                                     TeamGameId = 202053,
                                     Name = SeedNames.Benetton1994
                                 },
                                 new TeamEntity
                                 {
                                     Id = 107,
                                     TeamGameId = 202054,
                                     Name = SeedNames.Benetton1995
                                 },
                                 new TeamEntity
                                 {
                                     Id = 108,
                                     TeamGameId = 202055,
                                     Name = SeedNames.Ferrari2000
                                 },
                                 new TeamEntity
                                 {
                                     Id = 109,
                                     TeamGameId = 202056,
                                     Name = SeedNames.Jordan1991
                                 },
                                 new TeamEntity
                                 {
                                     Id = 110,
                                     TeamGameId = 2020255,
                                     Name = SeedNames.MyTeam20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 111,
                                     TeamGameId = 20210,
                                     Name = SeedNames.Mercedes
                                 },
                                 new TeamEntity
                                 {
                                     Id = 112,
                                     TeamGameId = 20211,
                                     Name = SeedNames.Ferrari
                                 },
                                 new TeamEntity
                                 {
                                     Id = 113,
                                     TeamGameId = 20212,
                                     Name = SeedNames.RedBullRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 114,
                                     TeamGameId = 20213,
                                     Name = SeedNames.Williams
                                 },
                                 new TeamEntity
                                 {
                                     Id = 115,
                                     TeamGameId = 20214,
                                     Name = SeedNames.AstonMartin
                                 },
                                 new TeamEntity
                                 {
                                     Id = 116,
                                     TeamGameId = 20215,
                                     Name = SeedNames.Alpine
                                 },
                                 new TeamEntity
                                 {
                                     Id = 117,
                                     TeamGameId = 20216,
                                     Name = SeedNames.AlphaTauri
                                 },
                                 new TeamEntity
                                 {
                                     Id = 118,
                                     TeamGameId = 20217,
                                     Name = SeedNames.Haas
                                 },
                                 new TeamEntity
                                 {
                                     Id = 119,
                                     TeamGameId = 20218,
                                     Name = SeedNames.McLaren
                                 },
                                 new TeamEntity
                                 {
                                     Id = 120,
                                     TeamGameId = 20219,
                                     Name = SeedNames.AlfaRomeo
                                 },
                                 new TeamEntity
                                 {
                                     Id = 121,
                                     TeamGameId = 202142,
                                     Name = SeedNames.ArtGP19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 122,
                                     TeamGameId = 202143,
                                     Name = SeedNames.Campos19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 123,
                                     TeamGameId = 202144,
                                     Name = SeedNames.Carlin19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 124,
                                     TeamGameId = 202145,
                                     Name = SeedNames.SauberJuniorCharouz19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 125,
                                     TeamGameId = 202146,
                                     Name = SeedNames.Dams19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 126,
                                     TeamGameId = 202147,
                                     Name = SeedNames.UniVirtuosi19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 127,
                                     TeamGameId = 202148,
                                     Name = SeedNames.MPMotorsport19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 128,
                                     TeamGameId = 202149,
                                     Name = SeedNames.Prema19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 129,
                                     TeamGameId = 202150,
                                     Name = SeedNames.Trident19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 130,
                                     TeamGameId = 202151,
                                     Name = SeedNames.Arden19
                                 },
                                 new TeamEntity
                                 {
                                     Id = 131,
                                     TeamGameId = 202170,
                                     Name = SeedNames.ArtGP20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 132,
                                     TeamGameId = 202171,
                                     Name = SeedNames.Campos20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 133,
                                     TeamGameId = 202172,
                                     Name = SeedNames.Carlin20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 134,
                                     TeamGameId = 202173,
                                     Name = SeedNames.Charouz20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 135,
                                     TeamGameId = 202174,
                                     Name = SeedNames.Dams20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 136,
                                     TeamGameId = 202175,
                                     Name = SeedNames.UniVirtuosi20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 137,
                                     TeamGameId = 202176,
                                     Name = SeedNames.MPMotorsport20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 138,
                                     TeamGameId = 202177,
                                     Name = SeedNames.Prema20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 139,
                                     TeamGameId = 202178,
                                     Name = SeedNames.Trident20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 140,
                                     TeamGameId = 202179,
                                     Name = SeedNames.BWT20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 141,
                                     TeamGameId = 202180,
                                     Name = SeedNames.Hitech20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 142,
                                     TeamGameId = 202185,
                                     Name = SeedNames.Mercedes2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 143,
                                     TeamGameId = 202186,
                                     Name = SeedNames.Ferrari2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 144,
                                     TeamGameId = 202187,
                                     Name = SeedNames.RedBull2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 145,
                                     TeamGameId = 202188,
                                     Name = SeedNames.Williams2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 146,
                                     TeamGameId = 202189,
                                     Name = SeedNames.RacingPoint2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 147,
                                     TeamGameId = 202190,
                                     Name = SeedNames.Renault2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 148,
                                     TeamGameId = 202191,
                                     Name = SeedNames.AlphaTauri2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 149,
                                     TeamGameId = 202192,
                                     Name = SeedNames.Haas2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 150,
                                     TeamGameId = 202193,
                                     Name = SeedNames.McLaren2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 151,
                                     TeamGameId = 202194,
                                     Name = SeedNames.AlfaRomeo2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 152,
                                     TeamGameId = 2021106,
                                     Name = SeedNames.Prema21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 153,
                                     TeamGameId = 2021107,
                                     Name = SeedNames.UniVirtuosi21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 154,
                                     TeamGameId = 2021108,
                                     Name = SeedNames.Carlin21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 155,
                                     TeamGameId = 2021109,
                                     Name = SeedNames.Hitech21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 156,
                                     TeamGameId = 2021110,
                                     Name = SeedNames.ArtGP21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 157,
                                     TeamGameId = 2021111,
                                     Name = SeedNames.MPMotorsport21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 158,
                                     TeamGameId = 2021112,
                                     Name = SeedNames.Charouz21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 159,
                                     TeamGameId = 2021113,
                                     Name = SeedNames.Dams21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 160,
                                     TeamGameId = 2021114,
                                     Name = SeedNames.Campos21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 161,
                                     TeamGameId = 2021115,
                                     Name = SeedNames.BWT21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 162,
                                     TeamGameId = 2021116,
                                     Name = SeedNames.Trident21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 163,
                                     TeamGameId = 20220,
                                     Name = SeedNames.Mercedes
                                 },
                                 new TeamEntity
                                 {
                                     Id = 164,
                                     TeamGameId = 20221,
                                     Name = SeedNames.Ferrari
                                 },
                                 new TeamEntity
                                 {
                                     Id = 165,
                                     TeamGameId = 20222,
                                     Name = SeedNames.RedBullRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 166,
                                     TeamGameId = 20223,
                                     Name = SeedNames.Williams
                                 },
                                 new TeamEntity
                                 {
                                     Id = 167,
                                     TeamGameId = 20224,
                                     Name = SeedNames.AstonMartin
                                 },
                                 new TeamEntity
                                 {
                                     Id = 168,
                                     TeamGameId = 20225,
                                     Name = SeedNames.Alpine
                                 },
                                 new TeamEntity
                                 {
                                     Id = 169,
                                     TeamGameId = 20226,
                                     Name = SeedNames.AlphaTauri
                                 },
                                 new TeamEntity
                                 {
                                     Id = 170,
                                     TeamGameId = 20227,
                                     Name = SeedNames.Haas
                                 },
                                 new TeamEntity
                                 {
                                     Id = 171,
                                     TeamGameId = 20228,
                                     Name = SeedNames.McLaren
                                 },
                                 new TeamEntity
                                 {
                                     Id = 172,
                                     TeamGameId = 20229,
                                     Name = SeedNames.AlfaRomeo
                                 },
                                 new TeamEntity
                                 {
                                     Id = 173,
                                     TeamGameId = 202285,
                                     Name = SeedNames.Mercedes2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 174,
                                     TeamGameId = 202286,
                                     Name = SeedNames.Ferrari2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 175,
                                     TeamGameId = 202287,
                                     Name = SeedNames.RedBull2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 176,
                                     TeamGameId = 202288,
                                     Name = SeedNames.Williams2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 177,
                                     TeamGameId = 202289,
                                     Name = SeedNames.RacingPoint2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 178,
                                     TeamGameId = 202290,
                                     Name = SeedNames.Renault2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 179,
                                     TeamGameId = 202291,
                                     Name = SeedNames.AlphaTauri2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 180,
                                     TeamGameId = 202292,
                                     Name = SeedNames.Haas2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 181,
                                     TeamGameId = 202293,
                                     Name = SeedNames.McLaren2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 182,
                                     TeamGameId = 202294,
                                     Name = SeedNames.AlfaRomeo2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 183,
                                     TeamGameId = 202295,
                                     Name = SeedNames.AstonMartinDB11V12
                                 },
                                 new TeamEntity
                                 {
                                     Id = 184,
                                     TeamGameId = 202296,
                                     Name = SeedNames.AstonMartinVantageF1Edition
                                 },
                                 new TeamEntity
                                 {
                                     Id = 185,
                                     TeamGameId = 202297,
                                     Name = SeedNames.AstonMartinVantageSafetyCar
                                 },
                                 new TeamEntity
                                 {
                                     Id = 186,
                                     TeamGameId = 202298,
                                     Name = SeedNames.FerrariF8Tributo
                                 },
                                 new TeamEntity
                                 {
                                     Id = 187,
                                     TeamGameId = 202299,
                                     Name = SeedNames.FerrariRoma
                                 },
                                 new TeamEntity
                                 {
                                     Id = 188,
                                     TeamGameId = 2022100,
                                     Name = SeedNames.McLaren720S
                                 },
                                 new TeamEntity
                                 {
                                     Id = 189,
                                     TeamGameId = 2022101,
                                     Name = SeedNames.McLarenArtura
                                 },
                                 new TeamEntity
                                 {
                                     Id = 190,
                                     TeamGameId = 2022102,
                                     Name = SeedNames.MercedesAMGGTBlackSeriesSafetyCar
                                 },
                                 new TeamEntity
                                 {
                                     Id = 191,
                                     TeamGameId = 2022103,
                                     Name = SeedNames.MercedesAMGGTRPro
                                 },
                                 new TeamEntity
                                 {
                                     Id = 192,
                                     TeamGameId = 2022104,
                                     Name = SeedNames.F1CustomTeam
                                 },
                                 new TeamEntity
                                 {
                                     Id = 193,
                                     TeamGameId = 2022106,
                                     Name = SeedNames.Prema21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 194,
                                     TeamGameId = 2022107,
                                     Name = SeedNames.FerrariUniVirtuosi21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 195,
                                     TeamGameId = 2022108,
                                     Name = SeedNames.Carlin21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 196,
                                     TeamGameId = 2022109,
                                     Name = SeedNames.Hitech21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 197,
                                     TeamGameId = 2022110,
                                     Name = SeedNames.ArtGP21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 198,
                                     TeamGameId = 2022111,
                                     Name = SeedNames.MPMotorsport21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 199,
                                     TeamGameId = 2022112,
                                     Name = SeedNames.Charouz21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 200,
                                     TeamGameId = 2022113,
                                     Name = SeedNames.Dams21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 201,
                                     TeamGameId = 2022114,
                                     Name = SeedNames.Campos21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 202,
                                     TeamGameId = 2022115,
                                     Name = SeedNames.BWT21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 203,
                                     TeamGameId = 2022116,
                                     Name = SeedNames.Trident21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 204,
                                     TeamGameId = 2022117,
                                     Name = SeedNames.MercedesAMGGTBlackSeries
                                 },
                                 new TeamEntity
                                 {
                                     Id = 205,
                                     TeamGameId = 2022118,
                                     Name = SeedNames.Prema22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 206,
                                     TeamGameId = 2022119,
                                     Name = SeedNames.Virtuosi22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 207,
                                     TeamGameId = 2022120,
                                     Name = SeedNames.Carlin22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 208,
                                     TeamGameId = 2022121,
                                     Name = SeedNames.Hitech22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 209,
                                     TeamGameId = 2022122,
                                     Name = SeedNames.ArtGP22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 210,
                                     TeamGameId = 2022123,
                                     Name = SeedNames.MPMotorsport22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 211,
                                     TeamGameId = 2022124,
                                     Name = SeedNames.Charouz22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 212,
                                     TeamGameId = 2022125,
                                     Name = SeedNames.Dams22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 213,
                                     TeamGameId = 2022126,
                                     Name = SeedNames.Campos22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 214,
                                     TeamGameId = 2022127,
                                     Name = SeedNames.VanAmersfoortRacing22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 215,
                                     TeamGameId = 2022128,
                                     Name = SeedNames.Trident22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 216,
                                     TeamGameId = 20230,
                                     Name = SeedNames.Mercedes
                                 },
                                 new TeamEntity
                                 {
                                     Id = 217,
                                     TeamGameId = 20231,
                                     Name = SeedNames.Ferrari
                                 },
                                 new TeamEntity
                                 {
                                     Id = 218,
                                     TeamGameId = 20232,
                                     Name = SeedNames.RedBullRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 219,
                                     TeamGameId = 20233,
                                     Name = SeedNames.Williams
                                 },
                                 new TeamEntity
                                 {
                                     Id = 220,
                                     TeamGameId = 20234,
                                     Name = SeedNames.AstonMartin
                                 },
                                 new TeamEntity
                                 {
                                     Id = 221,
                                     TeamGameId = 20235,
                                     Name = SeedNames.Alpine
                                 },
                                 new TeamEntity
                                 {
                                     Id = 222,
                                     TeamGameId = 20236,
                                     Name = SeedNames.RB
                                 },
                                 new TeamEntity
                                 {
                                     Id = 223,
                                     TeamGameId = 20237,
                                     Name = SeedNames.Haas
                                 },
                                 new TeamEntity
                                 {
                                     Id = 224,
                                     TeamGameId = 20238,
                                     Name = SeedNames.McLaren
                                 },
                                 new TeamEntity
                                 {
                                     Id = 225,
                                     TeamGameId = 20239,
                                     Name = SeedNames.AlfaRomeo
                                 },
                                 new TeamEntity
                                 {
                                     Id = 226,
                                     TeamGameId = 202385,
                                     Name = SeedNames.Mercedes2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 227,
                                     TeamGameId = 202386,
                                     Name = SeedNames.Ferrari2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 228,
                                     TeamGameId = 202387,
                                     Name = SeedNames.RedBull2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 229,
                                     TeamGameId = 202388,
                                     Name = SeedNames.Williams2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 230,
                                     TeamGameId = 202389,
                                     Name = SeedNames.RacingPoint2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 231,
                                     TeamGameId = 202390,
                                     Name = SeedNames.Renault2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 232,
                                     TeamGameId = 202391,
                                     Name = SeedNames.AlphaTauri2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 233,
                                     TeamGameId = 202392,
                                     Name = SeedNames.Haas2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 234,
                                     TeamGameId = 202393,
                                     Name = SeedNames.McLaren2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 235,
                                     TeamGameId = 202394,
                                     Name = SeedNames.AlfaRomeo2020
                                 },
                                 new TeamEntity
                                 {
                                     Id = 236,
                                     TeamGameId = 202395,
                                     Name = SeedNames.AstonMartinDB11V12
                                 },
                                 new TeamEntity
                                 {
                                     Id = 237,
                                     TeamGameId = 202396,
                                     Name = SeedNames.AstonMartinVantageF1Edition
                                 },
                                 new TeamEntity
                                 {
                                     Id = 238,
                                     TeamGameId = 202397,
                                     Name = SeedNames.AstonMartinVantageSafetyCar
                                 },
                                 new TeamEntity
                                 {
                                     Id = 239,
                                     TeamGameId = 202398,
                                     Name = SeedNames.FerrariF8Tributo
                                 },
                                 new TeamEntity
                                 {
                                     Id = 240,
                                     TeamGameId = 202399,
                                     Name = SeedNames.FerrariRoma
                                 },
                                 new TeamEntity
                                 {
                                     Id = 241,
                                     TeamGameId = 2023100,
                                     Name = SeedNames.McLaren720S
                                 },
                                 new TeamEntity
                                 {
                                     Id = 242,
                                     TeamGameId = 2023101,
                                     Name = SeedNames.McLarenArtura
                                 },
                                 new TeamEntity
                                 {
                                     Id = 243,
                                     TeamGameId = 2023102,
                                     Name = SeedNames.MercedesAMGGTBlackSeriesSafetyCar
                                 },
                                 new TeamEntity
                                 {
                                     Id = 244,
                                     TeamGameId = 2023103,
                                     Name = SeedNames.MercedesAMGGTRPro
                                 },
                                 new TeamEntity
                                 {
                                     Id = 245,
                                     TeamGameId = 2023104,
                                     Name = SeedNames.F1CustomTeam
                                 },
                                 new TeamEntity
                                 {
                                     Id = 246,
                                     TeamGameId = 2023106,
                                     Name = SeedNames.Prema21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 247,
                                     TeamGameId = 2023107,
                                     Name = SeedNames.FerrariUniVirtuosi21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 248,
                                     TeamGameId = 2023108,
                                     Name = SeedNames.Carlin21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 249,
                                     TeamGameId = 2023109,
                                     Name = SeedNames.Hitech21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 250,
                                     TeamGameId = 2023110,
                                     Name = SeedNames.ArtGP21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 251,
                                     TeamGameId = 2023111,
                                     Name = SeedNames.MPMotorsport21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 252,
                                     TeamGameId = 2023112,
                                     Name = SeedNames.Charouz21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 253,
                                     TeamGameId = 2023113,
                                     Name = SeedNames.Dams21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 254,
                                     TeamGameId = 2023114,
                                     Name = SeedNames.Campos21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 255,
                                     TeamGameId = 2023115,
                                     Name = SeedNames.BWT21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 256,
                                     TeamGameId = 2023116,
                                     Name = SeedNames.Trident21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 257,
                                     TeamGameId = 2023117,
                                     Name = SeedNames.MercedesAMGGTBlackSeries
                                 },
                                 new TeamEntity
                                 {
                                     Id = 258,
                                     TeamGameId = 2023118,
                                     Name = SeedNames.Mercedes22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 259,
                                     TeamGameId = 2023119,
                                     Name = SeedNames.Ferrari22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 260,
                                     TeamGameId = 2023120,
                                     Name = SeedNames.RedBullRacing22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 261,
                                     TeamGameId = 2023121,
                                     Name = SeedNames.Williams22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 262,
                                     TeamGameId = 2023122,
                                     Name = SeedNames.AstonMartin22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 263,
                                     TeamGameId = 2023123,
                                     Name = SeedNames.Alpine22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 264,
                                     TeamGameId = 2023124,
                                     Name = SeedNames.AlphaTauri22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 265,
                                     TeamGameId = 2023125,
                                     Name = SeedNames.Haas22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 266,
                                     TeamGameId = 2023126,
                                     Name = SeedNames.McLaren22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 267,
                                     TeamGameId = 2023127,
                                     Name = SeedNames.AlfaRomeo22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 268,
                                     TeamGameId = 2023128,
                                     Name = SeedNames.Konnersport22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 269,
                                     TeamGameId = 2023129,
                                     Name = SeedNames.Konnersport
                                 },
                                 new TeamEntity
                                 {
                                     Id = 270,
                                     TeamGameId = 2023130,
                                     Name = SeedNames.Prema22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 271,
                                     TeamGameId = 2023131,
                                     Name = SeedNames.Virtuosi22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 272,
                                     TeamGameId = 2023132,
                                     Name = SeedNames.Carlin22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 273,
                                     TeamGameId = 2023133,
                                     Name = SeedNames.MPMotorsport22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 274,
                                     TeamGameId = 2023134,
                                     Name = SeedNames.Charouz22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 275,
                                     TeamGameId = 2023135,
                                     Name = SeedNames.Dams22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 276,
                                     TeamGameId = 2023136,
                                     Name = SeedNames.Campos22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 277,
                                     TeamGameId = 2023137,
                                     Name = SeedNames.VanAmersfoortRacing22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 278,
                                     TeamGameId = 2023138,
                                     Name = SeedNames.Trident22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 279,
                                     TeamGameId = 2023139,
                                     Name = SeedNames.Hitech22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 280,
                                     TeamGameId = 2023140,
                                     Name = SeedNames.ArtGP22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 281,
                                     TeamGameId = 20240,
                                     Name = SeedNames.Mercedes
                                 },
                                 new TeamEntity
                                 {
                                     Id = 282,
                                     TeamGameId = 20241,
                                     Name = SeedNames.Ferrari
                                 },
                                 new TeamEntity
                                 {
                                     Id = 283,
                                     TeamGameId = 20242,
                                     Name = SeedNames.RedBullRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 284,
                                     TeamGameId = 20243,
                                     Name = SeedNames.Williams
                                 },
                                 new TeamEntity
                                 {
                                     Id = 285,
                                     TeamGameId = 20244,
                                     Name = SeedNames.AstonMartin
                                 },
                                 new TeamEntity
                                 {
                                     Id = 286,
                                     TeamGameId = 20245,
                                     Name = SeedNames.Alpine
                                 },
                                 new TeamEntity
                                 {
                                     Id = 287,
                                     TeamGameId = 20246,
                                     Name = SeedNames.RB
                                 },
                                 new TeamEntity
                                 {
                                     Id = 288,
                                     TeamGameId = 20247,
                                     Name = SeedNames.Haas
                                 },
                                 new TeamEntity
                                 {
                                     Id = 289,
                                     TeamGameId = 20248,
                                     Name = SeedNames.McLaren
                                 },
                                 new TeamEntity
                                 {
                                     Id = 290,
                                     TeamGameId = 20249,
                                     Name = SeedNames.Sauber
                                 },
                                 new TeamEntity
                                 {
                                     Id = 291,
                                     TeamGameId = 202441,
                                     Name = SeedNames.F1Generic
                                 },
                                 new TeamEntity
                                 {
                                     Id = 292,
                                     TeamGameId = 2024104,
                                     Name = SeedNames.F1CustomTeam
                                 },
                                 new TeamEntity
                                 {
                                     Id = 293,
                                     TeamGameId = 2024143,
                                     Name = SeedNames.ArtGP23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 294,
                                     TeamGameId = 2024144,
                                     Name = SeedNames.Campos23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 295,
                                     TeamGameId = 2024145,
                                     Name = SeedNames.Carlin23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 296,
                                     TeamGameId = 2024146,
                                     Name = SeedNames.PHM23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 297,
                                     TeamGameId = 2024147,
                                     Name = SeedNames.Dams23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 298,
                                     TeamGameId = 2024148,
                                     Name = SeedNames.Hitech23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 299,
                                     TeamGameId = 2024149,
                                     Name = SeedNames.MPMotorsport23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 300,
                                     TeamGameId = 2024150,
                                     Name = SeedNames.Prema23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 301,
                                     TeamGameId = 2024151,
                                     Name = SeedNames.Trident23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 302,
                                     TeamGameId = 2024152,
                                     Name = SeedNames.VanAmersfoortRacing23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 303,
                                     TeamGameId = 2024153,
                                     Name = SeedNames.Virtuosi23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 304,
                                     TeamGameId = 20250,
                                     Name = SeedNames.Mercedes
                                 },
                                 new TeamEntity
                                 {
                                     Id = 305,
                                     TeamGameId = 20251,
                                     Name = SeedNames.Ferrari
                                 },
                                 new TeamEntity
                                 {
                                     Id = 306,
                                     TeamGameId = 20252,
                                     Name = SeedNames.RedBullRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 307,
                                     TeamGameId = 20253,
                                     Name = SeedNames.Williams
                                 },
                                 new TeamEntity
                                 {
                                     Id = 308,
                                     TeamGameId = 20254,
                                     Name = SeedNames.AstonMartin
                                 },
                                 new TeamEntity
                                 {
                                     Id = 309,
                                     TeamGameId = 20255,
                                     Name = SeedNames.Alpine
                                 },
                                 new TeamEntity
                                 {
                                     Id = 310,
                                     TeamGameId = 20256,
                                     Name = SeedNames.RB
                                 },
                                 new TeamEntity
                                 {
                                     Id = 311,
                                     TeamGameId = 20257,
                                     Name = SeedNames.Haas
                                 },
                                 new TeamEntity
                                 {
                                     Id = 312,
                                     TeamGameId = 20258,
                                     Name = SeedNames.McLaren
                                 },
                                 new TeamEntity
                                 {
                                     Id = 313,
                                     TeamGameId = 20259,
                                     Name = SeedNames.Sauber
                                 },
                                 new TeamEntity
                                 {
                                     Id = 314,
                                     TeamGameId = 202541,
                                     Name = SeedNames.F1Generic
                                 },
                                 new TeamEntity
                                 {
                                     Id = 315,
                                     TeamGameId = 2025104,
                                     Name = SeedNames.F1CustomTeam
                                 },
                                 new TeamEntity
                                 {
                                     Id = 316,
                                     TeamGameId = 2025129,
                                     Name = SeedNames.Konnersport
                                 },
                                 new TeamEntity
                                 {
                                     Id = 317,
                                     TeamGameId = 2025142,
                                     Name = SeedNames.APXGP24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 318,
                                     TeamGameId = 2025154,
                                     Name = SeedNames.APXGP25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 319,
                                     TeamGameId = 2025155,
                                     Name = SeedNames.Konnersport24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 320,
                                     TeamGameId = 2025158,
                                     Name = SeedNames.ArtGP24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 321,
                                     TeamGameId = 2025159,
                                     Name = SeedNames.Campos24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 322,
                                     TeamGameId = 2025160,
                                     Name = SeedNames.RodinMotorsport24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 323,
                                     TeamGameId = 2025161,
                                     Name = SeedNames.AIXRacing24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 324,
                                     TeamGameId = 2025162,
                                     Name = SeedNames.Dams24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 325,
                                     TeamGameId = 2025163,
                                     Name = SeedNames.Hitech24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 326,
                                     TeamGameId = 2025164,
                                     Name = SeedNames.MPMotorsport24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 327,
                                     TeamGameId = 2025165,
                                     Name = SeedNames.Prema24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 328,
                                     TeamGameId = 2025166,
                                     Name = SeedNames.Trident24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 329,
                                     TeamGameId = 2025167,
                                     Name = SeedNames.VanAmersfoortRacing24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 330,
                                     TeamGameId = 2025168,
                                     Name = SeedNames.Invicta24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 331,
                                     TeamGameId = 2025185,
                                     Name = SeedNames.Mercedes24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 332,
                                     TeamGameId = 2025186,
                                     Name = SeedNames.Ferrari24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 333,
                                     TeamGameId = 2025187,
                                     Name = SeedNames.RedBullRacing24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 334,
                                     TeamGameId = 2025188,
                                     Name = SeedNames.Williams24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 335,
                                     TeamGameId = 2025189,
                                     Name = SeedNames.AstonMartin24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 336,
                                     TeamGameId = 2025190,
                                     Name = SeedNames.Alpine24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 337,
                                     TeamGameId = 2025191,
                                     Name = SeedNames.RB24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 338,
                                     TeamGameId = 2025192,
                                     Name = SeedNames.Haas24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 339,
                                     TeamGameId = 2025193,
                                     Name = SeedNames.McLaren24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 340,
                                     TeamGameId = 2025194,
                                     Name = SeedNames.Sauber24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 341,
                                     TeamGameId = 20260,
                                     Name = SeedNames.Mercedes
                                 },
                                 new TeamEntity
                                 {
                                     Id = 342,
                                     TeamGameId = 20261,
                                     Name = SeedNames.Ferrari
                                 },
                                 new TeamEntity
                                 {
                                     Id = 343,
                                     TeamGameId = 20262,
                                     Name = SeedNames.RedBullRacing
                                 },
                                 new TeamEntity
                                 {
                                     Id = 344,
                                     TeamGameId = 20263,
                                     Name = SeedNames.Williams
                                 },
                                 new TeamEntity
                                 {
                                     Id = 345,
                                     TeamGameId = 20264,
                                     Name = SeedNames.AstonMartin
                                 },
                                 new TeamEntity
                                 {
                                     Id = 346,
                                     TeamGameId = 20265,
                                     Name = SeedNames.Alpine
                                 },
                                 new TeamEntity
                                 {
                                     Id = 347,
                                     TeamGameId = 20266,
                                     Name = SeedNames.RB
                                 },
                                 new TeamEntity
                                 {
                                     Id = 348,
                                     TeamGameId = 20267,
                                     Name = SeedNames.Haas
                                 },
                                 new TeamEntity
                                 {
                                     Id = 349,
                                     TeamGameId = 20268,
                                     Name = SeedNames.McLaren
                                 },
                                 new TeamEntity
                                 {
                                     Id = 350,
                                     TeamGameId = 20269,
                                     Name = SeedNames.Sauber
                                 },
                                 new TeamEntity
                                 {
                                     Id = 351,
                                     TeamGameId = 202641,
                                     Name = SeedNames.F1Generic
                                 },
                                 new TeamEntity
                                 {
                                     Id = 352,
                                     TeamGameId = 2026104,
                                     Name = SeedNames.F1CustomTeam
                                 },
                                 new TeamEntity
                                 {
                                     Id = 353,
                                     TeamGameId = 2026129,
                                     Name = SeedNames.Konnersport
                                 },
                                 new TeamEntity
                                 {
                                     Id = 354,
                                     TeamGameId = 2026142,
                                     Name = SeedNames.APXGP24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 355,
                                     TeamGameId = 2026154,
                                     Name = SeedNames.APXGP25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 356,
                                     TeamGameId = 2026155,
                                     Name = SeedNames.Konnersport24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 357,
                                     TeamGameId = 2026158,
                                     Name = SeedNames.ArtGP24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 358,
                                     TeamGameId = 2026159,
                                     Name = SeedNames.Campos24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 359,
                                     TeamGameId = 2026160,
                                     Name = SeedNames.RodinMotorsport24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 360,
                                     TeamGameId = 2026161,
                                     Name = SeedNames.AIXRacing24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 361,
                                     TeamGameId = 2026162,
                                     Name = SeedNames.Dams24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 362,
                                     TeamGameId = 2026163,
                                     Name = SeedNames.Hitech24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 363,
                                     TeamGameId = 2026164,
                                     Name = SeedNames.MPMotorsport24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 364,
                                     TeamGameId = 2026165,
                                     Name = SeedNames.Prema24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 365,
                                     TeamGameId = 2026166,
                                     Name = SeedNames.Trident24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 366,
                                     TeamGameId = 2026167,
                                     Name = SeedNames.VanAmersfoortRacing24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 367,
                                     TeamGameId = 2026168,
                                     Name = SeedNames.Invicta24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 368,
                                     TeamGameId = 2026185,
                                     Name = SeedNames.Mercedes24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 369,
                                     TeamGameId = 2026186,
                                     Name = SeedNames.Ferrari24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 370,
                                     TeamGameId = 2026187,
                                     Name = SeedNames.RedBullRacing24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 371,
                                     TeamGameId = 2026188,
                                     Name = SeedNames.Williams24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 372,
                                     TeamGameId = 2026189,
                                     Name = SeedNames.AstonMartin24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 373,
                                     TeamGameId = 2026190,
                                     Name = SeedNames.Alpine24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 374,
                                     TeamGameId = 2026191,
                                     Name = SeedNames.RB24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 375,
                                     TeamGameId = 2026192,
                                     Name = SeedNames.Haas24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 376,
                                     TeamGameId = 2026193,
                                     Name = SeedNames.McLaren24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 377,
                                     TeamGameId = 2026194,
                                     Name = SeedNames.Sauber24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 378,
                                     TeamGameId = 2026465,
                                     Name = SeedNames.ArtGP25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 379,
                                     TeamGameId = 2026466,
                                     Name = SeedNames.Campos25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 380,
                                     TeamGameId = 2026467,
                                     Name = SeedNames.RodinMotorsport25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 381,
                                     TeamGameId = 2026468,
                                     Name = SeedNames.AIXRacing25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 382,
                                     TeamGameId = 2026469,
                                     Name = SeedNames.Dams25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 383,
                                     TeamGameId = 2026470,
                                     Name = SeedNames.Hitech25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 384,
                                     TeamGameId = 2026471,
                                     Name = SeedNames.MPMotorsport25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 385,
                                     TeamGameId = 2026472,
                                     Name = SeedNames.Prema25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 386,
                                     TeamGameId = 2026473,
                                     Name = SeedNames.Trident25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 387,
                                     TeamGameId = 2026474,
                                     Name = SeedNames.VanAmersfoortRacing25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 388,
                                     TeamGameId = 2026475,
                                     Name = SeedNames.Invicta25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 389,
                                     TeamGameId = 2026476,
                                     Name = SeedNames.Mercedes26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 390,
                                     TeamGameId = 2026477,
                                     Name = SeedNames.Ferrari26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 391,
                                     TeamGameId = 2026478,
                                     Name = SeedNames.RedBullRacing26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 392,
                                     TeamGameId = 2026479,
                                     Name = SeedNames.Williams26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 393,
                                     TeamGameId = 2026480,
                                     Name = SeedNames.AstonMartin26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 394,
                                     TeamGameId = 2026481,
                                     Name = SeedNames.Alpine26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 395,
                                     TeamGameId = 2026482,
                                     Name = SeedNames.RB26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 396,
                                     TeamGameId = 2026483,
                                     Name = SeedNames.Haas26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 397,
                                     TeamGameId = 2026484,
                                     Name = SeedNames.McLaren26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 398,
                                     TeamGameId = 2026485,
                                     Name = SeedNames.Audi26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 399,
                                     TeamGameId = 2026486,
                                     Name = SeedNames.Cadillac26
                                 },
                                 new TeamEntity
                                 {
                                     Id = 1000,
                                     TeamGameId = 2020255,
                                     Name = SeedNames.MyTeam20
                                 },
                                 new TeamEntity
                                 {
                                     Id = 1001,
                                     TeamGameId = 2021255,
                                     Name = SeedNames.MyTeam21
                                 },
                                 new TeamEntity
                                 {
                                     Id = 1002,
                                     TeamGameId = 2022255,
                                     Name = SeedNames.MyTeam22
                                 },
                                 new TeamEntity
                                 {
                                     Id = 1003,
                                     TeamGameId = 2023255,
                                     Name = SeedNames.MyTeam23
                                 },
                                 new TeamEntity
                                 {
                                     Id = 1004,
                                     TeamGameId = 2024255,
                                     Name = SeedNames.MyTeam24
                                 },
                                 new TeamEntity
                                 {
                                     Id = 1005,
                                     TeamGameId = 2025255,
                                     Name = SeedNames.MyTeam25
                                 },
                                 new TeamEntity
                                 {
                                     Id = 1006,
                                     TeamGameId = 2026255,
                                     Name = SeedNames.MyTeam26
                                 });
        }
        catch
        {
            // Ignore exceptions in this step
        }
    }

    #endregion // Private methods

    #region DbContext

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured == false)
        {
            ConfigureOptions(optionsBuilder, Logger, AppMetrics);
        }

        base.OnConfiguring(optionsBuilder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameVersionEntity>();
        modelBuilder.Entity<TrackEntity>();
        modelBuilder.Entity<SessionEntity>();
        modelBuilder.Entity<ParticipantEntity>();
        modelBuilder.Entity<TeamEntity>();
        modelBuilder.Entity<LapEntity>();
        modelBuilder.Entity<DriverEntity>();
        modelBuilder.Entity<TeamEntity>();
        modelBuilder.Entity<FinalClassificationEntity>();
        modelBuilder.Entity<CarTelemetryEntity>();
        modelBuilder.Entity<SessionAttributesEntity>();
        modelBuilder.Entity<ChampionshipEntity>();
        modelBuilder.Entity<ChampionshipTrackEntity>();
        modelBuilder.Entity<ChampionshipPointsEntity>();

        modelBuilder.Entity<ParticipantEntity>().Navigation(p => p.Nationality).AutoInclude();
        modelBuilder.Entity<ParticipantEntity>().Navigation(p => p.Team).AutoInclude();
        modelBuilder.Entity<ParticipantEntity>().Navigation(p => p.Driver).AutoInclude();
        modelBuilder.Entity<LapEntity>().Navigation(p => p.Participant).AutoInclude();
        modelBuilder.Entity<CarTelemetryEntity>().Navigation(t => t.Lap).AutoInclude();
        modelBuilder.Entity<ChampionshipEntity>().Navigation(t => t.GameVersion).AutoInclude();
        modelBuilder.Entity<ChampionshipEntity>().Navigation(t => t.Tracks).AutoInclude();
        modelBuilder.Entity<ChampionshipEntity>().Navigation(t => t.Points).AutoInclude();
        modelBuilder.Entity<ChampionshipTrackEntity>().Navigation(t => t.QualifyingSession).AutoInclude();
        modelBuilder.Entity<ChampionshipTrackEntity>().Navigation(t => t.SprintQualifyingSession).AutoInclude();
        modelBuilder.Entity<ChampionshipTrackEntity>().Navigation(t => t.SprintSession).AutoInclude();
        modelBuilder.Entity<ChampionshipTrackEntity>().Navigation(t => t.RaceSession).AutoInclude();

        modelBuilder.Entity<SessionEntity>()
                    .HasMany(obj => obj.Participants)
                    .WithOne(obj => obj.Session)
                    .HasForeignKey(obj => obj.SessionId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SessionEntity>()
                    .HasMany(obj => obj.FinalClassifications)
                    .WithOne(obj => obj.Session)
                    .HasForeignKey(obj => obj.SessionId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ParticipantEntity>()
                    .HasMany(obj => obj.Laps)
                    .WithOne(obj => obj.Participant)
                    .HasForeignKey(obj => obj.ParticipantId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LapEntity>()
                    .HasMany(obj => obj.Telemetries)
                    .WithOne(obj => obj.Lap)
                    .HasForeignKey(obj => obj.LapNumberId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChampionshipEntity>()
                    .HasMany(obj => obj.Tracks)
                    .WithOne(obj => obj.Championship)
                    .HasForeignKey(obj => obj.ChampionshipId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChampionshipEntity>()
                    .HasMany(obj => obj.Points)
                    .WithOne(obj => obj.Championship)
                    .HasForeignKey(obj => obj.ChampionshipId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

        SeedTracks(modelBuilder);
        SeedDrivers(modelBuilder);
        SeedNationalities(modelBuilder);
        SeedTeams(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    #endregion // DbContext
}