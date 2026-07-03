namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Car status data - F1 2019
/// </summary>
public interface ICarStatusData2019 : ICarStatusDataBase
{
    #region Properties

    /// <summary>
    /// Tyres wear percentage
    /// </summary>
    ushort[] TyresWear { get; set; }

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

    #endregion // Properties
}