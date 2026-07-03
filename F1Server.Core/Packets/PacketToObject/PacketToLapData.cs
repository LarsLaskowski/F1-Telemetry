using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;
using F1Server.Core.Utils;

namespace F1Server.Core.Packets.PacketToObject;

/// <summary>
/// Class to extract a lap data object from received packet
/// </summary>
/// <param name="packetHeader">Header of packet</param>
internal class PacketToLapData(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Extract lap data depending on actual game version of packet
    /// </summary>
    /// <param name="dataPacket">Received packet data</param>
    /// <returns>Lap information</returns>
    public object? ExtractLapData(ReadOnlySpan<byte> dataPacket)
    {
        object? lapDataObject = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToLapData");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            PacketDataBase<ILapDataComplete>? packetDataBase = GameVersion switch
                                                               {
                                                                   2019 => new LapData(PacketHeader, new LapDataComplete2019()),
                                                                   2020 => new LapData(PacketHeader, new LapDataComplete2020()),
                                                                   2021 => new LapData(PacketHeader, new LapDataComplete2021()),
                                                                   2022 => new LapData(PacketHeader, new LapDataComplete2022()),
                                                                   2023 => new LapData(PacketHeader, new LapDataComplete2023()),
                                                                   2024 => new LapData(PacketHeader, new LapDataComplete2024()),
                                                                   2025 => new LapData(PacketHeader, new LapDataComplete2025()),
                                                                   2026 => new LapData(PacketHeader, new LapDataComplete2026()),
                                                                   _ => null
                                                               };

            if (packetDataBase != null
                && ExtractCompleteLapData(ref memRef, HeaderSize, dataPacket.Length, packetDataBase.PacketData))
            {
                lapDataObject = packetDataBase;
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return lapDataObject;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Extract all lap data from received data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="lapDataComplete">Data object to store into</param>
    /// <returns>Status</returns>
    private bool ExtractCompleteLapData(ref byte dataPacket, int offsetToStart, int packetLength, ILapDataComplete? lapDataComplete)
    {
        var retValue = false;

        if (offsetToStart > 0 && lapDataComplete?.LapData != null)
        {
            try
            {
                int actOffset = offsetToStart;

                for (int carIndex = 0; carIndex < lapDataComplete.LapData.Length; carIndex++)
                {
                    ExtractLapDataByGameVersion(ref dataPacket, packetLength, lapDataComplete, ref actOffset, carIndex);
                }

                ExtractAdditionalData(ref dataPacket, packetLength, lapDataComplete, actOffset);

                retValue = true;
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
            }
        }

        return retValue;
    }

    /// <summary>
    /// Additional data extraction
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="lapDataComplete">Complete lap data interface</param>
    /// <param name="actOffset">Current offset</param>
    private void ExtractAdditionalData(ref byte dataPacket, int packetLength, ILapDataComplete? lapDataComplete, int actOffset)
    {
        // The trailing time trial car indexes (2 x uint8) must fit into the received packet
        if (packetLength < actOffset + (2 * ConstData.TypeUInt8))
        {
            return;
        }

        if (GameVersion == 2022 && lapDataComplete is ILapDataComplete2022 lapData2022)
        {
            lapData2022.TimeTrialPersonalBestCarIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData2022.TimeTrialRivalCarIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
        }

        if (GameVersion >= 2023 && lapDataComplete is ILapDataComplete2023 lapData)
        {
            lapData.TimeTrialPersonalBestCarIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimeTrialRivalCarIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
        }
    }

    /// <summary>
    /// Extracts lap data for a specific car based on the game version and updates the provided offset
    /// </summary>
    /// <param name="dataPacket">A reference to the raw data packet containing lap data</param>
    /// <param name="packetLength">The total length of the data packet</param>
    /// <param name="lapDataComplete">An object that stores the extracted lap data for all cars</param>
    /// <param name="actOffset">A reference to the current offset within the data packet. This will be updated after extracting the lap data</param>
    /// <param name="carIndex">The index of the car for which lap data is being extracted</param>
    private void ExtractLapDataByGameVersion(ref byte dataPacket, int packetLength, ILapDataComplete lapDataComplete, ref int actOffset, int carIndex)
    {
        if (GameVersion == 2019)
        {
            lapDataComplete.LapData[carIndex] = ExtractSingleCarLapData2019(ref dataPacket, actOffset, packetLength);

            actOffset += ConstData.F12019LapSize;
        }
        else if (GameVersion == 2020)
        {
            lapDataComplete.LapData[carIndex] = ExtractSingleCarLapData2020(ref dataPacket, actOffset, packetLength);

            actOffset += ConstData.F12020LapSize;
        }
        else if (GameVersion == 2021)
        {
            lapDataComplete.LapData[carIndex] = ExtractSingleCarLapData2021(ref dataPacket, actOffset, packetLength);

            actOffset += ConstData.F12021LapSize;
        }
        else if (GameVersion == 2022)
        {
            lapDataComplete.LapData[carIndex] = ExtractSingleCarLapData2022(ref dataPacket, actOffset, packetLength);

            actOffset += ConstData.F12022LapSize;
        }
        else if (GameVersion == 2023)
        {
            lapDataComplete.LapData[carIndex] = ExtractSingleCarLapData2023(ref dataPacket, actOffset, packetLength);

            actOffset += ConstData.F12023LapSize;
        }
        else if (GameVersion == 2024)
        {
            lapDataComplete.LapData[carIndex] = ExtractSingleCarLapData2024(ref dataPacket, actOffset, packetLength);

            actOffset += ConstData.F12024LapSize;
        }
        else if (GameVersion == 2025)
        {
            lapDataComplete.LapData[carIndex] = ExtractSingleCarLapData2025(ref dataPacket, actOffset, packetLength);

            actOffset += ConstData.F12025LapSize;
        }
        else if (GameVersion == 2026)
        {
            lapDataComplete.LapData[carIndex] = ExtractSingleCarLapData2026(ref dataPacket, actOffset, packetLength);

            actOffset += ConstData.F12026LapSize;
        }
        else
        {
            ThrowInvalidGameVersion();
        }
    }

    /// <summary>
    /// Extraction of one lap data for one car
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Lap data</returns>
    private LapData2019 ExtractSingleCarLapData2019(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var lapData = new LapData2019();

        if (packetLength >= offsetToStart + ConstData.F12019LapSize)
        {
            var actOffset = offsetToStart;

            lapData.LastLapTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CurrentLapTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.BestLapTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.Sector1Time = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.Sector2Time = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.LapDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.TotalDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SafetyCarDelta = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CarPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CurrentLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var pitStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentPitStatus = (PitStatus)Enum.ToObject(typeof(PitStatus), pitStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var actSector = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentSector = (Sector)Enum.ToObject(typeof(Sector), actSector + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.IsCurrentLapInvalid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimePenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var driverStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentDriverStatus = (DriverStatus)Enum.ToObject(typeof(DriverStatus), driverStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentResultStatus = ResultStatusMapper.MapResultStatus2019(resultStatus);
        }

        return lapData;
    }

    /// <summary>
    /// Extraction of one lap data for one car
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Lap data</returns>
    private LapData2020 ExtractSingleCarLapData2020(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var lapData = new LapData2020();

        if (packetLength >= offsetToStart + ConstData.F12020LapSize)
        {
            var actOffset = offsetToStart;

            lapData.LastLapTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CurrentLapTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.BestLapTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.BestLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.BestLapSector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.BestLapSector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.BestLapSector3Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.BestOverallSector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.BestOverallSector1LapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.BestOverallSector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.BestOverallSector2LapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.BestOverallSector3Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.BestOverallSector3LapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.LapDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.TotalDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SafetyCarDelta = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CarPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CurrentLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var pitStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentPitStatus = (PitStatus)Enum.ToObject(typeof(PitStatus), pitStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var actSector = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentSector = (Sector)Enum.ToObject(typeof(Sector), actSector + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.IsCurrentLapInvalid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimePenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var driverStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentDriverStatus = (DriverStatus)Enum.ToObject(typeof(DriverStatus), driverStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentResultStatus = ResultStatusMapper.MapResultStatus2020(resultStatus);
        }

        return lapData;
    }

    /// <summary>
    /// Extraction of one lap data for one car
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Lap data</returns>
    private LapData2021 ExtractSingleCarLapData2021(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var lapData = new LapData2021();

        if (packetLength >= offsetToStart + ConstData.F12021LapSize)
        {
            var actOffset = offsetToStart;

            lapData.LastLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.CurrentLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.LapDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.TotalDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SafetyCarDelta = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CarPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CurrentLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var pitStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentPitStatus = (PitStatus)Enum.ToObject(typeof(PitStatus), pitStatus + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.NumberPitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var actSector = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentSector = (Sector)Enum.ToObject(typeof(Sector), actSector + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.IsCurrentLapInvalid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimePenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Warnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedDriveThroughPens = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedStopAndGoPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var driverStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentDriverStatus = (DriverStatus)Enum.ToObject(typeof(DriverStatus), driverStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentResultStatus = ResultStatusMapper.MapResultStatus2021(resultStatus);

            actOffset += ConstData.TypeUInt8;

            lapData.IsPitLaneTimerActive = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.PitLaneTimeInLane = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopTimer = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopShouldServePenalty = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));
        }

        return lapData;
    }

    /// <summary>
    /// Extraction of one lap data for one car
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Lap data</returns>
    private LapData2022 ExtractSingleCarLapData2022(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var lapData = new LapData2022();

        if (packetLength >= offsetToStart + ConstData.F12022LapSize)
        {
            var actOffset = offsetToStart;

            lapData.LastLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.CurrentLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.LapDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.TotalDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SafetyCarDelta = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CarPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CurrentLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var pitStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentPitStatus = (PitStatus)Enum.ToObject(typeof(PitStatus), pitStatus + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.NumberPitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var actSector = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentSector = (Sector)Enum.ToObject(typeof(Sector), actSector + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.IsCurrentLapInvalid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimePenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Warnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedDriveThroughPens = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedStopAndGoPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var driverStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentDriverStatus = (DriverStatus)Enum.ToObject(typeof(DriverStatus), driverStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentResultStatus = ResultStatusMapper.MapResultStatus2022(resultStatus);

            actOffset += ConstData.TypeUInt8;

            lapData.IsPitLaneTimerActive = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.PitLaneTimeInLane = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopTimer = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopShouldServePenalty = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));
        }

        return lapData;
    }

    /// <summary>
    /// Extraction of one lap data for one car
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Lap data</returns>
    private LapData2023 ExtractSingleCarLapData2023(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var lapData = new LapData2023();

        if (packetLength >= offsetToStart + ConstData.F12023LapSize)
        {
            var actOffset = offsetToStart;

            lapData.LastLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.CurrentLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.DeltaToCarInFront = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.DeltaToRaceLeader = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.LapDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.TotalDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SafetyCarDelta = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CarPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CurrentLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var pitStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentPitStatus = (PitStatus)Enum.ToObject(typeof(PitStatus), pitStatus + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.NumberPitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var actSector = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentSector = (Sector)Enum.ToObject(typeof(Sector), actSector + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.IsCurrentLapInvalid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimePenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Warnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CornerCuttingWarnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedDriveThroughPens = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedStopAndGoPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var driverStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentDriverStatus = (DriverStatus)Enum.ToObject(typeof(DriverStatus), driverStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentResultStatus = ResultStatusMapper.MapResultStatus2023(resultStatus);

            actOffset += ConstData.TypeUInt8;

            lapData.IsPitLaneTimerActive = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.PitLaneTimeInLane = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopTimer = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopShouldServePenalty = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));
        }

        return lapData;
    }

    /// <summary>
    /// Extraction of one lap data for one car
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Lap data</returns>
    private LapData2024 ExtractSingleCarLapData2024(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var lapData = new LapData2024();

        if (packetLength >= offsetToStart + ConstData.F12024LapSize)
        {
            var actOffset = offsetToStart;

            lapData.LastLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.CurrentLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.DeltaToCarInFront = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.DeltaToCarInFrontMinutes = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.DeltaToRaceLeader = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.DeltaToRaceLeaderMinutes = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.LapDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.TotalDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SafetyCarDelta = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CarPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CurrentLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var pitStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentPitStatus = (PitStatus)Enum.ToObject(typeof(PitStatus), pitStatus + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.NumberPitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var actSector = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentSector = (Sector)Enum.ToObject(typeof(Sector), actSector + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.IsCurrentLapInvalid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimePenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Warnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CornerCuttingWarnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedDriveThroughPens = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedStopAndGoPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var driverStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentDriverStatus = (DriverStatus)Enum.ToObject(typeof(DriverStatus), driverStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentResultStatus = ResultStatusMapper.MapResultStatus2023(resultStatus);

            actOffset += ConstData.TypeUInt8;

            lapData.IsPitLaneTimerActive = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.PitLaneTimeInLane = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopTimer = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopShouldServePenalty = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.SpeedTrapFastestSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SpeedTrapFastestLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
        }

        return lapData;
    }

    /// <summary>
    /// Extraction of one lap data for one car
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Lap data</returns>
    private LapData2025 ExtractSingleCarLapData2025(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var lapData = new LapData2025();

        if (packetLength >= offsetToStart + ConstData.F12025LapSize)
        {
            var actOffset = offsetToStart;

            lapData.LastLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.CurrentLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.DeltaToCarInFront = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.DeltaToCarInFrontMinutes = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.DeltaToRaceLeader = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.DeltaToRaceLeaderMinutes = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.LapDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.TotalDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SafetyCarDelta = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CarPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CurrentLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var pitStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentPitStatus = (PitStatus)Enum.ToObject(typeof(PitStatus), pitStatus + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.NumberPitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var actSector = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentSector = (Sector)Enum.ToObject(typeof(Sector), actSector + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.IsCurrentLapInvalid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimePenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Warnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CornerCuttingWarnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedDriveThroughPens = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedStopAndGoPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var driverStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentDriverStatus = (DriverStatus)Enum.ToObject(typeof(DriverStatus), driverStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentResultStatus = ResultStatusMapper.MapResultStatus2023(resultStatus);

            actOffset += ConstData.TypeUInt8;

            lapData.IsPitLaneTimerActive = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.PitLaneTimeInLane = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopTimer = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopShouldServePenalty = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.SpeedTrapFastestSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SpeedTrapFastestLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
        }

        return lapData;
    }

    /// <summary>
    /// Extraction of one lap data for one car (F1 2026 - layout identical to 2025)
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Lap data</returns>
    private LapData2026 ExtractSingleCarLapData2026(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var lapData = new LapData2026();

        if (packetLength >= offsetToStart + ConstData.F12026LapSize)
        {
            var actOffset = offsetToStart;

            lapData.LastLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.CurrentLapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.DeltaToCarInFront = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.DeltaToCarInFrontMinutes = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.DeltaToRaceLeader = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.DeltaToRaceLeaderMinutes = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.LapDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.TotalDistance = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SafetyCarDelta = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.CarPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CurrentLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var pitStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentPitStatus = (PitStatus)Enum.ToObject(typeof(PitStatus), pitStatus + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.NumberPitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var actSector = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentSector = (Sector)Enum.ToObject(typeof(Sector), actSector + 1);

            actOffset += ConstData.TypeUInt8;

            lapData.IsCurrentLapInvalid = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.TimePenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.Warnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.CornerCuttingWarnings = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedDriveThroughPens = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.NumberUnservedStopAndGoPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var driverStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentDriverStatus = (DriverStatus)Enum.ToObject(typeof(DriverStatus), driverStatus + 1);

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            lapData.CurrentResultStatus = ResultStatusMapper.MapResultStatus2023(resultStatus);

            actOffset += ConstData.TypeUInt8;

            lapData.IsPitLaneTimerActive = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.PitLaneTimeInLane = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopTimer = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            lapData.PitStopShouldServePenalty = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            lapData.SpeedTrapFastestSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            lapData.SpeedTrapFastestLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
        }

        return lapData;
    }

    #endregion // Private methods
}