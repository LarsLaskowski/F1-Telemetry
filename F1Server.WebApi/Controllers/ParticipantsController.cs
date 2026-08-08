using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
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

        _logger?.LoadingParticipants(sessionId);

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

                using (var participantsLoop = AppActivity.ApiSource.StartActivity("Participants_Loop"))
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

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }

        _logger?.ParticipantsLoaded(participants?.Count ?? 0, sessionId);

        return Ok(participants);
    }

    #endregion // Methods
}