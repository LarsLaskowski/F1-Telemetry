namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Extend the new data of the final classification packet in F1 2023
/// </summary>
public interface IFinalClassification2023
{
    #region Properties

    /// <summary>
    /// The lap number stints end on
    /// </summary>
    ushort[] TyreStintsEndLaps { get; set; }

    #endregion // Properties
}