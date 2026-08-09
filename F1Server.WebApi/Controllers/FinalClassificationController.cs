using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Service.FinalClassifications;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Final classification controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FinalClassificationController : ControllerBase
{
    #region Fields

    private readonly ILogger<FinalClassificationController> _logger;
    private readonly FinalClassificationService _finalClassificationService;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="finalClassificationService">Final classification business logic</param>
    public FinalClassificationController(ILogger<FinalClassificationController> logger, FinalClassificationService finalClassificationService)
    {
        _logger = logger;
        _finalClassificationService = finalClassificationService;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Get final classification for session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Final classification data</returns>
    [Route("{sessionId?}")]
    [HttpGet]
    public async Task<IActionResult> GetFromSession(long? sessionId)
    {
        var finalClassifications = new List<FinalClassificationViewData>();

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFromSession));

        _logger?.LoadingFinalClassification(sessionId);

        try
        {
            finalClassifications = await _finalClassificationService.GetFinalClassificationsOfSessionAsync(sessionId).ConfigureAwait(false);

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            _logger?.ErrorLoadingFinalClassifications(ex);

            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
            currentActivity?.AddException(ex);
        }

        _logger?.FinalClassificationLoaded(finalClassifications.Count);

        return Ok(finalClassifications);
    }

    #endregion // Controller methods
}