using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Car telemetry data
/// </summary>
public class CarTelemetry : PacketDataBase<ICarTelemetryBase>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="telemetryData">Car telemetry data</param>
    public CarTelemetry(PacketHeader packetHeader, ICarTelemetryBase telemetryData)
        : base(packetHeader, telemetryData)
    {
    }

    #endregion // Constructors
}