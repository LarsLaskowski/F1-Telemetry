using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Weather forecast data
/// </summary>
public class WeatherForecastSample
{
    #region Properties

    /// <summary>
    /// Session type
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Time in minutes the forecast is for
    /// </summary>
    public int TimeOffset { get; set; }

    /// <summary>
    /// Weather conditions
    /// </summary>
    public WeatherCondition Weather { get; set; }

    /// <summary>
    /// Track temperature
    /// </summary>
    public ushort TrackTemperature { get; set; }

    /// <summary>
    /// Change indicator of track temperature (since F1 2021)
    /// </summary>
    public TemperatureChange TrackTemperatureChange { get; set; } = TemperatureChange.NotAvailable;

    /// <summary>
    /// Air temperature
    /// </summary>
    public ushort AirTemperature { get; set; }

    /// <summary>
    /// Change indicator of air temperature (since F1 2021)
    /// </summary>
    public TemperatureChange AirTemperatureChange { get; set; } = TemperatureChange.NotAvailable;

    /// <summary>
    /// Rain percentage (since F1 2021)
    /// </summary>
    public ushort RainPercentage { get; set; }

    #endregion // Properties
}