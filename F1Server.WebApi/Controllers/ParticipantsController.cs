using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Queryable;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    public ParticipantsController(ILogger<ParticipantsController> logger)
    {
        _logger = logger;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Get participants of specific session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>List of participants</returns>
    [Route("ParticipantsOfSession/{sessionId?}")]
    [HttpGet]
    public async Task<IActionResult> GetParticipantsOfSession(long? sessionId)
    {
        List<ParticipantViewData>? participants = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetParticipantsOfSession));

        _logger?.LogInformation("Load participants for session {SessionId}...", sessionId);

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
                participants = new List<ParticipantViewData>();

                var driverQuery = dbFactory.GetRepository<DriverRepository>()?.GetQuery();
                var natQuery = dbFactory.GetRepository<NationalityRepository>()?.GetQuery();
                var teamQuery = dbFactory.GetRepository<TeamRepository>()?.GetQuery();

                using (var participantsLoop = AppActivity.ApiSource.StartActivity("Participants_Loop"))
                {
                    foreach (var dbParticipant in dbParticipants)
                    {
                        var driverName = await GetDriverNameAsync(driverQuery, dbParticipant, dbParticipant.DriverName).ConfigureAwait(false);
                        var driverNat = await GetNationalityAsync(natQuery, dbParticipant).ConfigureAwait(false);
                        var teamName = await GetTeamAsync(teamQuery, dbParticipant).ConfigureAwait(false);

                        var participant = new ParticipantViewData
                                          {
                                              ParticipantDbId = dbParticipant.Id,
                                              DriverName = driverName,
                                              DriverNationality = driverNat,
                                              IsHumanControlled = dbParticipant.IsHumanControlled,
                                              IsMyTeam = dbParticipant.IsMyTeam != null && dbParticipant.IsMyTeam.Value,
                                              CarRaceNumber = dbParticipant.CarRaceNumber,
                                              TeamName = teamName
                                          };

                        participants.Add(participant);
                    }
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.LogInformation("Loaded {Participants} participants for session {SessionId}.", participants?.Count ?? 0, sessionId);

        return Ok(participants);
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Reading the team name
    /// </summary>
    /// <param name="teamQuery">Team query</param>
    /// <param name="dbParticipant">Participant</param>
    /// <returns>Team name</returns>
    private async Task<string> GetTeamAsync(TeamQueryable? teamQuery, ParticipantEntity dbParticipant)
    {
        var teamName = string.Empty;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetTeamAsync));

        if (teamQuery != null)
        {
            var team = await teamQuery.FirstOrDefaultAsync(t => t.Id == dbParticipant.TeamId)
                                      .ConfigureAwait(false);

            if (team != null)
            {
                teamName = team.Name;
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return teamName;
    }

    /// <summary>
    /// Reading drivers nationality
    /// </summary>
    /// <param name="natQuery">Nationality query</param>
    /// <param name="dbParticipant">Participant</param>
    /// <returns>Nationality</returns>
    private async Task<string> GetNationalityAsync(NationalityQueryable? natQuery, ParticipantEntity dbParticipant)
    {
        var driverNat = string.Empty;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetNationalityAsync));

        if (natQuery != null)
        {
            var nat = await natQuery.FirstOrDefaultAsync(n => n.Id == dbParticipant.NationalityId)
                                    .ConfigureAwait(false);

            if (nat != null)
            {
                driverNat = nat.Name;
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return driverNat;
    }

    /// <summary>
    /// Reading driver name
    /// </summary>
    /// <param name="driverQuery">Driver query</param>
    /// <param name="dbParticipant">Participant</param>
    /// <param name="driverName">Driver name</param>
    /// <returns>Driver name</returns>
    private async Task<string> GetDriverNameAsync(DriverQueryable? driverQuery, ParticipantEntity dbParticipant, string driverName)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetDriverNameAsync));

        if (driverQuery != null)
        {
            var driver = await driverQuery.FirstOrDefaultAsync(d => d.Id == dbParticipant.DriverId)
                                          .ConfigureAwait(false);

            if (driver != null)
            {
                driverName = driver.Name;
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        return driverName;
    }

    #endregion // Private methods
}