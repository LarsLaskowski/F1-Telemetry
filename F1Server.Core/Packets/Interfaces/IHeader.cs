using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Basic interface for each packet
/// </summary>
public interface IHeader
{
    #region Properties

    /// <summary>
    /// Packet format (2020, 2021)
    /// </summary>
    ushort GameVersion { get; }

    /// <summary>
    /// Major version
    /// </summary>
    byte MajorGameVersion { get; }

    /// <summary>
    /// Minor version
    /// </summary>
    byte MinorGameVersion { get; }

    /// <summary>
    /// Packet version
    /// </summary>
    byte PacketVersion { get; }

    /// <summary>
    /// Identifiert for the packet type
    /// </summary>
    PacketTypes PacketType { get; }

    /// <summary>
    /// Unique id of session
    /// </summary>
    ulong UniqueSessionId { get; }

    /// <summary>
    /// Timestamp of session
    /// </summary>
    float SessionTime { get; }

    /// <summary>
    /// ID for the frame the data is retrieved on
    /// </summary>
    uint FrameIdentifier { get; }

    /// <summary>
    /// Index of player's car
    /// </summary>
    ushort PlayerCarIndex { get; }

    #endregion // Properties
}