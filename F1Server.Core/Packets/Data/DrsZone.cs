namespace F1Server.Core.Packets.Data;

/// <summary>
/// DRS zone data (F1 2026 and newer)
/// </summary>
public class DrsZone
{
    #region Properties

    /// <summary>
    /// Fraction (0..1) of way through the lap the DRS zone starts
    /// </summary>
    public float ZoneStart { get; set; }

    /// <summary>
    /// Fraction (0..1) of way through the lap the DRS zone ends
    /// </summary>
    public float ZoneEnd { get; set; }

    #endregion // Properties
}