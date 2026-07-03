namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Car telemetry data - F1 2021
/// </summary>
public interface ICarTelemetryData2021 : ICarTelemetryDataBase
{
    #region Properties

    /// <summary>
    /// Rev lights (bit 0 = leftmost, bit 14 = rightmost LED)
    /// </summary>
    ushort RevLightsBitValue { get; }

    #endregion // Properties
}