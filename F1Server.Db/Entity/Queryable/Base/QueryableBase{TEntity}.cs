using System.Collections;
using System.Linq.Expressions;

namespace F1Server.Db.Entity.Queryable.Base;

/// <summary>
/// Base class for creating queryable objects
/// </summary>
/// <typeparam name="TEntity">Type of entity</typeparam>
public class QueryableBase<TEntity> : IQueryable<TEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public QueryableBase(IQueryable<TEntity> queryable)
    {
        QueryableInternal = queryable;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Internal queryable
    /// </summary>
    protected IQueryable<TEntity> QueryableInternal { get; private set; }

    #endregion // Properties

    #region IQeryable

    #region Properties

    /// <summary>
    /// Element type
    /// </summary>
    public Type ElementType => QueryableInternal.ElementType;

    /// <summary>
    /// Expression
    /// </summary>
    public Expression Expression => QueryableInternal.Expression;

    /// <summary>
    /// Provider
    /// </summary>
    public IQueryProvider Provider => QueryableInternal.Provider;

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Returns an enumerator
    /// </summary>
    /// <returns>Enumerator</returns>
    public IEnumerator<TEntity> GetEnumerator()
    {
        return QueryableInternal.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator
    /// </summary>
    /// <returns>Enumerator</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)QueryableInternal).GetEnumerator();
    }

    #endregion // Methods

    #endregion // IQeryable
}