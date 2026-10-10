using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event data of F1 2021
/// </summary>
public class EventData2021 : IEventDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public EventData2021()
    {
        EventDetails = new EventDataDetails2021();
    }

    #endregion // Constructors

    #region IEventDataBase

    /// <inheritdoc/>
    public string EventCode { get; set; }

    /// <inheritdoc/>
    public IEventDataDetailsBase EventDetails { get; }

    #endregion // IEventDataBase
}