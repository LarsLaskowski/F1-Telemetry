using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class packet header
/// </summary>
public class PacketHeader : IHeader
{
    #region Properties

    /// <summary>
    /// Raw bit pattern of <see cref="SessionTime"/>, read as an unsigned integer from the same
    /// packet offset. Used as an ordering and comparison key when packets of the same session
    /// are sequenced, because the raw value is directly comparable without floating point
    /// tolerance. Not part of any header interface - an implementation-only addition.
    /// </summary>
    public uint SessionTimeNum { get; set; }

    /// <summary>
    /// Size of the packet header in bytes, derived from <see cref="GameVersion"/>. Zero for an
    /// unrecognized game version. Not part of any header interface - an implementation-only addition.
    /// </summary>
    public int HeaderSize => GameVersion switch
                             {
                                 2019 => ConstData.F12019HeaderSize,
                                 2020 => ConstData.F12020HeaderSize,
                                 2021 => ConstData.F12020HeaderSize,
                                 2022 => ConstData.F12020HeaderSize,
                                 2023 => ConstData.F12023HeaderSize,
                                 2024 => ConstData.F12024HeaderSize,
                                 2025 => ConstData.F12025HeaderSize,
                                 2026 => ConstData.F12026HeaderSize,
                                 _ => 0
                             };

    #endregion // Properties

    #region IHeader

    /// <inheritdoc/>
    public ushort GameVersion { get; set; }

    /// <inheritdoc/>
    public ushort GameYear { get; set; }

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
    public uint OverallFrameIdentifier { get; set; }

    /// <inheritdoc/>
    public ushort PlayerCarIndex { get; set; }

    /// <inheritdoc/>
    public ushort PlayerCarIndexSecondary { get; set; }

    #endregion // IHeader
}