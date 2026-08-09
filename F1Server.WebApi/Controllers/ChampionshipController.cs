using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data;
using F1Server.Data.ViewData;
using F1Server.Service.Championships;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Championship controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ChampionshipController : ControllerBase
{
    #region Constants

    /// <summary>
    /// Message returned when the given championship data cannot be used to create a championship
    /// </summary>
    private const string InvalidChampionshipDataMessage = "Invalid championship data. GameVersionId must be greater than 0 and at least 6 tracks are required.";

    /// <summary>
    /// Message returned when the given session cannot be used
    /// </summary>
    private const string InvalidSessionMessage = "Invalid session";

    /// <summary>
    /// Message returned when the track of the session is not part of a running championship
    /// </summary>
    private const string TrackNotInChampionshipMessage = "Track is not part of the championship!";

    /// <summary>
    /// Message returned when the session could not be stored at the track of the championship
    /// </summary>
    private const string SessionNotAddedMessage = "Session not added to championship!";

    #endregion // Constants

    #region Fields

    private readonly ILogger<ChampionshipController> _logger;
    private readonly ChampionshipService _championshipService;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="championshipService">Championship business logic</param>
    public ChampionshipController(ILogger<ChampionshipController> logger, ChampionshipService championshipService)
    {
        _logger = logger;
        _championshipService = championshipService;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Get championships
    /// </summary>
    /// <returns>Championships</returns>
    [HttpGet]
    public async Task<IEnumerable<ChampionshipViewData>> Get()
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity("GetChampionships");

        _logger?.LoadingChampionships();

        var championships = await _championshipService.GetChampionshipsAsync().ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.ChampionshipsLoaded(championships.Count);

        return championships;
    }

    /// <summary>
    /// Creates a new championship
    /// </summary>
    /// <param name="championshipCreateData">Data to create a new championship</param>
    /// <returns>Status</returns>
    [Route("CreateChampionship")]
    [HttpPost]
    public async Task<IActionResult> CreateChampionship([FromBody] ChampionshipCreateData championshipCreateData)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(CreateChampionship));

        _logger?.CreatingChampionship();

        var createResult = await _championshipService.CreateChampionshipAsync(championshipCreateData).ConfigureAwait(false);

        currentActivity?.SetStatus(createResult == ChampionshipCreateResult.Created ? ActivityStatusCode.Ok : ActivityStatusCode.Error);

        return createResult switch
               {
                   ChampionshipCreateResult.Created => Ok(true),
                   ChampionshipCreateResult.InvalidData => BadRequest(InvalidChampionshipDataMessage),
                   _ => StatusCode(500, false)
               };
    }

    /// <summary>
    /// Is a championship available for this session?
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Id of championship</returns>
    [Route("IsActiveChampionship/{sessionId}")]
    [HttpGet]
    public async Task<long> IsActiveChampionship(long sessionId)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(IsActiveChampionship));

        _logger?.CheckingActiveChampionship(sessionId);

        var activeChampionship = await _championshipService.GetActiveChampionshipIdAsync(sessionId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.ActiveChampionshipChecked(activeChampionship);

        return activeChampionship;
    }

    /// <summary>
    /// Is a session active in a running championship?
    /// </summary>
    /// <param name="championshipId">Id of championship</param>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Status</returns>
    [Route("IsSessionInChampionshipActive/{championshipId}/{sessionId}")]
    [HttpGet]
    public async Task<bool> IsSessionInChampionshipActive(long championshipId, long sessionId)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(IsSessionInChampionshipActive));

        _logger?.CheckingSessionActiveInChampionship(sessionId, championshipId);

        var sessionIsActive = await _championshipService.IsSessionInChampionshipActiveAsync(championshipId, sessionId).ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.SessionActiveInChampionshipChecked(sessionIsActive);

        return sessionIsActive;
    }

    /// <summary>
    /// Adds a session to an active championship based on the provided session ID
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session to be added to the championship. Must be greater than zero</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the success of the operation. Returns <see langword="true"/> if the
    /// operation completes successfully
    /// </returns>
    [Route("AddToChampionship/{sessionId}")]
    [HttpPost]
    public async Task<IActionResult> AddToChampionship(long sessionId)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(AddToChampionship));

        currentActivity?.SetTag("SessionId", sessionId);

        _logger?.AddingSessionToChampionship(sessionId);

        var sessionResult = await _championshipService.AddSessionToChampionshipAsync(sessionId).ConfigureAwait(false);

        if (sessionResult != ChampionshipSessionResult.InvalidSession)
        {
            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return sessionResult switch
               {
                   ChampionshipSessionResult.Added => Ok(),
                   ChampionshipSessionResult.InvalidSession or ChampionshipSessionResult.SessionNotFound => BadRequest(InvalidSessionMessage),
                   ChampionshipSessionResult.NotAdded => NotFound(SessionNotAddedMessage),
                   _ => NotFound(TrackNotInChampionshipMessage)
               };
    }

    #endregion // Controller methods
}