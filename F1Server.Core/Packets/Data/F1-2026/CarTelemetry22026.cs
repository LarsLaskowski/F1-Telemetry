using F1Server.Core.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Additional car telemetry (CarTelemetry2) data of all cars - F1 2026
/// </summary>
public class CarTelemetry22026 : ICarTelemetry22026
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public CarTelemetry22026()
    {
        CarTelemetry2Data = new CarTelemetry2Data2026[ConstData.F12026MaxCars];

        for (int car = 0; car < CarTelemetry2Data.Length; ++car)
        {
            CarTelemetry2Data[car] = new CarTelemetry2Data2026();
        }
    }

    #endregion // Constructors

    #region ICarTelemetry2Base

    /// <inheritdoc/>
    public ICarTelemetry2DataBase[] CarTelemetry2Data { get; }

    #endregion // ICarTelemetry2Base
}