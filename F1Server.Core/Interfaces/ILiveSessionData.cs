using F1Server.Core.Enumerations;

namespace F1Server.Core.Interfaces;

/// <summary>
/// Live session information
/// </summary>
public interface ILiveSessionData : ILiveBaseData
{
    #region Properties

    /// <summary>
    /// Current session game id from game
    /// </summary>
    ulong SessionGameId { get; }

    /// <summary>
    /// Session is finished
    /// </summary>
    bool IsFinished { get; }

    /// <summary>
    /// Current number of cars on track
    /// </summary>
    int CurrentCarsOnTrack { get; }

    /// <summary>
    /// Current session type
    /// </summary>
    SessionType SessionType { get; }

    /// <summary>
    /// Duration of session in seconds
    /// </summary>
    int SessionDuration { get; }

    /// <summary>
    /// Session time left in seconds
    /// </summary>
    int SessionTimeLeft { get; }

    /// <summary>
    /// Air temperature
    /// </summary>
    int AirTemperature { get; }

    /// <summary>
    /// Track temperature
    /// </summary>
    int TrackTemperature { get; }

    /// <summary>
    /// Safety car on track?
    /// </summary>
    bool IsSafetyCar { get; }

    /// <summary>
    /// Weather
    /// </summary>
    WeatherCondition Weather { get; }

    /// <summary>
    /// Fastest sector 1 time in milliseconds
    /// </summary>
    uint FastestSector1 { get; }

    /// <summary>
    /// Fastest sector 1 driver (index from game array)
    /// </summary>
    int FastestSector1Driver { get; }

    /// <summary>
    /// Fastest sector 2 time in milliseconds
    /// </summary>
    uint FastestSector2 { get; }

    /// <summary>
    /// Fastest sector 2 driver (index from game array)
    /// </summary>
    int FastestSector2Driver { get; }

    /// <summary>
    /// Fastest sector3 time in milliseconds
    /// </summary>
    uint FastestSector3 { get; }

    /// <summary>
    /// Fastest sector 3 driver (index from game array)
    /// </summary>
    int FastestSector3Driver { get; }

    /// <summary>
    /// Fastest lap time in milliseconds
    /// </summary>
    uint FastestLap { get; }

    /// <summary>
    /// Fastest lap driver (index from game array)
    /// </summary>
    int FastestLapDriver { get; }

    /// <summary>
    /// Participants in session
    /// </summary>
    List<ILiveDriverData> Drivers { get; }

    /// <summary>
    /// Current time table
    /// </summary>
    List<int> TimeTable { get; }

    #endregion // Properties
}