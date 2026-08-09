using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Time trial data (F1 2025)
/// </summary>
public class TimeTrialDataSet2025 : ITimeTrialDataSet2025
{
    #region ITimeTrialDataSetBase

    /// <inheritdoc/>
    public ushort CarIndex { get; set; }

    /// <inheritdoc/>
    public ushort TeamId { get; set; }

    /// <inheritdoc/>
    public uint LapTime { get; set; }

    /// <inheritdoc/>
    public uint Sector1Time { get; set; }

    /// <inheritdoc/>
    public uint Sector2Time { get; set; }

    /// <inheritdoc/>
    public uint Sector3Time { get; set; }

    /// <inheritdoc/>
    public bool TractionControl { get; set; }

    /// <inheritdoc/>
    public bool GearboxAssist { get; set; }

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