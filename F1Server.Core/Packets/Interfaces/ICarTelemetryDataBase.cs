using F1Server.Core.Packets.Data;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for car telemetry packets across all F1 game versions
/// </summary>
public interface ICarTelemetryDataBase
{
    #region Properties

    /// <summary>
    /// Speed
    /// </summary>
    ushort Speed { get; set; }

    /// <summary>
    /// Amount of throttle applied (0.0 - 1.0)
    /// </summary>
    float Throttle { get; set; }

    /// <summary>
    /// Steering (-1.0 [full left] - 1.0 [full right])
    /// </summary>
    float Steer { get; set; }

    /// <summary>
    /// Amount of brake applied (0.0 - 1.0)
    /// </summary>
    float Brake { get; set; }

    /// <summary>
    /// Amount of clutch applied (0 - 100)
    /// </summary>
    ushort Clutch { get; set; }

    /// <summary>
    /// Current gear (1-8, N = 0, R = -1)
    /// </summary>
    short Gear { get; set; }

    /// <summary>
    /// Engine RPM
    /// </summary>
    ushort EngineRPM { get; set; }

    /// <summary>
    /// DRS active or not
    /// </summary>
    bool IsDRS { get; set; }

    /// <summary>
    /// Rev lights indicator in percent
    /// </summary>
    ushort RevLightsIndicator { get; set; }

    /// <summary>
    /// Brakes temperatures (celsius)
    /// </summary>
    Temperature BrakesTemperature { get; set; }

    /// <summary>
    /// Tyres surface temperature (celsius)
    /// </summary>
    Temperature TyresSurfaceTemperature { get; set; }

    /// <summary>
    /// Tyres inner temperature (celsius)
    /// </summary>
    Temperature TyresInnerTemperature { get; set; }

    /// <summary>
    /// Engine temperature (celsius)
    /// </summary>
    ushort EngineTemperature { get; set; }

    /// <summary>
    /// Tyres pressure
    /// </summary>
    TyresPressure TyresPressure { get; set; }

    /// <summary>
    /// Type of surface
    /// </summary>
    WheelSurface SurfaceType { get; set; }

    #endregion // Properties
}