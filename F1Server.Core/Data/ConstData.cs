namespace F1Server.Core.Data;

/// <summary>
/// Definition of const sizes
/// </summary>
public static class ConstData
{
    #region Global consts

    /// <summary>
    /// Timeout in milliseconds
    /// </summary>
    public const int TimeoutInMs = 500;

    /// <summary>
    /// Timeout for statistics output
    /// </summary>
    public const int StatisticTimeoutInMs = 300000;

    #endregion // Global consts

    #region Consts F1 2019

    /// <summary>
    /// Headersize for F1 2019
    /// </summary>
    public const int F12019HeaderSize = 23;

    /// <summary>
    /// Size of session data
    /// </summary>
    public const int F12019SessionSize = 126;

    /// <summary>
    /// Size of event data
    /// </summary>
    public const int F12019EventSize = 9;

    /// <summary>
    /// Size of one lap data
    /// </summary>
    public const int F12019LapSize = 41;

    /// <summary>
    /// Size of one lap data x 20 cars
    /// </summary>
    public const int F12019TotalLapSize = 840;

    /// <summary>
    /// Size of one car telemetry packet
    /// </summary>
    public const int F12019CarTelemetrySize = 1324;

    /// <summary>
    /// Size of one participants packet
    /// </summary>
    public const int F12019ParticipantsSize = 1081;

    /// <summary>
    /// Size of one car status packet
    /// </summary>
    public const int F12019CarStatusSize = 1120;

    #endregion // Consts F1 2019

    #region Consts F1 2020

    /// <summary>
    /// Headersize for F1 2020, 2021 and 2022
    /// </summary>
    public const int F12020HeaderSize = 24;

    /// <summary>
    /// Size of session data
    /// </summary>
    public const int F12020SessionSize = 227;

    /// <summary>
    /// Size of event data
    /// </summary>
    public const int F12020EventSize = 11;

    /// <summary>
    /// Size of a single weather forecast sample
    /// </summary>
    public const int F12020WeatherForecastSize = 5;

    /// <summary>
    /// Size of one lap data
    /// </summary>
    public const int F12020LapSize = 53;

    /// <summary>
    /// Size of one lap data x 20 cars
    /// </summary>
    public const int F12020TotalLapSize = 1166;

    /// <summary>
    /// Size of one car telemetry packet
    /// </summary>
    public const int F12020CarTelemetrySize = 1283;

    /// <summary>
    /// Size of one participants packet
    /// </summary>
    public const int F12020ParticipantsSize = 1189;

    /// <summary>
    /// Size of one car status packet
    /// </summary>
    public const int F12020CarStatusSize = 1320;

    /// <summary>
    /// Size of one final classification car data packet
    /// </summary>
    public const int F12020FinalClassificationCarSize = 37;

    /// <summary>
    /// Size of one complete final classification packet
    /// </summary>
    public const int F12020FinalClassificationSize = 815;

    #endregion // Consts F1 2020

    #region Consts F1 2021

    /// <summary>
    /// Size of session data
    /// </summary>
    public const int F12021SessionSize = 601;

    /// <summary>
    /// Size of event data
    /// </summary>
    public const int F12021EventSize = 12;

    /// <summary>
    /// Size of a single weather forecast sample
    /// </summary>
    public const int F12021WeatherForecastSize = 8;

    /// <summary>
    /// Size of one lap data
    /// </summary>
    public const int F12021LapSize = 43;

    /// <summary>
    /// Size of one lap data x 22 cars
    /// </summary>
    public const int F12021TotalLapSize = 946;

    /// <summary>
    /// Size of one car telemetry packet
    /// </summary>
    public const int F12021CarTelemetrySize = 1323;

    /// <summary>
    /// Size of one participants packet
    /// </summary>
    public const int F12021ParticipantsSize = 1233;

    /// <summary>
    /// Size of one car status packet
    /// </summary>
    public const int F12021CarStatusSize = 1034;

    /// <summary>
    /// Size of one session history lap data
    /// </summary>
    public const int F12021SessionHistoryLapSize = 11;

    /// <summary>
    /// Size of one session history tyre stint data
    /// </summary>
    public const int F12021SessionHistoryTyreStintSize = 3;

    /// <summary>
    /// Size of one complete session history packet
    /// </summary>
    public const int F12021SessionHistorySize = 1131;

    /// <summary>
    /// Size of one final classification car data packet
    /// </summary>
    public const int F12021FinalClassificationCarSize = 37;

