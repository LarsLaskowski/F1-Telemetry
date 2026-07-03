using F1Server.Core.Packets.Data;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Session data for F1 2020
/// </summary>
public interface ISessionData2020 : ISessionDataBase
{
    #region Properties

    /// <summary>
    /// Number of weather samples to follow
    /// </summary>
    ushort NumberWeatherForecastSamples { get; }

    /// <summary>
    /// Weather forecast data
    /// </summary>
    WeatherForecastSample[] WeatherForecastSamples { get; }

    #endregion // Properties
}