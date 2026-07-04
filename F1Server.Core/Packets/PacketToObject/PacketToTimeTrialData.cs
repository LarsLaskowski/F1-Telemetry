using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.PacketToObject;

/// <summary>
/// Class to extract a time trial object from received packet
/// </summary>
/// <param name="packetHeader">Packet header</param>
internal class PacketToTimeTrialData(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Get time trial data from received packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <returns>Session history data object</returns>
    public object? ExtractTimeTrialDataPacket(ReadOnlySpan<byte> dataPacket)
    {
        object? timeTrial = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToTimeTrial");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            TimeTrialData? timeTrialData = GameVersion switch
                                           {
                                               2024 => new TimeTrialData(PacketHeader, new TimeTrialData2024()),
                                               2025 => new TimeTrialData(PacketHeader, new TimeTrialData2025()),
                                               2026 => new TimeTrialData(PacketHeader, new TimeTrialData2026()),
                                               _ => null
                                           };

            if (timeTrialData != null
                && HasValidPacketLength(dataPacket.Length, GetExpectedPayloadSize())
                && ExtractTimeTrialData(ref memRef, HeaderSize, timeTrialData.PacketData))
            {
                timeTrial = timeTrialData;
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return timeTrial;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Returns the expected time trial payload size for the current game version
    /// </summary>
    /// <returns>Expected payload size in bytes without the packet header</returns>
    private int GetExpectedPayloadSize()
    {
        return GameVersion switch
               {
                   2024 => ConstData.F12024TimeTrialSize,
                   2025 => ConstData.F12025TimeTrialSize,
                   2026 => ConstData.F12026TimeTrialSize,
                   _ => 0
               };
    }

    /// <summary>
    /// Extract time trial data
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="timeTrialData">Base time trial data</param>
    /// <returns>Status</returns>
    private bool ExtractTimeTrialData(ref byte dataPacket, int offsetToStart, ITimeTrialDataBase? timeTrialData)
    {
        var retValue = false;

        if (timeTrialData is TimeTrialData2024 timeTrialData2024)
        {
            // First: player session best data set
            var actOffset = ExtractTimeTrialData(ref dataPacket, offsetToStart, timeTrialData2024.PlayerSessionBestDataSet);

            // Second: personal best data set
            actOffset = ExtractTimeTrialData(ref dataPacket, actOffset, timeTrialData2024.PersonalBestDataSet);

            // Third: rival data set
            ExtractTimeTrialData(ref dataPacket, actOffset, timeTrialData2024.RivalDataSet);

            retValue = true;
        }

        if (timeTrialData is TimeTrialData2025 timeTrialData2025)
        {
            // First: player session best data set
            var actOffset = ExtractTimeTrialData(ref dataPacket, offsetToStart, timeTrialData2025.PlayerSessionBestDataSet);

            // Second: personal best data set
            actOffset = ExtractTimeTrialData(ref dataPacket, actOffset, timeTrialData2025.PersonalBestDataSet);

            // Third: rival data set
            ExtractTimeTrialData(ref dataPacket, actOffset, timeTrialData2025.RivalDataSet);

            retValue = true;
        }

        if (timeTrialData is TimeTrialData2026 timeTrialData2026)
        {
            // First: player session best data set
            var actOffset = ExtractTimeTrialData(ref dataPacket, offsetToStart, timeTrialData2026.PlayerSessionBestDataSet);

            // Second: personal best data set
            actOffset = ExtractTimeTrialData(ref dataPacket, actOffset, timeTrialData2026.PersonalBestDataSet);

            // Third: rival data set
            ExtractTimeTrialData(ref dataPacket, actOffset, timeTrialData2026.RivalDataSet);

            retValue = true;
        }

        return retValue;
    }

    /// <summary>
    /// Extract time trial base data
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="timeTrialData">Base session history data</param>
    /// <returns>Offset</returns>
    private int ExtractTimeTrialData(ref byte dataPacket, int offsetToStart, ITimeTrialDataSetBase? timeTrialData)
    {
        var actOffset = offsetToStart;

        if (timeTrialData != null)
        {
            try
            {
                timeTrialData.CarIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                timeTrialData.TeamId = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                timeTrialData.LapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt32;

                timeTrialData.Sector1Time = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt32;

                timeTrialData.Sector2Time = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt32;

                timeTrialData.Sector3Time = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt32;

                var enumValue = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                timeTrialData.TractionControl = (TractionControl)Enum.ToObject(typeof(TractionControl), enumValue);

                actOffset += ConstData.TypeUInt8;

                enumValue = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                timeTrialData.GearboxAssist = (GearboxAssist)Enum.ToObject(typeof(GearboxAssist), enumValue);

                actOffset += ConstData.TypeUInt8;

                timeTrialData.AntiLockBrakes = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                var perfValue = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                timeTrialData.IsRealisticCarPerformance = perfValue == false;

                actOffset += ConstData.TypeUInt8;

                timeTrialData.IsCustomSetup = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                timeTrialData.IsValid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
            }
        }

        return actOffset;
    }

    #endregion // Private methods
}