    /// <summary>
    /// Size of one complete final classification packet
    /// </summary>
    public const int F12021FinalClassificationSize = 815;

    #endregion // Consts F1 2021

    #region Consts F1 2022

    /// <summary>
    /// Size of session data
    /// </summary>
    public const int F12022SessionSize = 608;

    /// <summary>
    /// Size of event data
    /// </summary>
    public const int F12022EventSize = 12;

    /// <summary>
    /// Size of a single weather forecast sample
    /// </summary>
    public const int F12022WeatherForecastSize = 8;

    /// <summary>
    /// Size of one lap data
    /// </summary>
    public const int F12022LapSize = 43;

    /// <summary>
    /// Size of one lap data x 22 cars
    /// </summary>
    public const int F12022TotalLapSize = 948;

    /// <summary>
    /// Size of one car telemetry packet
    /// </summary>
    public const int F12022CarTelemetrySize = 1323;

    /// <summary>
    /// Size of one participants packet
    /// </summary>
    public const int F12022ParticipantsSize = 1233;

    /// <summary>
    /// Size of one car status packet
    /// </summary>
    public const int F12022CarStatusSize = 1034;

    /// <summary>
    /// Size of one session history lap data
    /// </summary>
    public const int F12022SessionHistoryLapSize = 11;

    /// <summary>
    /// Size of one session history tyre stint data
    /// </summary>
    public const int F12022SessionHistoryTyreStintSize = 3;

    /// <summary>
    /// Size of one complete session history packet
    /// </summary>
    public const int F12022SessionHistorySize = 1131;

    /// <summary>
    /// Size of one final classification car data packet
    /// </summary>
    public const int F12022FinalClassificationCarSize = 45;

    /// <summary>
    /// Size of one complete final classification packet
    /// </summary>
    public const int F12022FinalClassificationSize = 991;

    #endregion // Consts F1 2022

    #region Consts F1 2023

    /// <summary>
    /// Headersize for F1 2023
    /// </summary>
    public const int F12023HeaderSize = 29;

    /// <summary>
    /// Size of session data
    /// </summary>
    public const int F12023SessionSize = 615;

    /// <summary>
    /// Size of event data
    /// </summary>
    public const int F12023EventSize = 12;

    /// <summary>
    /// Size of a single weather forecast sample
    /// </summary>
    public const int F12023WeatherForecastSize = 8;

    /// <summary>
    /// Size of one lap data
    /// </summary>
    public const int F12023LapSize = 50;

    /// <summary>
    /// Size of one lap data x 22 cars
    /// </summary>
    public const int F12023TotalLapSize = 1102;

    /// <summary>
    /// Size of one car telemetry packet
    /// </summary>
    public const int F12023CarTelemetrySize = 1323;

    /// <summary>
    /// Size of one participants packet
    /// </summary>
    public const int F12023ParticipantsSize = 1277;

    /// <summary>
    /// Size of one car status packet
    /// </summary>
    public const int F12023CarStatusSize = 1210;

    /// <summary>
    /// Size of one session history lap data
    /// </summary>
    public const int F12023SessionHistoryLapSize = 14;

    /// <summary>
    /// Size of one session history tyre stint data
    /// </summary>
    public const int F12023SessionHistoryTyreStintSize = 3;

    /// <summary>
    /// Size of one complete session history packet
    /// </summary>
    public const int F12023SessionHistorySize = 1431;

    /// <summary>
    /// Size of one final classification car data packet
    /// </summary>
    public const int F12023FinalClassificationCarSize = 45;

    /// <summary>
    /// Size of one complete final classification packet
    /// </summary>
    public const int F12023FinalClassificationSize = 991;

    #endregion // Consts F1 2023

    #region Consts F1 2024

    /// <summary>
    /// Headersize for F1 2024
    /// </summary>
    public const int F12024HeaderSize = 29;

    /// <summary>
    /// Size of session data
    /// </summary>
    public const int F12024SessionSize = 724;

    /// <summary>
    /// Size of event data
    /// </summary>
    public const int F12024EventSize = 12;

    /// <summary>
    /// Size of a single weather forecast sample
    /// </summary>
    public const int F12024WeatherForecastSize = 8;

    /// <summary>
    /// Size of one lap data
    /// </summary>
    public const int F12024LapSize = 57;

    /// <summary>
    /// Size of one lap data x 22 cars
    /// </summary>
    public const int F12024TotalLapSize = 1256;

