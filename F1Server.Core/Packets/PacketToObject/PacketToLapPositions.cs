using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using F1Server.Core.Data;
using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.PacketToObject;

/// <summary>
/// Class to extract a lap positions object from received packet
/// </summary>
/// <param name="packetHeader">Packet header</param>
internal class PacketToLapPositions(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Get session history data from received packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <returns>Session history data object</returns>
    public object? ExtractLapPositionsPacket(ReadOnlySpan<byte> dataPacket)
    {
        object? lapPositions = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToLapPositions");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            LapPositions? lapPositionsData = GameVersion switch
                                             {
                                                 2025 => new LapPositions(PacketHeader, new LapPositions2025()),
                                                 2026 => new LapPositions(PacketHeader, new LapPositions2026()),
                                                 _ => null
                                             };

            if (lapPositionsData != null
                && HasValidPacketLength(dataPacket.Length, GetExpectedPayloadSize())
                && ExtractLapPositions(ref memRef, HeaderSize, lapPositionsData.PacketData))
            {
                lapPositions = lapPositionsData;
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return lapPositions;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Returns the expected lap positions payload size for the current game version
    /// </summary>
    /// <returns>Expected payload size in bytes without the packet header</returns>
    private int GetExpectedPayloadSize()
    {
        return GameVersion switch
               {
                   2025 => ConstData.F12025LapPositionSize,
                   2026 => ConstData.F12026LapPositionSize,
                   _ => 0
               };
    }

    /// <summary>
    /// Extract session history
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="lapPositions">Base lap positions</param>
    /// <returns>Status</returns>
    private bool ExtractLapPositions(ref byte dataPacket, int offsetToStart, ILapPositionsBase? lapPositions)
    {
        var retValue = false;

        var actOffset = offsetToStart;

        if (lapPositions is LapPositions2025 lapPositions2025)
        {
            retValue = ExtractLapPositions2025(ref dataPacket, ref actOffset, lapPositions2025);
        }

        if (lapPositions is LapPositions2026 lapPositions2026)
        {
            retValue = ExtractLapPositions2026(ref dataPacket, ref actOffset, lapPositions2026);
        }

        return retValue;
    }

    /// <summary>
    /// Extract lap positions for F1 2025
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="lapPositions2025">Lap positions for F1 2025</param>
    /// <returns>Status</returns>
    private bool ExtractLapPositions2025(ref byte dataPacket, ref int actOffset, LapPositions2025 lapPositions2025)
    {
        var retValue = false;

        try
        {
            lapPositions2025.NumberOfLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapPositions2025.LapStartIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            // Clamp the packet-provided lap count to the fixed packet layout so manipulated values cannot cause reads past the packet
            var lapCount = Math.Min(lapPositions2025.NumberOfLaps, ConstData.F12025MaxLapPositions);

            for (var lap = 0; lap < lapCount; lap++)
            {
                for (var car = 0; car < ConstData.F12025MaxCars; ++car)
                {
                    var carPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    lapPositions2025.CarPositionOnLaps[lap, car] = carPosition;

                    actOffset += ConstData.TypeUInt8;
                }
            }

            retValue = true;
        }
        catch (Exception ex)
        {
            LastError = ex.ToString();
        }

        return retValue;
    }

    /// <summary>
    /// Extract lap positions for F1 2026
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="lapPositions2026">Lap positions for F1 2026</param>
    /// <returns>Status</returns>
    private bool ExtractLapPositions2026(ref byte dataPacket, ref int actOffset, LapPositions2026 lapPositions2026)
    {
        var retValue = false;

        try
        {
            lapPositions2026.NumberOfLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapPositions2026.LapStartIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            // Clamp the packet-provided lap count to the fixed packet layout so manipulated values cannot cause reads past the packet
            var lapCount = Math.Min(lapPositions2026.NumberOfLaps, ConstData.F12026MaxLapPositions);

            for (var lap = 0; lap < lapCount; lap++)
            {
                for (var car = 0; car < ConstData.F12026MaxCars; ++car)
                {
                    var carPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    lapPositions2026.CarPositionOnLaps[lap, car] = carPosition;

                    actOffset += ConstData.TypeUInt8;
                }
            }
        }
        catch (Exception ex)
        {
            LastError = ex.ToString();
        }

        return retValue;
    }

    #endregion // Private methods
}