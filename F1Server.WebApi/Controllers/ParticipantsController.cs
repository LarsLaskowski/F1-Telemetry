using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Queryable;
using F1Server.Db.Entity.Repositories;
using F1Server.Db.Entity.Tables;

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
    public IActionResult GetParticipantsOfSession(long? sessionId)
    {
        List<ParticipantViewData>? participants = null;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetParticipantsOfSession));

        _logger?.LogInformation("Load participants for session {SessionId}...", sessionId);

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var dbParticipants = dbFactory.GetRepository<ParticipantRepository>()
                                          ?.GetQuery()
                                          ?.Where(s => s.SessionId == sessionId)
                                          .OrderByDescending(s => s.DriverId)
                                          .ToList() ?? [];

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
                        var driverName = GetDriverName(driverQuery, dbParticipant, dbParticipant.DriverName);
                        var driverNat = GetNationality(natQuery, dbParticipant);
                        var teamName = GetTeam(teamQuery, dbParticipant);

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
    private string GetTeam(TeamQueryable? teamQuery, ParticipantEntity dbParticipant)
    {
        var teamName = string.Empty;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetTeam));

        if (teamQuery != null)
        {
            var team = teamQuery.FirstOrDefault(t => t.Id == dbParticipant.TeamId);

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
    private string GetNationality(NationalityQueryable? natQuery, ParticipantEntity dbParticipant)
    {
        var driverNat = string.Empty;

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetNationality));

        if (natQuery != null)
        {
            var nat = natQuery.FirstOrDefault(n => n.Id == dbParticipant.NationalityId);

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
    private string GetDriverName(DriverQueryable? driverQuery, ParticipantEntity dbParticipant, string driverName)
    {
        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetDriverName));

        if (driverQuery != null)
        {
            var driver = driverQuery.FirstOrDefault(d => d.Id == dbParticipant.DriverId);

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