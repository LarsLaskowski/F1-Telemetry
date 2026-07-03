namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Car status data - F1 2021
/// </summary>
public interface ICarStatusData2021 : ICarStatusDataBase
{
    #region Properties

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