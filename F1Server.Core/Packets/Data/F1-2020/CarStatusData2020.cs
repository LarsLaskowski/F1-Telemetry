using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation car status data - F1 2020
/// </summary>
public class CarStatusData2020 : ICarStatusData2020
{
    #region ICarStatusData2020

    /// <summary>
    /// DRS activation distance - 0 - DRS not available - non-zero - DRS will be available
    /// </summary>
    public int DRSActivationDistance { get; set; }

    /// <summary>
    /// Age of laps of the current set of tyres
    /// </summary>
    public ushort TyresAgeLaps { get; set; }

    /// <summary>
    /// Tyre damage in percent
    /// </summary>
    public ushort[] TyreDamage { get; set; }

    /// <summary>
    /// Front left wing damage in percent
    /// </summary>
    public ushort FrontLeftWingDamage { get; set; }

    /// <summary>
    /// Front right wing damage in percent
    /// </summary>
    public ushort FrontRightWingDamage { get; set; }

    /// <summary>
    /// Rear wing damage in percent
    /// </summary>
    public ushort RearWingDamage { get; set; }

    /// <summary>
    /// Engine Damage in percent
    /// </summary>
    public ushort EngineDamage { get; set; }

    /// <summary>
    /// Gearbox damage in percent
    /// </summary>
    public ushort GearBoxDamage { get; set; }

    /// <summary>
    /// Indicator for DRS fault - 0 - ok - 1 - fault
    /// </summary>
    public ushort DRSFault { get; set; }

    /// <summary>
    /// Tyres wear percentage
    /// </summary>
    public ushort[] TyresWear { get; set; }

    #endregion // ICarStatusData2020

    #region ICarStatusDataBase

    /// <summary>
    /// Traction control 0 - off - 2 - high
    /// </summary>
    public ushort TractionControl { get; set; }

    /// <summary>
    /// Anti lock brakes - 0 - off - 1 - on
    /// </summary>
    public ushort AntiLockBrakes { get; set; }

    /// <summary>
    /// Fuel mix - 0 - lean - 1 - standard - 2 - rich - 3 - max
    /// </summary>
    public ushort FuelMix { get; set; }

    /// <summary>
    /// Front brake bias in percentage
    /// </summary>
    public ushort FrontBrakeBias { get; set; }

    /// <summary>
    /// Pit limiter status - 0 - off - 1 - on
    /// </summary>
    public uint PitLimiterStatus { get; set; }

    /// <summary>
    /// Current fuel mass
    /// </summary>
    public float FuelInTank { get; set; }

    /// <summary>
    /// Fuel capacity
    /// </summary>
    public float FuelCapacity { get; set; }

    /// <summary>
    /// Fuel remaining in terms of laps (value on MFD)
    /// </summary>
    public float FuelRemainingLaps { get; set; }

    /// <summary>
    /// Cars max RPM, point of rev limiter
    /// </summary>
    public uint MaxRPM { get; set; }

    /// <summary>
    /// Cars idle RPM
    /// </summary>
    public uint IdleRPM { get; set; }

    /// <summary>
    /// Maximum number of gears
    /// </summary>
    public ushort MaxGears { get; set; }

    /// <summary>
    /// DRS allowed - 0 - not allowed - 1 - allowed - -1 - unknown
    /// </summary>
    public short DRSAllowed { get; set; }

    /// <summary>
    /// Tyre compound
    /// </summary>
    public ushort ActualTyreCompound { get; set; }

    /// <summary>
    /// Visual tyre compound - can be different from actual compound
    /// </summary>
    public VisualTyreCompound VisualTyreCompound { get; set; }

    /// <summary>
    /// FIA flags
    /// </summary>
    public VehicleFiaFlagColor FiaFlags { get; set; }

    /// <summary>
    /// ERS energy store in Joules
    /// </summary>
    public float ERSStoreEnergy { get; set; }

    /// <summary>
    /// ERS deployment mode - 0 - none - 1 - medium - 2 - hotlap - 3 - overtake
    /// </summary>
    public ushort ERSDeployMode { get; set; }

    /// <summary>
    /// ERS energy harvested this lap by MGU-K
    /// </summary>
    public float ERSHarvestedThisLapMGUK { get; set; }

    /// <summary>
    /// ERS energy harvested this lap by MGU-H
    /// </summary>
    public float ERSHarvestedThisLapMGUH { get; set; }

    /// <summary>
    /// ERS energy deployed this lap
    /// </summary>
    public float ERSDeployedThisLap { get; set; }

    #endregion // ICarStatusDataBase
}