using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation car status data - F1 2026
/// </summary>
public class CarStatusData2026 : CarStatusData2025, ICarStatusData2026
{
    #region ICarStatusData2026

    /// <inheritdoc/>
    public float ERSHarvestLimitPerLap { get; set; }

    #endregion // ICarStatusData2026
}