using System.Diagnostics.CodeAnalysis;

using F1Server.Core.Data;
using F1Server.Core.Packets.Data;

namespace F1Server.Core.Packets.PacketToObject;

/// <summary>
/// Base class for bytes to object transformations
/// </summary>
internal abstract class PacketToXBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="packetHeader">Header of packet</param>
    protected PacketToXBase(PacketHeader packetHeader)
    {
        PacketHeader = packetHeader;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Header of packet
    /// </summary>
    public PacketHeader PacketHeader { get; }

    /// <summary>
    /// Returns the current game version
    /// </summary>
    public int GameVersion => PacketHeader?.GameVersion ?? 0;

    /// <summary>
    /// Size of game version dependet packet header size
    /// </summary>
    public int HeaderSize => GetHeaderSize();

    /// <summary>
    /// Last error
    /// </summary>
    public string LastError { get; internal set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Adjust the session type if neccessary
    /// </summary>
    /// <param name="sessionType">Session type value from game</param>
    /// <returns>Adjusted session type</returns>
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
                              _ => throw new InvalidDataException("Unknown session type!")
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