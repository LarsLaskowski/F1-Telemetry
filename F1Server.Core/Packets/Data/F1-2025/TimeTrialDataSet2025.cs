using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Time trial data (F1 2025)
/// </summary>
public class TimeTrialDataSet2025 : ITimeTrialDataSetBase
{
    #region ITimeTrialDataSetBase

    /// <inheritdoc/>
    public int CarIndex { get; set; }

    /// <inheritdoc/>
    public int TeamId { get; set; }

    /// <inheritdoc/>
    public uint LapTime { get; set; }

    /// <inheritdoc/>
    public uint Sector1Time { get; set; }

    /// <inheritdoc/>
    public uint Sector2Time { get; set; }

    /// <inheritdoc/>
    public uint Sector3Time { get; set; }

    /// <inheritdoc/>
    public TractionControl TractionControl { get; set; }

    /// <inheritdoc/>
    public GearboxAssist GearboxAssist { get; set; }

    /// <inheritdoc/>
    public bool AntiLockBrakes { get; set; }

    /// <inheritdoc/>
    public bool IsRealisticCarPerformance { get; set; }

    /// <inheritdoc/>
    public bool IsCustomSetup { get; set; }

    /// <inheritdoc/>
    public bool IsValid { get; set; }

    #endregion // ITimeTrialDataSetBase
}