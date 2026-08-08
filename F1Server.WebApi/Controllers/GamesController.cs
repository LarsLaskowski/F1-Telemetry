using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    public GamesController(ILogger<GamesController> logger)
    {
        _logger = logger;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Get games
    /// </summary>
    /// <returns>Games</returns>
    [HttpGet]
    public async Task<IEnumerable<GameViewData>?> Get()
    {
        List<GameViewData>? games = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity("GetGames");

        _logger?.LogInformation("Games loading...");

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var gameQuery = dbFactory.GetRepository<GameVersionRepository>()?.GetQuery();

            if (gameQuery != null)
            {
                games = await gameQuery.OrderBy(g => g.Version)
                                       .Select(obj => new GameViewData
                                                      {
                                                          Id = obj.Id,
                                                          GameVersion = obj.Name,
                                                          GameVersionCode = $"{obj.MajorVersion}.{obj.MinorVersion}",
                                                          LastUsed = obj.LastUsed.HasValue
                                                                         ? $"{obj.LastUsed.Value.ToShortDateString()} {obj.LastUsed.Value.ToShortTimeString()}"
                                                                         : "-"
                                                      })
                                       .ToListAsync()
                                       .ConfigureAwait(false);
            }

            if (games?.Count > 0)
            {
                var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

                foreach (var game in games)
                {
                    game.Sessions = sessionQuery == null
                                        ? 0
                                        : await sessionQuery.CountAsync(s => s.DbIsFinished == 1 && s.GameVersionId == game.Id)
                                                            .ConfigureAwait(false);
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.LogInformation("Games loaded ({LoadedGames}).", games?.Count ?? 0);

        return games;
    }

    #endregion // Methods
}