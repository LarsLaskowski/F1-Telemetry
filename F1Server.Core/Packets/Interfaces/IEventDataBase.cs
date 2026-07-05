namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Base interface for all event data interfaces
/// </summary>
public interface IEventDataBase
{
    #region Properties

    /// <summary>
    /// Event code
    /// </summary>
    string EventCode { get; set; }

    /// <summary>
    /// Event details
    /// </summary>
    IEventDataDetailsBase EventDetails { get; }

    #endregion // Properties
}