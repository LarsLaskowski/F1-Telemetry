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
/// Class to extract a session data object from received packet
/// </summary>
/// <param name="packetHeader">Header of packet</param>
internal class PacketToSessionData(PacketHeader packetHeader) : PacketToXBase(packetHeader)
{
    #region Methods

    /// <summary>
    /// Get session data from received data packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <returns>Session data object</returns>
    public object? ExtractSessionDataPacket(ReadOnlySpan<byte> dataPacket)
    {
        object? sessionObject = null;

        using var currentActivity = AppActivity.SrvSource.StartActivity("PacketToSession");

        LastError = string.Empty;

        if (dataPacket.Length > 0 && HasValidPacketLength(dataPacket.Length, GetExpectedPayloadSize()))
        {
            ref var memRef = ref MemoryMarshal.GetReference(dataPacket);

            sessionObject = GameVersion switch
                            {
                                2019 => GetSessionData2019(ref memRef, dataPacket.Length),
                                2020 => GetSessionData2020(ref memRef, dataPacket.Length),
                                2021 => GetSessionData2021(ref memRef, dataPacket.Length),
                                2022 => GetSessionData2022(ref memRef, dataPacket.Length),
                                2023 => GetSessionData2023(ref memRef, dataPacket.Length),
                                2024 => GetSessionData2024(ref memRef, dataPacket.Length),
                                2025 => GetSessionData2025(ref memRef, dataPacket.Length),
                                2026 => GetSessionData2026(ref memRef, dataPacket.Length),
                                _ => null
                            };
        }

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        if (string.IsNullOrWhiteSpace(LastError) == false)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error, LastError);
        }

        return sessionObject;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Returns the expected session payload size for the current game version
    /// </summary>
    /// <returns>Expected payload size in bytes without the packet header</returns>
    private int GetExpectedPayloadSize()
    {
        return GameVersion switch
               {
                   2019 => ConstData.F12019SessionSize,
                   2020 => ConstData.F12020SessionSize,
                   2021 => ConstData.F12021SessionSize,
                   2022 => ConstData.F12022SessionSize,
                   2023 => ConstData.F12023SessionSize,
                   2024 => ConstData.F12024SessionSize,
                   2025 => ConstData.F12025SessionSize,
                   2026 => ConstData.F12026SessionSize,
                   _ => 0
               };
    }

    /// <summary>
    /// Get session data from F1 2019 session packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Session data object</returns>
    private object? GetSessionData2019(ref byte dataPacket, int packetLength)
    {
        object? sessionObject = null;
        var sessionObject2019 = new SessionData(PacketHeader, new SessionData2019());

        if (ExtractSessionData2019(ref dataPacket, packetLength, HeaderSize, sessionObject2019.PacketData))
        {
            sessionObject = sessionObject2019;
        }

        return sessionObject;
    }

    /// <summary>
    /// Get session data from F1 2020 session packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Session data object</returns>
    private object? GetSessionData2020(ref byte dataPacket, int packetLength)
    {
        object? sessionObject = null;
        var sessionObject2020 = new SessionData(PacketHeader, new SessionData2020());

        if (ExtractSessionData2020(ref dataPacket, packetLength, HeaderSize, sessionObject2020.PacketData))
        {
            sessionObject = sessionObject2020;
        }

        return sessionObject;
    }

    /// <summary>
    /// Get session data from F1 2021 session packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Session data object</returns>
    private object? GetSessionData2021(ref byte dataPacket, int packetLength)
    {
        object? sessionObject = null;
        var sessionObject2021 = new SessionData(PacketHeader, new SessionData2021());

        if (ExtractSessionData2021(ref dataPacket, packetLength, HeaderSize, sessionObject2021.PacketData))
        {
            sessionObject = sessionObject2021;
        }

        return sessionObject;
    }

    /// <summary>
    /// Get session data from F1 2022 session packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Session data object</returns>
    private object? GetSessionData2022(ref byte dataPacket, int packetLength)
    {
        object? sessionObject = null;
        var sessionObject2022 = new SessionData(PacketHeader, new SessionData2022());

        if (ExtractSessionData2022(ref dataPacket, packetLength, HeaderSize, sessionObject2022.PacketData))
        {
            sessionObject = sessionObject2022;
        }

        return sessionObject;
    }

    /// <summary>
    /// Get session data from F1 2023 session packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Session data object</returns>
    private object? GetSessionData2023(ref byte dataPacket, int packetLength)
    {
        object? sessionObject = null;
        var sessionObject2023 = new SessionData(PacketHeader, new SessionData2023());

        if (ExtractSessionData2023(ref dataPacket, packetLength, HeaderSize, sessionObject2023.PacketData))
        {
            sessionObject = sessionObject2023;
        }

        return sessionObject;
    }

    /// <summary>
    /// Get session data from F1 2024 session packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Session data object</returns>
    private object? GetSessionData2024(ref byte dataPacket, int packetLength)
    {
        object? sessionObject = null;
        var sessionObject2024 = new SessionData(PacketHeader, new SessionData2024());

        if (ExtractSessionData2024(ref dataPacket, packetLength, HeaderSize, sessionObject2024.PacketData))
        {
            sessionObject = sessionObject2024;
        }

        return sessionObject;
    }

    /// <summary>
    /// Get session data from F1 2025 session packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Session data object</returns>
    private object? GetSessionData2025(ref byte dataPacket, int packetLength)
    {
        object? sessionObject = null;
        var sessionObject2025 = new SessionData(PacketHeader, new SessionData2025());

        if (ExtractSessionData2024(ref dataPacket, packetLength, HeaderSize, sessionObject2025.PacketData))
        {
            sessionObject = sessionObject2025;
        }

        return sessionObject;
    }

    /// <summary>
    /// Get session data from F1 2026 session packet
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <returns>Session data object</returns>
    private object? GetSessionData2026(ref byte dataPacket, int packetLength)
    {
        object? sessionObject = null;
        var sessionObject2026 = new SessionData(PacketHeader, new SessionData2026());

        if (ExtractSessionData2026(ref dataPacket, packetLength, HeaderSize, sessionObject2026.PacketData))
        {
            sessionObject = sessionObject2026;
        }

        return sessionObject;
    }

    /// <summary>
    /// Match track id to name
    /// </summary>
    /// <param name="trackId">Id of track</param>
    /// <returns>Name of track</returns>
    private string MatchTrackIdToName(ushort trackId)
    {
        string trackName = trackId switch
                           {
                               0 => "Melbourne",
                               1 => "Paul Ricard",
                               2 => "Shanghai",
                               3 => "Sakhir (Bahrain)",
                               4 => "Catalunya",
                               5 => "Monaco",
                               6 => "Montreal",
                               7 => "Silverstone",
                               8 => "Hockenheim",
                               9 => "Hungaroring",
                               10 => "Spa",
                               11 => "Monza",
                               12 => "Singapore",
                               13 => "Suzuka",
                               14 => "Abu Dhabi",
                               15 => "Texas",
                               16 => "Brazil",
                               17 => "Austria",
                               18 => "Sochi",
                               19 => "Mexico",
                               20 => "Baku (Azerbaijan)",
                               21 => "Sakhir Short",
                               22 => "Silverstone Short",
                               23 => "Texas Short",
                               24 => "Suzuka Short",
                               25 => "Hanoi",
                               26 => "Zandvoort",
                               27 => "Imola",
                               28 => "Portimão",
                               29 => "Jeddah",
                               30 => "Miami",
                               31 => "Las Vegas",
                               32 => "Losail",
                               39 => "Silverstone (Reverse)",
                               40 => "Austria (Reverse)",
                               41 => "Zandvoort (Reverse)",
                               42 => "Madrid",
                               _ => "Unknown",
                           };

        return trackName;
    }

    /// <summary>
    /// Assign base fields which are equal in all game versions
    /// </summary>
    /// <param name="dataPacket">Received data packet</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Offset to start</param>
    /// <param name="sessionData">Session data object</param>
    /// <returns>Last offset</returns>
    private int ExtractBaseSessionData(ref byte dataPacket, int packetLength, int offsetToStart, ISessionDataBase sessionData)
    {
        var actOffset = offsetToStart;

        if (sessionData != null && packetLength >= offsetToStart + ConstData.F12019SessionSize)
        {
            var weather = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.Weather = (WeatherCondition)Enum.ToObject(typeof(WeatherCondition), weather);

            actOffset += ConstData.TypeUInt8;

            sessionData.TrackTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeInt8;

            sessionData.AirTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeInt8;

            sessionData.TotalLaps = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.TrackLength = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            ushort sessionType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionType = AdjustSessionType(sessionType);

            sessionData.SessionType = (SessionType)Enum.ToObject(typeof(SessionType), sessionType);

            actOffset += ConstData.TypeUInt8;

            sessionData.TrackId = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.TrackName = MatchTrackIdToName(sessionData.TrackId);

            actOffset += ConstData.TypeInt8;

            var formula = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.FormulaType = (Formula)Enum.ToObject(typeof(Formula), formula);

            actOffset += ConstData.TypeUInt8;

            sessionData.SessionTimeLeft = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            sessionData.SessionDuration = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt16;

            sessionData.PitSpeedLimit = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.IsGamePaused = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.IsSpectating = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.SpectatorCarIndex = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.IsSliProNativeSupport = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.MarshalZones = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var zoneOffset = actOffset;

            // Clamp the packet-provided zone count to the fixed packet layout so manipulated values cannot cause reads past the packet
            var marshalZoneCount = Math.Min(sessionData.MarshalZones, sessionData.MarshalZone.Length);

            for (int marshalZoneIndex = 0; marshalZoneIndex < marshalZoneCount; ++marshalZoneIndex)
            {
                MarshalZone? marshalZone = new()
                                           {
                                               ZoneStart = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, zoneOffset))
                                           };

                zoneOffset += ConstData.TypeFloat;

                var zoneFlag = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, zoneOffset));

                marshalZone.ZoneFlag = (ZoneFlagColor)Enum.ToObject(typeof(ZoneFlagColor), zoneFlag);

                zoneOffset += ConstData.TypeInt8;

                sessionData.MarshalZone[marshalZoneIndex] = marshalZone;
            }

            actOffset += ConstData.TotalMarshalZoneSize;

            var safetyCarStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.SafetyCar = (SafetyCarStatus)Enum.ToObject(typeof(SafetyCarStatus), safetyCarStatus);

            actOffset += ConstData.TypeUInt8;

            sessionData.IsNetworkGame = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.IsRecordable = IsSessionRecordable(sessionData);
        }

        return actOffset;
    }

    /// <summary>
    /// Check wether the session is recordable or not
    /// </summary>
    /// <param name="sessionData">Session data</param>
    /// <returns>Is recordable?</returns>
    private bool IsSessionRecordable(ISessionDataBase sessionData)
    {
        var isRecordable = true;

        // Time trials or unknown sessions should not be recorded
        if (sessionData.SessionType is SessionType.TimeTrial or SessionType.Unknown)
        {
            isRecordable = false;
        }

        // Supercars or E-Sports should not be recorded
        if (sessionData.FormulaType is Formula.SuperCars or Formula.ESports)
        {
            isRecordable = false;
        }

        // Network games are not recordable
        if (sessionData.IsNetworkGame)
        {
            isRecordable = false;
        }

        if (isRecordable && sessionData.SafetyCar == SafetyCarStatus.FormationLap)
        {
            isRecordable = false;
        }

        return isRecordable;
    }

    /// <summary>
    /// Converts received data to a session object for F1 2019
    /// </summary>
    /// <param name="dataPacket">Recevied data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionDataBase">Base session data object</param>
    /// <returns>Status</returns>
    private bool ExtractSessionData2019(ref byte dataPacket, int packetLength, int offsetToStart, ISessionDataBase? sessionDataBase)
    {
        var retValue = false;

        if (sessionDataBase != null)
        {
            try
            {
                ExtractBaseSessionData(ref dataPacket, packetLength, offsetToStart, sessionDataBase);

                retValue = true;
            }
            catch
            {
                // Ignore exceptions in this step
            }
        }

        return retValue;
    }

    /// <summary>
    /// Converts received data to a session object for F1 2020
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionDataBase">Base session data object</param>
    /// <returns>Status</returns>
    private bool ExtractSessionData2020(ref byte dataPacket, int packetLength, int offsetToStart, ISessionDataBase? sessionDataBase)
    {
        var retValue = false;

        if (sessionDataBase is SessionData2020 sessionData)
        {
            try
            {
                var actOffset = ExtractBaseSessionData(ref dataPacket, packetLength, offsetToStart, sessionDataBase);

                if (actOffset > offsetToStart)
                {
                    sessionData.NumberWeatherForecastSamples = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    ExtractWeatherForecast2020(ref dataPacket, actOffset, sessionData);

                    retValue = true;
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
    /// Converts received data to a session object for F1 2021
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionDataBase">Base session data object</param>
    /// <returns>Status</returns>
    private bool ExtractSessionData2021(ref byte dataPacket, int packetLength, int offsetToStart, ISessionDataBase? sessionDataBase)
    {
        var retValue = false;

        if (sessionDataBase != null)
        {
            try
            {
                var actOffset = ExtractBaseSessionData(ref dataPacket, packetLength, offsetToStart, sessionDataBase);

                if (actOffset > offsetToStart && sessionDataBase is SessionData2021 sessionData)
                {
                    sessionData.NumberWeatherForecastSamples = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    actOffset = ExtractWeatherForecast2021(ref dataPacket, actOffset, sessionDataBase);

                    var accuracy = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.ForecastAccuracy = (ForecastAccuracy)Enum.ToObject(typeof(ForecastAccuracy), accuracy);

                    actOffset += ConstData.TypeUInt8;

                    sessionData.AiDifficulty = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.SeasonLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt32;

                    sessionData.WeekendLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt32;

                    sessionData.SessionLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt32;

                    sessionData.PitStopWindowIdealLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.PitStopWindowLatestLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.PitStopRejoinPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsSteeringAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var brakingAssist = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.BrakingAssist = (BrakingAssist)Enum.ToObject(typeof(BrakingAssist), brakingAssist);

                    actOffset += ConstData.TypeUInt8;

                    var gearboxAssist = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.GearboxAssist = (GearboxAssist)Enum.ToObject(typeof(GearboxAssist), gearboxAssist);

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsPitAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsPitReleaseAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsERSAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsDRSAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var raceLine = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.DynamicRaceLine = (DynamicRaceLine)Enum.ToObject(typeof(DynamicRaceLine), raceLine);

                    actOffset += ConstData.TypeUInt8;

                    var raceLineType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.DynamicRaceLineType = (DynamicRaceLineType)Enum.ToObject(typeof(DynamicRaceLineType), raceLineType);

                    retValue = true;
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
    /// Converts received data to a session object for F1 2022
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionDataBase">Base session data object</param>
    /// <returns>Status</returns>
    private bool ExtractSessionData2022(ref byte dataPacket, int packetLength, int offsetToStart, ISessionDataBase? sessionDataBase)
    {
        var retValue = false;

        if (sessionDataBase is SessionData2022 sessionData)
        {
            try
            {
                var actOffset = ExtractBaseSessionData(ref dataPacket, packetLength, offsetToStart, sessionDataBase);

                if (actOffset > offsetToStart)
                {
                    sessionData.NumberWeatherForecastSamples = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    actOffset = ExtractWeatherForecast2022(ref dataPacket, actOffset, sessionDataBase);

                    var accuracy = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.ForecastAccuracy = (ForecastAccuracy)Enum.ToObject(typeof(ForecastAccuracy), accuracy);

                    actOffset += ConstData.TypeUInt8;

                    sessionData.AiDifficulty = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.SeasonLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt32;

                    sessionData.WeekendLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt32;

                    sessionData.SessionLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt32;

                    sessionData.PitStopWindowIdealLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.PitStopWindowLatestLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.PitStopRejoinPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsSteeringAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var brakingAssist = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.BrakingAssist = (BrakingAssist)Enum.ToObject(typeof(BrakingAssist), brakingAssist);

                    actOffset += ConstData.TypeUInt8;

                    var gearboxAssist = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.GearboxAssist = (GearboxAssist)Enum.ToObject(typeof(GearboxAssist), gearboxAssist);

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsPitAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsPitReleaseAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsERSAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    sessionData.IsDRSAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var raceLine = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.DynamicRaceLine = (DynamicRaceLine)Enum.ToObject(typeof(DynamicRaceLine), raceLine);

                    actOffset += ConstData.TypeUInt8;

                    var raceLineType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.DynamicRaceLineType = (DynamicRaceLineType)Enum.ToObject(typeof(DynamicRaceLineType), raceLineType);

                    actOffset += ConstData.TypeUInt8;

                    // Game mode - uint8
                    var gameMode = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.GameMode = (GameMode)Enum.ToObject(typeof(GameMode), gameMode);

                    actOffset += ConstData.TypeUInt8;

                    // Rule set - uint8
                    var ruleSet = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.RuleSet = (RuleSet)Enum.ToObject(typeof(RuleSet), ruleSet);

                    actOffset += ConstData.TypeUInt8;

                    // Time of day - uint32
                    sessionData.LocalTimeOfDay = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt32;

                    // Session length - uint8
                    var sessionLength = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData.SessionLength = (SessionLength)Enum.ToObject(typeof(SessionLength), sessionLength);

                    retValue = true;
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
    /// Converts received data to a session object for F1 2023
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionDataBase">Base session data object</param>
    /// <returns>Status</returns>
    private bool ExtractSessionData2023(ref byte dataPacket, int packetLength, int offsetToStart, ISessionDataBase? sessionDataBase)
    {
        var retValue = false;

        if (sessionDataBase is SessionData2023)
        {
            try
            {
                var actOffset = ExtractBaseSessionData(ref dataPacket, packetLength, offsetToStart, sessionDataBase);

                if (actOffset > offsetToStart)
                {
                    ExtractSessionData2023Core(ref dataPacket, actOffset, sessionDataBase);

                    retValue = true;
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
    /// Converts received data to a session object for F1 2024
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionDataBase">Base session data object</param>
    /// <returns>Status</returns>
    private bool ExtractSessionData2024(ref byte dataPacket, int packetLength, int offsetToStart, ISessionDataBase? sessionDataBase)
    {
        var retValue = false;

        if (sessionDataBase is ISessionData2024 sessionData)
        {
            try
            {
                var actOffset = ExtractBaseSessionData(ref dataPacket, packetLength, offsetToStart, sessionDataBase);

                if (actOffset > offsetToStart)
                {
                    offsetToStart = actOffset;

                    actOffset = ExtractSessionData2023Core(ref dataPacket, actOffset, sessionDataBase);

                    if (actOffset > offsetToStart)
                    {
                        sessionData.EqualCarPerformance = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        var recoveryMode = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        sessionData.RecoveryMode = (RecoveryMode)Enum.ToObject(typeof(RecoveryMode), recoveryMode);

                        actOffset += ConstData.TypeUInt8;

                        var flashbackLimit = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        sessionData.FlashbackLimit = (FlashbackLimit)Enum.ToObject(typeof(FlashbackLimit), flashbackLimit);

                        actOffset += ConstData.TypeUInt8;

                        sessionData.IsRealisticSurfaceType = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.IsHardLowFuelMode = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.AssistedRaceStarts = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.TyreTemperatureWithCarcass = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.PitLaneTyreSimulation = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        var carDamage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        sessionData.CarDamage = (CarDamage)Enum.ToObject(typeof(CarDamage), carDamage);

                        actOffset += ConstData.TypeUInt8;

                        var carDamageRate = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        sessionData.CarDamageRate = (CarDamageRate)Enum.ToObject(typeof(CarDamageRate), carDamageRate);

                        actOffset += ConstData.TypeUInt8;

                        var collisions = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        sessionData.Collisions = (Collisions)Enum.ToObject(typeof(Collisions), collisions);

                        actOffset += ConstData.TypeUInt8;

                        sessionData.CollisionsOffForFirstLapOnly = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.MultiPlayerUnsafePitRelease = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.MultiPlayerOffForGriefing = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.IsStrictCornerCuttingStringency = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.ParcFermeRules = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        var pitStopExperience = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        sessionData.PitStopExperience = (PitStopExperience)Enum.ToObject(typeof(PitStopExperience), pitStopExperience);

                        actOffset += ConstData.TypeUInt8;

                        var safetyCar = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        sessionData.SafetyCarAppearance = (SafetyCarAppearance)Enum.ToObject(typeof(SafetyCarAppearance), safetyCar);

                        actOffset += ConstData.TypeUInt8;

                        sessionData.IsImmersiveSafetyCarExperience = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.WithFormationLap = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.IsImmersiveFormationLapExperience = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        var redFlags = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        sessionData.RedFlagAppearance = (RedFlagAppearance)Enum.ToObject(typeof(RedFlagAppearance), redFlags);

                        actOffset += ConstData.TypeUInt8;

                        sessionData.AffectsLicenceLevelSolo = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.AffectsLicenceLevelMultiPlayer = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        sessionData.SessionsInWeekend = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeUInt8;

                        actOffset = ExtractWeekendSessionTypes(ref dataPacket, actOffset, sessionDataBase);

                        sessionData.Sector2LapDistanceStart = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                        actOffset += ConstData.TypeFloat;

                        sessionData.Sector3LapDistanceStart = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

                        retValue = true;
                    }
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
    /// Converts received data to a session object for F1 2026
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="packetLength">Size of received packet</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionDataBase">Base session data object</param>
    /// <returns>Status</returns>
    private bool ExtractSessionData2026(ref byte dataPacket, int packetLength, int offsetToStart, ISessionDataBase? sessionDataBase)
    {
        var retValue = false;

        if (sessionDataBase is ISessionData2026 sessionData)
        {
            try
            {
                // The layout is identical to F1 2024/2025 up to and including the sector 3 lap distance start
                if (ExtractSessionData2024(ref dataPacket, packetLength, offsetToStart, sessionDataBase))
                {
                    var actOffset = offsetToStart + ConstData.F12025SessionSize;

                    ExtractSessionData2026Core(ref dataPacket, actOffset, sessionData);

                    retValue = true;
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
    /// Extract the additional session fields introduced in F1 2026
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionData">Session data object</param>
    private void ExtractSessionData2026Core(ref byte dataPacket, int offsetToStart, ISessionData2026 sessionData)
    {
        var actOffset = offsetToStart;

        sessionData.ActiveAeroTrackStatus = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        sessionData.NumberActiveAeroZonesFull = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        actOffset = ExtractActiveAeroZones(ref dataPacket, actOffset, sessionData.ActiveAeroZonesFull);

        sessionData.NumberActiveAeroZonesPartial = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        actOffset = ExtractActiveAeroZones(ref dataPacket, actOffset, sessionData.ActiveAeroZonesPartial);

        sessionData.NumberDrsZones = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        actOffset = ExtractDrsZones(ref dataPacket, actOffset, sessionData.DrsZones);

        sessionData.StartReactionTime = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeFloat;

        sessionData.AntiLockBrakesAssist = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        sessionData.TractionControlAssist = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        sessionData.DynamicRacingLineHiVis = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        sessionData.DynamicRacingLineColourBlind = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

        actOffset += ConstData.TypeUInt8;

        sessionData.RecurringRewindPrompt = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));
    }

    /// <summary>
    /// Extract the fixed-size active aero zone array (F1 2026 and newer)
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="zones">Target zone array</param>
    /// <returns>Current offset</returns>
    private int ExtractActiveAeroZones(ref byte dataPacket, int offsetToStart, ActiveAeroZone[] zones)
    {
        var actOffset = offsetToStart;

        for (int zone = 0; zone < zones.Length; zone++)
        {
            zones[zone] = new ActiveAeroZone
                          {
                              ZoneStart = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset)),
                              ZoneEnd = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset + ConstData.TypeFloat))
                          };

            actOffset += ConstData.ActiveAeroZoneSize;
        }

        return actOffset;
    }

    /// <summary>
    /// Extract the fixed-size DRS zone array (F1 2026 and newer)
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="zones">Target zone array</param>
    /// <returns>Current offset</returns>
    private int ExtractDrsZones(ref byte dataPacket, int offsetToStart, DrsZone[] zones)
    {
        var actOffset = offsetToStart;

        for (int zone = 0; zone < zones.Length; zone++)
        {
            zones[zone] = new DrsZone
                          {
                              ZoneStart = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset)),
                              ZoneEnd = Unsafe.ReadUnaligned<float>(ref Unsafe.Add(ref dataPacket, actOffset + ConstData.TypeFloat))
                          };

            actOffset += ConstData.DrsZoneSize;
        }

        return actOffset;
    }

    /// <summary>
    /// Converts received data to a session object for F1 2023/2024/2025
    /// </summary>
    /// <param name="dataPacket">Received data</param>
    /// <param name="offsetToStart">Data offset</param>
    /// <param name="sessionDataBase">Base session data object</param>
    /// <returns>Current offset</returns>
    private int ExtractSessionData2023Core(ref byte dataPacket, int offsetToStart, ISessionDataBase sessionDataBase)
    {
        var actOffset = offsetToStart;

        if (sessionDataBase is SessionData2023 sessionData)
        {
            sessionData.NumberWeatherForecastSamples = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            actOffset = ExtractWeatherForecast2023(ref dataPacket, actOffset, sessionDataBase);

            var accuracy = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.ForecastAccuracy = (ForecastAccuracy)Enum.ToObject(typeof(ForecastAccuracy), accuracy);

            actOffset += ConstData.TypeUInt8;

            sessionData.AiDifficulty = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.SeasonLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            sessionData.WeekendLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            sessionData.SessionLinkIdentifier = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            sessionData.PitStopWindowIdealLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.PitStopWindowLatestLap = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.PitStopRejoinPosition = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.IsSteeringAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var brakingAssist = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.BrakingAssist = (BrakingAssist)Enum.ToObject(typeof(BrakingAssist), brakingAssist);

            actOffset += ConstData.TypeUInt8;

            var gearboxAssist = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.GearboxAssist = (GearboxAssist)Enum.ToObject(typeof(GearboxAssist), gearboxAssist);

            actOffset += ConstData.TypeUInt8;

            sessionData.IsPitAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.IsPitReleaseAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.IsERSAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            sessionData.IsDRSAssist = Unsafe.ReadUnaligned<bool>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            var raceLine = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.DynamicRaceLine = (DynamicRaceLine)Enum.ToObject(typeof(DynamicRaceLine), raceLine);

            actOffset += ConstData.TypeUInt8;

            var raceLineType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.DynamicRaceLineType = (DynamicRaceLineType)Enum.ToObject(typeof(DynamicRaceLineType), raceLineType);

            actOffset += ConstData.TypeUInt8;

            // Game mode - uint8
            var gameMode = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.GameMode = (GameMode)Enum.ToObject(typeof(GameMode), gameMode);

            actOffset += ConstData.TypeUInt8;

            // Rule set - uint8
            var ruleSet = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.RuleSet = (RuleSet)Enum.ToObject(typeof(RuleSet), ruleSet);

            actOffset += ConstData.TypeUInt8;

            // Time of day - uint32
            sessionData.LocalTimeOfDay = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt32;

            // Session length - uint8
            var sessionLength = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.SessionLength = (SessionLength)Enum.ToObject(typeof(SessionLength), sessionLength);

            actOffset += ConstData.TypeUInt8;

            // Speed unit lead player - uint8
            var unit = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.SpeedUnit = (SpeedUnits)Enum.ToObject(typeof(SpeedUnits), unit);

            actOffset += ConstData.TypeUInt8;

            // Temperature unit lead player - uint8
            unit = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.TemperatureUnit = (TemperatureUnits)Enum.ToObject(typeof(TemperatureUnits), unit);

            actOffset += ConstData.TypeUInt8;

            // Speed unit secondary player - uint8
            unit = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.SpeedUnitSecondaryPlayer = (SpeedUnits)Enum.ToObject(typeof(SpeedUnits), unit);

            actOffset += ConstData.TypeUInt8;

            // Temperature unit secondary player - uint8
            unit = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            sessionData.TemperatureUnitSecondaryPlayer = (TemperatureUnits)Enum.ToObject(typeof(TemperatureUnits), unit);

            actOffset += ConstData.TypeUInt8;

            // Safety car periods - uint8
            sessionData.SafetyCarPeriods = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            // Virtual safety car periods
            sessionData.VirtualSafetyCarPeriods = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;

            // Red flags periods
            sessionData.RedFlags = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

            actOffset += ConstData.TypeUInt8;
        }

        return actOffset;
    }

    /// <summary>
    /// Extract weather information from packet
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="sessionDataBase">Session data</param>
    private void ExtractWeatherForecast2020(ref byte dataPacket, int actOffset, ISessionDataBase sessionDataBase)
    {
        if (sessionDataBase is SessionData2020 sessionData20)
        {
            for (int weatherSample = 0; weatherSample < sessionData20.WeatherForecastSamples.Length; weatherSample++)
            {
                if (weatherSample < sessionData20.NumberWeatherForecastSamples)
                {
                    sessionData20.WeatherForecastSamples[weatherSample] = new WeatherForecastSample();

                    var sessionType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    // Match time trial to be 13
                    if (sessionType == 12)
                    {
                        ++sessionType;
                    }

                    sessionData20.WeatherForecastSamples[weatherSample].SessionType = (SessionType)Enum.ToObject(typeof(SessionType), sessionType);

                    actOffset += ConstData.TypeUInt8;

                    sessionData20.WeatherForecastSamples[weatherSample].TimeOffset = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var weather = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData20.WeatherForecastSamples[weatherSample].Weather = (WeatherCondition)Enum.ToObject(typeof(WeatherCondition), weather);

                    actOffset += ConstData.TypeUInt8;

                    sessionData20.WeatherForecastSamples[weatherSample].TrackTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeInt8;

                    sessionData20.WeatherForecastSamples[weatherSample].AirTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeInt8;
                }
                else
                {
                    actOffset += ConstData.F12020WeatherForecastSize;
                }
            }
        }
    }

    /// <summary>
    /// Extract weather information from packet
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="sessionDataBase">Session data</param>
    /// <returns>New offset</returns>
    private int ExtractWeatherForecast2021(ref byte dataPacket, int actOffset, ISessionDataBase sessionDataBase)
    {
        if (sessionDataBase is SessionData2021 sessionData21)
        {
            for (int weatherSampleIndex = 0; weatherSampleIndex < sessionData21.WeatherForecastSamples.Length; weatherSampleIndex++)
            {
                if (weatherSampleIndex < sessionData21.NumberWeatherForecastSamples)
                {
                    sessionData21.WeatherForecastSamples[weatherSampleIndex] = new WeatherForecastSample();

                    var sessionType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData21.WeatherForecastSamples[weatherSampleIndex].SessionType = (SessionType)Enum.ToObject(typeof(SessionType), sessionType);

                    actOffset += ConstData.TypeUInt8;

                    sessionData21.WeatherForecastSamples[weatherSampleIndex].TimeOffset = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var weather = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData21.WeatherForecastSamples[weatherSampleIndex].Weather = (WeatherCondition)Enum.ToObject(typeof(WeatherCondition), weather);

                    actOffset += ConstData.TypeUInt8;

                    sessionData21.WeatherForecastSamples[weatherSampleIndex].TrackTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeInt8;

                    var tempChange = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData21.WeatherForecastSamples[weatherSampleIndex].TrackTemperatureChange = (TemperatureChange)Enum.ToObject(typeof(TemperatureChange), tempChange);

                    actOffset += ConstData.TypeInt8;

                    sessionData21.WeatherForecastSamples[weatherSampleIndex].AirTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeInt8;

                    tempChange = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData21.WeatherForecastSamples[weatherSampleIndex].AirTemperatureChange = (TemperatureChange)Enum.ToObject(typeof(TemperatureChange), tempChange);

                    actOffset += ConstData.TypeInt8;

                    sessionData21.WeatherForecastSamples[weatherSampleIndex].RainPercentage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
                else
                {
                    actOffset += ConstData.F12021WeatherForecastSize;
                }
            }
        }

        return actOffset;
    }

    /// <summary>
    /// Extract weather information from packet
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="sessionDataBase">Session data</param>
    /// <returns>New offset</returns>
    private int ExtractWeatherForecast2022(ref byte dataPacket, int actOffset, ISessionDataBase sessionDataBase)
    {
        if (sessionDataBase is SessionData2022 sessionData22)
        {
            for (int forecastSample = 0; forecastSample < sessionData22.WeatherForecastSamples.Length; forecastSample++)
            {
                if (forecastSample < sessionData22.NumberWeatherForecastSamples)
                {
                    sessionData22.WeatherForecastSamples[forecastSample] = new WeatherForecastSample();

                    var sessionType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData22.WeatherForecastSamples[forecastSample].SessionType = (SessionType)Enum.ToObject(typeof(SessionType), sessionType);

                    actOffset += ConstData.TypeUInt8;

                    sessionData22.WeatherForecastSamples[forecastSample].TimeOffset = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var weather = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData22.WeatherForecastSamples[forecastSample].Weather = (WeatherCondition)Enum.ToObject(typeof(WeatherCondition), weather);

                    actOffset += ConstData.TypeUInt8;

                    sessionData22.WeatherForecastSamples[forecastSample].TrackTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeInt8;

                    var tempChange = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData22.WeatherForecastSamples[forecastSample].TrackTemperatureChange = (TemperatureChange)Enum.ToObject(typeof(TemperatureChange), tempChange);

                    actOffset += ConstData.TypeInt8;

                    sessionData22.WeatherForecastSamples[forecastSample].AirTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeInt8;

                    tempChange = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData22.WeatherForecastSamples[forecastSample].AirTemperatureChange = (TemperatureChange)Enum.ToObject(typeof(TemperatureChange), tempChange);

                    actOffset += ConstData.TypeInt8;

                    sessionData22.WeatherForecastSamples[forecastSample].RainPercentage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
                else
                {
                    actOffset += ConstData.F12022WeatherForecastSize;
                }
            }
        }

        return actOffset;
    }

    /// <summary>
    /// Extract weather information from packet
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="sessionDataBase">Session data</param>
    /// <returns>New offset</returns>
    private int ExtractWeatherForecast2023(ref byte dataPacket, int actOffset, ISessionDataBase sessionDataBase)
    {
        if (sessionDataBase is SessionData2023 sessionData23)
        {
            for (int forecastSample = 0; forecastSample < sessionData23.WeatherForecastSamples.Length; forecastSample++)
            {
                if (forecastSample < sessionData23.NumberWeatherForecastSamples)
                {
                    sessionData23.WeatherForecastSamples[forecastSample] = new WeatherForecastSample();

                    var sessionType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData23.WeatherForecastSamples[forecastSample].SessionType = (SessionType)Enum.ToObject(typeof(SessionType), sessionType);

                    actOffset += ConstData.TypeUInt8;

                    sessionData23.WeatherForecastSamples[forecastSample].TimeOffset = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;

                    var weather = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData23.WeatherForecastSamples[forecastSample].Weather = (WeatherCondition)Enum.ToObject(typeof(WeatherCondition), weather);

                    actOffset += ConstData.TypeUInt8;

                    sessionData23.WeatherForecastSamples[forecastSample].TrackTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeInt8;

                    var tempChange = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData23.WeatherForecastSamples[forecastSample].TrackTemperatureChange = (TemperatureChange)Enum.ToObject(typeof(TemperatureChange), tempChange);

                    actOffset += ConstData.TypeInt8;

                    sessionData23.WeatherForecastSamples[forecastSample].AirTemperature = (sbyte)Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeInt8;

                    tempChange = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionData23.WeatherForecastSamples[forecastSample].AirTemperatureChange = (TemperatureChange)Enum.ToObject(typeof(TemperatureChange), tempChange);

                    actOffset += ConstData.TypeInt8;

                    sessionData23.WeatherForecastSamples[forecastSample].RainPercentage = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    actOffset += ConstData.TypeUInt8;
                }
                else
                {
                    actOffset += ConstData.F12023WeatherForecastSize;
                }
            }
        }

        return actOffset;
    }

    /// <summary>
    /// Extract weekend session types from packet
    /// </summary>
    /// <param name="dataPacket">Received packet</param>
    /// <param name="actOffset">Current offset</param>
    /// <param name="sessionDataBase">Session data</param>
    /// <returns>New offset</returns>
    private int ExtractWeekendSessionTypes(ref byte dataPacket, int actOffset, ISessionDataBase sessionDataBase)
    {
        if (sessionDataBase is ISessionData2024 sessionData)
        {
            for (int weekendStructureIndex = 0; weekendStructureIndex < sessionData.WeekendStructure.Length; weekendStructureIndex++)
            {
                if (weekendStructureIndex < sessionData.SessionsInWeekend)
                {
                    ushort sessionType = Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref dataPacket, actOffset));

                    sessionType = AdjustSessionType(sessionType);

                    sessionData.WeekendStructure[weekendStructureIndex] = (SessionType)Enum.ToObject(typeof(SessionType), sessionType);
                }

                actOffset += ConstData.TypeUInt8;
            }
        }

        return actOffset;
    }

    #endregion // Private methods
}