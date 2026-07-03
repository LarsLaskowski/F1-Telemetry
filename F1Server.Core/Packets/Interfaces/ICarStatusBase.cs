namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface car status data of all cars
/// </summary>
public interface ICarStatusBase
{
    #region Properties

    /// <summary>
    /// Car status data of all cars
    /// </summary>
    ICarStatusDataBase[] CarStatusData { get; }

    #endregion // Properties
}