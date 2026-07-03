namespace F1Server.Data.ViewData;

/// <summary>
/// Viewdata of a fastest lap information of specific session
/// </summary>
public class FastestLapSessionViewData
{
    #region Properties

    /// <summary>
    /// Id of session
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// Driver of fastest lap
    /// </summary>
    public string FastestLapDriver { get; set; }

    /// <summary>
    /// Id of driver
    /// </summary>
    public long FastestLapDriverId { get; set; }

    /// <summary>
    /// Is fastest lap driver a human player?
    /// </summary>
    public bool IsFastestLapDriverHuman { get; set; }

    /// <summary>
    /// Time of fastest lap
    /// </summary>
    public string FastestLap { get; set; }

    /// <summary>
    /// Sector 1 time of fastest lap
    /// </summary>
    public string FastestLapSector1 { get; set; }

    /// <summary>
    /// Is sector 1 time of fastest lap the fastest in session?
    /// </summary>
    public bool IsFastestLapSector1 { get; set; }

    /// <summary>
    /// Sector 2 time of fastest lap
    /// </summary>
    public string FastestLapSector2 { get; set; }

    /// <summary>
    /// Is sector 2 time of fastest lap the fastest in session?
    /// </summary>
    public bool IsFastestLapSector2 { get; set; }

    /// <summary>
    /// Sector 3 time of fastest lap
    /// </summary>
    public string FastestLapSector3 { get; set; }

    /// <summary>
    /// Is sector 3 time of fastest lap the fastest in session?
    /// </summary>
    public bool IsFastestLapSector3 { get; set; }

    /// <summary>
    /// Sector 1 time
    /// </summary>
    public string FastestSector1 { get; set; }

    /// <summary>
    /// Human sector 1 time
    /// </summary>
    public string FastestHumanSector1 { get; set; }

    /// <summary>
    /// Driver of fastest sector 1
    /// </summary>
    public string FastestSector1Driver { get; set; }

    /// <summary>
    /// Id of driver of fastest sector 1
    /// </summary>
    public long FastestSector1DriverId { get; set; }

    /// <summary>
    /// Is fastest sector 1 driver a human player?
    /// </summary>
    public bool IsFastestSector1DriverHuman { get; set; }

    /// <summary>
    /// Sector 2 time
    /// </summary>
    public string FastestSector2 { get; set; }

    /// <summary>
    /// Human sector 2 time
    /// </summary>
    public string FastestHumanSector2 { get; set; }

    /// <summary>
    /// Driver of fastest sector 2
    /// </summary>
    public string FastestSector2Driver { get; set; }

    /// <summary>
    /// Id of driver of fastest sector 2
    /// </summary>
    public long FastestSector2DriverId { get; set; }

    /// <summary>
    /// Is fastest sector 2 driver a human player?
    /// </summary>
    public bool IsFastestSector2DriverHuman { get; set; }

    /// <summary>
    /// Sector 3 time
    /// </summary>
    public string FastestSector3 { get; set; }

    /// <summary>
    /// Human sector 3 time
    /// </summary>
    public string FastestHumanSector3 { get; set; }

    /// <summary>
    /// Driver of fastest sector 3
    /// </summary>
    public string FastestSector3Driver { get; set; }

    /// <summary>
    /// Id of driver of fastest sector 3
    /// </summary>
    public long FastestSector3DriverId { get; set; }

    /// <summary>
    /// Is fastest sector 3 driver a human player?
    /// </summary>
    public bool IsFastestSector3DriverHuman { get; set; }

    /// <summary>
    /// Theoretical fastest lap
    /// </summary>
    public string TheoreticalFastestLap { get; set; }

    /// <summary>
    /// Human players fastest lap - equals fastest lap if driven by human player
    /// </summary>
    public string HumanPlayersFastestLap { get; set; }

    /// <summary>
    /// Reference lap time
    /// </summary>
    public string ReferenceLapTime { get; set; }

    /// <summary>
    /// Reference sector 1 time
    /// </summary>
    public string ReferenceSector1Time { get; set; }

    /// <summary>
    /// Reference sector 2 time
    /// </summary>
    public string ReferenceSector2Time { get; set; }

    /// <summary>
    /// Reference sector 3 time
    /// </summary>
    public string ReferenceSector3Time { get; set; }

    /// <summary>
    /// Difference between human and reference lap time
    /// </summary>
    public string ReferenceDifferenceHumanLapTime { get; set; }

    /// <summary>
    /// Difference between human and reference sector 1 time
    /// </summary>
    public string ReferenceDifferenceHumanSector1Time { get; set; }

    /// <summary>
    /// Difference between human and reference sector 2 time
    /// </summary>
    public string ReferenceDifferenceHumanSector2Time { get; set; }

    /// <summary>
    /// Difference between human and reference sector 3 time
    /// </summary>
    public string ReferenceDifferenceHumanSector3Time { get; set; }

    #endregion // Properties
}