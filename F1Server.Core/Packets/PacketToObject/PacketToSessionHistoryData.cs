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
/// Class to extract a session history object from received packet
/// </summary>
/// <param name="packetHeader">Packet header</param>
internal class PacketToSessionHistoryData(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Get session history data from received packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <returns>Session history data object</returns>
    public object? ExtractSessionHistoryDataPacket(ReadOnlySpan<byte> dataPacket)
    {
        object? sessionHistory = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToSessionHistory");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            SessionHistoryData? sessionHistoryData = GameVersion switch
                                                     {
                                                         2021 => new SessionHistoryData(PacketHeader, new SessionHistoryData2021()),
                                                         2022 => new SessionHistoryData(PacketHeader, new SessionHistoryData2022()),
                                                         2023 => new SessionHistoryData(PacketHeader, new SessionHistoryData2023()),
                                                         2024 => new SessionHistoryData(PacketHeader, new SessionHistoryData2024()),
                                                         2025 => new SessionHistoryData(PacketHeader, new SessionHistoryData2025()),
                                                         2026 => new SessionHistoryData(PacketHeader, new SessionHistoryData2026()),
                                                         _ => null
                                                     };

            if (sessionHistoryData != null
                && HasValidPacketLength(dataPacket.Length, GetExpectedPayloadSize())
                && ExtractSessionHistoryData(ref memRef, HeaderSize, sessionHistoryData.PacketData))
            {
                sessionHistory = sessionHistoryData;
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return sessionHistory;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Returns the expected session history payload size for the current game version
    /// </summary>
    /// <returns>Expected payload size in bytes without the packet header</returns>
    private int GetExpectedPayloadSize()
    {
        return GameVersion switch
               {
                   2021 => ConstData.F12021SessionHistorySize,
                   2022 => ConstData.F12022SessionHistorySize,
                   2023 => ConstData.F12023SessionHistorySize,
                   2024 => ConstData.F12024SessionHistorySize,
                   2025 => ConstData.F12025SessionHistorySize,
                   2026 => ConstData.F12026SessionHistorySize,
                   _ => 0
               };
    }

    /// <summary>
    /// Extract session history
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <returns>Status</returns>
    private bool ExtractSessionHistoryData(ref byte dataPacket, int offsetToStart, ISessionHistoryDataBase? sessionHistoryData)
    {
        var retValue = false;

        if (sessionHistoryData is SessionHistoryData2021 historyData2021)
        {
            retValue = ExtractSessionHistoryDataPacket2021(ref dataPacket, offsetToStart, historyData2021);
        }
        else if (sessionHistoryData is SessionHistoryData2022 historyData2022)
        {
            retValue = ExtractSessionHistoryDataPacket2022(ref dataPacket, offsetToStart, historyData2022);
        }
        else if (sessionHistoryData is SessionHistoryData2023 historyData2023)
        {
            retValue = ExtractSessionHistoryDataPacket2023(ref dataPacket, offsetToStart, historyData2023);
        }
        else if (sessionHistoryData is SessionHistoryData2024 historyData2024)
        {
            retValue = ExtractSessionHistoryDataPacket2024(ref dataPacket, offsetToStart, historyData2024);
        }
        else if (sessionHistoryData is SessionHistoryData2025 historyData2025)
        {
            retValue = ExtractSessionHistoryDataPacket2025(ref dataPacket, offsetToStart, historyData2025);
        }
        else if (sessionHistoryData is SessionHistoryData2026 historyData2026)
        {
            retValue = ExtractSessionHistoryDataPacket2026(ref dataPacket, offsetToStart, historyData2026);
        }
        else
        {
            ThrowInvalidGameVersion();
        }

        return retValue;
    }

    /// <summary>
    /// Extract session history for F1 2021
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <returns>Status</returns>
    private bool ExtractSessionHistoryDataPacket2021(ref byte dataPacket, int offsetToStart, ISessionHistoryDataBase? sessionHistoryData)
    {
        var retValue = false;

        if (sessionHistoryData != null)
        {
            var actOffset = offsetToStart;

            try
            {
                actOffset = ExtractSessionHistoryDataBase(ref dataPacket, actOffset, sessionHistoryData);

                actOffset = ExtractSessionHistoryLaps(ref dataPacket, actOffset, sessionHistoryData, ConstData.F12021SessionHistoryLapSize);

                ExtractSessionHistoryTyreStints(ref dataPacket, actOffset, sessionHistoryData, ConstData.F12021SessionHistoryTyreStintSize);

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
    /// Extract session history for F1 2022
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <returns>Status</returns>
    private bool ExtractSessionHistoryDataPacket2022(ref byte dataPacket, int offsetToStart, ISessionHistoryDataBase? sessionHistoryData)
    {
        var retValue = false;

        if (sessionHistoryData != null)
        {
            var actOffset = offsetToStart;

            try
            {
                actOffset = ExtractSessionHistoryDataBase(ref dataPacket, actOffset, sessionHistoryData);

                actOffset = ExtractSessionHistoryLaps(ref dataPacket, actOffset, sessionHistoryData, ConstData.F12022SessionHistoryLapSize);

                ExtractSessionHistoryTyreStints(ref dataPacket, actOffset, sessionHistoryData, ConstData.F12022SessionHistoryTyreStintSize);

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
    /// Extract session history for F1 2023
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <returns>Status</returns>
    private bool ExtractSessionHistoryDataPacket2023(ref byte dataPacket, int offsetToStart, SessionHistoryData2023? sessionHistoryData)
    {
        var retValue = false;

        if (sessionHistoryData != null)
        {
            var actOffset = offsetToStart;

            try
            {
                actOffset = ExtractSessionHistoryDataBase(ref dataPacket, actOffset, sessionHistoryData);

                for (int lap = 0; lap < sessionHistoryData.LapHistory.Length; lap++)
                {
                    if (lap < sessionHistoryData.NumberOfLaps)
                    {
                        var lapData = new SessionHistoryLapData2023
                                      {
                                          LapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset))
                                      };

                        actOffset += ConstData.TypeUInt32;

                        lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.Sector3Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector3TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.LapValidFlag = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionHistoryData.LapHistory[lap] = lapData;
                    }
                    else
                    {
                        actOffset += ConstData.F12023SessionHistoryLapSize;
                    }
                }

                ExtractSessionHistoryTyreStints(ref dataPacket, actOffset, sessionHistoryData, ConstData.F12023SessionHistoryTyreStintSize);

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
    /// Extract session history for F1 2024
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <returns>Status</returns>
    private bool ExtractSessionHistoryDataPacket2024(ref byte dataPacket, int offsetToStart, SessionHistoryData2024? sessionHistoryData)
    {
        var retValue = false;

        if (sessionHistoryData != null)
        {
            var actOffset = offsetToStart;

            try
            {
                actOffset = ExtractSessionHistoryDataBase(ref dataPacket, actOffset, sessionHistoryData);

                for (int lap = 0; lap < sessionHistoryData.LapHistory.Length; lap++)
                {
                    if (lap < sessionHistoryData.NumberOfLaps)
                    {
                        var lapData = new SessionHistoryLapData2024
                                      {
                                          LapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset))
                                      };

                        actOffset += ConstData.TypeUInt32;

                        lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.Sector3Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector3TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.LapValidFlag = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionHistoryData.LapHistory[lap] = lapData;
                    }
                    else
                    {
                        actOffset += ConstData.F12024SessionHistoryLapSize;
                    }
                }

                ExtractSessionHistoryTyreStints(ref dataPacket, actOffset, sessionHistoryData, ConstData.F12024SessionHistoryTyreStintSize);

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
    /// Extract session history for F1 2025
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <returns>Status</returns>
    private bool ExtractSessionHistoryDataPacket2025(ref byte dataPacket, int offsetToStart, SessionHistoryData2025? sessionHistoryData)
    {
        var retValue = false;

        if (sessionHistoryData != null)
        {
            var actOffset = offsetToStart;

            try
            {
                actOffset = ExtractSessionHistoryDataBase(ref dataPacket, actOffset, sessionHistoryData);

                for (int lap = 0; lap < sessionHistoryData.LapHistory.Length; lap++)
                {
                    if (lap < sessionHistoryData.NumberOfLaps)
                    {
                        var lapData = new SessionHistoryLapData2025
                                      {
                                          LapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset))
                                      };

                        actOffset += ConstData.TypeUInt32;

                        lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.Sector3Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector3TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.LapValidFlag = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionHistoryData.LapHistory[lap] = lapData;
                    }
                    else
                    {
                        actOffset += ConstData.F12025SessionHistoryLapSize;
                    }
                }

                ExtractSessionHistoryTyreStints(ref dataPacket, actOffset, sessionHistoryData, ConstData.F12025SessionHistoryTyreStintSize);

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
    /// Extract session history for F1 2026
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <returns>Status</returns>
    private bool ExtractSessionHistoryDataPacket2026(ref byte dataPacket, int offsetToStart, SessionHistoryData2026? sessionHistoryData)
    {
        var retValue = false;

        if (sessionHistoryData != null)
        {
            var actOffset = offsetToStart;

            try
            {
                actOffset = ExtractSessionHistoryDataBase(ref dataPacket, actOffset, sessionHistoryData);

                for (int lap = 0; lap < sessionHistoryData.LapHistory.Length; lap++)
                {
                    if (lap < sessionHistoryData.NumberOfLaps)
                    {
                        var lapData = new SessionHistoryLapData2026
                                      {
                                          LapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset))
                                      };

                        actOffset += ConstData.TypeUInt32;

                        lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.Sector3Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt16;

                        lapData.Sector3TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        lapData.LapValidFlag = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionHistoryData.LapHistory[lap] = lapData;
                    }
                    else
                    {
                        actOffset += ConstData.F12026SessionHistoryLapSize;
                    }
                }

                ExtractSessionHistoryTyreStints(ref dataPacket, actOffset, sessionHistoryData, ConstData.F12026SessionHistoryTyreStintSize);

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
    /// Extract session history base data
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <returns>Offset</returns>
    private int ExtractSessionHistoryDataBase(ref byte dataPacket, int offsetToStart, ISessionHistoryDataBase? sessionHistoryData)
    {
        var actOffset = offsetToStart;

        if (sessionHistoryData != null)
        {
            try
            {
                sessionHistoryData.CarIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                sessionHistoryData.NumberOfLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                sessionHistoryData.NumberOfTyreStints = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                sessionHistoryData.BestLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                sessionHistoryData.BestSector1LapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                sessionHistoryData.BestSector2LapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                sessionHistoryData.BestSector3LapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
            }
        }

        return actOffset;
    }

    /// <summary>
    /// Extract session history base data
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <param name="lapSize">Size of one lap</param>
    /// <returns>Offset</returns>
    private int ExtractSessionHistoryLaps(ref byte dataPacket, int offsetToStart, ISessionHistoryDataBase? sessionHistoryData, int lapSize)
    {
        var actOffset = offsetToStart;

        if (sessionHistoryData != null)
        {
            try
            {
                for (int lap = 0; lap < sessionHistoryData.LapHistory.Length; lap++)
                {
                    var lapData = CreateGameDependentLapHistoryObject();

                    if (lap < sessionHistoryData.NumberOfLaps && lapData != null)
                    {
                        ExtractSessionHistoryLapData(ref dataPacket, ref actOffset, lapData!);

                        sessionHistoryData.LapHistory[lap] = lapData;
                    }
                    else
                    {
                        actOffset += lapSize;
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
            }
        }

        return actOffset;
    }

    /// <summary>
    /// Extract lap data
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="actOffset">Offset in packet data</param>
    /// <param name="lapData">Lap object</param>
    private void ExtractSessionHistoryLapData(ref byte dataPacket, ref int actOffset, ILapHistoryDataBase lapData)
    {
        var lapData23 = lapData as ILapHistoryData2023;

        lapData.LapTime = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt32;

        lapData.Sector1Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt16;

        if (lapData23 != null)
        {
            lapData23.Sector1TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;
        }

        lapData.Sector2Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt16;

        if (lapData23 != null)
        {
            lapData23.Sector2TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;
        }

        lapData.Sector3Time = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt16;

        if (lapData23 != null)
        {
            lapData23.Sector3TimeMinutes = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;
        }

        lapData.LapValidFlag = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;
    }

    /// <summary>
    /// Extract tyre stints from session history packet
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionHistoryData">Base session history data</param>
    /// <param name="tyreStintSize">Size of one tyre stint data</param>
    private void ExtractSessionHistoryTyreStints(ref byte dataPacket, int offsetToStart, ISessionHistoryDataBase? sessionHistoryData, int tyreStintSize)
    {
        var actOffset = offsetToStart;

        if (sessionHistoryData != null)
        {
            try
            {
                for (int stint = 0; stint < sessionHistoryData.TyreStintHistory.Length; stint++)
                {
                    var tyreData = CreateGameDependentTyreStintObject();

                    if (stint < sessionHistoryData.NumberOfTyreStints && tyreData != null)
                    {
                        tyreData.EndLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        tyreData.TyreActualCompound = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        tyreData.TyreVisualCompound = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionHistoryData.TyreStintHistory[stint] = tyreData;
                    }
                    else
                    {
                        actOffset += tyreStintSize;
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
            }
        }
    }

    /// <summary>
    /// Create a <see cref="ILapHistoryDataBase"/> object
    /// </summary>
    /// <returns>Object</returns>
    private ILapHistoryDataBase? CreateGameDependentLapHistoryObject()
    {
        return GameVersion switch
               {
                   2021 => new SessionHistoryLapData2021(),
                   2022 => new SessionHistoryLapData2022(),
                   2023 => new SessionHistoryLapData2023(),
                   2024 => new SessionHistoryLapData2024(),
                   2025 => new SessionHistoryLapData2025(),
                   2026 => new SessionHistoryLapData2026(),
                   _ => null
               };
    }

    /// <summary>
    /// Create a <see cref="ITyreStintHistoryDataBase"/> object
    /// </summary>
    /// <returns>Object</returns>
    private ITyreStintHistoryDataBase? CreateGameDependentTyreStintObject()
    {
        return GameVersion switch
               {
                   2021 => new SessionHistoryTyreStintData2021(),
                   2022 => new SessionHistoryTyreStintData2022(),
                   2023 => new SessionHistoryTyreStintData2023(),
                   2024 => new SessionHistoryTyreStintData2024(),
                   2025 => new SessionHistoryTyreStintData2025(),
                   2026 => new SessionHistoryTyreStintData2026(),
                   _ => null
               };
    }

    #endregion // Private methods
}