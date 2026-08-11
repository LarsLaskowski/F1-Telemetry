namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Car telemetry data - F1 2023
/// </summary>
public interface ICarTelemetry2023 : ICarTelemetryBase
{
    #region Properties

    /// <summary>
    /// Index of MFD panel open - 255 = closed
    /// </summary>
    ushort MfdPanelIndex { get; }

    /// <summary>
    /// Index of MFD panel open (second player)
    /// </summary>
    ushort MfdPanelIndexSecondary { get; }

    /// <summary>
    /// Suggested gear
    /// </summary>
    ushort SuggestedGear { get; }

    #endregion // Properties
}