using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Session data for F1 2023
/// </summary>
public interface ISessionData2023 : ISessionData2022
{
    #region Properties

    /// <summary>
    /// Players speed unit
    /// </summary>
    SpeedUnits SpeedUnit { get; }

    /// <summary>
    /// Players temperature unit
    /// </summary>
    TemperatureUnits TemperatureUnit { get; }

    /// <summary>
    /// Secondary players speed unit
    /// </summary>
    SpeedUnits SpeedUnitSecondaryPlayer { get; }

    /// <summary>
    /// Secondary players temperature unit
    /// </summary>
    TemperatureUnits TemperatureUnitSecondaryPlayer { get; }

    /// <summary>
    /// Number of safety car periods during session
    /// </summary>
    uint SafetyCarPeriods { get; }

    /// <summary>
    /// Number of virtual safety car periods during session
    /// </summary>
    uint VirtualSafetyCarPeriods { get; }

    /// <summary>
    /// Number of red flags during session
    /// </summary>
    uint RedFlags { get; }

    #endregion // Properties
}