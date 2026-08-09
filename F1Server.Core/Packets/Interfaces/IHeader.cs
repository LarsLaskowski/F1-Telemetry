using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Basic interface for each packet
/// </summary>
public interface IHeader
{
    #region Properties

    /// <summary>
    /// Packet format (2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026)
    /// </summary>
    ushort GameVersion { get; }

    /// <summary>
    /// Game year - last two digits
    /// </summary>
    ushort GameYear { get; }

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
    /// Identifier for the packet type
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
    /// Overall identifier for the frame the data was received (doesn't go back after flashbacks)
    /// </summary>
    uint OverallFrameIdentifier { get; }

    /// <summary>
    /// Index of player's car
    /// </summary>
    ushort PlayerCarIndex { get; }

    /// <summary>
    /// Index of secondary player's car (splitscreen) - 255 if no second car
    /// </summary>
    ushort PlayerCarIndexSecondary { get; }

    #endregion // Properties
}