using F1Server.Data;
using F1Server.WebApi.Controllers;

using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying that the <see cref="StatisticsController"/> returns the telemetry statistics of the application
/// </summary>
[TestClass]
public class StatisticsControllerTests
{
    #region Methods

    /// <summary>
    /// Verifies that the statistics of the application data are returned
    /// </summary>
    [TestMethod]
    public void StatisticsControllerGetReturnsApplicationStatistics()
    {
        var appData = new F1ServerApplicationData();
        var controller = new StatisticsController(appData, NullLogger<StatisticsController>.Instance);

        var statistics = controller.Get();

        Assert.AreSame(appData.Statistics, statistics, "The statistics of the application data should be returned!");
    }

    /// <summary>
    /// Verifies that missing application data returns empty statistics instead of failing
    /// </summary>
    [TestMethod]
    public void StatisticsControllerGetWithoutApplicationDataReturnsEmptyStatistics()
    {
        var controller = new StatisticsController(null!, NullLogger<StatisticsController>.Instance);

        var statistics = controller.Get();

        Assert.IsNotNull(statistics, "Missing application data should still return statistics!");
        Assert.AreEqual(0, statistics.PacketsReceivedTotal, "Statistics without application data should not contain received packets!");
    }

    #endregion // Methods
}