    /// <summary>
    /// Size of one car telemetry packet
    /// </summary>
    public const int F12024CarTelemetrySize = 1323;

    /// <summary>
    /// Size of one participants packet
    /// </summary>
    public const int F12024ParticipantsSize = 1321;

    /// <summary>
    /// Size of one car status packet
    /// </summary>
    public const int F12024CarStatusSize = 1210;

    /// <summary>
    /// Size of one session history lap data
    /// </summary>
    public const int F12024SessionHistoryLapSize = 14;

    /// <summary>
    /// Size of one session history tyre stint data
    /// </summary>
    public const int F12024SessionHistoryTyreStintSize = 3;

    /// <summary>
    /// Size of one complete session history packet
    /// </summary>
    public const int F12024SessionHistorySize = 1431;

    /// <summary>
    /// Size of one final classification car data packet
    /// </summary>
    public const int F12024FinalClassificationCarSize = 45;

    /// <summary>
    /// Size of one complete final classification packet
    /// </summary>
    public const int F12024FinalClassificationSize = 991;

    #endregion // Consts F1 2024

    #region Consts F1 2025

    /// <summary>
    /// Maximum number of cars in F1 2025
    /// </summary>
    public const int F12025MaxCars = 22;

    /// <summary>
    /// Represents the maximum number of laps in the lap positions packet
    /// </summary>
    public const int F12025MaxLapPositions = 50;

    /// <summary>
    /// Represents the maximum number of laps allowed in the F1 2025 racing simulation
    /// </summary>
    public const int F12025MaxLaps = 100;

    /// <summary>
    /// Headersize for F1 2025
    /// </summary>
    public const int F12025HeaderSize = 29;

    /// <summary>
    /// Size of session data
    /// </summary>
    public const int F12025SessionSize = 724;

    /// <summary>
    /// Size of event data
    /// </summary>
    public const int F12025EventSize = 16;

    /// <summary>
    /// Size of a single weather forecast sample
    /// </summary>
    public const int F12025WeatherForecastSize = 8;

    /// <summary>
    /// Size of one lap data
    /// </summary>
    public const int F12025LapSize = 57;

    /// <summary>
    /// Size of one lap data x 22 cars
    /// </summary>
    public const int F12025TotalLapSize = 1256;

    /// <summary>
    /// Size of one car telemetry packet
    /// </summary>
    public const int F12025CarTelemetrySize = 1323;

    /// <summary>
    /// Size of one participants packet
    /// </summary>
    public const int F12025ParticipantsSize = 1255;

    /// <summary>
    /// Size of one car status packet
    /// </summary>
    public const int F12025CarStatusSize = 1210;

    /// <summary>
    /// Size of one session history lap data
    /// </summary>
    public const int F12025SessionHistoryLapSize = 14;

    /// <summary>
    /// Size of one session history tyre stint data
    /// </summary>
    public const int F12025SessionHistoryTyreStintSize = 3;

    /// <summary>
    /// Size of one complete session history packet
    /// </summary>
    public const int F12025SessionHistorySize = 1431;

    /// <summary>
    /// Size of one final classification car data packet
    /// </summary>
    public const int F12025FinalClassificationCarSize = 46;

    /// <summary>
    /// Size of one complete final classification packet
    /// </summary>
    public const int F12025FinalClassificationSize = 1013;

    /// <summary>
    /// Size of one lap position packet
    /// </summary>
    public const int F12025LapPositionSize = 1102;

    #endregion // Consts F1 2025

    #region Consts F1 2026

    /// <summary>
    /// Maximum number of cars in F1 2026
    /// </summary>
    public const int F12026MaxCars = 24;

    /// <summary>
    /// Represents the maximum number of laps in the lap positions packet
    /// </summary>
    public const int F12026MaxLapPositions = 50;

    /// <summary>
    /// Represents the maximum number of laps allowed in the F1 2026 racing simulation
    /// </summary>
    public const int F12026MaxLaps = 100;

    /// <summary>
    /// Headersize for F1 2026
    /// </summary>
    public const int F12026HeaderSize = 29;

    /// <summary>
    /// Size of session data
    /// </summary>
    public const int F12026SessionSize = 897;

    /// <summary>
    /// Size of event data
    /// </summary>
    public const int F12026EventSize = 16;

    /// <summary>
    /// Size of a single weather forecast sample
    /// </summary>
    public const int F12026WeatherForecastSize = 8;

