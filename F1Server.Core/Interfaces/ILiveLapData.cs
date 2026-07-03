namespace F1Server.Core.Interfaces;

/// <summary>
/// Live lap information
/// </summary>
public interface ILiveLapData
{
    #region Properties

    /// <summary>
    /// Sector 1 time in milliseconds
    /// </summary>
    uint Sector1Time { get; }

    /// <summary>
    /// Sector 2 time in milliseconds
    /// </summary>
    uint Sector2Time { get; }

    /// <summary>
    /// Sector3Time in milliseconds
    /// </summary>
    uint Sector3Time { get; }

    #endregion // Properties
}