using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Data;

namespace F1Server.Core.EventArgs;

/// <summary>
/// Eventargument class for received packets
/// </summary>
public class PacketReceivedEventArgs
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    public PacketReceivedEventArgs(PacketHeader packetHeader)
    {
        GameVersion = packetHeader.GameVersion;
        ProductVersion = $"{packetHeader.MajorGameVersion}.{packetHeader.MinorGameVersion}";
        PacketType = packetHeader.PacketType;
        SessionId = packetHeader.UniqueSessionId;
        Timestamp = packetHeader.SessionTime;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Number of game version
    /// </summary>
    public int GameVersion { get; }

    /// <summary>
    /// Product version
    /// </summary>
    public string ProductVersion { get; }

    /// <summary>
    /// Type of packet
    /// </summary>
    public PacketTypes PacketType { get; }

    /// <summary>
    /// Id of session
    /// </summary>
    public ulong SessionId { get; }

    /// <summary>
    /// Timestamp of packet
    /// </summary>
    public float Timestamp { get; }

    #endregion // Properties
}