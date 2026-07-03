namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Car status data - F1 2020
/// </summary>
public interface ICarStatusData2020 : ICarStatusDataBase
{
    #region Properties

    /// <summary>
    /// Tyres wear percentage
    /// </summary>
    ushort[] TyresWear { get; set; }

    /// <summary>
    /// DRS activation distance - 0 - DRS not available - non-zero - DRS will be available
    /// </summary>
    int DRSActivationDistance { get; set; }

    /// <summary>
    /// Age of laps of the current set of tyres
    /// </summary>
    ushort TyresAgeLaps { get; set; }

    /// <summary>
    /// Tyre damage in percent
    /// </summary>
    ushort[] TyreDamage { get; set; }

    /// <summary>
    /// Front left wing damage in percent
    /// </summary>
    ushort FrontLeftWingDamage { get; set; }

    /// <summary>
    /// Front right wing damage in percent
    /// </summary>
    ushort FrontRightWingDamage { get; set; }

    /// <summary>
    /// Rear wing damage in percent
    /// </summary>
    ushort RearWingDamage { get; set; }

    /// <summary>
    /// Engine Damage in percent
    /// </summary>
    ushort EngineDamage { get; set; }

    /// <summary>
    /// Gearbox damage in percent
    /// </summary>
    ushort GearBoxDamage { get; set; }

    /// <summary>
    /// Indicator for DRS fault - 0 - ok - 1 - fault
    /// </summary>
    ushort DRSFault { get; set; }

    #endregion // Properties
}