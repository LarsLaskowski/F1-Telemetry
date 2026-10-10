using F1Server.Core.Enumerations;
using F1Server.Core.Interfaces;

namespace F1Server.Data;

/// <summary>
/// Runtime session data
/// </summary>
public class LiveSessionData : ILiveSessionData
{
    #region ILiveBaseData

    /// <inheritdoc/>
    public long DbId { get; set; }

    #endregion // ILiveBaseData

    #region ILiveSessionData

    /// <inheritdoc/>
    public ulong SessionGameId { get; set; }

    /// <inheritdoc/>
    public bool IsFinished { get; set; }

    /// <inheritdoc/>
    public int CurrentCarsOnTrack { get; set; }

    /// <inheritdoc/>
    public SessionType SessionType { get; set; }

    /// <inheritdoc/>
    public int SessionDuration { get; set; }

    /// <inheritdoc/>
    public int SessionTimeLeft { get; set; }

    /// <inheritdoc/>
    public int AirTemperature { get; set; }

    /// <inheritdoc/>
    public int TrackTemperature { get; set; }

    /// <inheritdoc/>
    public bool IsSafetyCar { get; set; }

    /// <inheritdoc/>
    public WeatherCondition Weather { get; set; }

    /// <inheritdoc/>
    public uint FastestSector1 { get; set; }

    /// <inheritdoc/>
    public int FastestSector1Driver { get; set; }

    /// <inheritdoc/>
    public uint FastestSector2 { get; set; }

    /// <inheritdoc/>
    public int FastestSector2Driver { get; set; }

    /// <inheritdoc/>
    public uint FastestSector3 { get; set; }

    /// <inheritdoc/>
    public int FastestSector3Driver { get; set; }

    /// <inheritdoc/>
    public uint FastestLap { get; set; }

    /// <inheritdoc/>
    public int FastestLapDriver { get; set; }

    /// <summary>
    /// Participants in session
    /// </summary>
    public List<ILiveDriverData> Drivers { get; } = [];

    /// <summary>
    /// Current time table
    /// </summary>
    public List<int> TimeTable { get; set; } = [];

    /// <inheritdoc/>
    IReadOnlyList<ILiveDriverData> ILiveSessionData.Drivers => Drivers;

    /// <inheritdoc/>
    IReadOnlyList<int> ILiveSessionData.TimeTable => TimeTable;

    #endregion // ILiveSessionData
}