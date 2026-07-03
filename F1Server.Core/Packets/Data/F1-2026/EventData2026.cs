using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Event data of F1 2026
/// </summary>
public class EventData2026 : IEventDataBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public EventData2026()
    {
        EventDetails = new EventDataDetails2026();
    }

    #endregion // Constructors

    #region Properties

    /// <inheritdoc/>
    public string EventCode { get; set; }

    /// <inheritdoc/>
    public IEventDataDetailsBase EventDetails { get; }

    #endregion // Properties
}