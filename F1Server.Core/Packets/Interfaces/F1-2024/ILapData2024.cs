namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for lap data packets (F1 2024)
/// </summary>
public interface ILapData2024 : ILapDataBase
{
    #region Properties

    /// <summary>
    /// Time delta to car in front whole minute part
    /// </summary>
    ushort DeltaToCarInFrontMinutes { get; }

    /// <summary>
    /// Time delta to race leader whole minute part
    /// </summary>
    ushort DeltaToRaceLeaderMinutes { get; }

    /// <summary>
    /// Fastest speed through speed trap for this car in kmph
    /// </summary>
    float SpeedTrapFastestSpeed { get; }

    /// <summary>
    /// Lap no the fastest speed was achieved, 255 = not set
    /// </summary>
    ushort SpeedTrapFastestLap { get; }

    #endregion // Properties
}