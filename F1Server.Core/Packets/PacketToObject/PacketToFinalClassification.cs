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
/// Class to extract a final classification object from received packet
/// </summary>
/// <param name="packetHeader">Header of packet</param>
internal class PacketToFinalClassification(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Extract final classification data  on actual game version of packet
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <returns>Final classification</returns>
    public object? ExtractFinalClassificationData(ReadOnlySpan<byte> dataPacket)
    {
        object? finalClassObject = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToFinalClassification");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            PacketDataBase<IFinalClassificationData>? packetDataBase = GameVersion switch
                                                                       {
                                                                           2020 => new FinalClassificationData(PacketHeader, new FinalClassificationData2020()),
                                                                           2021 => new FinalClassificationData(PacketHeader, new FinalClassificationData2021()),
                                                                           2022 => new FinalClassificationData(PacketHeader, new FinalClassificationData2022()),
                                                                           2023 => new FinalClassificationData(PacketHeader, new FinalClassificationData2023()),
                                                                           2024 => new FinalClassificationData(PacketHeader, new FinalClassificationData2024()),
                                                                           2025 => new FinalClassificationData(PacketHeader, new FinalClassificationData2025()),
                                                                           2026 => new FinalClassificationData(PacketHeader, new FinalClassificationData2026()),
                                                                           _ => null
                                                                       };

            if (packetDataBase != null
                && ExtractFinalClassificationData(ref memRef, dataPacket.Length, HeaderSize, packetDataBase.PacketData))
            {
                finalClassObject = packetDataBase;
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return finalClassObject;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Extract all final classification data from received data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="finalClassificationData">Data object to store into</param>
    /// <returns>Status</returns>
    private bool ExtractFinalClassificationData(ref byte dataPacket, int packetLength, int offsetToStart, IFinalClassificationData? finalClassificationData)
    {
        var retValue = false;

        if (packetLength > 0 && offsetToStart > 0 && finalClassificationData?.FinalClassifications != null)
        {
            try
            {
                var actOffset = offsetToStart;

                // Number of cars - uint8
                finalClassificationData.NumberOfCars = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                var carDataSize = GetCarDataSize();

                for (var finalClassification = 0; finalClassification < finalClassificationData.FinalClassifications.Length; finalClassification++)
                {
                    finalClassificationData.FinalClassifications[finalClassification] = ExtractSingleCarData(ref dataPacket, actOffset, packetLength);

                    actOffset += carDataSize;
                }

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
    /// Returns the car data stride for the current game version
    /// </summary>
    /// <returns>Size of a single car's final classification data in bytes</returns>
    private int GetCarDataSize()
    {
        return GameVersion switch
               {
                   2020 => ConstData.F12020FinalClassificationCarSize,
                   2021 => ConstData.F12021FinalClassificationCarSize,
                   2022 => ConstData.F12022FinalClassificationCarSize,
                   2023 => ConstData.F12023FinalClassificationCarSize,
                   2024 => ConstData.F12024FinalClassificationCarSize,
                   2025 => ConstData.F12025FinalClassificationCarSize,
                   2026 => ConstData.F12026FinalClassificationCarSize,
                   _ => throw new InvalidOperationException("Invalid game version!")
               };
    }

    /// <summary>
    /// Extracts a single car's final classification data for the current game version
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Single final classification car data</returns>
    private IFinalClassificationCarBase ExtractSingleCarData(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        return GameVersion switch
               {
                   2020 => ExtractFinalClassificationSingleCarData2020(ref dataPacket, offsetToStart, packetLength),
                   2021 => ExtractFinalClassificationSingleCarData2021(ref dataPacket, offsetToStart, packetLength),
                   2022 => ExtractFinalClassificationSingleCarData2022(ref dataPacket, offsetToStart, packetLength),
                   2023 => ExtractFinalClassificationSingleCarData2023(ref dataPacket, offsetToStart, packetLength),
                   2024 => ExtractFinalClassificationSingleCarData2024(ref dataPacket, offsetToStart, packetLength),
                   2025 => ExtractFinalClassificationSingleCarData2025(ref dataPacket, offsetToStart, packetLength),
                   2026 => ExtractFinalClassificationSingleCarData2026(ref dataPacket, offsetToStart, packetLength),
                   _ => throw new InvalidOperationException("Invalid game version!")
               };
    }

    /// <summary>
    /// Extract all data for a single car from final classification data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Single final classification data</returns>
    private FinalClassificationCarData2020 ExtractFinalClassificationSingleCarData2020(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var finalClassData = new FinalClassificationCarData2020();

        if (packetLength >= offsetToStart + ConstData.F12020FinalClassificationCarSize)
        {
            var actOffset = offsetToStart;

            finalClassData.Position = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.LapsCompleted = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.Points = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.PitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultStatus = ResultStatusMapper.MapResultStatus2021(resultStatus);

            actOffset += ConstData.TypeUInt8;

            var bestLapTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.BestLapTimeInMs = (uint)bestLapTime * 1000;

            actOffset += ConstData.TypeFloat;

            finalClassData.TotalRaceTime = Unsafe.ReadUnaligned<double>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeDouble;

            finalClassData.PenaltiesTime = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumTyreStints = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            for (var actualTyreStint = 0; actualTyreStint < 8; actualTyreStint++)
            {
                finalClassData.TyreStintsActual[actualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var visualTyreStint = 0; visualTyreStint < 8; visualTyreStint++)
            {
                finalClassData.TyreStintsVisual[visualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
        }

        return finalClassData;
    }

    /// <summary>
    /// Extract all data for a single car from final classification data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Single final classification data</returns>
    private FinalClassificationCarData2021 ExtractFinalClassificationSingleCarData2021(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var finalClassData = new FinalClassificationCarData2021();

        if (packetLength >= offsetToStart + ConstData.F12020FinalClassificationCarSize)
        {
            var actOffset = offsetToStart;

            finalClassData.Position = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.LapsCompleted = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.Points = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.PitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultStatus = ResultStatusMapper.MapResultStatus2021(resultStatus);

            actOffset += ConstData.TypeUInt8;

            finalClassData.BestLapTimeInMs = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            finalClassData.TotalRaceTime = Unsafe.ReadUnaligned<double>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeDouble;

            finalClassData.PenaltiesTime = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumTyreStints = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            for (var actualTyreStint = 0; actualTyreStint < 8; actualTyreStint++)
            {
                finalClassData.TyreStintsActual[actualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var visualTyreStint = 0; visualTyreStint < 8; visualTyreStint++)
            {
                finalClassData.TyreStintsVisual[visualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
        }

        return finalClassData;
    }

    /// <summary>
    /// Extract all data for a single car from final classification data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Single final classification data</returns>
    private FinalClassificationCarData2022 ExtractFinalClassificationSingleCarData2022(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var finalClassData = new FinalClassificationCarData2022();

        if (packetLength >= offsetToStart + ConstData.F12022FinalClassificationCarSize)
        {
            var actOffset = offsetToStart;

            finalClassData.Position = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.LapsCompleted = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.Points = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.PitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultStatus = ResultStatusMapper.MapResultStatus2022(resultStatus);

            actOffset += ConstData.TypeUInt8;

            finalClassData.BestLapTimeInMs = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            finalClassData.TotalRaceTime = Unsafe.ReadUnaligned<double>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeDouble;

            finalClassData.PenaltiesTime = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumTyreStints = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            for (var actualTyreStint = 0; actualTyreStint < 8; actualTyreStint++)
            {
                finalClassData.TyreStintsActual[actualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var visualTyreStint = 0; visualTyreStint < 8; visualTyreStint++)
            {
                finalClassData.TyreStintsVisual[visualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var endLap = 0; endLap < 8; endLap++)
            {
                finalClassData.TyreStintsEndLaps[endLap] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
        }

        return finalClassData;
    }

    /// <summary>
    /// Extract all data for a single car from final classification data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Single final classification data</returns>
    private FinalClassificationCarData2023 ExtractFinalClassificationSingleCarData2023(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var finalClassData = new FinalClassificationCarData2023();

        if (packetLength >= offsetToStart + ConstData.F12023FinalClassificationCarSize)
        {
            var actOffset = offsetToStart;

            finalClassData.Position = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.LapsCompleted = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.Points = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.PitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultStatus = ResultStatusMapper.MapResultStatus2023(resultStatus);

            actOffset += ConstData.TypeUInt8;

            finalClassData.BestLapTimeInMs = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            finalClassData.TotalRaceTime = Unsafe.ReadUnaligned<double>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeDouble;

            finalClassData.PenaltiesTime = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumTyreStints = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            for (var actualTyreStint = 0; actualTyreStint < 8; actualTyreStint++)
            {
                finalClassData.TyreStintsActual[actualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var visualTyreStint = 0; visualTyreStint < 8; visualTyreStint++)
            {
                finalClassData.TyreStintsVisual[visualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var endLap = 0; endLap < 8; endLap++)
            {
                finalClassData.TyreStintsEndLaps[endLap] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
        }

        return finalClassData;
    }

    /// <summary>
    /// Extract all data for a single car from final classification data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Single final classification data</returns>
    private FinalClassificationCarData2024 ExtractFinalClassificationSingleCarData2024(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var finalClassData = new FinalClassificationCarData2024();

        if (packetLength >= offsetToStart + ConstData.F12024FinalClassificationCarSize)
        {
            var actOffset = offsetToStart;

            finalClassData.Position = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.LapsCompleted = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.Points = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.PitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultStatus = ResultStatusMapper.MapResultStatus2024(resultStatus);

            actOffset += ConstData.TypeUInt8;

            finalClassData.BestLapTimeInMs = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            finalClassData.TotalRaceTime = Unsafe.ReadUnaligned<double>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeDouble;

            finalClassData.PenaltiesTime = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumTyreStints = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            for (var actualTyreStint = 0; actualTyreStint < 8; actualTyreStint++)
            {
                finalClassData.TyreStintsActual[actualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var visualTyreStint = 0; visualTyreStint < 8; visualTyreStint++)
            {
                finalClassData.TyreStintsVisual[visualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var endLap = 0; endLap < 8; endLap++)
            {
                finalClassData.TyreStintsEndLaps[endLap] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
        }

        return finalClassData;
    }

    /// <summary>
    /// Extract all data for a single car from final classification data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Single final classification data</returns>
    private FinalClassificationCarData2025 ExtractFinalClassificationSingleCarData2025(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var finalClassData = new FinalClassificationCarData2025();

        if (packetLength >= offsetToStart + ConstData.F12025FinalClassificationCarSize)
        {
            var actOffset = offsetToStart;

            finalClassData.Position = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.LapsCompleted = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.Points = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.PitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultStatus = ResultStatusMapper.MapResultStatus2025(resultStatus);

            actOffset += ConstData.TypeUInt8;

            var resultReason = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultReason = (ResultReason)Enum.ToObject(typeof(ResultReason), resultReason + 1);

            actOffset += ConstData.TypeUInt8;

            finalClassData.BestLapTimeInMs = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            finalClassData.TotalRaceTime = Unsafe.ReadUnaligned<double>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeDouble;

            finalClassData.PenaltiesTime = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumTyreStints = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            for (var actualTyreStint = 0; actualTyreStint < 8; actualTyreStint++)
            {
                finalClassData.TyreStintsActual[actualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var visualTyreStint = 0; visualTyreStint < 8; visualTyreStint++)
            {
                finalClassData.TyreStintsVisual[visualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var endLap = 0; endLap < 8; endLap++)
            {
                finalClassData.TyreStintsEndLaps[endLap] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
        }

        return finalClassData;
    }

    /// <summary>
    /// Extract all data for a single car from final classification data packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Single final classification data</returns>
    private FinalClassificationCarData2026 ExtractFinalClassificationSingleCarData2026(ref byte dataPacket, int offsetToStart, int packetLength)
    {
        var finalClassData = new FinalClassificationCarData2026();

        if (packetLength >= offsetToStart + ConstData.F12026FinalClassificationCarSize)
        {
            var actOffset = offsetToStart;

            finalClassData.Position = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.LapsCompleted = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.GridPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.Points = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.PitStops = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var resultStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultStatus = ResultStatusMapper.MapResultStatus2025(resultStatus);

            actOffset += ConstData.TypeUInt8;

            var resultReason = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            finalClassData.ResultReason = (ResultReason)Enum.ToObject(typeof(ResultReason), resultReason + 1);

            actOffset += ConstData.TypeUInt8;

            finalClassData.BestLapTimeInMs = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            finalClassData.TotalRaceTime = Unsafe.ReadUnaligned<double>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeDouble;

            finalClassData.PenaltiesTime = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumPenalties = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            finalClassData.NumTyreStints = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            for (var actualTyreStint = 0; actualTyreStint < 8; actualTyreStint++)
            {
                finalClassData.TyreStintsActual[actualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var visualTyreStint = 0; visualTyreStint < 8; visualTyreStint++)
            {
                finalClassData.TyreStintsVisual[visualTyreStint] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }

            for (var endLap = 0; endLap < 8; endLap++)
            {
                finalClassData.TyreStintsEndLaps[endLap] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
        }

        return finalClassData;
    }

    #endregion // Private methods
}