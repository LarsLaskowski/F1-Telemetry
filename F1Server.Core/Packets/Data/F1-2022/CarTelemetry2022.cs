using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car telemetry data - F1 2022
/// </summary>
public class CarTelemetry2022 : ICarTelemetry2022
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarTelemetry2022()
    {
        CarTelemetryData = new CarTelemetryData2022[22];

        for (int carTelemetry = 0; carTelemetry < CarTelemetryData.Length; ++carTelemetry)
        {
            var carTelemetryData = new CarTelemetryData2022
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

    /// <summary>
    /// Car telemetry of all cars
    /// </summary>
    public ICarTelemetryDataBase[] CarTelemetryData { get; }

    #endregion // ICarTelemetryBase
}