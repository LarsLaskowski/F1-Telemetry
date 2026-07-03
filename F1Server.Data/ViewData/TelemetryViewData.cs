namespace F1Server.Data.ViewData;

/// <summary>
/// View of telemetry data from one lap and driver
/// </summary>
public class TelemetryViewData
{
    #region Properties

    /// <summary>
    /// Id
    /// </summary>
    public int PacketId { get; set; }

    /// <summary>
    /// Lap distance
    /// </summary>
    public float Distance { get; set; }

    /// <summary>
    /// Speed
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Throttle
    /// </summary>
    public float Throttle { get; set; }

    /// <summary>
    /// Brake
    /// </summary>
    public float Brake { get; set; }

    /// <summary>
    /// RPM of engine
    /// </summary>
    public ushort EngineRPM { get; set; }

    /// <summary>
    /// Gear
    /// </summary>
    public short Gear { get; set; }

    #endregion // Properties
}