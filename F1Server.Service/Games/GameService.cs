using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Service.Games;

/// <summary>
/// Business logic of the game versions - reads the known game versions together with the number of their finished
/// sessions
/// </summary>
public class GameService
{
    #region Methods

    /// <summary>
    /// Loads the known game versions with the number of their finished sessions
    /// </summary>
    /// <returns>Game versions ordered by their version, or <see langword="null"/> when the game versions cannot be read</returns>
    public async Task<List<GameViewData>?> GetGamesAsync()
    {
        List<GameViewData>? games = null;

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
                                                                         ? $"{obj.LastUsed.Value:d} {obj.LastUsed.Value:t}"
                                                                         : "-"
                                                      })
                                       .ToListAsync()
                                       .ConfigureAwait(false);
            }

            if (games?.Count > 0)
            {
                var sessionCounts = await LoadSessionCountsAsync(dbFactory).ConfigureAwait(false);

                foreach (var game in games)
                {
                    game.Sessions = sessionCounts.TryGetValue(game.Id, out var sessions)
                                        ? sessions
                                        : 0;
                }
            }
        }

        return games;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Counts the finished sessions of every game version with a single grouped query
    /// </summary>
    /// <param name="dbFactory">Database factory</param>
    /// <returns>Number of finished sessions per database id of the game version</returns>
    private static async Task<Dictionary<long, int>> LoadSessionCountsAsync(RepositoryFactory dbFactory)
    {
        var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

        if (sessionQuery == null)
        {
            return [];
        }

        var sessionCounts = await sessionQuery.Where(s => s.DbIsFinished == 1)
                                              .GroupBy(s => s.GameVersionId)
                                              .Select(group => new
                                                               {
                                                                   GameVersionId = group.Key,
                                                                   Sessions = group.Count()
                                                               })
                                              .ToListAsync()
                                              .ConfigureAwait(false);

        return sessionCounts.ToDictionary(entry => entry.GameVersionId, entry => entry.Sessions);
    }

    #endregion // Private methods
}