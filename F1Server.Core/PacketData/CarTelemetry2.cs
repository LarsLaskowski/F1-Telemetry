using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Additional car telemetry (CarTelemetry2) data (F1 2026 and newer)
/// </summary>
public class CarTelemetry2 : PacketDataBase<ICarTelemetry2Base>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="telemetryData">Additional car telemetry data</param>
    public CarTelemetry2(PacketHeader packetHeader, ICarTelemetry2Base telemetryData)
        : base(packetHeader, telemetryData)
    {
    }

    #endregion // Constructors
}