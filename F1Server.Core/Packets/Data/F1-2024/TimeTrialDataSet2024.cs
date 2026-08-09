using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Time trial data (F1 2024)
/// </summary>
public class TimeTrialDataSet2024 : ITimeTrialDataSetBase
{
    #region Properties

    /// <summary>
    /// Traction control assist (off = false, on = true). Unlike <see cref="Enumerations.TractionControl"/>
    /// on car status/setup data, the time trial spec only carries a binary assist flag for this field
    /// </summary>
    public bool TractionControl { get; set; }

    /// <summary>
    /// Gearbox assist (off = false, on = true). Unlike <see cref="Enumerations.GearboxAssist"/>
    /// on car status/setup data, the time trial spec only carries a binary assist flag for this field
    /// </summary>
    public bool GearboxAssist { get; set; }

    #endregion // Properties

    #region ITimeTrialDataSetBase

    /// <summary>
    /// Index of the car this data relates to
    /// </summary>
    public ushort CarIndex { get; set; }

    /// <summary>
    /// Id of the team
    /// </summary>
    public ushort TeamId { get; set; }

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