namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for the additional car telemetry (CarTelemetry2) data per car (F1 2026 and newer)
/// </summary>
public interface ICarTelemetry2DataBase
{
    #region Properties

    /// <summary>
    /// Active aero mode - 0 = Corner mode, 1 = Straight mode
    /// </summary>
    ushort ActiveAeroMode { get; set; }

    /// <summary>
    /// Active aero available - 0 = not available, 1 = available
    /// </summary>
    bool ActiveAeroAvailable { get; set; }

    /// <summary>
    /// Active aero activation distance - 0 = not available, otherwise available in given metres
    /// </summary>
    ushort ActiveAeroActivationDistance { get; set; }

    /// <summary>
    /// Overtake mode available - 0 = not available, 1 = available
    /// </summary>
    bool OvertakeAvailable { get; set; }

    /// <summary>
    /// Overtake mode active - 0 = not active, 1 = active
    /// </summary>
    bool OvertakeActive { get; set; }

    /// <summary>
    /// Overtake activation distance - 0 = not available, otherwise available in given metres
    /// </summary>
    ushort OvertakeActivationDistance { get; set; }

    /// <summary>
    /// Whether the vehicle conforms to the 2026 regulations - 0 = pre-2026, 1 = 2026 regulations applicable
    /// </summary>
    bool Is2026Regulations { get; set; }

    /// <summary>
    /// Whether the car is driving the wrong way
    /// </summary>
    bool IsDrivingWrongWay { get; set; }

    #endregion // Properties
}