namespace F1Server.Core.Packets.Data;

/// <summary>
/// Active aero zone data (F1 2026 and newer)
/// </summary>
public class ActiveAeroZone
{
    #region Properties

    /// <summary>
    /// Fraction (0..1) of way through the lap the active aero zone starts
    /// </summary>
    public float ZoneStart { get; set; }

    /// <summary>
    /// Fraction (0..1) of way through the lap the active aero zone ends
    /// </summary>
    public float ZoneEnd { get; set; }

    #endregion // Properties
}