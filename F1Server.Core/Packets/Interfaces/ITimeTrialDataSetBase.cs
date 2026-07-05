using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Time trial base data set (starting with F1 2024)
/// </summary>
public interface ITimeTrialDataSetBase
{
    #region Properties

    /// <summary>
    /// Index of the car this data relates to
    /// </summary>
    int CarIndex { get; set; }

    /// <summary>
    /// Id of the team
    /// </summary>
    int TeamId { get; set; }

    /// <summary>
    /// Lap time in milliseconds
    /// </summary>
    uint LapTime { get; set; }

    /// <summary>
    /// Sector 1 time in milliseconds
    /// </summary>
    uint Sector1Time { get; set; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    uint Sector2Time { get; set; }

    /// <summary>
    /// Sector 3 time in milliseconds
    /// </summary>
    uint Sector3Time { get; set; }

    /// <summary>
    /// Traction control
    /// </summary>
    TractionControl TractionControl { get; set; }

    /// <summary>
    /// Assist settings of the gear box
    /// </summary>
    GearboxAssist GearboxAssist { get; set; }

    /// <summary>
    /// Anti lock brakes (off = false, on = true)
    /// </summary>
    bool AntiLockBrakes { get; set; }

    /// <summary>
    /// Realistic car performance, otherwise equal
    /// </summary>
    bool IsRealisticCarPerformance { get; set; }

    /// <summary>
    /// Custom setup
    /// </summary>
    bool IsCustomSetup { get; set; }

    /// <summary>
    /// Valid or invalid
    /// </summary>
    bool IsValid { get; set; }

    #endregion // Properties
}