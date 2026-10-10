using F1Server.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace F1Server.Tests.WebApi.Controllers;

/// <summary>
/// Contains unit tests verifying the health check of the <see cref="HealthController"/>
/// </summary>
[TestClass]
public class HealthControllerTests
{
    #region Methods

    /// <summary>
    /// Verifies that an initialized database together with an available SignalR hub reports a healthy server
    /// </summary>
    [TestMethod]
    public void HealthControllerGetWithHubReturnsOk()
    {
        using (var hub = new RecordingHubContext())
        {
            var controller = new HealthController(NullLogger<HealthController>.Instance, hub);

            var result = controller.Get();

            Assert.IsInstanceOfType<OkResult>(result, "An initialized database and an available hub should report a healthy server!");
        }
    }

    /// <summary>
    /// Verifies that a missing SignalR hub reports an unhealthy server
    /// </summary>
    [TestMethod]
    public void HealthControllerGetWithoutHubReturnsBadRequest()
    {
        var controller = new HealthController(NullLogger<HealthController>.Instance, null!);

        var result = controller.Get();

        Assert.IsInstanceOfType<BadRequestResult>(result, "A missing hub should report an unhealthy server!");
    }

    #endregion // Methods
}