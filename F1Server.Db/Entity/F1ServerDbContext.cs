using F1Server.Core.Exceptions;
using F1Server.Core.Interfaces;
using F1Server.Data;
using F1Server.Db.Data;
using F1Server.Db.Entity.Seed;
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
                AppMetrics?.DbErrorCount.Add(1);
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

        var traceDbCommands = Environment.GetEnvironmentVariable("F1SERVER_TRACE_DB_COMMANDS");

        if (bool.TryParse(traceDbCommands, out var isTraceDbCommandsEnabled) && isTraceDbCommandsEnabled)
        {
            optionsBuilder.AddInterceptors(_commandInterceptor);
        }
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
        appMetrics?.DbErrorCount.Add(1, new KeyValuePair<string, object?>("ExceptionType", exception.GetType().Name));
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

        modelBuilder.ApplyConfiguration(new TrackSeedConfiguration());
        modelBuilder.ApplyConfiguration(new DriverSeedConfiguration());
        modelBuilder.ApplyConfiguration(new NationalitySeedConfiguration());
        modelBuilder.ApplyConfiguration(new TeamSeedConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    #endregion // DbContext
}