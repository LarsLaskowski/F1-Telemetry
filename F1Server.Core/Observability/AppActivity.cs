using System.Diagnostics;

using F1Server.Core.Enumerations;

namespace F1Server.Core.Observability;

/// <summary>
/// Application telemetry object
/// </summary>
public static class AppActivity
{
    #region Fields

    /// <summary>
    /// Application source name WebAPI
    /// </summary>
    public static readonly ActivitySource ApiSource = new("F1-Telemetry-WebAPI", "1.0");

    /// <summary>
    /// Application source name server
    /// </summary>
    public static readonly ActivitySource SrvSource = new("F1-Telemetry", "1.0");

    #endregion // Fields

    #region Methods

    /// <summary>
    /// Determines whether a packet type is sent at a high frequency (multiple times per second), so that
    /// tracing spans for it should be limited to slow outliers instead of every single packet
    /// </summary>
    /// <param name="packetType">Packet type to check</param>
    /// <returns>True if the packet type is high-frequency, otherwise false</returns>
    public static bool IsHighFrequencyPacketType(PacketTypes? packetType)
    {
        return packetType is PacketTypes.CarTelemetry
                          or PacketTypes.Motion
                          or PacketTypes.LapData
                          or PacketTypes.CarStatus
                          or PacketTypes.SessionHistory;
    }

    #endregion // Methods
}