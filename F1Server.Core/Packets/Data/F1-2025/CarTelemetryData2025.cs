using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation car telemetry data - F1 2025
/// </summary>
public class CarTelemetryData2025 : ICarTelemetryData2025
{
    #region ICarTelemetryData2025

    /// <inheritdoc/>
    public ushort Speed { get; set; }

    /// <inheritdoc/>
    public float Throttle { get; set; }

    /// <inheritdoc/>
    public float Steer { get; set; }

    /// <inheritdoc/>
    public float Brake { get; set; }

    /// <inheritdoc/>
    public ushort Clutch { get; set; }

    /// <inheritdoc/>
    public short Gear { get; set; }

    /// <inheritdoc/>
    public ushort EngineRPM { get; set; }

    /// <inheritdoc/>
    public bool IsDRS { get; set; }

    /// <inheritdoc/>
    public ushort RevLightsIndicator { get; set; }

    /// <inheritdoc/>
    public Temperature BrakesTemperature { get; set; }

    /// <inheritdoc/>
    public Temperature TyresSurfaceTemperature { get; set; }

    /// <inheritdoc/>
    public Temperature TyresInnerTemperature { get; set; }

    /// <inheritdoc/>
    public ushort EngineTemperature { get; set; }

    /// <inheritdoc/>
    public TyresPressure TyresPressure { get; set; }

    /// <inheritdoc/>
    public WheelSurface SurfaceType { get; set; }

    /// <inheritdoc/>
    public ushort RevLightsBitValue { get; set; }

    #endregion // ICarTelemetryData2025
}