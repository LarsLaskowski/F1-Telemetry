using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car telemetry data - F1 2024
/// </summary>
public class CarTelemetry2024 : ICarTelemetry2024
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarTelemetry2024()
    {
        CarTelemetryData = new CarTelemetryData2024[22];

        for (int carTelemtry = 0; carTelemtry < CarTelemetryData.Length; ++carTelemtry)
        {
            var carTelemetryData = new CarTelemetryData2024
                                   {
                                       BrakesTemperature = new Temperature(),
                                       TyresSurfaceTemperature = new Temperature(),
                                       TyresInnerTemperature = new Temperature(),
                                       TyresPressure = new TyresPressure(),
                                       SurfaceType = new WheelSurface()
                                   };

            CarTelemetryData[carTelemtry] = carTelemetryData;
        }
    }

    #endregion // Constructors

    #region ICarTelemetry2024

    /// <inheritdoc/>
    public ushort MfdPanelIndex { get; set; }

    /// <inheritdoc/>
    public ushort MfdPanelIndexSecondary { get; set; }

    /// <inheritdoc/>
    public ushort SuggestedGear { get; set; }

    #endregion // ICarTelemetry2024

    #region ICarTelemetryBase

    /// <inheritdoc/>
    public ICarTelemetryDataBase[] CarTelemetryData { get; }

    #endregion // ICarTelemetryBase
}