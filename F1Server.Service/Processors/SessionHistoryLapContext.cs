using F1Server.Core.Packets.Interfaces;
using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.Service.Runtime;

namespace F1Server.Service.Processors;

/// <summary>
/// Bundles the data that stays constant while the laps of one session history packet are processed
/// </summary>
internal sealed class SessionHistoryLapContext
{
    #region Properties

    /// <summary>
    /// Runtime data of the participant the current packet belongs to
    /// </summary>
    public ParticipantRuntimeData ParticipantRuntimeData { get; set; }

    /// <summary>
    /// Live driver data of the participant, or null when the driver is not tracked live
    /// </summary>
    public LiveDriverData? LiveDriverData { get; set; }

    /// <summary>
    /// Live session data of the current session
    /// </summary>
    public LiveSessionData LiveSessionData { get; set; }

    /// <summary>
    /// Session history data of the current packet
    /// </summary>
    public ISessionHistoryDataBase SessionHistoryData { get; set; }

    /// <summary>
    /// Lazily created database factory shared by all laps of the current packet
    /// </summary>
    public Lazy<RepositoryFactory> DbFactory { get; set; }

    /// <summary>
    /// Indicates whether the final classification has already been received
    /// </summary>
    public bool IsFinalDataReceived { get; set; }

    #endregion // Properties
}