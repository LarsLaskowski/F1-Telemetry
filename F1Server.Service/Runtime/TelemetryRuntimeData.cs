using System.Collections.Concurrent;

using F1Server.Db.Entity.Tables;

namespace F1Server.Service.Runtime;

/// <summary>
/// Telemetry runtime data
/// </summary>
internal class TelemetryRuntimeData
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public TelemetryRuntimeData()
    {
        TelemetryQueue = new ConcurrentQueue<CarTelemetryEntity>();
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Lap is completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Queue with all telemetry data points in one lap
    /// </summary>
    public ConcurrentQueue<CarTelemetryEntity> TelemetryQueue { get; }

    #endregion // Properties
}