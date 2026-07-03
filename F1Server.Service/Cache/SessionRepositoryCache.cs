using System.Collections.Concurrent;

using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

namespace F1Server.Service.Cache;

/// <summary>
/// Caches SessionEntity objects for fast lookup by session unique id or Id
/// </summary>
public static class SessionRepositoryCache
{
    #region Fields

    private static readonly ConcurrentDictionary<ulong, SessionEntity> _bySessionId = new();
    private static readonly ConcurrentDictionary<long, SessionEntity> _byId = new();
    private static readonly ConcurrentDictionary<long, SessionAttributesEntity> _byAttributesId = new();
    private static readonly Lock _lock = new();

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Gets a session by unique session id from cache, or null if not found
    /// </summary>
    /// <param name="sessionUniqueId">The unique session id to look up</param>
    /// <returns>The SessionEntity or null</returns>
    public static SessionEntity? GetByUniqueSessionId(ulong sessionUniqueId)
    {
        _bySessionId.TryGetValue(sessionUniqueId, out var session);

        session ??= CheckSessionIsInDatabaseBySessionUniqueId(sessionUniqueId);

        return session;
    }

    /// <summary>
    /// Gets a session by Id from cache, or null if not found
    /// </summary>
    /// <param name="id">The database Id to look up</param>
    /// <returns>The SessionEntity or null</returns>
    public static SessionEntity? GetById(long id)
    {
        _byId.TryGetValue(id, out var session);

        session ??= CheckSessionIsInDatabaseBySessionId(id);

        return session;
    }

    /// <summary>
    /// Retrieves the session attributes associated with the specified session ID
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which attributes are to be retrieved</param>
    /// <returns>
    /// A <see cref="SessionAttributesEntity"/> object containing the attributes of the session if found; otherwise,
    /// <see langword="null"/>
    /// </returns>
    public static SessionAttributesEntity? GetAttributesBySessionId(long sessionId)
    {
        _byAttributesId.TryGetValue(sessionId, out var attributes);

        attributes ??= CheckSessionAttributesIsInDatabase(sessionId);

        return attributes;
    }

    /// <summary>
    /// Adds or updates a session in the cache
    /// </summary>
    /// <param name="session">The SessionEntity to add or update</param>
    public static void AddOrUpdate(SessionEntity session)
    {
        _bySessionId[session.SessionId] = session;
        _byId[session.Id] = session;
    }

    /// <summary>
    /// Adds or updates session attributes in the internal collection
    /// </summary>
    /// <param name="attributes">
    /// The session attributes to add or update. The <see cref="SessionAttributesEntity.SessionId"/> property  must be
    /// set to a valid session identifier
    /// </param>
    public static void AddOrUpdateAttributes(SessionAttributesEntity? attributes)
    {
        if (attributes != null)
        {
            _byAttributesId[attributes.SessionId] = attributes;
        }
    }

    /// <summary>
    /// Clears the cache (e.g. after DB changes)
    /// </summary>
    public static void Clear()
    {
        lock (_lock)
        {
            _bySessionId.Clear();
            _byId.Clear();
            _byAttributesId.Clear();
        }
    }

    /// <summary>
    /// Checks whether a session with the specified unique identifier exists in the database
    /// </summary>
    /// <param name="sessionUniqueId">The unique identifier of the session to check</param>
    /// <returns>
    /// A <see cref="SessionEntity"/> representing the session if it exists in the database; otherwise, <see
    /// langword="null"/>
    /// </returns>
    private static SessionEntity? CheckSessionIsInDatabaseBySessionUniqueId(ulong sessionUniqueId)
    {
        lock (_lock)
        {
            using var dbFactory = RepositoryFactory.CreateInstance();

            var sessionFromDb = dbFactory.GetRepository<SessionRepository>()
                                         ?.GetQuery()
                                         ?.FirstOrDefault(s => s.SessionId == sessionUniqueId);

            if (sessionFromDb != null)
            {
                AddOrUpdate(sessionFromDb);
            }

            return sessionFromDb;
        }
    }

    /// <summary>
    /// Checks whether a session with the specified unique identifier exists in the database
    /// </summary>
    /// <param name="sessionId">The database id of the session to check</param>
    /// <returns>
    /// A <see cref="SessionEntity"/> representing the session if it exists in the database; otherwise, <see
    /// langword="null"/>
    /// </returns>
    private static SessionEntity? CheckSessionIsInDatabaseBySessionId(long sessionId)
    {
        lock (_lock)
        {
            using var dbFactory = RepositoryFactory.CreateInstance();

            var sessionFromDb = dbFactory.GetRepository<SessionRepository>()
                                         ?.GetQuery()
                                         ?.FirstOrDefault(s => s.Id == sessionId);

            if (sessionFromDb != null)
            {
                AddOrUpdate(sessionFromDb);
            }

            return sessionFromDb;
        }
    }

    /// <summary>
    /// Checks whether a session with the specified unique identifier exists in the database
    /// </summary>
    /// <param name="sessionId">The session database identifier of the session to check</param>
    /// <returns>
    /// A <see cref="SessionEntity"/> representing the session if it exists in the database; otherwise, <see
    /// langword="null"/>
    /// </returns>
    private static SessionAttributesEntity? CheckSessionAttributesIsInDatabase(long sessionId)
    {
        lock (_lock)
        {
            using var dbFactory = RepositoryFactory.CreateInstance();

            var attributesDb = dbFactory.GetRepository<SessionAttributesRepository>()
                                        ?.GetQuery()
                                        ?.FirstOrDefault(s => s.SessionId == sessionId);

            if (attributesDb != null)
            {
                AddOrUpdateAttributes(attributesDb);
            }

            return attributesDb;
        }
    }

    #endregion // Methods
}