using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class packet header
/// </summary>
public class PacketHeader : IHeader, IHeaderExtended, IHeaderExtended2
{
    #region Properties

    /// <summary>
    /// Raw bit pattern of <see cref="SessionTime"/>, read as an unsigned integer from the same
    /// packet offset. Used as an ordering and comparison key when packets of the same session
    /// are sequenced, because the raw value is directly comparable without floating point
    /// tolerance. Not part of any header interface - an implementation-only addition.
    /// </summary>
    public uint SessionTimeNum { get; set; }

    #endregion // Properties

    #region IHeader

    /// <inheritdoc/>
    public ushort GameVersion { get; set; }

    /// <inheritdoc/>
    public byte MajorGameVersion { get; set; }

    /// <inheritdoc/>
    public byte MinorGameVersion { get; set; }

    /// <inheritdoc/>
    public byte PacketVersion { get; set; }

    /// <inheritdoc/>
    public PacketTypes PacketType { get; set; }

    /// <inheritdoc/>
    public ulong UniqueSessionId { get; set; }

    /// <inheritdoc/>
    public float SessionTime { get; set; }

    /// <inheritdoc/>
    public uint FrameIdentifier { get; set; }

    /// <inheritdoc/>
    public ushort PlayerCarIndex { get; set; }

    #endregion // IHeader

    #region IHeaderExtended

    /// <inheritdoc/>
    public ushort PlayerCarIndexSecondary { get; set; }

    #endregion // IHeaderExtended

    #region IHeaderExtended2

    /// <inheritdoc/>
    public ushort GameYear { get; set; }

    /// <inheritdoc/>
    public uint OverallFrameIdentifier { get; set; }

    #endregion // IHeaderExtended2
}