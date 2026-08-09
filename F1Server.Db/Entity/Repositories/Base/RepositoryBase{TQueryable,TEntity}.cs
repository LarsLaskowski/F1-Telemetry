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
    /// Compiled factory delegate constructing a <typeparamref name="TQueryable"/>, built once per closed
    /// generic type instead of resolving the constructor via reflection on every <see cref="GetQuery"/> call
    /// </summary>
    private static readonly Func<IQueryable<TEntity>, TQueryable> _queryableFactory = CreateQueryableFactory();

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
    public ILogger? Logger => _dbContext.Logger;

    /// <summary>
    /// Error message from the most recently executed operation, or <see langword="null"/>/empty when none occurred.
    /// The boolean/int result of the write and read methods below does not by itself distinguish a caught
    /// exception from a successful no-op (e.g. no matching entity found); check this property to tell them apart.
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
    /// <param name="ignoreAutoIncludes">Ignore auto-included navigations configured on the model?</param>
    /// <returns>IQueryable object</returns>
    public TQueryable? GetQuery(bool ignoreAutoIncludes = false)
    {
        var queryable = _dbContext.Set<TEntity>().AsNoTracking();

        if (ignoreAutoIncludes)
        {
            queryable = queryable.IgnoreAutoIncludes();
        }

        return _queryableFactory(queryable);
    }

    /// <summary>
    /// Add a new entity object
    /// </summary>
    /// <param name="entity">Entity object</param>
    /// <returns><see langword="true"/> if the entity was added; <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
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
    /// Add a new entity object without blocking the calling thread while the changes are saved
    /// </summary>
    /// <param name="entity">Entity object</param>
    /// <returns><see langword="true"/> if the entity was added; <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public async Task<bool> AddAsync(TEntity entity)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            _dbContext.Set<TEntity>()
                      .Add(entity);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

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
    /// <returns><see langword="true"/> if the entities were added; <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
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
    /// Inserts a batch of new entities without automatic change detection, so large batches avoid the per-entity tracking overhead
    /// </summary>
    /// <param name="entities">Entities to insert</param>
    /// <returns><see langword="true"/> if the entities were inserted; <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public async Task<bool> InsertBatchAsync(IEnumerable<TEntity> entities)
    {
        var success = false;

        LastError = string.Empty;

        var autoDetectChangesEnabled = _dbContext.ChangeTracker.AutoDetectChangesEnabled;

        try
        {
            _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            _dbContext.Set<TEntity>()
                      .AddRange(entities);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error inserting batch of entities!");

            LastError = ex.ToString();
        }
        finally
        {
            _dbContext.ChangeTracker.AutoDetectChangesEnabled = autoDetectChangesEnabled;
        }

        return success;
    }

    /// <summary>
    /// Adds or refresh an entity
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="refreshAction">Action to refresh</param>
    /// <param name="after">Action after refresh</param>
    /// <returns><see langword="true"/> if the entity was added or refreshed; <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public bool AddOrRefresh(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction, Action<TEntity>? after = null)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var newEntity = false;

            var entity = _dbContext.Set<TEntity>().FirstOrDefault(expression);

            if (entity is null)
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
    /// <returns><see langword="true"/> if the entities were updated; <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public bool UpdateRange(IEnumerable<TEntity> entities)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            _dbContext.UpdateRange(entities);

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
    /// Adds or refresh a range of entities
    /// </summary>
    /// <param name="entities">Entities</param>
    /// <returns><see langword="true"/> if the entities were updated; <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public async Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            _dbContext.UpdateRange(entities);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

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
    /// Refresh an entity object
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="refreshAction">Refresh action</param>
    /// <returns><see langword="true"/> if the operation completed without an exception, including when no matching entity was found (no-op); <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public bool Refresh(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entity = dbSet.FirstOrDefault(expression);

            if (entity is not null)
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
    /// Refresh an entity object without blocking the calling thread while the database is accessed
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="refreshAction">Refresh action</param>
    /// <returns><see langword="true"/> if the operation completed without an exception, including when no matching entity was found (no-op); <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public async Task<bool> RefreshAsync(Expression<Func<TEntity, bool>> expression, Action<TEntity> refreshAction)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entity = await dbSet.FirstOrDefaultAsync(expression).ConfigureAwait(false);

            if (entity is not null)
            {
                refreshAction(entity);

                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
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
    /// <returns><see langword="true"/> if the operation completed without an exception, including when no matching entities were found (no-op); <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
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
    /// <returns><see langword="true"/> if the operation completed without an exception, including when no matching entities were found (no-op); <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public async Task<bool> RefreshRangeAsync(Expression<Func<TEntity, bool>> expression, Func<TEntity, Task> refreshAction)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            await foreach (var entry in dbSet.Where(expression).AsAsyncEnumerable().ConfigureAwait(false))
            {
                await refreshAction(entry).ConfigureAwait(false);
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

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
    /// Remove an entity object
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="beforeRemove">Action before removing</param>
    /// <returns><see langword="true"/> if the operation completed without an exception, including when no matching entity was found (no-op); <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public bool Remove(Expression<Func<TEntity, bool>> expression, Action<TEntity>? beforeRemove = null)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entity = dbSet.FirstOrDefault(expression);

            if (entity is not null)
            {
                beforeRemove?.Invoke(entity);

                dbSet.Remove(entity);

                _dbContext.SaveChanges();
            }

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error removing an entity object!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Remove an entity object without blocking the calling thread while the database is accessed
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="beforeRemove">Action before removing</param>
    /// <returns><see langword="true"/> if the operation completed without an exception, including when no matching entity was found (no-op); <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public async Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> expression, Action<TEntity>? beforeRemove = null)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            var entity = await dbSet.FirstOrDefaultAsync(expression).ConfigureAwait(false);

            if (entity is not null)
            {
                beforeRemove?.Invoke(entity);

                dbSet.Remove(entity);

                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }

            success = true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error removing an entity object!");

            LastError = ex.ToString();
        }

        return success;
    }

    /// <summary>
    /// Remove a range of entity object
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns><see langword="true"/> if the operation completed without an exception, including when no matching entities were found (no-op); <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public bool RemoveRange(Expression<Func<TEntity, bool>> expression)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            // Auto-included navigations are not needed for deleting and would drop rows whose principals are missing
            var query = _dbContext.Set<TEntity>()
                                  .IgnoreAutoIncludes()
                                  .Where(expression);

            if (_dbContext.Database.IsRelational())
            {
                query.ExecuteDelete();
            }
            else
            {
                // Set-based deletes are not supported by non-relational providers (e.g. InMemory), so fall back to load-and-remove
                var dbSet = _dbContext.Set<TEntity>();

                dbSet.RemoveRange(query.ToList());

                _dbContext.SaveChanges();
            }

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
    /// <returns><see langword="true"/> if the operation completed without an exception, including when no matching entities were found (no-op); <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
    public async Task<bool> RemoveRangeAsync(Expression<Func<TEntity, bool>> expression)
    {
        var success = false;

        LastError = string.Empty;

        try
        {
            var dbSet = _dbContext.Set<TEntity>();

            await foreach (var entry in dbSet.Where(expression).AsAsyncEnumerable().ConfigureAwait(false))
            {
                dbSet.Remove(entry);
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

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
    /// Remove all entity objects matching the expression with a single set-based statement
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <returns>Number of removed entities, or -1 if an exception occurred (see <see cref="LastError"/>)</returns>
    public int RemoveWhere(Expression<Func<TEntity, bool>> expression)
    {
        var removedCount = -1;

        LastError = string.Empty;

        try
        {
            // Auto-included navigations are not needed for deleting and would drop rows whose principals are missing
            var query = _dbContext.Set<TEntity>()
                                  .IgnoreAutoIncludes()
                                  .Where(expression);

            if (_dbContext.Database.IsRelational())
            {
                removedCount = query.ExecuteDelete();
            }
            else
            {
                // Set-based deletes are not supported by non-relational providers (e.g. InMemory), so fall back to load-and-remove
                var entities = query.ToList();

                _dbContext.Set<TEntity>()
                          .RemoveRange(entities);

                _dbContext.SaveChanges();

                removedCount = entities.Count;
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error removing entity objects with a set-based delete!");

            LastError = ex.ToString();
        }

        return removedCount;
    }

    /// <summary>
    /// Execute a raw SQL statement against the database. <paramref name="sqlStatement"/> must be a fixed
    /// statement using positional placeholders (e.g. <c>@p0</c>); <paramref name="parameters"/> are passed
    /// through parameterized to EF Core's <c>ExecuteSqlRaw</c>, so callers must never interpolate
    /// untrusted values directly into <paramref name="sqlStatement"/>
    /// </summary>
    /// <param name="sqlStatement">Fixed SQL statement with positional parameter placeholders</param>
    /// <param name="parameters">Parameters substituted into the placeholders</param>
    /// <returns><see langword="true"/> if the statement executed successfully; <see langword="false"/> if an exception occurred (see <see cref="LastError"/>)</returns>
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

    /// <summary>
    /// Builds and caches a compiled factory delegate for <typeparamref name="TQueryable"/>
    /// </summary>
    /// <returns>Delegate constructing a <typeparamref name="TQueryable"/> from an <see cref="IQueryable{TEntity}"/></returns>
    private static Func<IQueryable<TEntity>, TQueryable> CreateQueryableFactory()
    {
        var constructor = typeof(TQueryable).GetConstructor([typeof(IQueryable<TEntity>)])
                              ?? throw new InvalidOperationException($"{typeof(TQueryable)} has no public constructor accepting IQueryable<{typeof(TEntity)}>.");

        var parameter = Expression.Parameter(typeof(IQueryable<TEntity>), "queryable");

        return Expression.Lambda<Func<IQueryable<TEntity>, TQueryable>>(Expression.New(constructor, parameter), parameter)
                         .Compile();
    }

    #endregion // Methods
}