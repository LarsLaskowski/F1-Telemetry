using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event data of F1 2020
/// </summary>
public class EventData2020 : IEventDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public EventData2020()
    {
        EventDetails = new EventDataDetails2020();
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