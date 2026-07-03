using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Interfaces;

/// <summary>
/// Defines a contract for writing car telemetry data to a storage system
/// </summary>
public interface ITelemetryWriter
{
    #region Properties

    /// <summary>
    /// Gets a value indicating whether the telemetry writer is ready to write data
    /// </summary>
    bool IsReady { get; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Writes session runtime data to the underlying storage or system
    /// </summary>
    /// <param name="sessionRuntimeData">The session runtime data to be written</param>
    /// <param name="liveDriverData">Live driver data from human player</param>
    void WriteSessionData(ISessionRuntimeData sessionRuntimeData, ILiveDriverData liveDriverData);

    /// <summary>
    /// Writes lap data to the storage system
    /// </summary>
    /// <param name="lapData">The independent lap data containing timing information</param>
    /// <param name="lapInfo">The lap information including position, distance, and status</param>
    /// <param name="sessionRuntimeData">The runtime data of the current session</param>
    void WriteLapData(IIndependentLapData lapData, ILapDataBase lapInfo, ISessionRuntimeData sessionRuntimeData);

    /// <summary>
    /// Writes the specified car telemetry data to the storage system
    /// </summary>
    /// <param name="carTelemetryData">The car telemetry data to write</param>
    /// <param name="sessionRuntimeData">The runtime data of the current session</param>
    /// <param name="currentLapNumber">Current lap number in the session</param>
    void WriteCarTelemetry(ICarTelemetryDataBase carTelemetryData, ISessionRuntimeData sessionRuntimeData, int currentLapNumber);

    /// <summary>
    /// Writes the current status of a car to the storage system
    /// </summary>
    /// <param name="carStatusData">The car status data to write</param>
    /// <param name="sessionRuntimeData">The runtime data of the current session</param>
    /// <param name="currentLapNumber">Current lap number in the session</param>
    void WriteCarStatus(ICarStatusDataBase carStatusData, ISessionRuntimeData sessionRuntimeData, int currentLapNumber);

    #endregion // Methods
}