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
/// Class to extract event information from event packet
/// </summary>
/// <param name="packetHeader">Header of packet</param>
internal class PacketToEventData(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Get event data from received data packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <returns>Object</returns>
    public object? ExtractEventData(ReadOnlySpan<byte> dataPacket)
    {
        object? eventDataObject = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToEventData");

        LastError = string.Empty;

        if (dataPacket.Length > 0)
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            PacketDataBase<IEventDataBase>? packetDataBase = GameVersion switch
                                                             {
                                                                 2019 => new EventData(PacketHeader, new EventData2019()),
                                                                 2020 => new EventData(PacketHeader, new EventData2020()),
                                                                 2021 => new EventData(PacketHeader, new EventData2021()),
                                                                 2022 => new EventData(PacketHeader, new EventData2022()),
                                                                 2023 => new EventData(PacketHeader, new EventData2023()),
                                                                 2024 => new EventData(PacketHeader, new EventData2024()),
                                                                 2025 => new EventData(PacketHeader, new EventData2025()),
                                                                 2026 => new EventData(PacketHeader, new EventData2026()),
                                                                 _ => null
                                                             };

            if (packetDataBase is not null
                && HasValidPacketLength(dataPacket.Length, GetExpectedPayloadSize())
                && ExtractEventData(ref memRef, HeaderSize, dataPacket.Length, packetDataBase.PacketData))
            {
                eventDataObject = packetDataBase;

                currentActivity?.AddTag("f1.event_code", packetDataBase.PacketData?.EventCode);
            }
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return eventDataObject;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Returns the expected event payload size for the current game version
    /// </summary>
    /// <returns>Expected payload size in bytes without the packet header</returns>
    private int GetExpectedPayloadSize()
    {
        return GameVersion switch
               {
                   2019 => ConstData.F12019EventSize,
                   2020 => ConstData.F12020EventSize,
                   2021 => ConstData.F12021EventSize,
                   2022 => ConstData.F12022EventSize,
                   2023 => ConstData.F12023EventSize,
                   2024 => ConstData.F12024EventSize,
                   2025 => ConstData.F12025EventSize,
                   2026 => ConstData.F12026EventSize,
                   _ => 0
               };
    }

    /// <summary>
    /// Converts received data to a event object
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="eventData">Object for details</param>
    /// <returns>Event data object</returns>
    private bool ExtractEventData(ref byte dataPacket, int offsetToStart, int packetLength, IEventDataBase? eventData)
    {
        var retValue = false;
        var actOffset = offsetToStart;

        if (packetLength >= offsetToStart && eventData is not null)
        {
            try
            {
                var eventCodeSpan = MemoryMarshal.CreateReadOnlySpan<byte>(ref Unsafe.Add(ref dataPacket, offsetToStart), ConstData.EventCodeLength);

                eventData.EventCode = Encoding.ASCII.GetString(eventCodeSpan).Trim('\0');

                actOffset += 4;

                // For all versions identically
                ExtractEventData2019(ref dataPacket, actOffset, eventData);

                retValue = GameVersion switch
                           {
                               2020 => ExtractEventData2020(ref dataPacket, actOffset, eventData),
                               2021 => ExtractEventData2021(ref dataPacket, actOffset, eventData),
                               2022 => ExtractEventData2022(ref dataPacket, actOffset, eventData),
                               2023 => ExtractEventData2023(ref dataPacket, actOffset, eventData),
                               2024 => ExtractEventData2024(ref dataPacket, actOffset, eventData),
                               2025 => ExtractEventData2025(ref dataPacket, actOffset, eventData),
                               2026 => ExtractEventData2025(ref dataPacket, actOffset, eventData),
                               _ => true
                           };
            }
            catch (Exception ex)
            {
                LastError = ex.ToString();
            }
        }

        return retValue;
    }

    /// <summary>
    /// Extract event data from packet, no changes since 2019
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventData">Event data</param>
    private void ExtractEventData2019(ref byte dataPacket, int actOffset, IEventDataBase eventData)
    {
        if (eventData.EventDetails is EventDataDetails2019 eventDetailsData)
        {
            switch (eventData.EventCode)
            {
                case "FTLP":
                    {
                        eventDetailsData.EventType = EventType.FastestLap;
                        eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetailsData.FastestLap = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "RTMT":
                    {
                        eventDetailsData.EventType = EventType.Retirement;
                        eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "TMPT":
                    {
                        eventDetailsData.EventType = EventType.TeamMateInPit;
                        eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "RCWN":
                    {
                        eventDetailsData.EventType = EventType.RaceWinner;
                        eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Extract F1 2020 relevant event data
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventData">Event data</param>
    /// <returns>Status</returns>
    private bool ExtractEventData2020(ref byte dataPacket, int actOffset, IEventDataBase eventData)
    {
        var retValue = false;

        if (eventData.EventDetails is EventDataDetails2020 eventDetailsData)
        {
            if (eventData.EventCode == "PENA")
            {
                eventDetailsData.EventType = EventType.Penalty;

                ushort penaltyType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
                penaltyType += 1;

                eventDetailsData.PenaltyType = (PenaltyType)Enum.ToObject(typeof(PenaltyType), penaltyType);

                actOffset += ConstData.TypeUInt8;

                ushort infringementType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
                infringementType += 1;

                // In F1 2022 two values are inserted and not appended
                if (infringementType >= 41)
                {
                    // InfringementType.FormationLapParking (41) was inserted in F1 2022
                    infringementType += 1;
                }

                if (infringementType >= 50)
                {
                    // InfringementType.ParcFermeChange (50) was inserted in F1 2022
                    infringementType += 1;
                }

                eventDetailsData.PenaltyInfringementType = (InfringementType)Enum.ToObject(typeof(InfringementType), infringementType);

                actOffset += ConstData.TypeUInt8;

                eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                eventDetailsData.PenaltyOtherVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                eventDetailsData.PenaltyTimeGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                eventDetailsData.PenaltyLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                eventDetailsData.PenaltyPlacesGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
            }

            if (eventData.EventCode == "SPTP")
            {
                eventDetailsData.EventType = EventType.SpeedTrap;
                eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                actOffset += ConstData.TypeUInt8;

                eventDetailsData.TopSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
            }

            retValue = true;
        }

        return retValue;
    }

    /// <summary>
    /// Extract F1 2021 relevant event data
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventData">Event data</param>
    /// <returns>Status</returns>
    private bool ExtractEventData2021(ref byte dataPacket, int actOffset, IEventDataBase eventData)
    {
        var retValue = false;

        if (eventData.EventDetails is EventDataDetails2021 eventDetails21Data)
        {
            switch (eventData.EventCode)
            {
                case "PENA":
                    {
                        eventDetails21Data.EventType = EventType.Penalty;

                        ExtractPenalty2021(ref dataPacket, actOffset, eventDetails21Data);
                    }
                    break;

                case "SPTP":
                    {
                        eventDetails21Data.EventType = EventType.SpeedTrap;
                        eventDetails21Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetails21Data.TopSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeFloat;

                        eventDetails21Data.IsTopSpeedOverallFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetails21Data.IsTopSpeedDriverFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "STLG":
                    {
                        eventDetails21Data.EventType = EventType.StartLights;
                        eventDetails21Data.StartLightsNumbers = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "DTSV":
                    {
                        eventDetails21Data.EventType = EventType.DriveThroughPenaltyServed;
                        eventDetails21Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "SGSV":
                    {
                        eventDetails21Data.EventType = EventType.StopAndGoPenaltyServed;
                        eventDetails21Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "FLBK":
                    {
                        eventDetails21Data.EventType = EventType.Flashback;
                        eventDetails21Data.FlashbackFrame = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt32;

                        eventDetails21Data.FlashbackSessionTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "BUTN":
                    {
                        eventDetails21Data.EventType = EventType.Buttons;
                        eventDetails21Data.ButtonsTriggered = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;
            }

            retValue = true;
        }

        return retValue;
    }

    /// <summary>
    /// Extract F1 2022 relevant event data
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventData">Event data</param>
    /// <returns>Status</returns>
    private bool ExtractEventData2022(ref byte dataPacket, int actOffset, IEventDataBase eventData)
    {
        var retValue = false;

        if (eventData.EventDetails is EventDataDetails2022 eventDetails22Data)
        {
            switch (eventData.EventCode)
            {
                case "PENA":
                    {
                        eventDetails22Data.EventType = EventType.Penalty;

                        ExtractPenalty2022(ref dataPacket, actOffset, eventDetails22Data);
                    }
                    break;

                case "SPTP":
                    {
                        eventDetails22Data.EventType = EventType.SpeedTrap;

                        ExtractSpeedTrap2022(ref dataPacket, actOffset, eventDetails22Data);
                    }
                    break;

                case "STLG":
                    {
                        eventDetails22Data.EventType = EventType.StartLights;

                        eventDetails22Data.StartLightsNumbers = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "DTSV":
                    {
                        eventDetails22Data.EventType = EventType.DriveThroughPenaltyServed;
                        eventDetails22Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "SGSV":
                    {
                        eventDetails22Data.EventType = EventType.StopAndGoPenaltyServed;
                        eventDetails22Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "FLBK":
                    {
                        eventDetails22Data.EventType = EventType.Flashback;
                        eventDetails22Data.FlashbackFrame = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt32;

                        eventDetails22Data.FlashbackSessionTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "BUTN":
                    {
                        eventDetails22Data.EventType = EventType.Buttons;
                        eventDetails22Data.ButtonsTriggered = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;
            }

            retValue = true;
        }

        return retValue;
    }

    /// <summary>
    /// Extract F1 2023 relevant event data
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventData">Event data</param>
    /// <returns>Status</returns>
    private bool ExtractEventData2023(ref byte dataPacket, int actOffset, IEventDataBase eventData)
    {
        var retValue = false;

        if (eventData.EventDetails is EventDataDetails2023 eventDetails23Data)
        {
            switch (eventData.EventCode)
            {
                case "PENA":
                    {
                        eventDetails23Data.EventType = EventType.Penalty;

                        ExtractPenalty2023(ref dataPacket, actOffset, eventDetails23Data);
                    }
                    break;

                case "SPTP":
                    {
                        eventDetails23Data.EventType = EventType.SpeedTrap;

                        ExtractSpeedTrap2023(ref dataPacket, actOffset, eventDetails23Data);
                    }
                    break;

                case "STLG":
                    {
                        eventDetails23Data.EventType = EventType.StartLights;
                        eventDetails23Data.StartLightsNumbers = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "DTSV":
                    {
                        eventDetails23Data.EventType = EventType.DriveThroughPenaltyServed;
                        eventDetails23Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "SGSV":
                    {
                        eventDetails23Data.EventType = EventType.StopAndGoPenaltyServed;
                        eventDetails23Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "FLBK":
                    {
                        eventDetails23Data.EventType = EventType.Flashback;
                        eventDetails23Data.FlashbackFrame = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt32;

                        eventDetails23Data.FlashbackSessionTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "BUTN":
                    {
                        eventDetails23Data.EventType = EventType.Buttons;
                        eventDetails23Data.ButtonsTriggered = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "RDFL":
                    {
                        eventDetails23Data.EventType = EventType.RedFlag;
                        eventDetails23Data.IsRedFlag = true;
                    }
                    break;

                case "OVTK":
                    {
                        eventDetails23Data.EventType = EventType.Overtake;
                        eventDetails23Data.OvertakingVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetails23Data.BeingOvertakenVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;
            }

            retValue = true;
        }

        return retValue;
    }

    /// <summary>
    /// Extract F1 2024 relevant event data
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventData">Event data</param>
    /// <returns>Status</returns>
    private bool ExtractEventData2024(ref byte dataPacket, int actOffset, IEventDataBase eventData)
    {
        var retValue = false;

        if (eventData.EventDetails is EventDataDetails2024 eventDetailsData)
        {
            switch (eventData.EventCode)
            {
                case "PENA":
                    {
                        eventDetailsData.EventType = EventType.Penalty;

                        ExtractPenalty2024(ref dataPacket, actOffset, eventDetailsData);
                    }
                    break;

                case "SPTP":
                    {
                        eventDetailsData.EventType = EventType.SpeedTrap;

                        ExtractSpeedTrap2024(ref dataPacket, actOffset, eventDetailsData);
                    }
                    break;

                case "STLG":
                    {
                        eventDetailsData.EventType = EventType.StartLights;
                        eventDetailsData.StartLightsNumbers = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "DTSV":
                    {
                        eventDetailsData.EventType = EventType.DriveThroughPenaltyServed;
                        eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "SGSV":
                    {
                        eventDetailsData.EventType = EventType.StopAndGoPenaltyServed;
                        eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "FLBK":
                    {
                        eventDetailsData.EventType = EventType.Flashback;
                        eventDetailsData.FlashbackFrame = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt32;

                        eventDetailsData.FlashbackSessionTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "BUTN":
                    {
                        eventDetailsData.EventType = EventType.Buttons;
                        eventDetailsData.ButtonsTriggered = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "RDFL":
                    {
                        eventDetailsData.EventType = EventType.RedFlag;
                        eventDetailsData.IsRedFlag = true;
                    }
                    break;

                case "OVTK":
                    {
                        eventDetailsData.EventType = EventType.Overtake;
                        eventDetailsData.OvertakingVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetailsData.BeingOvertakenVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "SCAR":
                    {
                        eventDetailsData.EventType = EventType.SafetyCar;

                        var safetyCarType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        eventDetailsData.SafetyCarType = (SafetyCarStatus)Enum.ToObject(typeof(SafetyCarStatus), safetyCarType);

                        actOffset += ConstData.TypeUInt8;

                        var eventType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
                        eventType += 1;

                        eventDetailsData.SafetyCarEvent = (SafetyCarEventType)Enum.ToObject(typeof(SafetyCarEventType), eventType);
                    }
                    break;

                case "COLL":
                    {
                        eventDetailsData.EventType = EventType.Collision;
                        eventDetailsData.CollisionVehicleIndex1 = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetailsData.CollisionVehicleIndex2 = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;
            }

            retValue = true;
        }

        return retValue;
    }

    /// <summary>
    /// Extract F1 2025 relevant event data
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventData">Event data</param>
    /// <returns>Status</returns>
    private bool ExtractEventData2025(ref byte dataPacket, int actOffset, IEventDataBase eventData)
    {
        var retValue = false;

        if (eventData.EventDetails is EventDataDetails2025 eventDetailsData)
        {
            switch (eventData.EventCode)
            {
                case "RTMT":
                    {
                        eventDetailsData.EventType = EventType.Retirement;

                        actOffset += ConstData.TypeUInt8;

                        var retirementReason = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        eventDetailsData.RetirementReason = (ResultReason)Enum.ToObject(typeof(ResultReason), retirementReason + 1);
                    }
                    break;

                case "PENA":
                    {
                        eventDetailsData.EventType = EventType.Penalty;

                        ExtractPenalty2025(ref dataPacket, actOffset, eventDetailsData);
                    }
                    break;

                case "SPTP":
                    {
                        eventDetailsData.EventType = EventType.SpeedTrap;

                        ExtractSpeedTrap2025(ref dataPacket, actOffset, eventDetailsData);
                    }
                    break;

                case "STLG":
                    {
                        eventDetailsData.EventType = EventType.StartLights;

                        eventDetailsData.StartLightsNumbers = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "DTSV":
                    {
                        eventDetailsData.EventType = EventType.DriveThroughPenaltyServed;
                        eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "SGSV":
                    {
                        eventDetailsData.EventType = EventType.StopAndGoPenaltyServed;
                        eventDetailsData.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetailsData.StopAndGoPenaltyTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "FLBK":
                    {
                        eventDetailsData.EventType = EventType.Flashback;
                        eventDetailsData.FlashbackFrame = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt32;

                        eventDetailsData.FlashbackSessionTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "BUTN":
                    {
                        eventDetailsData.EventType = EventType.Buttons;
                        eventDetailsData.ButtonsTriggered = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "RDFL":
                    {
                        eventDetailsData.EventType = EventType.RedFlag;
                        eventDetailsData.IsRedFlag = true;
                    }
                    break;

                case "OVTK":
                    {
                        eventDetailsData.EventType = EventType.Overtake;
                        eventDetailsData.OvertakingVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetailsData.BeingOvertakenVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "SCAR":
                    {
                        eventDetailsData.EventType = EventType.SafetyCar;

                        var safetyCarType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        eventDetailsData.SafetyCarType = (SafetyCarStatus)Enum.ToObject(typeof(SafetyCarStatus), safetyCarType);

                        actOffset += ConstData.TypeUInt8;

                        var eventType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
                        eventType += 1;

                        eventDetailsData.SafetyCarEvent = (SafetyCarEventType)Enum.ToObject(typeof(SafetyCarEventType), eventType);
                    }
                    break;

                case "COLL":
                    {
                        eventDetailsData.EventType = EventType.Collision;
                        eventDetailsData.CollisionVehicleIndex1 = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        eventDetailsData.CollisionVehicleIndex2 = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
                    }
                    break;

                case "DRSD":
                    {
                        eventDetailsData.EventType = EventType.DrsDisabled;

                        var drsDisabledReason = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        eventDetailsData.DrsDisabledReason = (DrsDisabledReason)Enum.ToObject(typeof(DrsDisabledReason), drsDisabledReason + 1);
                    }
                    break;
            }

            retValue = true;
        }

        return retValue;
    }

    /// <summary>
    /// Extract penalty data from F1 2021
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails21Data">Eventdata</param>
    private void ExtractPenalty2021(ref byte dataPacket, int actOffset, EventDataDetails2021 eventDetails21Data)
    {
        ushort penaltyType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        penaltyType += 1;

        eventDetails21Data.PenaltyType = (PenaltyType)Enum.ToObject(typeof(PenaltyType), penaltyType);

        actOffset += ConstData.TypeUInt8;

        ushort infringementType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        infringementType += 1;

        // In F1 2022 two values are inserted and not appended
        if (infringementType >= 41)
        {
            // InfringementType.FormationLapParking (41) was inserted in F1 2022
            infringementType += 1;
        }

        if (infringementType >= 50)
        {
            // InfringementType.ParcFermeChange (50) was inserted in F1 2022
            infringementType += 1;
        }

        eventDetails21Data.PenaltyInfringementType = (InfringementType)Enum.ToObject(typeof(InfringementType), infringementType);

        actOffset += ConstData.TypeUInt8;

        eventDetails21Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails21Data.PenaltyOtherVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails21Data.PenaltyTimeGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails21Data.PenaltyLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails21Data.PenaltyPlacesGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract penalty data from F1 2022
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails22Data">Eventdata</param>
    private void ExtractPenalty2022(ref byte dataPacket, int actOffset, EventDataDetails2022 eventDetails22Data)
    {
        ushort penaltyType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        penaltyType += 1;

        eventDetails22Data.PenaltyType = (PenaltyType)Enum.ToObject(typeof(PenaltyType), penaltyType);

        actOffset += ConstData.TypeUInt8;

        ushort infringementType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        infringementType += 1;

        eventDetails22Data.PenaltyInfringementType = (InfringementType)Enum.ToObject(typeof(InfringementType), infringementType);

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.PenaltyOtherVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.PenaltyTimeGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.PenaltyLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.PenaltyPlacesGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract penalty data in F1 2023
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails23Data">Eventdata</param>
    private void ExtractPenalty2023(ref byte dataPacket, int actOffset, EventDataDetails2023 eventDetails23Data)
    {
        ushort penaltyType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        penaltyType += 1;

        eventDetails23Data.PenaltyType = (PenaltyType)Enum.ToObject(typeof(PenaltyType), penaltyType);

        actOffset += ConstData.TypeUInt8;

        ushort infringementType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        infringementType += 1;

        eventDetails23Data.PenaltyInfringementType = (InfringementType)Enum.ToObject(typeof(InfringementType), infringementType);

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.PenaltyOtherVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.PenaltyTimeGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.PenaltyLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.PenaltyPlacesGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract penalty data in F1 2024
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails24Data">Eventdata</param>
    private void ExtractPenalty2024(ref byte dataPacket, int actOffset, EventDataDetails2024 eventDetails24Data)
    {
        ushort penaltyType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        penaltyType += 1;

        eventDetails24Data.PenaltyType = (PenaltyType)Enum.ToObject(typeof(PenaltyType), penaltyType);

        actOffset += ConstData.TypeUInt8;

        ushort infringementType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        infringementType += 1;

        eventDetails24Data.PenaltyInfringementType = (InfringementType)Enum.ToObject(typeof(InfringementType), infringementType);

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.PenaltyOtherVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.PenaltyTimeGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.PenaltyLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.PenaltyPlacesGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract penalty data in F1 2025
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails25Data">Eventdata</param>
    private void ExtractPenalty2025(ref byte dataPacket, int actOffset, EventDataDetails2025 eventDetails25Data)
    {
        ushort penaltyType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        penaltyType += 1;

        eventDetails25Data.PenaltyType = (PenaltyType)Enum.ToObject(typeof(PenaltyType), penaltyType);

        actOffset += ConstData.TypeUInt8;

        ushort infringementType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        // First value of game is zero based, enumeration zero value is a default value, corresponding type is one for first value and so on
        infringementType += 1;

        eventDetails25Data.PenaltyInfringementType = (InfringementType)Enum.ToObject(typeof(InfringementType), infringementType);

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.PenaltyOtherVehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.PenaltyTimeGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.PenaltyLapNumber = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.PenaltyPlacesGained = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract Speed trap data in F1 2022
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails22Data">Eventdata</param>
    private void ExtractSpeedTrap2022(ref byte dataPacket, int actOffset, EventDataDetails2022 eventDetails22Data)
    {
        eventDetails22Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.TopSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeFloat;

        eventDetails22Data.IsOverallFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.IsDriverFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.FastestVehicleIndexInSession = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails22Data.FastestSpeedInSession = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract Speed trap data in F1 2023
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails23Data">Eventdata</param>
    private void ExtractSpeedTrap2023(ref byte dataPacket, int actOffset, EventDataDetails2023 eventDetails23Data)
    {
        eventDetails23Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.TopSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeFloat;

        eventDetails23Data.IsOverallFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.IsDriverFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.FastestVehicleIndexInSession = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails23Data.FastestSpeedInSession = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract Speed trap data in F1 2024
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails24Data">Eventdata</param>
    private void ExtractSpeedTrap2024(ref byte dataPacket, int actOffset, EventDataDetails2024 eventDetails24Data)
    {
        eventDetails24Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.TopSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeFloat;

        eventDetails24Data.IsOverallFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.IsDriverFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.FastestVehicleIndexInSession = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails24Data.FastestSpeedInSession = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract Speed trap data in F1 2025
    /// </summary>
    /// <param name="dataPacket">Data packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="eventDetails25Data">Eventdata</param>
    private void ExtractSpeedTrap2025(ref byte dataPacket, int actOffset, EventDataDetails2025 eventDetails25Data)
    {
        eventDetails25Data.VehicleIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.TopSpeed = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeFloat;

        eventDetails25Data.IsOverallFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.IsDriverFastestInSession = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.FastestVehicleIndexInSession = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        eventDetails25Data.FastestSpeedInSession = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    #endregion // Private methods
}