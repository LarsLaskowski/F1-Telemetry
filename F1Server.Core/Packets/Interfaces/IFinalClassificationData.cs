namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Complete final classification data
/// </summary>
public interface IFinalClassificationData
{
    #region Properties

    /// <summary>
    /// Number of cars in the final classification
    /// </summary>
    ushort NumberOfCars { get; set; }

    /// <summary>
    /// Data of final classification for each car
    /// </summary>
    IFinalClassificationCarBase[] FinalClassifications { get; }

    #endregion // Properties
}