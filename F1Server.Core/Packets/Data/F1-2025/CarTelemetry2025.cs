using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car telemetry data - F1 2025
/// </summary>
public class CarTelemetry2025 : ICarTelemetry2025
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarTelemetry2025()
    {
        CarTelemetryData = new CarTelemetryData2025[22];

        for (int carTelemetry = 0; carTelemetry < CarTelemetryData.Length; ++carTelemetry)
        {
            var carTelemetryData = new CarTelemetryData2025
                                   {
                                       BrakesTemperature = new Temperature(),
                                       TyresSurfaceTemperature = new Temperature(),
                                       TyresInnerTemperature = new Temperature(),
                                       TyresPressure = new TyresPressure(),
                                       SurfaceType = new WheelSurface()
                                   };

            CarTelemetryData[carTelemetry] = carTelemetryData;
        }
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Index of MFD panel open - 255 = closed
    /// </summary>
    public ushort MfdPanelIndex { get; set; }

    /// <summary>
    /// Index of MFD panel open (second player)
    /// </summary>
    public ushort MfdPanelIndexSecondary { get; set; }

    /// <summary>
    /// Suggested gear
    /// </summary>
    public ushort SuggestedGear { get; set; }

    #endregion // Properties

    #region ICarTelemetryBase

    /// <inheritdoc/>
    public ICarTelemetryDataBase[] CarTelemetryData { get; }

    #endregion // ICarTelemetryBase
}