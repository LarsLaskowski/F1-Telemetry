using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Complete data of final classification in F1 2025
/// </summary>
public class FinalClassificationData2025 : IFinalClassificationData
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public FinalClassificationData2025()
    {
        FinalClassifications = new FinalClassificationCarData2025[22];
    }

    #endregion // Constructors

    #region IFinalClassificationData

    /// <inheritdoc/>
    public ushort NumberOfCars { get; set; }

    /// <inheritdoc/>
    public IFinalClassificationCarBase[] FinalClassifications { get; }

    #endregion // IFinalClassificationData
}