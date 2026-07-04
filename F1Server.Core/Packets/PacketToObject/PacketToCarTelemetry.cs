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
/// Class to extract a car telemetry object from recevied packet
/// </summary>
/// <param name="packetHeader">Header of packet</param>
internal class PacketToCarTelemetry(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Get car telemetry data from received data packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <returns>Object</returns>
    public object? ExtractCarTelemetryData(ReadOnlySpan<byte> dataPacket)
    {
        object? carTelemetryObject = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToCarTelemetry");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            PacketDataBase<ICarTelemetryBase>? packetDataBase = GameVersion switch
                                                                {
                                                                    2019 => new CarTelemetry(PacketHeader, new CarTelemetry2019()),
                                                                    2020 => new CarTelemetry(PacketHeader, new CarTelemetry2020()),
                                                                    2021 => new CarTelemetry(PacketHeader, new CarTelemetry2021()),
                                                                    2022 => new CarTelemetry(PacketHeader, new CarTelemetry2022()),
                                                                    2023 => new CarTelemetry(PacketHeader, new CarTelemetry2023()),
                                                                    2024 => new CarTelemetry(PacketHeader, new CarTelemetry2024()),
                                                                    2025 => new CarTelemetry(PacketHeader, new CarTelemetry2025()),
                                                                    2026 => new CarTelemetry(PacketHeader, new CarTelemetry2026()),
                                                                    _ => null
                                                                };

            if (packetDataBase != null
                && HasValidPacketLength(dataPacket.Length, GetExpectedPayloadSize())
                && ExtractCarTelemetry(ref memRef, HeaderSize, dataPacket.Length, packetDataBase.PacketData))
            {
                carTelemetryObject = packetDataBase;
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return carTelemetryObject;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Returns the expected car telemetry payload size for the current game version
    /// </summary>
    /// <returns>Expected payload size in bytes without the packet header</returns>
    private int GetExpectedPayloadSize()
    {
        return GameVersion switch
               {
                   2019 => ConstData.F12019CarTelemetrySize,
                   2020 => ConstData.F12020CarTelemetrySize,
                   2021 => ConstData.F12021CarTelemetrySize,
                   2022 => ConstData.F12022CarTelemetrySize,
                   2023 => ConstData.F12023CarTelemetrySize,
                   2024 => ConstData.F12024CarTelemetrySize,
                   2025 => ConstData.F12025CarTelemetrySize,
                   2026 => ConstData.F12026CarTelemetrySize,
                   _ => 0
               };
    }

    /// <summary>
    /// Converts received data to a car telemetry object
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="carTelemetryData">Object for car telemetry</param>
    /// <returns>Car telemetry data object</returns>
    private bool ExtractCarTelemetry(ref byte dataPacket, int offsetToStart, int packetLength, ICarTelemetryBase? carTelemetryData)
    {
        var retValue = false;

        if (packetLength >= offsetToStart)
        {
            try
            {
                int actOffset;

                if (carTelemetryData is CarTelemetry2019 carTelemetry2019)
                {
                    actOffset = ExtractSingleCarTelemetry(ref dataPacket, offsetToStart, carTelemetry2019.CarTelemetryData);

                    retValue = ExtractCarTelemetryParameters2019(ref dataPacket, actOffset, carTelemetry2019);
                }
                else if (carTelemetryData is CarTelemetry2020 carTelemetry2020)
                {
                    actOffset = ExtractSingleCarTelemetry(ref dataPacket, offsetToStart, carTelemetry2020.CarTelemetryData);

                    retValue = ExtractCarTelemetryParameters2020(ref dataPacket, actOffset, carTelemetry2020);
                }
                else if (carTelemetryData is CarTelemetry2021 carTelemetry2021)
                {
                    actOffset = ExtractSingleCarTelemetry(ref dataPacket, offsetToStart, carTelemetry2021.CarTelemetryData);

                    retValue = ExtractCarTelemetryParameters2021(ref dataPacket, actOffset, carTelemetry2021);
                }
                else if (carTelemetryData is CarTelemetry2022 carTelemetry2022)
                {
                    actOffset = ExtractSingleCarTelemetry(ref dataPacket, offsetToStart, carTelemetry2022.CarTelemetryData);

                    retValue = ExtractCarTelemetryParameters2022(ref dataPacket, actOffset, carTelemetry2022);
                }
                else if (carTelemetryData is CarTelemetry2023 carTelemetry2023)
                {
                    actOffset = ExtractSingleCarTelemetry(ref dataPacket, offsetToStart, carTelemetry2023.CarTelemetryData);

                    retValue = ExtractCarTelemetryParameters2023(ref dataPacket, actOffset, carTelemetry2023);
                }
                else if (carTelemetryData is CarTelemetry2024 carTelemetry2024)
                {
                    actOffset = ExtractSingleCarTelemetry(ref dataPacket, offsetToStart, carTelemetry2024.CarTelemetryData);

                    retValue = ExtractCarTelemetryParameters2024(ref dataPacket, actOffset, carTelemetry2024);
                }
                else if (carTelemetryData is CarTelemetry2025 carTelemetry2025)
                {
                    actOffset = ExtractSingleCarTelemetry(ref dataPacket, offsetToStart, carTelemetry2025.CarTelemetryData);

                    retValue = ExtractCarTelemetryParameters2025(ref dataPacket, actOffset, carTelemetry2025);
                }
                else if (carTelemetryData is CarTelemetry2026 carTelemetry2026)
                {
                    actOffset = ExtractSingleCarTelemetry(ref dataPacket, offsetToStart, carTelemetry2026.CarTelemetryData);

                    retValue = ExtractCarTelemetryParameters2026(ref dataPacket, actOffset, carTelemetry2026);
                }
                else
                {
                    ThrowInvalidGameVersion();
                }
            }
            catch
            {
                // Ignore exceptions in this step
            }
        }

        return retValue;
    }

    /// <summary>
    /// Extract additional parameters from telemetry packet (2019)
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetry2019">Car telemetry data class</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetryParameters2019(ref byte dataPacket, int actOffset, CarTelemetry2019 carTelemetry2019)
    {
        bool isExtracted = false;

        if (actOffset < HeaderSize + ConstData.F12019CarTelemetrySize)
        {
            carTelemetry2019.ButtonStatus = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract additional parameters from telemetry packet (2020)
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetry2020">Car telemetry data class</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetryParameters2020(ref byte dataPacket, int actOffset, CarTelemetry2020 carTelemetry2020)
    {
        var isExtracted = false;

        if (actOffset > ConstData.F12020CarTelemetrySize)
        {
            carTelemetry2020.ButtonStatus = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            carTelemetry2020.MfdPanelIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2020.MfdPanelIndexSecondary = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2020.SuggestedGear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract additional parameters from telemetry packet (2021)
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetry2021">Car telemetry data class</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetryParameters2021(ref byte dataPacket, int actOffset, CarTelemetry2021 carTelemetry2021)
    {
        var isExtracted = false;

        if (actOffset > ConstData.F12021CarTelemetrySize)
        {
            carTelemetry2021.MfdPanelIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2021.MfdPanelIndexSecondary = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2021.SuggestedGear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract additional parameters from telemetry packet (2022)
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetry2022">Car telemetry data class</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetryParameters2022(ref byte dataPacket, int actOffset, CarTelemetry2022 carTelemetry2022)
    {
        var isExtracted = false;

        if (actOffset > ConstData.F12022CarTelemetrySize)
        {
            carTelemetry2022.MfdPanelIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2022.MfdPanelIndexSecondary = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2022.SuggestedGear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract additional parameters from telemetry packet (2023)
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetry2023">Car telemetry data class</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetryParameters2023(ref byte dataPacket, int actOffset, CarTelemetry2023 carTelemetry2023)
    {
        var isExtracted = false;

        if (actOffset > ConstData.F12023CarTelemetrySize)
        {
            carTelemetry2023.MfdPanelIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2023.MfdPanelIndexSecondary = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2023.SuggestedGear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract additional parameters from telemetry packet (2024)
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetry2024">Car telemetry data class</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetryParameters2024(ref byte dataPacket, int actOffset, CarTelemetry2024 carTelemetry2024)
    {
        var isExtracted = false;

        if (actOffset > ConstData.F12024CarTelemetrySize)
        {
            carTelemetry2024.MfdPanelIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2024.MfdPanelIndexSecondary = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2024.SuggestedGear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract additional parameters from telemetry packet (2025)
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetry2025">Car telemetry data class</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetryParameters2025(ref byte dataPacket, int actOffset, CarTelemetry2025 carTelemetry2025)
    {
        var isExtracted = false;

        if (actOffset > ConstData.F12025CarTelemetrySize)
        {
            carTelemetry2025.MfdPanelIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2025.MfdPanelIndexSecondary = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2025.SuggestedGear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract additional parameters from telemetry packet (2026)
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetry2026">Car telemetry data class</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetryParameters2026(ref byte dataPacket, int actOffset, CarTelemetry2026 carTelemetry2026)
    {
        var isExtracted = false;

        if (actOffset > ConstData.F12026CarTelemetrySize)
        {
            carTelemetry2026.MfdPanelIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2026.MfdPanelIndexSecondary = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            carTelemetry2026.SuggestedGear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            isExtracted = true;
        }

        return isExtracted;
    }

    /// <summary>
    /// Extract all telemetry data packets of all cars, size depends on game version
    /// </summary>
    /// <param name="dataPacket">Received packet data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="carTelemetry">Data object</param>
    /// <returns>New offset</returns>
    private int ExtractSingleCarTelemetry(ref byte dataPacket, int offsetToStart, ICarTelemetryDataBase[] carTelemetry)
    {
        var actOffset = offsetToStart;

        if (carTelemetry?.Length > 0)
        {
            foreach (var carTelemetryData in carTelemetry)
            {
                carTelemetryData.Speed = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt16;

                carTelemetryData.Throttle = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeFloat;

                carTelemetryData.Steer = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeFloat;

                carTelemetryData.Brake = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeFloat;

                carTelemetryData.Clutch = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                carTelemetryData.Gear = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeInt8;

                carTelemetryData.EngineRPM = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt16;

                carTelemetryData.IsDRS = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                carTelemetryData.RevLightsIndicator = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                if (GameVersion > 2020)
                {
                    actOffset = ExtractAdditionalValuesSince2020(ref dataPacket, actOffset, carTelemetryData);
                }

                // Brakes temperature
                carTelemetryData.BrakesTemperature.RearLeft = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt16;

                carTelemetryData.BrakesTemperature.RearRight = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt16;

                carTelemetryData.BrakesTemperature.FrontLeft = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt16;

                carTelemetryData.BrakesTemperature.FrontRight = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt16;

                if (carTelemetryData is CarTelemetryData2019)
                {
                    // Tyres surface temperature
                    carTelemetryData.TyresSurfaceTemperature.RearLeft = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    carTelemetryData.TyresSurfaceTemperature.RearRight = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    carTelemetryData.TyresSurfaceTemperature.FrontLeft = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    carTelemetryData.TyresSurfaceTemperature.FrontRight = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    // Tyres inner temperature
                    carTelemetryData.TyresInnerTemperature.RearLeft = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    carTelemetryData.TyresInnerTemperature.RearRight = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    carTelemetryData.TyresInnerTemperature.FrontLeft = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    carTelemetryData.TyresInnerTemperature.FrontRight = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;
                }
                else
                {
                    // Tyres surface temperature
                    carTelemetryData.TyresSurfaceTemperature.RearLeft = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carTelemetryData.TyresSurfaceTemperature.RearRight = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carTelemetryData.TyresSurfaceTemperature.FrontLeft = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carTelemetryData.TyresSurfaceTemperature.FrontRight = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    // Tyres inner temperature
                    carTelemetryData.TyresInnerTemperature.RearLeft = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carTelemetryData.TyresInnerTemperature.RearRight = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carTelemetryData.TyresInnerTemperature.FrontLeft = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carTelemetryData.TyresInnerTemperature.FrontRight = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }

                // Engine temperature - narrowed from uint16 to uint8 in F1 2026
                if (GameVersion >= 2026)
                {
                    carTelemetryData.EngineTemperature = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
                else
                {
                    carTelemetryData.EngineTemperature = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;
                }

                // tyres pressure
                carTelemetryData.TyresPressure.RearLeft = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeFloat;

                carTelemetryData.TyresPressure.RearRight = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeFloat;

                carTelemetryData.TyresPressure.FrontLeft = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeFloat;

                carTelemetryData.TyresPressure.FrontRight = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeFloat;

                // Tyres surface type
                var surfaceByte = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                carTelemetryData.SurfaceType.RearLeft = (SurfaceType)Enum.ToObject(typeof(SurfaceType), surfaceByte);

                actOffset += ConstData.TypeUInt8;

                surfaceByte = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                carTelemetryData.SurfaceType.RearRight = (SurfaceType)Enum.ToObject(typeof(SurfaceType), surfaceByte);

                actOffset += ConstData.TypeUInt8;

                surfaceByte = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                carTelemetryData.SurfaceType.FrontLeft = (SurfaceType)Enum.ToObject(typeof(SurfaceType), surfaceByte);

                actOffset += ConstData.TypeUInt8;

                surfaceByte = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                carTelemetryData.SurfaceType.FrontRight = (SurfaceType)Enum.ToObject(typeof(SurfaceType), surfaceByte);

                actOffset += ConstData.TypeUInt8;
            }
        }

        return actOffset;
    }

    /// <summary>
    /// Extract additional values since F1 2020
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="carTelemetryData">Car telemetry datsa</param>
    /// <returns>New offset</returns>
    /// <exception cref="NotImplementedException">Invalid game version</exception>
    private int ExtractAdditionalValuesSince2020(ref byte dataPacket, int actOffset, ICarTelemetryDataBase carTelemetryData)
    {
        if (carTelemetryData is CarTelemetryData2021 carTelemetryData2021)
        {
            carTelemetryData2021.RevLightsBitValue = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;
        }
        else if (carTelemetryData is CarTelemetryData2022 carTelemetryData2022)
        {
            carTelemetryData2022.RevLightsBitValue = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;
        }
        else if (carTelemetryData is CarTelemetryData2023 carTelemetryData2023)
        {
            carTelemetryData2023.RevLightsBitValue = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;
        }
        else if (carTelemetryData is CarTelemetryData2024 carTelemetryData2024)
        {
            carTelemetryData2024.RevLightsBitValue = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;
        }
        else if (carTelemetryData is CarTelemetryData2025 carTelemetryData2025)
        {
            carTelemetryData2025.RevLightsBitValue = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;
        }
        else
        {
            ThrowInvalidGameVersion();
        }

        return actOffset;
    }

    #endregion // Private methods
}