namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Car status data - F1 2023
/// </summary>
public interface ICarStatusData2023 : ICarStatusDataBase
{
    #region Properties

    /// <summary>
    /// Engine power output of ICE (W)
    /// </summary>
    float EnginePowerICE { get; set; }

    /// <summary>
    /// Engine power output of MGU-K (W)
    /// </summary>
    float EnginePowerMGUK { get; set; }

    /// <summary>
    /// DRS activation distance - 0 - DRS not available - non-zero - DRS will be available
    /// </summary>
    int DRSActivationDistance { get; set; }

    /// <summary>
    /// Age of laps of the current set of tyres
    /// </summary>
    ushort TyresAgeLaps { get; set; }

    /// <summary>
    /// Whether the car is paused in a network game
    /// </summary>
    ushort NetworkPaused { get; set; }

    #endregion // Properties
}