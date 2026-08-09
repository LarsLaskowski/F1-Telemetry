using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Service.SessionParticipants;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Participants controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ParticipantsController : ControllerBase
{
    #region Fields

    private readonly ILogger<ParticipantsController> _logger;
    private readonly ParticipantService _participantService;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="participantService">Participant business logic</param>
    public ParticipantsController(ILogger<ParticipantsController> logger, ParticipantService participantService)
    {
        _logger = logger;
        _participantService = participantService;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Get participants of specific session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>List of participants</returns>
    [Route("ParticipantsOfSession/{sessionId?}")]
    [HttpGet]
    public async Task<IActionResult> GetParticipantsOfSession(long? sessionId)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetParticipantsOfSession));

        _logger?.LoadingParticipants(sessionId);

        var participants = await _participantService.GetParticipantsOfSessionAsync(sessionId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.ParticipantsLoaded(participants?.Count ?? 0, sessionId);

        return Ok(participants);
    }

    #endregion // Controller methods
}