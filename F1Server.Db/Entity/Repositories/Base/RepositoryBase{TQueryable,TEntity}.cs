using System.Linq.Expressions;

using F1Server.Db.Entity.Queryable.Base;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace F1Server.Db.Entity.Repositories.Base;

/// <summary>
/// Base class for creating a repository
/// </summary>
/// <typeparam name="TQueryable">Type of IQueryable</typeparam>
/// <typeparam name="TEntity">Type of Entity</typeparam>
public abstract class RepositoryBase<TQueryable, TEntity> : RepositoryBase
    where TQueryable : QueryableBase<TEntity>
    where TEntity : class
{
    #region Fields

    /// <summary>
    /// Database context
    /// </summary>
    private readonly F1ServerDbContext _dbContext;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext">Database context</param>
    protected RepositoryBase(F1ServerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Gets the logger instance used for logging messages and events
    /// </summary>
    public ILogger? Logger => _dbContext?.Logger;

    /// <summary>
    /// Last error
    /// </summary>
    public string? LastError
    {
        get => _dbContext.LastError;
        protected set => _dbContext.LastError = value;
    }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Get query
    /// </summary>
    /// <returns>IQueryable object</returns>
    public TQueryable? GetQuery()
    {
        return _dbContext != null ? Activator.CreateInstance(typeof(TQueryable), _dbContext.Set<TEntity>().AsNoTracking()) as TQueryable : null;
    }

    /// <summary>
    /// Add a new entity object
    /// </summary>
    /// <param name="entity">Entity object</param>
    /// <returns>Status</returns>
    public bool Add(TEntity entity)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            _dbContext.Set<TEntity>()
                      .Add(entity);

            _dbContext.SaveChanges();

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error adding entity object!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Add a range of entities
    /// </summary>
    /// <param name="entities">Entities</param>
    /// <returns>Status</returns>
    public bool AddRange(IEnumerable<TEntity> entities)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            _dbContext.Set<TEntity>()
                      .AddRange(entities);

            _dbContext.SaveChanges();

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error updating range of entities!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Adds or refresh an entity
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="refreshAction">Action to refresh</param>
    /// <param name="after">Action after refresh</param>
    /// <returns>Status</returns>
    public bool AddOrRefresh(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction, Action<TEntity>? after = null)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var newEntity = false;

            var entity = _dbContext.Set<TEntity>().FirstOrDefault(expression);

            if (entity == null)
            {
                entity = Activator.CreateInstance<TEntity>();

                newEntity = true;
            }

            refreshAction(entity);

            if (newEntity)
            {
                _dbContext.Set<TEntity>().Add(entity);
            }

            _dbContext.SaveChanges();

            after?.Invoke(entity);

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error adding or refreshing entity object!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Adds or refresh a range of entities
    /// </summary>
    /// <param name="entities">Entities</param>
    /// <returns>Status</returns>
    public bool UpdateRange(IEnumerable<TEntity> entities)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            _dbContext.UpdateRange(entities);

            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error updating range of entities!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Adds or refresh a range of entities
    /// </summary>
    /// <param name="entities">Entities</param>
    /// <returns>Status</returns>
    public async Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            _dbContext.UpdateRange(entities);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error updating range of entities!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Refresh an entity object
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="refreshAction">Refresh action</param>
    /// <returns>Status</returns>
    public bool Refresh(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entity = dbSet.FirstOrDefault(expression);

            if (entity != null)
            {
                refreshAction(entity);

                _dbContext.SaveChanges();
            }

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error refreshing entity object!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Refresh a range of entity objects
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="refreshAction">Refresh action</param>
    /// <returns>Status</returns>
    public bool RefreshRange(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            foreach (var entry in dbSet.Where(expression))
            {
                refreshAction(entry);
            }

            _dbContext.SaveChanges();

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error refreshing range of entity objects!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Refresh a range of entity objects asynchron
    /// </summary>
    /// <param name="expression">expression</param>
    /// <param name="refreshAction">Refresh action</param>
    /// <returns>Status</returns>
    public async Task<bool> RefreshRangeAsync(Expression<Func<TEntity, bool>> expression, Func<TEntity, Task> refreshAction)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            await foreach (var entry in dbSet.Where(expression).AsAsyncEnumerable().ConfigureAwait(true))
            {
                await refreshAction(entry).ConfigureAwait(true);
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(true);

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error removing a entity object!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Remove a entity object
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="beforeRemove">Action before removing</param>
    /// <returns>Status</returns>
    public bool Remove(Expression<Func<TEntity, bool>> expression, Action<TEntity>? beforeRemove = null)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entity = dbSet.FirstOrDefault(expression);

            if (entity != null)
            {
                beforeRemove?.Invoke(entity);

                dbSet.Remove(entity);

                _dbContext.SaveChanges();
            }

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error removing a entity object!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Remove a range of entity object
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>Status</returns>
    public bool RemoveRange(Expression<Func<TEntity, bool>> expression)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            foreach (var entry in dbSet.Where(expression).ToList())
            {
                dbSet.Remove(entry);
            }

            _dbContext.SaveChanges();

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error removing a range of entity objects!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Remove a range of entity object
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>Status</returns>
    public async Task<bool> RemoveRangeAsync(Expression<Func<TEntity, bool>> expression)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            await foreach (var entry in dbSet.Where(expression).AsAsyncEnumerable().ConfigureAwait(true))
            {
                dbSet.Remove(entry);
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(true);

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error removing a range of entity objects!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Execute a raw SQL statement against the database
    /// </summary>
    /// <param name="sqlStatement">SQL statement</param>
    /// <param name="parameters">Parameters</param>
    /// <returns>Status</returns>
    public bool ExecuteRawSql(string sqlStatement, params object[] parameters)
    {
        var success = false;
        LastError = string.Empty;

        try
        {
            _dbContext.Database.ExecuteSqlRaw(sqlStatement, parameters);

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing raw SQL - statement: {SqlStatement}!", sqlStatement);

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Returns the internal DbContext
    /// </summary>
    /// <returns>DbContext</returns>
    protected F1ServerDbContext GetDbContext()
    {
        return _dbContext;
    }

    #endregion // Methods
}