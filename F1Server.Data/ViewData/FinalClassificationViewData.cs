using System.Text.Json.Serialization;

namespace F1Server.Data.ViewData;

/// <summary>
/// Final classification data
/// </summary>
public class FinalClassificationViewData
{
    #region Properties

    /// <summary>
    /// Database id
    /// </summary>
    public long DbId { get; set; }

    /// <summary>
    /// Database id of participant
    /// </summary>
    public long ParticipantDbId { get; set; }

    /// <summary>
    /// Index in game packet
    /// </summary>
    public int ArrayIndex { get; set; }

    /// <summary>
    /// Name of driver
    /// </summary>
    public string DriverName { get; set; }

    /// <summary>
    /// Number of drivers car
    /// </summary>
    public int CarNumber { get; set; }

    /// <summary>
    /// Name of Team
    /// </summary>
    public string TeamName { get; set; }

    /// <summary>
    /// Nationality
    /// </summary>
    public string Nationality { get; set; }

    /// <summary>
    /// Starting grid position
    /// </summary>
    public int StartingPosition { get; set; }

    /// <summary>
    /// Finish position
    /// </summary>
    public int FinishPosition { get; set; }

    /// <summary>
    /// Laps completed
    /// </summary>
    public int LapsDriven { get; set; }

    /// <summary>
    /// Number of pit stops
    /// </summary>
    public int PitStops { get; set; }

    /// <summary>
    /// Fastest lap time as string
    /// </summary>
    public string FastestLapTime { get; set; }

    /// <summary>
    /// Fastest lap time
    /// </summary>
    [JsonIgnore]
    public uint FastestLapTimeRaw { get; set; }

    /// <summary>
    /// Difference between personal best lap time and fastest lap time in session
    /// </summary>
    public string FastestLapTimeDifference { get; set; }

    /// <summary>
    /// Is fastest lap time in this session?
    /// </summary>
    public bool IsFastestSessionLapTime { get; set; }

    /// <summary>
    /// Total race time as string
    /// </summary>
    public string TotalRaceTime { get; set; }

    /// <summary>
    /// Total race time
    /// </summary>
    [JsonIgnore]
    public double TotalRaceTimeRaw { get; set; }

    /// <summary>
    /// Time difference
    /// </summary>
    public string RaceTimeDifference { get; set; }

    /// <summary>
    /// Time of penalties in seconds
    /// </summary>
    public uint PenaltiesTime { get; set; }

    /// <summary>
    /// Number of penalties
    /// </summary>
    public uint NumberOfPenalties { get; set; }

    #endregion // Properties
}