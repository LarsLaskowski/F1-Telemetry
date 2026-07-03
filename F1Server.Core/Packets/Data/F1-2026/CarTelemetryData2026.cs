using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation car telemetry data - F1 2026
/// </summary>
public class CarTelemetryData2026 : CarTelemetryData2025, ICarTelemetryData2026;