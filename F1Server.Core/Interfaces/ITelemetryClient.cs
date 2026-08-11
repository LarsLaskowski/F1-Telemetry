namespace F1Server.Core.Interfaces;

/// <summary>
/// Marker interface for a telemetry client. It declares no members on purpose and only serves
/// to identify and register telemetry client implementations (such as the UDP listener in
/// F1Server.Telemetry) with the dependency injection container. The collection and transmission
/// behaviour is defined by the implementing type, not by this contract.
/// </summary>
public interface ITelemetryClient;