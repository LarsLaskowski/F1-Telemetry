using F1Server.Core.Interfaces;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Tests.Processors;

/// <summary>
/// Telemetry writer counting the written data to test the telemetry publishing of the processors
/// </summary>
internal sealed class RecordingTelemetryWriter : ITelemetryWriter
{
    #region Properties

    /// <summary>
    /// Number of written session data entries
    /// </summary>
    public int SessionDataCount { get; private set; }

    /// <summary>
    /// Number of written lap data entries
    /// </summary>
    public int LapDataCount { get; private set; }

    /// <summary>
    /// Number of written car telemetry entries
    /// </summary>
    public int CarTelemetryCount { get; private set; }

    /// <summary>
    /// Number of written car status entries
    /// </summary>
    public int CarStatusCount { get; private set; }

    #endregion // Properties

    #region ITelemetryWriter

    /// <inheritdoc/>
    public bool IsReady => true;

    /// <inheritdoc/>
    public void WriteSessionData(ISessionRuntimeData sessionRuntimeData, ILiveDriverData liveDriverData)
    {
        SessionDataCount++;
    }

    /// <inheritdoc/>
    public void WriteLapData(IIndependentLapData lapData, ILapDataBase lapInfo, ISessionRuntimeData sessionRuntimeData)
    {
        LapDataCount++;
    }

    /// <inheritdoc/>
    public void WriteCarTelemetry(ICarTelemetryDataBase carTelemetryData, ISessionRuntimeData sessionRuntimeData, int currentLapNumber)
    {
        CarTelemetryCount++;
    }

    /// <inheritdoc/>
    public void WriteCarStatus(ICarStatusDataBase carStatusData, ISessionRuntimeData sessionRuntimeData, int currentLapNumber)
    {
        CarStatusCount++;
    }

    #endregion // ITelemetryWriter
}