namespace F1Server.Core.Interfaces;

/// <summary>
/// Base live data interface
/// </summary>
public interface ILiveBaseData
{
    #region Properties

    /// <summary>
    /// Database id
    /// </summary>
    long DbId { get; set; }

    #endregion // Properties
}