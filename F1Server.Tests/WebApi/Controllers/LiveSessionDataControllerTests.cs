using F1Server.Core.Enumerations;
using F1Server.Data;
using F1Server.WebApi.Controllers;

using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the <see cref="LiveSessionDataController"/> maps the live session data into its view data
/// </summary>
[TestClass]
public class LiveSessionDataControllerTests
{
    #region Static methods

    /// <summary>
    /// Creates live session data with one driver
    /// </summary>
    /// <returns>Live session data</returns>
    private static LiveSessionData CreateLiveSessionData()
    {
        var liveSessionData = new LiveSessionData
                              {
                                  DbId = 460100,
                                  SessionGameId = 460101,
                                  CurrentCarsOnTrack = 1,
                                  SessionType = SessionType.Qualifying1,
                                  AirTemperature = 24,
                                  TrackTemperature = 38,
                                  FastestSector1 = 30250,
                                  FastestSector1Driver = 3,
                                  FastestLap = 91000,
                                  FastestLapDriver = 3,
                                  TimeTable = [3]
                              };

        liveSessionData.Drivers.Add(new LiveDriverData
                                    {
                                        ArrayIndex = 3,
                                        DriverName = "Live Test Driver",
                                        CarNumber = 44,
                                        TeamName = "Live Test Team",
                                        CarPosition = 1,
                                        CurrentDriverStatus = DriverStatus.FlyingLap,
                                        FastestLapTime = 91000,
                                        LapsDriven = 5,
                                        CurrentUsedTyre = VisualTyreCompound.Soft
                                    });

        return liveSessionData;
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Verifies that the session values of the live session data are mapped into the view data
    /// </summary>
    [TestMethod]
    public void LiveSessionDataControllerGetMapsSessionData()
    {
        var appData = new F1ServerApplicationData
                      {
                          LiveSessionData = CreateLiveSessionData()
                      };

        var controller = new LiveSessionDataController(NullLogger<LiveSessionDataController>.Instance, appData);

        var viewData = controller.Get();

        Assert.AreEqual(460100, viewData.DbId, "The database id of the session should be mapped!");
        Assert.AreEqual(460101UL, viewData.SessionGameId, "The game id of the session should be mapped!");
        Assert.AreEqual(SessionType.Qualifying1, viewData.SessionType, "The session type should be mapped!");
        Assert.AreEqual(1, viewData.CurrentCarsOnTrack, "The cars on track should be mapped!");
        Assert.AreEqual(24, viewData.AirTemperature, "The air temperature should be mapped!");
        Assert.AreEqual(38, viewData.TrackTemperature, "The track temperature should be mapped!");
        Assert.AreEqual(30250U, viewData.FastestSector1, "The fastest sector 1 should be mapped!");
        Assert.AreEqual(91000U, viewData.FastestLap, "The fastest lap should be mapped!");
        Assert.AreEqual(3, viewData.FastestLapDriver, "The driver of the fastest lap should be mapped!");
        Assert.AreSequenceEqual(new List<int>
                                {
                                    3
                                },
                                viewData.TimeTable,
                                "The time table should be mapped!");
    }

    /// <summary>
    /// Verifies that the drivers of the live session data are mapped into the view data
    /// </summary>
    [TestMethod]
    public void LiveSessionDataControllerGetMapsDrivers()
    {
        var appData = new F1ServerApplicationData
                      {
                          LiveSessionData = CreateLiveSessionData()
                      };

        var controller = new LiveSessionDataController(NullLogger<LiveSessionDataController>.Instance, appData);

        var viewData = controller.Get();

        Assert.HasCount(1, viewData.Drivers, "Every driver of the live session should be mapped!");

        var driver = viewData.Drivers[0];

        Assert.AreEqual(3, driver.ArrayIndex, "The array index of the driver should be mapped!");
        Assert.AreEqual("Live Test Driver", driver.DriverName, "The name of the driver should be mapped!");
        Assert.AreEqual(44, driver.CarNumber, "The car number should be mapped!");
        Assert.AreEqual("Live Test Team", driver.TeamName, "The team name should be mapped!");
        Assert.AreEqual(DriverStatus.FlyingLap, driver.CurrentDriverStatus, "The driver status should be mapped!");
        Assert.AreEqual(91000U, driver.FastestLapTime, "The fastest lap time of the driver should be mapped!");
        Assert.AreEqual(5, driver.LapsDriven, "The driven laps should be mapped!");
        Assert.AreEqual(VisualTyreCompound.Soft, driver.CurrentUsedTyre, "The used tyre should be mapped!");
    }

    /// <summary>
    /// Verifies that missing live session data returns empty view data
    /// </summary>
    [TestMethod]
    public void LiveSessionDataControllerGetWithoutLiveSessionReturnsEmptyViewData()
    {
        var controller = new LiveSessionDataController(NullLogger<LiveSessionDataController>.Instance, new F1ServerApplicationData());

        var viewData = controller.Get();

        Assert.AreEqual(0, viewData.DbId, "Without a live session no session id should be returned!");
        Assert.IsEmpty(viewData.Drivers, "Without a live session no drivers should be returned!");
    }

    #endregion // Methods
}