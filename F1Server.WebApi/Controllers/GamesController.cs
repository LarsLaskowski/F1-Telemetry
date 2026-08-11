using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Service.Games;

using Microsoft.AspNetCore.Mvc;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Games controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    #region Fields

    private readonly ILogger<GamesController> _logger;
    private readonly GameService _gameService;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    /// <param name="gameService">Game business logic</param>
    public GamesController(ILogger<GamesController> logger, GameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    #endregion // Constructors

    #region Controller methods

    /// <summary>
    /// Get games
    /// </summary>
    /// <returns>Games</returns>
    [HttpGet]
    public async Task<IEnumerable<GameViewData>?> Get()
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity("GetGames");

        _logger?.LoadingGames();

        var games = await _gameService.GetGamesAsync().ConfigureAwait(false);

        currentActivity?.SetStatus(ActivityStatusCode.Ok);

        _logger?.GamesLoaded(games?.Count ?? 0);

        return games;
    }

    #endregion // Controller methods
}