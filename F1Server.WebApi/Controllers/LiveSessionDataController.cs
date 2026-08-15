using System.Diagnostics;

using F1Server.Core.Interfaces;
using F1Server.Core.Observability;
using F1Server.Data;
using F1Server.Data.ViewData;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Returns runtime session data from a running live session
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LiveSessionDataController : ControllerBase
{
    #region Fields

    private readonly ILogger<LiveSessionDataController> _logger;
    private readonly F1ServerApplicationData _appData;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="appData">Application data</param>
    public LiveSessionDataController(ILogger<LiveSessionDataController> logger, F1ServerApplicationData appData)
    {
        _logger = logger;
        _appData = appData;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Returns live session data
    /// </summary>
    /// <returns>Live session data</returns>
    [HttpGet]
    public LiveSessionDataViewData Get()
    {
        var liveSessionData = new LiveSessionDataViewData();

        using var currentActivity = AppActivity.ApiSource.StartActivity("GetLiveSessionData");

        _logger?.LogInformation("Requesing live session data...");

        if (_appData?.LiveSessionData != null)
        {
            liveSessionData = CreateLiveSessionDataViewData(_appData.LiveSessionData);

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        else
        {
            currentActivity?.SetStatus(ActivityStatusCode.Error);

            _logger?.LogWarning("Live session data is not available.");
        }

        return liveSessionData;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Maps the runtime live session data to its view data contract
    /// </summary>
    /// <param name="liveSessionData">Runtime live session data</param>
    /// <returns>View data of the live session</returns>
    private static LiveSessionDataViewData CreateLiveSessionDataViewData(ILiveSessionData liveSessionData)
    {
        var viewData = new LiveSessionDataViewData
                       {
                           DbId = liveSessionData.DbId,
                           SessionGameId = liveSessionData.SessionGameId,
                           IsFinished = liveSessionData.IsFinished,
                           CurrentCarsOnTrack = liveSessionData.CurrentCarsOnTrack,
                           SessionType = liveSessionData.SessionType,
                           SessionDuration = liveSessionData.SessionDuration,
                           SessionTimeLeft = liveSessionData.SessionTimeLeft,
                           AirTemperature = liveSessionData.AirTemperature,
                           TrackTemperature = liveSessionData.TrackTemperature,
                           IsSafetyCar = liveSessionData.IsSafetyCar,
                           Weather = liveSessionData.Weather,
                           FastestSector1 = liveSessionData.FastestSector1,
                           FastestSector1Driver = liveSessionData.FastestSector1Driver,
                           FastestSector2 = liveSessionData.FastestSector2,
                           FastestSector2Driver = liveSessionData.FastestSector2Driver,
                           FastestSector3 = liveSessionData.FastestSector3,
                           FastestSector3Driver = liveSessionData.FastestSector3Driver,
                           FastestLap = liveSessionData.FastestLap,
                           FastestLapDriver = liveSessionData.FastestLapDriver,
                           TimeTable = [.. liveSessionData.TimeTable]
                       };

        foreach (var driver in liveSessionData.Drivers)
        {
            viewData.Drivers.Add(new LiveDriverDataViewData
                                 {
                                     ArrayIndex = driver.ArrayIndex,
                                     DriverName = driver.DriverName,
                                     CarNumber = driver.CarNumber,
                                     GridPosition = driver.GridPosition,
                                     CarPosition = driver.CarPosition,
                                     Nationality = driver.Nationality,
                                     TeamName = driver.TeamName,
                                     CurrentDriverStatus = driver.CurrentDriverStatus,
                                     CurrentLapTime = driver.CurrentLapTime,
                                     FastestSector1 = driver.FastestSector1,
                                     FastestSector2 = driver.FastestSector2,
                                     FastestSector3 = driver.FastestSector3,
                                     FastestLapTime = driver.FastestLapTime,
                                     LapsDriven = driver.LapsDriven,
                                     CurrentUsedTyre = driver.CurrentUsedTyre
                                 });
        }

        return viewData;
    }

    #endregion // Private methods
}