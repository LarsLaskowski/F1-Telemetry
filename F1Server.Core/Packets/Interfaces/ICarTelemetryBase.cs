namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface car telemetry data of all cars
/// </summary>
public interface ICarTelemetryBase
{
    #region Properties

    /// <summary>
    /// Car telemetry data of all cars
    /// </summary>
    ICarTelemetryDataBase[] CarTelemetryData { get; }

    #endregion // Properties
}