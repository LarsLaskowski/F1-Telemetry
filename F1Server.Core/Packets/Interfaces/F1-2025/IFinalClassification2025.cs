using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Extend the new data of the final classification packet in F1 2025
/// </summary>
public interface IFinalClassification2025
{
    #region Properties

    /// <summary>
    /// Result reason
    /// </summary>
    ResultReason ResultReason { get; set; }

    #endregion // Properties
}