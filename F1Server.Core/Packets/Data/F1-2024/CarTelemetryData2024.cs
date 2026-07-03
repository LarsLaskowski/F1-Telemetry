using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation car telemetry data - F1 2024
/// </summary>
public class CarTelemetryData2024 : ICarTelemetryData2024
{
    #region ICarTelemetryBase

    /// <summary>
    /// Speed
    /// </summary>
    public ushort Speed { get; set; }

    /// <summary>
    /// Amount of throttle applied (0.0 - 1.0)
    /// </summary>
    public float Throttle { get; set; }

    /// <summary>
    /// Steering (-1.0 [full left] - 1.0 [full right])
    /// </summary>
    public float Steer { get; set; }

    /// <summary>
    /// Amount of brake applied (0.0 - 1.0)
    /// </summary>
    public float Brake { get; set; }

    /// <summary>
    /// Amount of clutch applied (0 - 100)
    /// </summary>
    public ushort Clutch { get; set; }

    /// <summary>
    /// Current gear (1-8, N = 0, R = -1)
    /// </summary>
    public short Gear { get; set; }

    /// <summary>
    /// Engine RPM
    /// </summary>
    public ushort EngineRPM { get; set; }

    /// <summary>
    /// DRS active or not
    /// </summary>
    public bool IsDRS { get; set; }

    /// <summary>
    /// Rev ligthts indicator in percent
    /// </summary>
    public ushort RevLightsIndicator { get; set; }

    /// <summary>
    /// Brakes temperatures (celsius)
    /// </summary>
    public Temperature BrakesTemperature { get; set; }

    /// <summary>
    /// Tyres surface temperature (celsius)
    /// </summary>
    public Temperature TyresSurfaceTemperature { get; set; }

    /// <summary>
    /// Tyres inner temperature (celsius)
    /// </summary>
    public Temperature TyresInnerTemperature { get; set; }

    /// <summary>
    /// Engine temperature (celsius)
    /// </summary>
    public ushort EngineTemperature { get; set; }

    /// <summary>
    /// Tyres pressure
    /// </summary>
    public TyresPressure TyresPressure { get; set; }

    /// <summary>
    /// Type of surface
    /// </summary>
    public WheelSurface SurfaceType { get; set; }

    #endregion // ICarTelemetryBase

    #region ICarTelemetry2024

    /// <summary>
    /// Rev lights bit
    /// </summary>
    public ushort RevLightsBitValue { get; set; }

    #endregion // ICarTelemetry2024
}