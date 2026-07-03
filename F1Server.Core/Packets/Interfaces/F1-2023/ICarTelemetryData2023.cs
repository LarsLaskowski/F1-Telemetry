namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Car telemetry data - F1 2023
/// </summary>
public interface ICarTelemetryData2023 : ICarTelemetryDataBase
{
    #region Properties

    /// <summary>
    /// Rev lights (bit 0 = leftmost, bit 14 = rightmost LED)
    /// </summary>
    ushort RevLightsBitValue { get; }

    #endregion // Properties
}