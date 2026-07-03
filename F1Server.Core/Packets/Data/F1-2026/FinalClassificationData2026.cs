using F1Server.Core.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Complete data of final classification in F1 2026
/// </summary>
public class FinalClassificationData2026 : IFinalClassificationData
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public FinalClassificationData2026()
    {
        FinalClassifications = new FinalClassificationCarData2026[ConstData.F12026MaxCars];
    }

    #endregion // Constructors

    #region IFinalClassificationData

    /// <inheritdoc/>
    public ushort NumberOfCars { get; set; }

    /// <inheritdoc/>
    public IFinalClassificationCarBase[] FinalClassifications { get; }

    #endregion // IFinalClassificationData
}