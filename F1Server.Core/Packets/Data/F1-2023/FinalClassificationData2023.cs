using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Complete data of final classification in F1 2023
/// </summary>
public class FinalClassificationData2023 : IFinalClassificationData
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public FinalClassificationData2023()
    {
        FinalClassifications = new FinalClassificationCarData2023[22];
    }

    #endregion // Constructors

    #region IFinalClassificationData

    /// <summary>
    /// Number of cars in the final classification
    /// </summary>
    public ushort NumberOfCars { get; set; }

    /// <summary>
    /// Data of final classification for each car
    /// </summary>
    public IFinalClassificationCarBase[] FinalClassifications { get; }

    #endregion // IFinalClassificationData
}