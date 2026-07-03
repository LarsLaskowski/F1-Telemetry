using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation of the additional car telemetry (CarTelemetry2) data - F1 2026
/// </summary>
public class CarTelemetry2Data2026 : ICarTelemetry2Data2026
{
    #region ICarTelemetry2DataBase

    /// <inheritdoc/>
    public ushort ActiveAeroMode { get; set; }

    /// <inheritdoc/>
    public bool ActiveAeroAvailable { get; set; }

    /// <inheritdoc/>
    public ushort ActiveAeroActivationDistance { get; set; }

    /// <inheritdoc/>
    public bool OvertakeAvailable { get; set; }

    /// <inheritdoc/>
    public bool OvertakeActive { get; set; }

    /// <inheritdoc/>
    public ushort OvertakeActivationDistance { get; set; }

    /// <inheritdoc/>
    public bool Is2026Regulations { get; set; }

    /// <inheritdoc/>
    public bool IsDrivingWrongWay { get; set; }

    #endregion // ICarTelemetry2DataBase
}