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

    /// <summary>
    /// Car telemetry of all cars
    /// </summary>
    public ICarTelemetryDataBase[] CarTelemetryData { get; }

    #endregion // ICarTelemetryBase
}