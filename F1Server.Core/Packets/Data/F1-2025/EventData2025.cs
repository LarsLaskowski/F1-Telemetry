using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event data of F1 2025
/// </summary>
public class EventData2025 : IEventDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public EventData2025()
    {
        EventDetails = new EventDataDetails2025();
    }

    #endregion // Constructors

    #region IEventDataBase

    /// <inheritdoc/>
    public string EventCode { get; set; }

    /// <inheritdoc/>
    public IEventDataDetailsBase EventDetails { get; }

    #endregion // IEventDataBase
}