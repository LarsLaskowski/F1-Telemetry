using F1Server.Data;
using F1Server.Db.Entity;
using F1Server.WebApi.Hubs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Health controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    #region Fields

    private readonly F1ServerApplicationData _appData;
    private readonly ILogger<HealthController> _logger;
    private readonly IHubContext<LiveSessionHub> _hub;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="appData">Application data</param>
    /// <param name="logger">Logging interface</param>
    /// <param name="hub">SignalR hub</param>
    public HealthController(F1ServerApplicationData appData, ILogger<HealthController> logger, IHubContext<LiveSessionHub> hub)
    {
        _logger = logger;
        _appData = appData;
        _hub = hub;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Get action
    /// </summary>
    /// <returns>Healthy state</returns>
    [HttpGet]
    public IActionResult Get()
    {
        // Check database
        var isHealthy = CheckDatabase();

        // Check SignalR
        if (isHealthy)
        {
            isHealthy = CheckSignalR();
        }

        return isHealthy ? Ok() : BadRequest();
    }

    #endregion // Controller methods

    #region Methods

    /// <summary>
    /// Checks whether the database is initialized and ready for use
    /// </summary>
    /// <returns><see langword="true"/> if the database is initialized; otherwise, <see langword="false"/></returns>
    private bool CheckDatabase()
    {
        var isOk = false;

        if (RepositoryFactory.IsInitialized)
        {
            isOk = true;
        }
        else
        {
            _logger?.LogError("Database is not initialized!");
        }

        return isOk;
    }

    /// <summary>
    /// Determines whether the SignalR hub is initialized
    /// </summary>
    /// <returns><see langword="true"/> if the SignalR hub is initialized; otherwise, <see langword="false"/></returns>
    private bool CheckSignalR()
    {
        var isOk = false;

        if (_hub != null)
        {
            isOk = true;
        }
        else
        {
            _logger?.LogError("SignalR hub is not initialized!");
        }

        return isOk;
    }

    #endregion // Methods
}