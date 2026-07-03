namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface additional car telemetry (CarTelemetry2) data of all cars (F1 2026 and newer)
/// </summary>
public interface ICarTelemetry2Base
{
    #region Properties

    /// <summary>
    /// Additional car telemetry data of all cars
    /// </summary>
    ICarTelemetry2DataBase[] CarTelemetry2Data { get; }

    #endregion // Properties
}