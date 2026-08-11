using System.Diagnostics.CodeAnalysis;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.Packets.Data;

namespace F1Server.Core.Packets.PacketToObject;

/// <summary>
/// Base class for bytes to object transformations. An instance is reused for every packet of its type,
/// the packet header of the current transformation is provided by <see cref="Reset"/>. Instances are
/// therefore stateful and must only be used from a single thread at a time
/// </summary>
internal abstract class PacketToXBase
{
    #region Properties

    /// <summary>
    /// Returns the current game version
    /// </summary>
    public int GameVersion => PacketHeader?.GameVersion ?? 0;

    /// <summary>
    /// Size of game version dependent packet header size
    /// </summary>
    public int HeaderSize => GetHeaderSize();

    /// <summary>
    /// Last error of the transformation, <see cref="string.Empty"/> when the transformation succeeded
    /// </summary>
    public string LastError { get; internal set; } = string.Empty;

    /// <summary>
    /// Header of the packet that is currently transformed, set by <see cref="Reset"/> before every transformation
    /// </summary>
    protected PacketHeader PacketHeader { get; private set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Adjust the session type if neccessary
    /// </summary>
    /// <param name="sessionType">Session type value from game</param>
    /// <returns>Adjusted session type, falls back to <see cref="SessionType.Unknown"/> for values outside the known 2024+ range</returns>
    public ushort AdjustSessionType(ushort sessionType)
    {
        // Race3 is new in 2021 with number 12, TimeTrial is now 13
        if (GameVersion < 2021 && sessionType == 12)
        {
            sessionType++;
        }

        if (GameVersion >= 2024 && sessionType >= 10)
        {
            // Adjust new session types
            sessionType = sessionType switch
                          {
                              10 => 14,
                              11 => 15,
                              12 => 16,
                              13 => 17,
                              14 => 18,
                              15 => 10,
                              16 => 11,
                              17 => 12,
                              18 => 13,
                              _ => (ushort)SessionType.Unknown
                          };
        }

        return sessionType;
    }

    /// <summary>
    /// Throws an exception for unsupported game versions
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the current game version is not supported</exception>
    [DoesNotReturn]
    protected static void ThrowInvalidGameVersion()
    {
        throw new InvalidOperationException("Invalid game version!");
    }

    /// <summary>
    /// Prepares the reused transformation instance for the next packet by taking over its header
    /// and clearing the error of the previous transformation
    /// </summary>
    /// <param name="packetHeader">Header of the packet that is transformed next</param>
    protected void Reset(PacketHeader packetHeader)
    {
        PacketHeader = packetHeader;

        LastError = string.Empty;
    }

    /// <summary>
    /// Validates the actual packet length against the expected packet size (header plus payload)
    /// so that no extraction reads past the end of truncated or manipulated packets
    /// </summary>
    /// <param name="packetLength">Length of the received packet</param>
    /// <param name="expectedPayloadSize">Expected payload size in bytes without the packet header</param>
    /// <returns>True when the packet contains at least the expected number of bytes, otherwise false</returns>
    protected bool HasValidPacketLength(int packetLength, int expectedPayloadSize)
    {
        var isValid = packetLength >= HeaderSize + expectedPayloadSize;

        if (isValid == false)
        {
            LastError = $"Packet too short: received {packetLength} bytes, expected at least {HeaderSize + expectedPayloadSize} bytes";
        }

        return isValid;
    }

    /// <summary>
    /// Returns packet header size game version dependent
    /// </summary>
    /// <returns>Header size</returns>
    private int GetHeaderSize()
    {
        return GameVersion switch
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
    }

    #endregion // Methods
}