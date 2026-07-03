using F1Server.Core.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Session data of F1 2026
/// </summary>
public class SessionData2026 : SessionData2025, ISessionData2026
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public SessionData2026()
    {
        ActiveAeroZonesFull = new ActiveAeroZone[ConstData.MaxActiveAeroZones];
        ActiveAeroZonesPartial = new ActiveAeroZone[ConstData.MaxActiveAeroZones];
        DrsZones = new DrsZone[ConstData.MaxDrsZones];
    }

    #endregion // Constructors

    #region ISessionData2026

    /// <inheritdoc/>
    public ushort ActiveAeroTrackStatus { get; set; }

    /// <inheritdoc/>
    public ushort NumberActiveAeroZonesFull { get; set; }

    /// <inheritdoc/>
    public ActiveAeroZone[] ActiveAeroZonesFull { get; }

    /// <inheritdoc/>
    public ushort NumberActiveAeroZonesPartial { get; set; }

    /// <inheritdoc/>
    public ActiveAeroZone[] ActiveAeroZonesPartial { get; }

    /// <inheritdoc/>
    public ushort NumberDrsZones { get; set; }

    /// <inheritdoc/>
    public DrsZone[] DrsZones { get; }

    /// <inheritdoc/>
    public float StartReactionTime { get; set; }

    /// <inheritdoc/>
    public ushort AntiLockBrakesAssist { get; set; }

    /// <inheritdoc/>
    public ushort TractionControlAssist { get; set; }

    /// <inheritdoc/>
    public bool DynamicRacingLineHiVis { get; set; }

    /// <inheritdoc/>
    public ushort DynamicRacingLineColourBlind { get; set; }

    /// <inheritdoc/>
    public bool RecurringRewindPrompt { get; set; }

    #endregion // ISessionData2026
}