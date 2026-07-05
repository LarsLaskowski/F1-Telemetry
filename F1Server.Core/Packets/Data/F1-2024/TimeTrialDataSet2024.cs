using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Time trial data (F1 2024)
/// </summary>
public class TimeTrialDataSet2024 : ITimeTrialDataSetBase
{
    #region ITimeTrialDataSetBase

    /// <summary>
    /// Index of the car this data relates to
    /// </summary>
    public int CarIndex { get; set; }

    /// <summary>
    /// Id of the team
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// Lap time in milliseconds
    /// </summary>
    public uint LapTime { get; set; }

    /// <summary>
    /// Sector 1 time in milliseconds
    /// </summary>
    public uint Sector1Time { get; set; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    public uint Sector2Time { get; set; }

    /// <summary>
    /// Sector 3 time in milliseconds
    /// </summary>
    public uint Sector3Time { get; set; }

    /// <summary>
    /// Traction control
    /// </summary>
    public TractionControl TractionControl { get; set; }

    /// <summary>
    /// Assist settings of the gear box
    /// </summary>
    public GearboxAssist GearboxAssist { get; set; }

    /// <summary>
    /// Anti lock brakes (off = false, on = true)
    /// </summary>
    public bool AntiLockBrakes { get; set; }

    /// <summary>
    /// Realistic car performance, otherwise equal
    /// </summary>
    public bool IsRealisticCarPerformance { get; set; }

    /// <summary>
    /// Custom setup
    /// </summary>
    public bool IsCustomSetup { get; set; }

    /// <summary>
    /// Valid or invalid
    /// </summary>
    public bool IsValid { get; set; }

    #endregion // ITimeTrialDataSetBase
}