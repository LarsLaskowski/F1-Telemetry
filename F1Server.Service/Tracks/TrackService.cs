using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Service.Tracks;

/// <summary>
/// Business logic of the tracks - reads the known tracks with the number of their sessions and the data of a single
/// track
/// </summary>
public class TrackService
{
    #region Methods

    /// <summary>
    /// Loads the known tracks with the information whether they were already driven and how many sessions they carry
    /// </summary>
    /// <returns>Tracks ordered by their database id</returns>
    public async Task<List<TrackViewData>?> GetTracksAsync()
    {
        List<TrackViewData>? tracks = [];

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var sessionQuery = dbFactory.GetRepository<SessionRepository>()?.GetQuery();

            var uniqueSessionTracks = sessionQuery == null
                                          ? null
                                          : await sessionQuery.Select(s => s.TrackId)
                                                              .Distinct()
                                                              .ToListAsync()
                                                              .ConfigureAwait(false);

            var trackQuery = dbFactory.GetRepository<TrackRepository>()?.GetQuery();

            if (trackQuery != null)
            {
                tracks = await trackQuery.OrderBy(t => t.Id)
                                         .Select(obj => new TrackViewData
                                                        {
                                                            TrackId = obj.Id,
                                                            TrackNumber = obj.TrackNumber,
                                                            TrackName = obj.Name,
                                                            HasSession = uniqueSessionTracks != null && uniqueSessionTracks.Contains(obj.Id),
                                                            Sessions = sessionQuery != null ? sessionQuery.Count(s => s.TrackId == obj.Id) : 0,
                                                            ReferenceLapTime = TimeSpan.FromMilliseconds(obj.LapReferenceTime, 0).ToString(@"mm\:ss\.fff")
                                                        })
                                         .ToListAsync()
                                         .ConfigureAwait(false);
            }
        }

        return tracks;
    }

    /// <summary>
    /// Loads the data of a single track
    /// </summary>
    /// <param name="trackId">Database id of the track</param>
    /// <returns>Track data, or <see langword="null"/> when the track is unknown</returns>
    public async Task<TrackViewData?> GetTrackAsync(long trackId)
    {
        TrackViewData? trackData = null;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var trackQuery = dbFactory.GetRepository<TrackRepository>()?.GetQuery();

            var track = trackQuery == null
                            ? null
                            : await trackQuery.FirstOrDefaultAsync(t => t.Id == trackId)
                                              .ConfigureAwait(false);

            if (track != null)
            {
                trackData = new TrackViewData
                            {
                                TrackId = track.Id,
                                TrackName = track.Name,
                                TrackNumber = track.TrackNumber
                            };
            }
        }

        return trackData;
    }

    #endregion // Methods
}