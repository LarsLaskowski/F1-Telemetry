using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car telemetry data - F1 2019
/// </summary>
public class CarTelemetry2019 : ICarTelemetry2019
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarTelemetry2019()
    {
        CarTelemetryData = new CarTelemetryData2019[20];

        for (int carTelemetryData = 0; carTelemetryData < CarTelemetryData.Length; ++carTelemetryData)
        {
            var carTelemetryData2019 = new CarTelemetryData2019
                                       {
                                           BrakesTemperature = new Temperature(),
                                           TyresSurfaceTemperature = new Temperature(),
                                           TyresInnerTemperature = new Temperature(),
                                           TyresPressure = new TyresPressure(),
                                           SurfaceType = new WheelSurface()
                                       };

            CarTelemetryData[carTelemetryData] = carTelemetryData2019;
        }
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Bit flags specifying which button are being pressed
    /// </summary>
    public uint ButtonStatus { get; set; }

    #endregion // Properties

    #region ICarTelemetry2019

    /// <summary>
    /// Car telemetry of all cars
    /// </summary>
    public ICarTelemetryDataBase[] CarTelemetryData { get; }

    #endregion // ICarTelemetry2019
}