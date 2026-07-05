using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for car status packets across all F1 game versions
/// </summary>
public interface ICarStatusDataBase
{
    #region Properties

    /// <summary>
    /// Traction control 0 - off - 2 - high
    /// </summary>
    ushort TractionControl { get; set; }

    /// <summary>
    /// Anti lock brakes - 0 - off - 1 - on
    /// </summary>
    ushort AntiLockBrakes { get; set; }

    /// <summary>
    /// Fuel mix - 0 - lean - 1 - standard - 2 - rich - 3 - max
    /// </summary>
    ushort FuelMix { get; set; }

    /// <summary>
    /// Front brake bias in percentage
    /// </summary>
    ushort FrontBrakeBias { get; set; }

    /// <summary>
    /// Pit limiter status - 0 - off - 1 - on
    /// </summary>
    uint PitLimiterStatus { get; set; }

    /// <summary>
    /// Current fuel mass
    /// </summary>
    float FuelInTank { get; set; }

    /// <summary>
    /// Fuel capacity
    /// </summary>
    float FuelCapacity { get; set; }

    /// <summary>
    /// Fuel remaining in terms of laps (value on MFD)
    /// </summary>
    float FuelRemainingLaps { get; set; }

    /// <summary>
    /// Cars max RPM, point of rev limiter
    /// </summary>
    uint MaxRPM { get; set; }

    /// <summary>
    /// Cars idle RPM
    /// </summary>
    uint IdleRPM { get; set; }

    /// <summary>
    /// Maximum number of gears
    /// </summary>
    ushort MaxGears { get; set; }

    /// <summary>
    /// DRS allowed - 0 - not allowed - 1 - allowed - -1 - unknown
    /// </summary>
    short DRSAllowed { get; set; }

    /// <summary>
    /// Tyre compound
    /// </summary>
    ushort ActualTyreCompound { get; set; }

    /// <summary>
    /// Visual tyre compound - can be different from actual compound
    /// </summary>
    VisualTyreCompound VisualTyreCompound { get; set; }

    /// <summary>
    /// FIA flags
    /// </summary>
    VehicleFiaFlagColor FiaFlags { get; set; }

    /// <summary>
    /// ERS energy store in Joules
    /// </summary>
    float ERSStoreEnergy { get; set; }

    /// <summary>
    /// ERS deployment mode - 0 - none - 1 - medium - 2 - hotlap - 3 - overtake
    /// </summary>
    ushort ERSDeployMode { get; set; }

    /// <summary>
    /// ERS energy harvested this lap by MGU-K
    /// </summary>
    float ERSHarvestedThisLapMGUK { get; set; }

    /// <summary>
    /// ERS energy harvested this lap by MGU-H
    /// </summary>
    float ERSHarvestedThisLapMGUH { get; set; }

    /// <summary>
    /// ERS energy deployed this lap
    /// </summary>
    float ERSDeployedThisLap { get; set; }

    #endregion // Properties
}