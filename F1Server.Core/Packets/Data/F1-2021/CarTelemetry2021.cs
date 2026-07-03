using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car telemetry data - F1 2021
/// </summary>
public class CarTelemetry2021 : ICarTelemetry2021
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarTelemetry2021()
    {
        CarTelemetryData = new CarTelemetryData2021[22];

        for (int carTelemetryData = 0; carTelemetryData < CarTelemetryData.Length; ++carTelemetryData)
        {
            var carTelemetryData2021 = new CarTelemetryData2021
                                       {
                                           BrakesTemperature = new Temperature(),
                                           TyresSurfaceTemperature = new Temperature(),
                                           TyresInnerTemperature = new Temperature(),
                                           TyresPressure = new TyresPressure(),
                                           SurfaceType = new WheelSurface()
                                       };

            CarTelemetryData[carTelemetryData] = carTelemetryData2021;
        }
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Index of MFD panel open - 255 = closed
    /// </summary>
    public ushort MfdPanelIndex { get; set; }

    /// <summary>
    /// Index of MDF panel open (second player)
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