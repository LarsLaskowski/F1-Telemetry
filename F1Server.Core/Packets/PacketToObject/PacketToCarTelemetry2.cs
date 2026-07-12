using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using F1Server.Core.Data;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.PacketToObject;

/// <summary>
/// Class to extract the additional car telemetry (CarTelemetry2) object from a received packet (F1 2026 and newer)
/// </summary>
/// <param name="packetHeader">Header of packet</param>
internal class PacketToCarTelemetry2(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Get additional car telemetry data from received data packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <returns>Object</returns>
    public object? ExtractCarTelemetry2Data(ReadOnlySpan<byte> dataPacket)
    {
        object? carTelemetry2Object = null;

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            PacketDataBase<ICarTelemetry2Base>? packetDataBase = GameVersion switch
                                                                 {
                                                                     2026 => new CarTelemetry2(PacketHeader, new CarTelemetry22026()),
                                                                     _ => null
                                                                 };

            if (packetDataBase is not null
                && HasValidPacketLength(dataPacket.Length, ConstData.F12026CarTelemetry2Size)
                && ExtractCarTelemetry2(ref memRef, HeaderSize, dataPacket.Length, packetDataBase.PacketData))
            {
                carTelemetry2Object = packetDataBase;
            }
        }

        return carTelemetry2Object;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Converts received data to an additional car telemetry object
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="carTelemetry2Data">Object for additional car telemetry</param>
    /// <returns>Extracted?</returns>
    private bool ExtractCarTelemetry2(ref byte dataPacket, int offsetToStart, int packetLength, ICarTelemetry2Base? carTelemetry2Data)
    {
        var retValue = false;

        if (packetLength >= offsetToStart && carTelemetry2Data?.CarTelemetry2Data is not null)
        {
            try
            {
                var actOffset = offsetToStart;

                foreach (var carData in carTelemetry2Data.CarTelemetry2Data)
                {
                    carData.ActiveAeroMode = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carData.ActiveAeroAvailable = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carData.ActiveAeroActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    carData.OvertakeAvailable = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carData.OvertakeActive = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carData.OvertakeActivationDistance = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt16;

                    carData.Is2026Regulations = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    carData.IsDrivingWrongWay = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
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

    #endregion // Private methods
}