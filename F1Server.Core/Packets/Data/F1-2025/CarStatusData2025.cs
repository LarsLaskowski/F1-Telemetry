using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation car status data - F1 2025
/// </summary>
public class CarStatusData2025 : ICarStatusData2025
{
    #region ICarStatusDataBase

    /// <inheritdoc/>
    public ushort TractionControl { get; set; }

    /// <inheritdoc/>
    public ushort AntiLockBrakes { get; set; }

    /// <inheritdoc/>
    public ushort FuelMix { get; set; }

    /// <inheritdoc/>
    public ushort FrontBrakeBias { get; set; }

    /// <inheritdoc/>
    public uint PitLimiterStatus { get; set; }

    /// <inheritdoc/>
    public float FuelInTank { get; set; }

    /// <inheritdoc/>
    public float FuelCapacity { get; set; }

    /// <inheritdoc/>
    public float FuelRemainingLaps { get; set; }

    /// <inheritdoc/>
    public uint MaxRPM { get; set; }

    /// <inheritdoc/>
    public uint IdleRPM { get; set; }

    /// <inheritdoc/>
    public ushort MaxGears { get; set; }

    /// <inheritdoc/>
    public short DRSAllowed { get; set; }

    /// <inheritdoc/>
    public ushort ActualTyreCompound { get; set; }

    /// <inheritdoc/>
    public VisualTyreCompound VisualTyreCompound { get; set; }

    /// <inheritdoc/>
    public VehicleFiaFlagColor FiaFlags { get; set; }

    /// <inheritdoc/>
    public float ERSStoreEnergy { get; set; }

    /// <inheritdoc/>
    public ushort ERSDeployMode { get; set; }

    /// <inheritdoc/>
    public float ERSHarvestedThisLapMGUK { get; set; }

    /// <inheritdoc/>
    public float ERSHarvestedThisLapMGUH { get; set; }

    /// <inheritdoc/>
    public float ERSDeployedThisLap { get; set; }

    #endregion // ICarStatusDataBase

    #region ICarStatusData2023

    /// <inheritdoc/>
    public float EnginePowerICE { get; set; }

    /// <inheritdoc/>
    public float EnginePowerMGUK { get; set; }

    /// <inheritdoc/>
    public int DRSActivationDistance { get; set; }

    /// <inheritdoc/>
    public ushort TyresAgeLaps { get; set; }

    /// <inheritdoc/>
    public ushort NetworkPaused { get; set; }

    #endregion // ICarStatusData2023
}