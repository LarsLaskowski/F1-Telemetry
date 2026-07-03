using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event data of F1 2023
/// </summary>
public class EventData2023 : IEventDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public EventData2023()
    {
        EventDetails = new EventDataDetails2023();
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Event code
    /// </summary>
    public string EventCode { get; set; }

    /// <summary>
    /// Event details
    /// </summary>
    public IEventDataDetailsBase EventDetails { get; }

    #endregion // Properties
}