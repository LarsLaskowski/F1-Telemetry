namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Car status data - F1 2026
/// </summary>
public interface ICarStatusData2026 : ICarStatusData2025
{
    #region Properties

    /// <summary>
    /// ERS energy harvest limit for this lap in Joules (new in F1 2026)
    /// </summary>
    float ERSHarvestLimitPerLap { get; set; }

    #endregion // Properties
}