    /// <summary>
    /// Size of one lap data
    /// </summary>
    public const int F12026LapSize = 57;

    /// <summary>
    /// Size of one lap data x 24 cars
    /// </summary>
    public const int F12026TotalLapSize = 1368;

    /// <summary>
    /// Size of one car telemetry packet (24 cars with uint8 engine temperature plus three trailing bytes)
    /// </summary>
    public const int F12026CarTelemetrySize = 1419;

    /// <summary>
    /// Size of one participants packet (one count byte plus 24 cars with uint16 ids)
    /// </summary>
    public const int F12026ParticipantsSize = 1441;

    /// <summary>
    /// Size of one car status packet (24 cars including the additional ERS harvest limit per lap)
    /// </summary>
    public const int F12026CarStatusSize = 1416;

    /// <summary>
    /// Size of one session history lap data
    /// </summary>
    public const int F12026SessionHistoryLapSize = 14;

    /// <summary>
    /// Size of one session history tyre stint data
    /// </summary>
    public const int F12026SessionHistoryTyreStintSize = 3;

    /// <summary>
    /// Size of one complete session history packet
    /// </summary>
    public const int F12026SessionHistorySize = 1431;

    /// <summary>
    /// Size of one final classification car data packet
    /// </summary>
    public const int F12026FinalClassificationCarSize = 46;

    /// <summary>
    /// Size of one complete final classification packet
    /// </summary>
    public const int F12026FinalClassificationSize = 1105;

    /// <summary>
    /// Size of one lap position packet
    /// </summary>
    public const int F12026LapPositionSize = 1202;

    /// <summary>
    /// Size of one car telemetry 2 data block
    /// </summary>
    public const int F12026CarTelemetry2CarSize = 10;

    /// <summary>
    /// Size of one car telemetry 2 packet (24 cars)
    /// </summary>
    public const int F12026CarTelemetry2Size = 240;

    #endregion // Consts F1 2026

    #region Consts data sizes

    /// <summary>
    /// Game type uint8
    /// </summary>
    internal const int TypeUInt8 = 1;

    /// <summary>
    /// Game type int8
    /// </summary>
    internal const int TypeInt8 = 1;

    /// <summary>
    /// Game type uint16
    /// </summary>
    internal const int TypeUInt16 = 2;

    /// <summary>
    /// Game type int16
    /// </summary>
    internal const int TypeInt16 = 2;

    /// <summary>
    /// Game type float
    /// </summary>
    internal const int TypeFloat = 4;

    /// <summary>
    /// Game type uint
    /// </summary>
    internal const int TypeUInt = 4;

    /// <summary>
    /// Game type uint32
    /// </summary>
    internal const int TypeUInt32 = 4;

    /// <summary>
    /// Game type uint64
    /// </summary>
    internal const int TypeUInt64 = 8;

    /// <summary>
    /// Game type double
    /// </summary>
    internal const int TypeDouble = 8;

    #endregion // Consts data sizes

    #region Global sizes

    /// <summary>
    /// Size of marshal zone data
    /// </summary>
    internal const int MarshalZoneSize = 5;

    /// <summary>
    /// Size of all marshal zones
    /// </summary>
    internal const int TotalMarshalZoneSize = 21 * MarshalZoneSize;

    /// <summary>
    /// Length of Drivername
    /// </summary>
    internal const int DriverNameLength = 48;

    /// <summary>
    /// Length of Drivername for F1 2025 and newer
    /// </summary>
    internal const int DriverNameLength2025 = 32;

    /// <summary>
    /// Length of event code
    /// </summary>
    internal const int EventCodeLength = 4;

    /// <summary>
    /// Number of livery colors in participant data
    /// </summary>
    internal const int LiveryColors = 4;

    /// <summary>
    /// Size of one active aero zone (two floats) in F1 2026 and newer
    /// </summary>
    internal const int ActiveAeroZoneSize = 8;

    /// <summary>
    /// Size of one DRS zone (two floats) in F1 2026 and newer
    /// </summary>
    internal const int DrsZoneSize = 8;

    /// <summary>
    /// Maximum number of active aero zones per lap in F1 2026 and newer
    /// </summary>
    internal const int MaxActiveAeroZones = 8;

    /// <summary>
    /// Maximum number of DRS zones per lap in F1 2026 and newer
    /// </summary>
    internal const int MaxDrsZones = 4;

    #endregion // Global sizes
}