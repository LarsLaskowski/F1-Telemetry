using F1Server.Core.Enumerations;
using F1Server.Core.Interfaces;

namespace F1Server.Data;

/// <summary>
/// Runtime session data
/// </summary>
public class LiveSessionData : ILiveSessionData
{
    #region ILiveSessionData

    /// <summary>
    /// Database id
    /// </summary>
    public long DbId { get; set; }

    /// <summary>
    /// Current session game id from game
    /// </summary>
    public ulong SessionGameId { get; set; }

    /// <summary>
    /// Session is finished
    /// </summary>
    public bool IsFinished { get; set; }

    /// <summary>
    /// Current number of cars on track
    /// </summary>
    public int CurrentCarsOnTrack { get; set; }

    /// <summary>
    /// Current session type
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Duration of session in seconds
    /// </summary>
    public int SessionDuration { get; set; }

    /// <summary>
    /// Session time left in seconds
    /// </summary>
    public int SessionTimeLeft { get; set; }

    /// <summary>
    /// Air temperature
    /// </summary>
    public int AirTemperature { get; set; }

    /// <summary>
    /// Track temperature
    /// </summary>
    public int TrackTemperature { get; set; }

    /// <summary>
    /// Safety car on track?
    /// </summary>
    public bool IsSafetyCar { get; set; }

    /// <summary>
    /// Weather
    /// </summary>
    public WeatherCondition Weather { get; set; }

    /// <summary>
    /// Fastest sector 1 time in milliseconds
    /// </summary>
    public uint FastestSector1 { get; set; }

    /// <summary>
    /// Fastest sector 1 driver (index from game array)
    /// </summary>
    public int FastestSector1Driver { get; set; }

    /// <summary>
    /// Fastest sector 2 time in milliseconds
    /// </summary>
    public uint FastestSector2 { get; set; }

    /// <summary>
    /// Fastest sector 2 driver (index from game array)
    /// </summary>
    public int FastestSector2Driver { get; set; }

    /// <summary>
    /// Fastest sector3 time in milliseconds
    /// </summary>
    public uint FastestSector3 { get; set; }

    /// <summary>
    /// Fastest sector 3 driver (index from game array)
    /// </summary>
    public int FastestSector3Driver { get; set; }

    /// <summary>
    /// Fastest lap time in milliseconds
    /// </summary>
    public uint FastestLap { get; set; }

    /// <summary>
    /// Fastest lap driver (index from game array)
    /// </summary>
    public int FastestLapDriver { get; set; }

    /// <summary>
    /// Participants in session
    /// </summary>
    public List<ILiveDriverData> Drivers { get; } = new List<ILiveDriverData>();

    /// <summary>
    /// Current time table
    /// </summary>
    public List<int> TimeTable { get; set; } = new List<int>();

    #endregion // ILiveSessionData
}