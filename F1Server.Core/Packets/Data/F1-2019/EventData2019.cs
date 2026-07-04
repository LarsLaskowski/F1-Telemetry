using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event data of F1 2019
/// </summary>
public class EventData2019 : IEventDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public EventData2019()
    {
        EventDetails = new EventDataDetails2019();
    }

    #endregion // Constructors

    #region IEventDataBase

    /// <summary>
    /// Event code
    /// </summary>
    public string EventCode { get; set; }

    /// <summary>
    /// Event details
    /// </summary>
    public IEventDataDetailsBase EventDetails { get; }

    #endregion // IEventDataBase
}