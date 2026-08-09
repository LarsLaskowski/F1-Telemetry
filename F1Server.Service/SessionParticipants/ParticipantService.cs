using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Service.SessionParticipants;

/// <summary>
/// Business logic of the participants - reads the participants of a session with their driver, nationality and team
/// </summary>
public class ParticipantService
{
    #region Methods

    /// <summary>
    /// Loads the participants of a session
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Participants of the session, or <see langword="null"/> when the session carries no participants</returns>
    public async Task<List<ParticipantViewData>?> GetParticipantsOfSessionAsync(long? sessionId)
    {
        List<ParticipantViewData>? participants = null;

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var participantQuery = dbFactory.GetRepository<ParticipantRepository>()?.GetQuery();

            List<ParticipantEntity> dbParticipants = [];

            if (participantQuery != null)
            {
                dbParticipants = await participantQuery.Where(s => s.SessionId == sessionId)
                                                       .OrderByDescending(s => s.DriverId)
                                                       .ToListAsync()
                                                       .ConfigureAwait(false);
            }

            if (dbParticipants.Count > 0)
            {
                participants = [];

                using (AppActivity.SrvSource.StartActivity("Participants_Loop"))
                {
                    foreach (var dbParticipant in dbParticipants)
                    {
                        // Driver, nationality and team are auto-included navigations, so they are already loaded
                        // with the participant and need no additional query per row
                        var participant = new ParticipantViewData
                                          {
                                              ParticipantDbId = dbParticipant.Id,
                                              DriverName = dbParticipant.Driver?.Name ?? dbParticipant.DriverName,
                                              DriverNationality = dbParticipant.Nationality?.Name ?? string.Empty,
                                              IsHumanControlled = dbParticipant.IsHumanControlled,
                                              IsMyTeam = dbParticipant.IsMyTeam != null && dbParticipant.IsMyTeam.Value,
                                              CarRaceNumber = dbParticipant.CarRaceNumber,
                                              TeamName = dbParticipant.Team?.Name ?? string.Empty
                                          };

                        participants.Add(participant);
                    }
                }
            }
        }

        return participants;
    }

    #endregion // Methods
}