using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.PacketData;

/// <summary>
/// Data class with session data
/// </summary>
public class SessionData : PacketDataBase<ISessionDataBase>
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    /// <param name="sessionData">Session data</param>
    public SessionData(PacketHeader packetHeader, ISessionDataBase sessionData)
        : base(packetHeader, sessionData)
    {
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Record this session?
    /// </summary>
    public bool IsRecordable => PacketData?.IsRecordable ?? false;

    /// <summary>
    /// Current session id
    /// </summary>
    public ulong SessionId => PacketHeader?.UniqueSessionId ?? 0;

    /// <summary>
    /// Type of session
    /// </summary>
    public SessionType SessionType => PacketData?.SessionType ?? SessionType.Unknown;

    #endregion // Properties
}