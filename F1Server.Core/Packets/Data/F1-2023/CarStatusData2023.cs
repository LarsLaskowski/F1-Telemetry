using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation car status data - F1 2023
/// </summary>
public class CarStatusData2023 : ICarStatusData2023
{
    #region ICarStatusBase

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
    public ushort FronBrakeBias { get; set; }

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

    #endregion // ICarStatusBase

    #region ICarStatus2023

    /// <summary>
    /// Engine power output of ICE (W)
    /// </summary>
    public float EnginePowerICE { get; set; }

    /// <summary>
    /// Engine power output of MGU-K (W)
    /// </summary>
    public float EnginePowerMGUK { get; set; }

    /// <summary>
    /// DRS activation distance - 0 - DRS not available - non-zero - DRS will be available
    /// </summary>
    public int DRSActivationDistance { get; set; }

    /// <summary>
    /// Age of laps of the current set of tyres
    /// </summary>
    public ushort TyresAgeLaps { get; set; }

    /// <summary>
    /// Whether the car is paused in a network game
    /// </summary>
    public ushort NetworkPaused { get; set; }

    #endregion // ICarStatus2023
}