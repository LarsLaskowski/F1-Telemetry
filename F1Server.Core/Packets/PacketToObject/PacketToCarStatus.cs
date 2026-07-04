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
/// Class to extract a car status object from recevied packet
/// </summary>
/// <param name="packetHeader">Header of packet</param>
internal class PacketToCarStatus(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Get car status data from received data packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <returns>Object</returns>
    public object? ExtractCarStatusData(ReadOnlySpan<byte> dataPacket)
    {
        object? carStatusObject = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToCarStatus");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            PacketDataBase<ICarStatusBase>? packetDataBase = GameVersion switch
                                                             {
                                                                 2019 => new CarStatus(PacketHeader, new CarStatus2019()),
                                                                 2020 => new CarStatus(PacketHeader, new CarStatus2020()),
                                                                 2021 => new CarStatus(PacketHeader, new CarStatus2021()),
                                                                 2022 => new CarStatus(PacketHeader, new CarStatus2022()),
                                                                 2023 => new CarStatus(PacketHeader, new CarStatus2023()),
                                                                 2024 => new CarStatus(PacketHeader, new CarStatus2024()),
                                                                 2025 => new CarStatus(PacketHeader, new CarStatus2025()),
                                                                 2026 => new CarStatus(PacketHeader, new CarStatus2026()),
                                                                 _ => null
                                                             };

            if (packetDataBase != null
                && HasValidPacketLength(dataPacket.Length, GetExpectedPayloadSize())
                && ExtractCarStatus(ref memRef, HeaderSize, dataPacket.Length, packetDataBase.PacketData))
            {
                carStatusObject = packetDataBase;
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return carStatusObject;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Returns the expected car status payload size for the current game version
    /// </summary>
    /// <returns>Expected payload size in bytes without the packet header</returns>
    private int GetExpectedPayloadSize()
    {
        return GameVersion switch
               {
                   2019 => ConstData.F12019CarStatusSize,
                   2020 => ConstData.F12020CarStatusSize,
                   2021 => ConstData.F12021CarStatusSize,
                   2022 => ConstData.F12022CarStatusSize,
                   2023 => ConstData.F12023CarStatusSize,
                   2024 => ConstData.F12024CarStatusSize,
                   2025 => ConstData.F12025CarStatusSize,
                   2026 => ConstData.F12026CarStatusSize,
                   _ => 0
               };
    }

    /// <summary>
    /// Converts received data to a car status object
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="carStatusData">Object for car status</param>
    /// <returns>Car Status data object</returns>
    private bool ExtractCarStatus(ref byte dataPacket, int offsetToStart, int packetLength, ICarStatusBase? carStatusData)
    {
        var retValue = false;

        if (packetLength >= offsetToStart)
        {
            try
            {
                int actOffset = offsetToStart;

                if (carStatusData is CarStatus2019 carStatus2019)
                {
                    retValue = ExtractCarStatus2019(ref dataPacket, actOffset, carStatus2019);
                }
                else if (carStatusData is CarStatus2020 carStatus2020)
                {
                    retValue = ExtractCarStatus2020(ref dataPacket, actOffset, carStatus2020);
                }
                else if (carStatusData is CarStatus2021 carStatus2021)
                {
                    retValue = ExtractCarStatus2021(ref dataPacket, actOffset, carStatus2021);
                }
                else if (carStatusData is CarStatus2022 carStatus2022)
                {
                    retValue = ExtractCarStatus2022(ref dataPacket, actOffset, carStatus2022);
                }
                else if (carStatusData is CarStatus2023 carStatus2023)
                {
                    retValue = ExtractCarStatus2023(ref dataPacket, actOffset, carStatus2023);
                }
                else if (carStatusData is CarStatus2024 carStatus2024)
                {
                    retValue = ExtractCarStatus2024(ref dataPacket, actOffset, carStatus2024);
                }
                else if (carStatusData is CarStatus2025 carStatus2025)
                {
                    retValue = ExtractCarStatus2025(ref dataPacket, actOffset, carStatus2025);
                }
                else if (carStatusData is CarStatus2026 carStatus2026)
                {
                    retValue = ExtractCarStatus2026(ref dataPacket, actOffset, carStatus2026);
                }
                else
                {
                    ThrowInvalidGameVersion();
                }
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
            }
        }

        return retValue;
    }

    /// <summary>
    /// Extract car status from 2019
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carStatus2019">Car status 2019</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarStatus2019(ref byte dataPacket, int actOffset, CarStatus2019 carStatus2019)
    {
        var isExtracted = false;

        if (actOffset > 0)
        {
            for (var carStatus = 0; carStatus < carStatus2019.CarStatusData.Length; ++carStatus)
            {
                if (carStatus2019.CarStatusData[carStatus] is ICarStatusData2019 status2019)
                {
                    actOffset = ExtractBaseCarStatus(ref dataPacket, actOffset, status2019);

                    status2019.TyresWear[0] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.TyresWear[1] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.TyresWear[2] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.TyresWear[3] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    actOffset = ExtractBaseTyresCarStatus(ref dataPacket, actOffset, status2019);

                    status2019.TyreDamage[0] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.TyreDamage[1] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.TyreDamage[2] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.TyreDamage[3] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.FrontLeftWingDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.FrontRightWingDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.RearWingDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.EngineDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2019.GearBoxDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var flagsByte = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    status2019.FiaFlags = (VehicleFiaFlagColor)Enum.ToObject(typeof(VehicleFiaFlagColor), flagsByte);

                    actOffset += ConstData.TypeInt8;

                    actOffset = ExtractBaseEnergyCarStatus(ref dataPacket, actOffset, status2019);
                }
            }

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract car status from 2020
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carStatus2020">Car status 2020</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarStatus2020(ref byte dataPacket, int actOffset, CarStatus2020 carStatus2020)
    {
        var isExtracted = false;

        if (actOffset > 0)
        {
            for (var carStatus = 0; carStatus < carStatus2020.CarStatusData.Length; ++carStatus)
            {
                if (carStatus2020.CarStatusData[carStatus] is ICarStatusData2020 status2020)
                {
                    actOffset = ExtractBaseCarStatus(ref dataPacket, actOffset, status2020);

                    status2020.DRSActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    status2020.TyresWear[0] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.TyresWear[1] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.TyresWear[2] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.TyresWear[3] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    actOffset = ExtractBaseTyresCarStatus(ref dataPacket, actOffset, status2020);

                    status2020.TyresAgeLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.TyreDamage[0] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.TyreDamage[1] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.TyreDamage[2] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.TyreDamage[3] = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.FrontLeftWingDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.FrontRightWingDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.RearWingDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.DRSFault = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.EngineDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    status2020.GearBoxDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var flagsByte = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    status2020.FiaFlags = (VehicleFiaFlagColor)Enum.ToObject(typeof(VehicleFiaFlagColor), flagsByte);

                    actOffset += ConstData.TypeInt8;

                    actOffset = ExtractBaseEnergyCarStatus(ref dataPacket, actOffset, status2020);
                }
            }

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract car status from 2021
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carStatus2021">Car status 2021</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarStatus2021(ref byte dataPacket, int actOffset, CarStatus2021 carStatus2021)
    {
        var isExtracted = false;

        if (actOffset > 0)
        {
            for (var carStatus = 0; carStatus < carStatus2021.CarStatusData.Length; ++carStatus)
            {
                if (carStatus2021.CarStatusData[carStatus] is ICarStatusData2021 status2021)
                {
                    actOffset = ExtractBaseCarStatus(ref dataPacket, actOffset, status2021);

                    status2021.DRSActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    actOffset = ExtractBaseTyresCarStatus(ref dataPacket, actOffset, status2021);

                    status2021.TyresAgeLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var flagsByte = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    status2021.FiaFlags = (VehicleFiaFlagColor)Enum.ToObject(typeof(VehicleFiaFlagColor), flagsByte);

                    actOffset += ConstData.TypeInt8;

                    actOffset = ExtractBaseEnergyCarStatus(ref dataPacket, actOffset, status2021);

                    status2021.NetworkPaused = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
            }

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract car status from 2022
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carStatus2022">Car status 2022</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarStatus2022(ref byte dataPacket, int actOffset, CarStatus2022 carStatus2022)
    {
        var isExtracted = false;

        if (actOffset > 0)
        {
            for (var carStatus = 0; carStatus < carStatus2022.CarStatusData.Length; ++carStatus)
            {
                if (carStatus2022.CarStatusData[carStatus] is ICarStatusData2022 status2022)
                {
                    actOffset = ExtractBaseCarStatus(ref dataPacket, actOffset, status2022);

                    status2022.DRSActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    actOffset = ExtractBaseTyresCarStatus(ref dataPacket, actOffset, status2022);

                    status2022.TyresAgeLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var flagsByte = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    status2022.FiaFlags = (VehicleFiaFlagColor)Enum.ToObject(typeof(VehicleFiaFlagColor), flagsByte);

                    actOffset += ConstData.TypeInt8;

                    actOffset = ExtractBaseEnergyCarStatus(ref dataPacket, actOffset, status2022);

                    status2022.NetworkPaused = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
            }

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract car status from 2023
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carStatus2023">Car status 2023</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarStatus2023(ref byte dataPacket, int actOffset, CarStatus2023 carStatus2023)
    {
        var isExtracted = false;

        if (actOffset > 0)
        {
            for (var carStatus = 0; carStatus < carStatus2023.CarStatusData.Length; ++carStatus)
            {
                if (carStatus2023.CarStatusData[carStatus] is ICarStatusData2023 status2023)
                {
                    actOffset = ExtractBaseCarStatus(ref dataPacket, actOffset, status2023);

                    status2023.DRSActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    actOffset = ExtractBaseTyresCarStatus(ref dataPacket, actOffset, status2023);

                    status2023.TyresAgeLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var flagsByte = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    status2023.FiaFlags = (VehicleFiaFlagColor)Enum.ToObject(typeof(VehicleFiaFlagColor), flagsByte);

                    actOffset += ConstData.TypeInt8;

                    actOffset = ExtractBaseEnergyCarStatus2023(ref dataPacket, actOffset, status2023);

                    status2023.NetworkPaused = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
            }

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract car status from 2024
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carStatus2024">Car status 2024</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarStatus2024(ref byte dataPacket, int actOffset, CarStatus2024 carStatus2024)
    {
        var isExtracted = false;

        if (actOffset > 0)
        {
            for (var carStatus = 0; carStatus < carStatus2024.CarStatusData.Length; ++carStatus)
            {
                if (carStatus2024.CarStatusData[carStatus] is ICarStatusData2024 status2024)
                {
                    actOffset = ExtractBaseCarStatus(ref dataPacket, actOffset, status2024);

                    status2024.DRSActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    actOffset = ExtractBaseTyresCarStatus(ref dataPacket, actOffset, status2024);

                    status2024.TyresAgeLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var flagsByte = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    status2024.FiaFlags = (VehicleFiaFlagColor)Enum.ToObject(typeof(VehicleFiaFlagColor), flagsByte);

                    actOffset += ConstData.TypeInt8;

                    actOffset = ExtractBaseEnergyCarStatus2023(ref dataPacket, actOffset, status2024);

                    status2024.NetworkPaused = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
            }

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract car status from 2025
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carStatus2025">Car status 2025</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarStatus2025(ref byte dataPacket, int actOffset, CarStatus2025 carStatus2025)
    {
        var isExtracted = false;

        if (actOffset > 0)
        {
            for (var carStatus = 0; carStatus < carStatus2025.CarStatusData.Length; ++carStatus)
            {
                if (carStatus2025.CarStatusData[carStatus] is ICarStatusData2025 status2025)
                {
                    actOffset = ExtractBaseCarStatus(ref dataPacket, actOffset, status2025);

                    status2025.DRSActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    actOffset = ExtractBaseTyresCarStatus(ref dataPacket, actOffset, status2025);

                    status2025.TyresAgeLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var flagsByte = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    status2025.FiaFlags = (VehicleFiaFlagColor)Enum.ToObject(typeof(VehicleFiaFlagColor), flagsByte);

                    actOffset += ConstData.TypeInt8;

                    actOffset = ExtractBaseEnergyCarStatus2023(ref dataPacket, actOffset, status2025);

                    status2025.NetworkPaused = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
            }

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract car status from 2026
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carStatus2026">Car status 2026</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarStatus2026(ref byte dataPacket, int actOffset, CarStatus2026 carStatus2026)
    {
        var isExtracted = false;

        if (actOffset > 0)
        {
            for (var carStatus = 0; carStatus < carStatus2026.CarStatusData.Length; ++carStatus)
            {
                if (carStatus2026.CarStatusData[carStatus] is ICarStatusData2026 status2026)
                {
                    actOffset = ExtractBaseCarStatus(ref dataPacket, actOffset, status2026);

                    status2026.DRSActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    actOffset = ExtractBaseTyresCarStatus(ref dataPacket, actOffset, status2026);

                    status2026.TyresAgeLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var flagsByte = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    status2026.FiaFlags = (VehicleFiaFlagColor)Enum.ToObject(typeof(VehicleFiaFlagColor), flagsByte);

                    actOffset += ConstData.TypeInt8;

                    actOffset = ExtractBaseEnergyCarStatus2026(ref dataPacket, actOffset, status2026);

                    status2026.NetworkPaused = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
            }

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract all Status data packets of all cars, size depends on game version
    /// </summary>
    /// <param name="dataPacket">Received packet data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="carStatusData">Data object</param>
    /// <returns>New offset</returns>
    private int ExtractBaseCarStatus(ref byte dataPacket, int offsetToStart, ICarStatusDataBase carStatusData)
    {
        int actOffset = offsetToStart;

        if (carStatusData != null)
        {
            carStatusData.TractionControl = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.AntiLockBrakes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.FuelMix = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.FronBrakeBias = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.PitLimiterStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.FuelInTank = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.FuelCapacity = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.FuelRemainingLaps = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.MaxRPM = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            carStatusData.IdleRPM = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            carStatusData.MaxGears = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.DRSAllowed = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;
        }

        return actOffset;
    }

    /// <summary>
    /// Extract all tyre status data packets of all cars, size depends on game version
    /// </summary>
    /// <param name="dataPacket">Received packet data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="carStatusData">Data object</param>
    /// <returns>New offset</returns>
    private int ExtractBaseTyresCarStatus(ref byte dataPacket, int offsetToStart, ICarStatusDataBase carStatusData)
    {
        int actOffset = offsetToStart;

        if (carStatusData != null)
        {
            carStatusData.ActualTyreCompound = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var visualTyreCompoundByte = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            carStatusData.VisualTyreCompound = TyreCompoundMapper.MapVisualTyreCompoundToEnum(visualTyreCompoundByte);

            actOffset += ConstData.TypeUInt8;
        }

        return actOffset;
    }

    /// <summary>
    /// Extract all energy status data packets of all cars, size depends on game version
    /// </summary>
    /// <param name="dataPacket">Received packet data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="carStatusData">Data object</param>
    /// <returns>New offset</returns>
    private int ExtractBaseEnergyCarStatus(ref byte dataPacket, int offsetToStart, ICarStatusDataBase carStatusData)
    {
        int actOffset = offsetToStart;

        if (carStatusData != null)
        {
            carStatusData.ERSStoreEnergy = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSDeployMode = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.ERSHarvestedThisLapMGUK = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSHarvestedThisLapMGUH = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSDeployedThisLap = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;
        }

        return actOffset;
    }

    /// <summary>
    /// Extract all energy status data packets of all cars in F1 2023 and newer
    /// </summary>
    /// <param name="dataPacket">Received packet data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="carStatusData">Data object</param>
    /// <returns>New offset</returns>
    private int ExtractBaseEnergyCarStatus2023(ref byte dataPacket, int offsetToStart, ICarStatusDataBase carStatusData)
    {
        int actOffset = offsetToStart;

        if (carStatusData is ICarStatusData2023 carStatusData2023)
        {
            carStatusData2023.EnginePowerICE = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData2023.EnginePowerMGUK = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSStoreEnergy = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSDeployMode = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.ERSHarvestedThisLapMGUK = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSHarvestedThisLapMGUH = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSDeployedThisLap = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;
        }

        return actOffset;
    }

    /// <summary>
    /// Extract all energy status data packets of all cars in F1 2026 and newer, where the ERS harvest
    /// limit per lap is inserted between MGU-H harvested and ERS deployed this lap
    /// </summary>
    /// <param name="dataPacket">Received packet data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="carStatusData">Data object</param>
    /// <returns>New offset</returns>
    private int ExtractBaseEnergyCarStatus2026(ref byte dataPacket, int offsetToStart, ICarStatusData2026 carStatusData)
    {
        int actOffset = offsetToStart;

        if (carStatusData != null)
        {
            carStatusData.EnginePowerICE = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.EnginePowerMGUK = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSStoreEnergy = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSDeployMode = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carStatusData.ERSHarvestedThisLapMGUK = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSHarvestedThisLapMGUH = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSHarvestLimitPerLap = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;

            carStatusData.ERSDeployedThisLap = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeFloat;
        }

        return actOffset;
    }

    #endregion // Private methods
}