using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using F1Server.Core.Data;
using F1Server.Core.Enumerations;
using F1Server.Core.Observability;
using F1Server.Core.PacketData;
using F1Server.Core.Packets.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.PacketToObject;

/// <summary>
/// Class to create a participants data object from received packet data
/// </summary>
/// <param name="packetHeader">Header of packet</param>
internal class PacketToParticipants(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Extract data from received participants packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <returns>Participants data object</returns>
    public object? ExtractParticipantsDataPacket(ReadOnlySpan<byte> dataPacket)
    {
        object? participantsData = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToParticipants");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            PacketDataBase<IParticipantsBase>? packetDataBase = GameVersion switch
                                                                {
                                                                    2019 => new Participants(PacketHeader, new ParticipantsData2019()),
                                                                    2020 => new Participants(PacketHeader, new ParticipantsData2020()),
                                                                    2021 => new Participants(PacketHeader, new ParticipantsData2021()),
                                                                    2022 => new Participants(PacketHeader, new ParticipantsData2022()),
                                                                    2023 => new Participants(PacketHeader, new ParticipantsData2023()),
                                                                    2024 => new Participants(PacketHeader, new ParticipantsData2024()),
                                                                    2025 => new Participants(PacketHeader, new ParticipantsData2025()),
                                                                    2026 => new Participants(PacketHeader, new ParticipantsData2026()),
                                                                    _ => null
                                                                };

            if (packetDataBase is not null
                && HasValidPacketLength(dataPacket.Length, GetExpectedPayloadSize())
                && ExtractParticipantsData(ref memRef, HeaderSize, packetDataBase.PacketData))
            {
                participantsData = packetDataBase;
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return participantsData;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Returns the expected participants payload size for the current game version
    /// </summary>
    /// <returns>Expected payload size in bytes without the packet header</returns>
    private int GetExpectedPayloadSize()
    {
        return GameVersion switch
               {
                   2019 => ConstData.F12019ParticipantsSize,
                   2020 => ConstData.F12020ParticipantsSize,
                   2021 => ConstData.F12021ParticipantsSize,
                   2022 => ConstData.F12022ParticipantsSize,
                   2023 => ConstData.F12023ParticipantsSize,
                   2024 => ConstData.F12024ParticipantsSize,
                   2025 => ConstData.F12025ParticipantsSize,
                   2026 => ConstData.F12026ParticipantsSize,
                   _ => 0
               };
    }

    /// <summary>
    /// Extract all data from received packet
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="participantsData">Data object</param>
    /// <returns>Status</returns>
    private bool ExtractParticipantsData(ref byte dataPacket, int offsetToStart, IParticipantsBase? participantsData)
    {
        var retValue = false;

        if (offsetToStart > 0 && participantsData is not null)
        {
            int actOffset = offsetToStart;

            try
            {
                participantsData.ActiveCars = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                retValue = ExtractParticipantsBaseData(ref dataPacket, actOffset, participantsData.Participants);
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
            }
        }

        return retValue;
    }

    /// <summary>
    /// Extract all data from received packet
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="participantsData">Data object</param>
    /// <returns>Status</returns>
    private bool ExtractParticipantsBaseData(ref byte dataPacket, int offsetToStart, IParticipantDataBase[] participantsData)
    {
        var retValue = false;

        if (offsetToStart > 0 && participantsData is not null)
        {
            try
            {
                int actOffset = offsetToStart;

                for (int participants = 0; participants < participantsData.Length; participants++)
                {
                    IParticipantDataBase? participantData = GameVersion switch
                                                            {
                                                                2019 => new ParticipantData2019(),
                                                                2020 => new ParticipantData2020(),
                                                                2021 => new ParticipantData2021(),
                                                                2022 => new ParticipantData2022(),
                                                                2023 => new ParticipantData2023(),
                                                                2024 => new ParticipantData2024(),
                                                                2025 => new ParticipantData2025(),
                                                                2026 => new ParticipantData2026(),
                                                                _ => null
                                                            };

                    if (participantData is not null)
                    {
                        participantsData[participants] = participantData;

                        actOffset = ExtractParticipantBaseData(ref dataPacket, actOffset, participantData);
                    }
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
    /// Extract data for one participant
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="participantData">Data of participant</param>
    /// <returns>New offset</returns>
    private int ExtractParticipantBaseData(ref byte dataPacket, int actOffset, IParticipantDataBase participantData)
    {
        participantData.IsAIControlled = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        if (GameVersion >= 2026)
        {
            actOffset = ExtractParticipantDataSince2026(ref dataPacket, actOffset, participantData);
        }
        else
        {
            participantData.DriverId = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            if (GameVersion >= 2021)
            {
                actOffset = ExtractParticipantDataSince2021(ref dataPacket, actOffset, participantData);
            }
            else
            {
                participantData.TeamId = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;
            }
        }

        participantData.RaceNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        participantData.Nationality = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        if (GameVersion >= 2025)
        {
            var sourceSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref dataPacket, actOffset), ConstData.DriverNameLength2025);

            participantData.DriverName = Encoding.UTF8.GetString(sourceSpan).Trim('\0');

            actOffset += ConstData.DriverNameLength2025;
        }
        else
        {
            var sourceSpan = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref dataPacket, actOffset), ConstData.DriverNameLength);

            participantData.DriverName = Encoding.UTF8.GetString(sourceSpan).Trim('\0');

            actOffset += ConstData.DriverNameLength;
        }

        participantData.IsPublicTelemetry = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        if (GameVersion >= 2023)
        {
            actOffset = ExtractParticipantDataSince2023(ref dataPacket, actOffset, participantData);
        }

        return actOffset;
    }

    /// <summary>
    /// Extract received data to a participant object for F1 2021
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="participantData">Participant data object</param>
    /// <returns>New offset</returns>
    private int ExtractParticipantDataSince2021(ref byte dataPacket, int actOffset, IParticipantDataBase participantData)
    {
        if (participantData is IParticipantData2021 participantData2021)
        {
            participantData2021.NetworkId = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            participantData.TeamId = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            participantData2021.IsMyTeam = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;
        }

        return actOffset;
    }

    /// <summary>
    /// Extract received data to a participant object for F1 2026 (network and team ids are uint16)
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="participantData">Participant data object</param>
    /// <returns>New offset</returns>
    private int ExtractParticipantDataSince2026(ref byte dataPacket, int actOffset, IParticipantDataBase participantData)
    {
        if (participantData is IParticipantData2021 participantData2021)
        {
            participantData.DriverId = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            participantData2021.NetworkId = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            participantData.TeamId = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            participantData2021.IsMyTeam = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;
        }

        return actOffset;
    }

    /// <summary>
    /// Extract received data to a participant object for F1 2023
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="participantData">Participant data object</param>
    /// <returns>New offset</returns>
    private int ExtractParticipantDataSince2023(ref byte dataPacket, int actOffset, IParticipantDataBase participantData)
    {
        if (participantData is IParticipantData2023 participantData2023)
        {
            participantData2023.IsShowOnlineNames = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            if (participantData is IParticipantData2024 participantData2024)
            {
                participantData2024.TechLevel = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt16;
            }

            var platform = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            try
            {
                participantData2023.Platform = (Platforms)Enum.ToObject(typeof(Platforms), platform);
            }
            catch
            {
                participantData2023.Platform = Platforms.Unknown;
            }

            actOffset += ConstData.TypeUInt8;

            if (GameVersion >= 2025)
            {
                actOffset = ExtractParticipantDataSince2025(ref dataPacket, actOffset, participantData);
            }
        }

        return actOffset;
    }

    /// <summary>
    /// Extract received data to a participant object for F1 2025
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="participantData">Participant data object</param>
    /// <returns>New offset</returns>
    private int ExtractParticipantDataSince2025(ref byte dataPacket, int actOffset, IParticipantDataBase participantData)
    {
        if (participantData is IParticipantData2025 participantData2025)
        {
            participantData2025.NumColors = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            for (int color = 0; color < ConstData.LiveryColors; color++)
            {
                if (color < participantData2025.NumColors)
                {
                    participantData2025.LiveryColors[color] = new LiveryColor
                                                              {
                                                                  Red = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset)),
                                                                  Green = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset + 1)),
                                                                  Blue = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset + 2)),
                                                              };
                }

                actOffset += ConstData.TypeUInt8 * 3;
            }
        }

        return actOffset;
    }

    #endregion // Private methods
}