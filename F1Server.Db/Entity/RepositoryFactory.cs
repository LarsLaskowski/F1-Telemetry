using F1Server.Db.Entity.Repositories.Base;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace F1Server.Db.Entity;

/// <summary>
/// Factory for creating repositories
/// </summary>
public sealed class RepositoryFactory : IDisposable
{
    #region Fields

    /// <summary>
    /// Repositories
    /// </summary>
    private readonly Dictionary<Type, RepositoryBase> _repositories;

    /// <summary>
    /// DbContext
    /// </summary>
    private F1ServerDbContext? _dbContext;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public RepositoryFactory()
    {
        _dbContext = new F1ServerDbContext(ServiceProvider);
        _repositories = new Dictionary<Type, RepositoryBase>();
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Is initialized?
    /// </summary>
    public static bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the application's service provider, which is used to resolve dependencies and access registered services
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// Last error
    /// </summary>
    public string LastError => _dbContext?.LastError ?? string.Empty;

    #endregion // Properties

    #region Static methods

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <returns>Repository factory</returns>
    public static RepositoryFactory CreateInstance()
    {
        return new RepositoryFactory();
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <returns>Repository factory</returns>
    public static RepositoryFactory InitDatabase(IServiceProvider serviceProvider)
    {
        ServiceProvider ??= serviceProvider;

        return CreateInstance();
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Creates a new repository object or returns the already existing object
    /// </summary>
    /// <typeparam name="TRepository">Type of the repository to be created</typeparam>
    /// <returns>Repository</returns>
    public TRepository? GetRepository<TRepository>()
        where TRepository : RepositoryBase
    {
        if (_repositories.TryGetValue(typeof(TRepository), out var repository) == false && _dbContext != null
            && Activator.CreateInstance(typeof(TRepository), _dbContext) is TRepository repo)
        {
            repository = _repositories[typeof(TRepository)] = repo;
        }

        return (TRepository?)repository;
    }

    /// <summary>
    /// Initialize the database
    /// </summary>
    public void InitDatabase()
    {
        if (_dbContext != null && IsInitialized == false)
        {
            int? cmdTimeout = null;

            try
            {
                if (_dbContext.Database.IsRelational())
                {
                    cmdTimeout = _dbContext.Database.GetCommandTimeout();

                    _dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

                    // Execute migration
                    _dbContext.Database.Migrate();

                    _dbContext.Database.GenerateCreateScript();
                }

                IsInitialized = true;
            }
            catch (Exception ex)
            {
                _dbContext.LastError = ex.ToString();
            }
            finally
            {
                if (cmdTimeout != null)
                {
                    _dbContext.Database.SetCommandTimeout(cmdTimeout);
                }
            }
        }
    }

    /// <summary>
    /// Begins a new transaction. This transaction is valid for all created repositories
    /// </summary>
    /// <returns>Transaction object</returns>
    public IDbContextTransaction? BeginTransaction()
    {
        return _dbContext?.Database.BeginTransaction();
    }

    #endregion // Methods

    #region IDisposable

    /// <summary>
    /// Dipose
    /// </summary>
    public void Dispose()
    {
        _dbContext?.Dispose();

        _dbContext = null;
    }

    #endregion // IDisposable
}