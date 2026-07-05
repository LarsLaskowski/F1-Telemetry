using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class packet header
/// </summary>
public class PacketHeader : IHeader, IHeaderExtended, IHeaderExtended2
{
    #region IHeader

    /// <summary>
    /// Packet format (2019, 2020, 2021, 2022, 2023, ...)
    /// </summary>
    public ushort GameVersion { get; set; }

    /// <summary>
    /// Major version
    /// </summary>
    public byte MajorGameVersion { get; set; }

    /// <summary>
    /// Minor version
    /// </summary>
    public byte MinorGameVersion { get; set; }

    /// <summary>
    /// Packet version
    /// </summary>
    public byte PacketVersion { get; set; }

    /// <summary>
    /// Packet type
    /// </summary>
    public PacketTypes PacketType { get; set; }

    /// <summary>
    /// Unique id of session
    /// </summary>
    public ulong UniqueSessionId { get; set; }

    /// <summary>
    /// Timestamp of session
    /// </summary>
    public float SessionTime { get; set; }

    /// <summary>
    /// Numeric session timestamp
    /// </summary>
    public uint SessionTimeNum { get; set; }

    /// <summary>
    /// ID for the frame the data is retrieved on
    /// </summary>
    public uint FrameIdentifier { get; set; }

    /// <summary>
    /// Index of player's car
    /// </summary>
    public ushort PlayerCarIndex { get; set; }

    #endregion // IHeader

    #region IHeaderExtended

    /// <summary>
    /// Index of secondary player's car (splitscreen) - 255 if no second car
    /// </summary>
    public ushort PlayerCarIndexSecondary { get; set; }

    #endregion // IHeaderExtended

    #region IHeaderExtended2

    /// <summary>
    /// Game year - last two digits
    /// </summary>
    public ushort GameYear { get; set; }

    /// <summary>
    /// Overall identifier for the frame the data was received (doesn't go back after flashbacks)
    /// </summary>
    public uint OverallFrameIdentifier { get; set; }

    #endregion // IHeaderExtended2
}