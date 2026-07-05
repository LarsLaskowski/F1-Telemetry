using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Car telemetry data - F1 2020
/// </summary>
public class CarTelemetry2020 : ICarTelemetry2020
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarTelemetry2020()
    {
        CarTelemetryData = new CarTelemetryData2020[22];

        for (int catTelemetry = 0; catTelemetry < CarTelemetryData.Length; ++catTelemetry)
        {
            var carTelemetryData2020 = new CarTelemetryData2020
                                       {
                                           BrakesTemperature = new Temperature(),
                                           TyresSurfaceTemperature = new Temperature(),
                                           TyresInnerTemperature = new Temperature(),
                                           TyresPressure = new TyresPressure(),
                                           SurfaceType = new WheelSurface()
                                       };

            CarTelemetryData[catTelemetry] = carTelemetryData2020;
        }
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Bit flags specifying which buttons are being pressed
    /// </summary>
    public uint ButtonStatus { get; set; }

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