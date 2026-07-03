using F1Server.Db.Entity.Queryable.Base;
using F1Server.Db.Entity.Tables;

namespace F1Server.Db.Entity.Queryable;

/// <summary>
/// Queryable for accessing car telemetry entity
/// </summary>
public class CarTelemetryQueryable : QueryableBase<CarTelemetryEntity>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryable">Queryable object</param>
    public CarTelemetryQueryable(IQueryable<CarTelemetryEntity> queryable)
        : base(queryable)
    {
    }

    #endregion // Constructors
}