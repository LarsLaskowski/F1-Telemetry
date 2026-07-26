using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Data;

namespace F1Server.Core.Data;

/// <summary>
/// Data class representing one received packet
/// </summary>
public sealed class ReceivedPacketData
{
    #region Fields

    /// <summary>
    /// Packet counter
    /// </summary>
    private static ulong _packetCounter;

    /// <summary>
    /// Array with received packet data
    /// </summary>
    private byte[] _rawData;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public ReceivedPacketData()
    {
        PacketNumber = Interlocked.Increment(ref _packetCounter);
        Timestamp = DateTime.UtcNow;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Complete raw packet data
    /// </summary>
    public ReadOnlySpan<byte> PacketRawData => _rawData.AsSpan(0, PacketLength);

    /// <summary>
    /// Packet
    /// </summary>
    public ulong PacketNumber { get; }

    /// <summary>
    /// Timestamp of received packet
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Length of received packet
    /// </summary>
    public int PacketLength { get; private set; }

    /// <summary>
    /// Header data from raw packet
    /// </summary>
    public PacketHeader? PacketHeader { get; private set; }

    /// <summary>
    /// Bounded classification of why the packet header was rejected, safe to use as a metric dimension
    /// </summary>
    public HeaderRejectionCode HeaderRejectionCode { get; private set; }

    /// <summary>
    /// Exception caught while parsing the packet header, set only when <see cref="HeaderRejectionCode"/> is <see cref="Enumerations.HeaderRejectionCode.ParseException"/>
    /// </summary>
    public Exception? HeaderParseException { get; private set; }

    /// <summary>
    /// Game version reported by the packet, set only when <see cref="HeaderRejectionCode"/> is <see cref="Enumerations.HeaderRejectionCode.Undersized2023Header"/>
    /// </summary>
    public ushort ReportedGameVersion { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Set incoming packet raw data
    /// </summary>
    /// <param name="rawData">Received bytes from game</param>
    public void SetRawData(byte[] rawData)
    {
        PacketHeader = null;
        HeaderRejectionCode = HeaderRejectionCode.None;
        HeaderParseException = null;
        ReportedGameVersion = 0;

        _rawData = new byte[rawData.Length];

        rawData.CopyTo(_rawData, 0);

        PacketLength = rawData.Length;

        AnalyzePacketHeader(PacketRawData);
    }

    /// <summary>
    /// Analyze received packet and return only the header
    /// </summary>
    /// <param name="dataPacket">Complete received packet content</param>
    private void AnalyzePacketHeader(ReadOnlySpan<byte> dataPacket)
    {
        if (dataPacket.Length >= ConstData.F12019HeaderSize)
        {
            ParseHeader(dataPacket);
        }
        else
        {
            HeaderRejectionCode = HeaderRejectionCode.PacketTooShort;
        }
    }

    /// <summary>
    /// Parses the packet header fields, containing any parsing exception so a malformed packet cannot crash the receiver
    /// </summary>
    /// <param name="dataPacket">Complete received packet content, at least <see cref="ConstData.F12019HeaderSize"/> bytes long</param>
    [ExcludeFromCodeCoverage(Justification = "The try/catch wrapper itself cannot be exercised: every offset ParseHeaderFields reads is validated against the packet length by the guards in AnalyzePacketHeader before this method is called, and neither Enum.ToObject nor the object initializer can throw, so no packet observed in practice reaches this catch. Unsafe.ReadUnaligned itself performs no bounds checking; safety here comes entirely from those length guards, not from the read call")]
    private void ParseHeader(ReadOnlySpan<byte> dataPacket)
    {
        try
        {
            ParseHeaderFields(dataPacket);
        }
        catch (Exception ex)
        {
            PacketHeader = null;
            HeaderRejectionCode = HeaderRejectionCode.ParseException;
            HeaderParseException = ex;
        }
    }

    /// <summary>
    /// Reads the packet header fields from the raw packet bytes
    /// </summary>
    /// <param name="dataPacket">Complete received packet content, at least <see cref="ConstData.F12019HeaderSize"/> bytes long</param>
    private void ParseHeaderFields(ReadOnlySpan<byte> dataPacket)
    {
        ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

        var contentOffset = 0;

        var gameVersion = Unsafe.ReadUnaligned<ushort>(ref memRef);

        contentOffset += ConstData.TypeUInt16;

        // From 2023 the header carries additional fields (GameYear, OverallFrameIdentifier)
        // that are read further below without their own bounds check; reject undersized
        // packets here so those reads cannot go past the end of the array.
        if (gameVersion >= 2023 && dataPacket.Length < ConstData.F12023HeaderSize)
        {
            HeaderRejectionCode = HeaderRejectionCode.Undersized2023Header;
            ReportedGameVersion = gameVersion;

            return;
        }

        PacketHeader = new()
                       {
                           // Format - uint16
                           GameVersion = gameVersion
                       };

        // Game year (since 2023) - uint8
        if (PacketHeader.GameVersion >= 2023)
        {
            PacketHeader.GameYear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, contentOffset));

            contentOffset += ConstData.TypeUInt8;
        }

        // Major version - uint8
        PacketHeader.MajorGameVersion = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, contentOffset));

        contentOffset += ConstData.TypeUInt8;

        // Minor version - uint8
        PacketHeader.MinorGameVersion = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, contentOffset));

        contentOffset += ConstData.TypeUInt8;

        // Packet version - uint8
        PacketHeader.PacketVersion = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, contentOffset));

        contentOffset += ConstData.TypeUInt8;

        // Packet type (id) - uint8
        var packetType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, contentOffset));

        PacketHeader.PacketType = (PacketTypes)Enum.ToObject(typeof(PacketTypes), packetType + 1);

        contentOffset += ConstData.TypeUInt8;

        // Session id - uint64
        PacketHeader.UniqueSessionId = Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref memRef, contentOffset));

        contentOffset += ConstData.TypeUInt64;

        // Session time - float
        PacketHeader.SessionTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref memRef, contentOffset));
        PacketHeader.SessionTimeNum = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref memRef, contentOffset));

        contentOffset += ConstData.TypeFloat;

        // Frame identifier - uint32
        PacketHeader.FrameIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref memRef, contentOffset));

        contentOffset += ConstData.TypeUInt32;

        // Overall frame identifier (doesn't go back after flashbacks)
        if (PacketHeader.GameVersion >= 2023)
        {
            PacketHeader.OverallFrameIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref memRef, contentOffset));

            contentOffset += ConstData.TypeUInt32;
        }

        // Car index - uint8
        PacketHeader.PlayerCarIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, contentOffset));

        if (PacketHeader.GameVersion >= 2020 && dataPacket.Length >= ConstData.F12020HeaderSize)
        {
            contentOffset += ConstData.TypeUInt8;

            // Secondary car index - uint8
            PacketHeader.PlayerCarIndexSecondary = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref memRef, contentOffset));
        }
        else
        {
            PacketHeader.PlayerCarIndexSecondary = 255;
        }
    }

    #endregion // Methods
}