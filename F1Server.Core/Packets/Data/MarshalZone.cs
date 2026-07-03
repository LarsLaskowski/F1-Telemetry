using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class for a zone on track
/// </summary>
public class MarshalZone
{
    #region Properties

    /// <summary>
    /// Start of zone
    /// </summary>
    public float ZoneStart { get; set; }

    /// <summary>
    /// Flag of zone
    /// </summary>
    public ZoneFlagColor ZoneFlag { get; set; }

    #endregion // Properties
}