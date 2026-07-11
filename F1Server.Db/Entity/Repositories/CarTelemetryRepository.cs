using F1Server.Db.Entity.Queryable;
using F1Server.Db.Entity.Repositories.Base;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Db.Entity.Repositories;

/// <summary>
/// Repository for accessing <see cref="Tables.CarTelemetryEntity"/>
/// </summary>
public class CarTelemetryRepository : RepositoryBase<CarTelemetryQueryable, CarTelemetryEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext">DbContext</param>
    public CarTelemetryRepository(F1ServerDbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Removes all car telemetry rows belonging to laps of a session with a single set-based statement
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <param name="invalidLapTimesOnly">Restrict the delete to laps with an invalid lap time?</param>
    /// <returns>Status</returns>
    public bool RemoveBySessionId(long sessionId, bool invalidLapTimesOnly = false)
    {
        if (GetDbContext().Database.IsRelational())
        {
            var sqlStatement = invalidLapTimesOnly
                                   ? "DELETE FROM CarTelemetries WHERE LapNumberId IN (SELECT Id FROM Laps WHERE SessionId = @p0 AND IsInvalidLapTime = 1)"
                                   : "DELETE FROM CarTelemetries WHERE LapNumberId IN (SELECT Id FROM Laps WHERE SessionId = @p0)";

            return ExecuteRawSql(sqlStatement, sessionId);
        }

        // Non-relational providers (e.g. InMemory) do not support raw SQL, so delete through the guarded set-based helper
        var removedCount = invalidLapTimesOnly
                               ? RemoveWhere(t => t.Lap.SessionId == sessionId && t.Lap.DbIsInvalidLapTime == 1)
                               : RemoveWhere(t => t.Lap.SessionId == sessionId);

        return removedCount >= 0;
    }

    #endregion // Methods
}