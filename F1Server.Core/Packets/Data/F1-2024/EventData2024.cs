using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event data of F1 2024
/// </summary>
public class EventData2024 : IEventDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public EventData2024()
    {
        EventDetails = new EventDataDetails2024();
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