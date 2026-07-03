using F1Server.Core.Packets.Data;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Session data for F1 2026
/// </summary>
public interface ISessionData2026 : ISessionData2025
{
    #region Properties

    /// <summary>
    /// Active aero track status - 0 = Full, 1 = Partial
    /// </summary>
    ushort ActiveAeroTrackStatus { get; set; }

    /// <summary>
    /// Number of full active aero zones to follow
    /// </summary>
    ushort NumberActiveAeroZonesFull { get; set; }

    /// <summary>
    /// List of full active aero zones - max 8
    /// </summary>
    ActiveAeroZone[] ActiveAeroZonesFull { get; }

    /// <summary>
    /// Number of partial active aero zones to follow
    /// </summary>
    ushort NumberActiveAeroZonesPartial { get; set; }

    /// <summary>
    /// List of partial active aero zones - max 8
    /// </summary>
    ActiveAeroZone[] ActiveAeroZonesPartial { get; }

    /// <summary>
    /// Number of DRS zones to follow
    /// </summary>
    ushort NumberDrsZones { get; set; }

    /// <summary>
    /// List of DRS zones - max 4
    /// </summary>
    DrsZone[] DrsZones { get; }

    /// <summary>
    /// Driver start reaction time in seconds, 0.0 if assisted starts
    /// </summary>
    float StartReactionTime { get; set; }

    /// <summary>
    /// Anti lock brakes assist - 0 = off, 1 = on
    /// </summary>
    ushort AntiLockBrakesAssist { get; set; }

    /// <summary>
    /// Traction control assist - 0 = off, 1 = medium, 2 = full
    /// </summary>
    ushort TractionControlAssist { get; set; }

    /// <summary>
    /// Dynamic racing line high visibility - 0 = off, 1 = on
    /// </summary>
    bool DynamicRacingLineHiVis { get; set; }

    /// <summary>
    /// Dynamic racing line colour blind mode - 0 = off, 1 = Protanopia, 2 = Deuteranopia, 3 = Tritanopia
    /// </summary>
    ushort DynamicRacingLineColourBlind { get; set; }

    /// <summary>
    /// Recurring rewind prompt - 0 = off, 1 = on
    /// </summary>
    bool RecurringRewindPrompt { get; set; }

    #endregion // Properties
}