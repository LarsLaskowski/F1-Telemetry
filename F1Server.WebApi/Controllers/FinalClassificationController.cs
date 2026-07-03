using System.Diagnostics;

using F1Server.Core.Observability;
using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace F1Server.WebApi.Controllers;

/// <summary>
/// Final classification controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FinalClassificationController : ControllerBase
{
    #region Fields

    private readonly ILogger<FinalClassificationController> _logger;

    #endregion // Fields

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logging interface</param>
    public FinalClassificationController(ILogger<FinalClassificationController> logger)
    {
        _logger = logger;
    }

    #endregion // Constructors

    #region Methods

    /// <summary>
    /// Get final classification for session
    /// </summary>
    /// <param name="sessionId">Id of session</param>
    /// <returns>Final classification data</returns>
    [Route("{sessionId?}")]
    [HttpGet]
    public IActionResult GetFromSession(long? sessionId)
    {
        var finalClassifications = new List<FinalClassificationViewData>();

        using var currentActivity = AppActivity.ApiSource.StartActivity(nameof(GetFromSession));

        _logger?.LogInformation("Get final classification for session {SessionId}...", sessionId);

        try
        {
            using (var dbFactory = RepositoryFactory.CreateInstance())
            {
                var dbFinals = dbFactory.GetRepository<FinalClassificationRepository>()
                                        ?.GetQuery()
                                        ?.Include(f => f.Participant)
                                        ?.Where(f => f.SessionId == sessionId)
                                        ?.OrderBy(f => f.FinishPosition)
                                        ?.ToList();

                if (dbFinals?.Count > 0)
                {
                    var leaderTime = dbFinals.FirstOrDefault()?.TotalRaceTime;
                    var leaderLaps = dbFinals.FirstOrDefault()?.LapsDriven;

                    foreach (var finalClassification in dbFinals)
                    {
                        finalClassifications.Add(new FinalClassificationViewData
                                                 {
                                                     DbId = finalClassification.Id,
                                                     ParticipantDbId = finalClassification.ParticipantId,
                                                     ArrayIndex = finalClassification.Participant.ArrayIndex,
                                                     DriverName = finalClassification.Participant.Driver.Name,
                                                     CarNumber = finalClassification.Participant.CarRaceNumber,
                                                     TeamName = finalClassification.Participant.Team.Name,
                                                     Nationality = finalClassification.Participant.Nationality.Name,
                                                     LapsDriven = finalClassification.LapsDriven,
                                                     StartingPosition = finalClassification.GridPosition,
                                                     FinishPosition = finalClassification.FinishPosition,
                                                     NumberOfPenalties = finalClassification.NumberOfPenalties,
                                                     PitStops = finalClassification.PitStops,
                                                     PenaltiesTime = finalClassification.PenaltiesTime,
                                                     TotalRaceTime = TimeSpan.FromSeconds(finalClassification.TotalRaceTime).ToString(@"mm\:ss\.fff"),
                                                     TotalRaceTimeRaw = finalClassification.TotalRaceTime,
                                                     FastestLapTime = finalClassification.FastestLapTime > 0 ? TimeSpan.FromMilliseconds(finalClassification.FastestLapTime).ToString(@"mm\:ss\.fff") : "-",
                                                     FastestLapTimeRaw = finalClassification.FastestLapTime,
                                                     RaceTimeDifference = BuildTimeDifference(leaderTime, finalClassification.TotalRaceTime, leaderLaps, finalClassification.LapsDriven)
                                                 });
                    }

                    var fastestSessionLapTime = finalClassifications.Where(f => f.FastestLapTimeRaw > 0)
                                                                    .MinBy(f => f.FastestLapTimeRaw);

                    if (fastestSessionLapTime != null)
                    {
                        fastestSessionLapTime.IsFastestSessionLapTime = true;

                        BuildTimeDifferenceFastestLap(fastestSessionLapTime.FastestLapTimeRaw, finalClassifications);
                    }
                }
            }

            currentActivity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Exception while loading final classifications => {Exception}", ex.ToString());

            currentActivity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
            currentActivity?.AddException(ex);
        }

        _logger?.LogInformation("Final classification for session loaded: {Count}", finalClassifications.Count);

        return Ok(finalClassifications);
    }

    /// <summary>
    /// Build time difference between fastest lap in session and personal best lap time
    /// </summary>
    /// <param name="fastestSessionLapTime">Fastest lap time</param>
    /// <param name="finalClassifications">Time table</param>
    private void BuildTimeDifferenceFastestLap(uint fastestSessionLapTime, List<FinalClassificationViewData> finalClassifications)
    {
        if (finalClassifications.Count > 0 && fastestSessionLapTime > 0)
        {
            foreach (var attendee in finalClassifications)
            {
                attendee.FastestLapTimeDifference = BuildTimeDifference(fastestSessionLapTime, attendee.FastestLapTimeRaw);
            }
        }
    }

    /// <summary>
    /// Build time difference between leader and race time of current driver
    /// </summary>
    /// <param name="leaderTime">Fastest race time</param>
    /// <param name="raceTime">Current race time</param>
    /// <param name="leaderLaps">Driven laps of leader</param>
    /// <param name="driverLaps">Driven laps of current driver</param>
    /// <returns>Time difference as string</returns>
    private string BuildTimeDifference(double? leaderTime, double raceTime, int? leaderLaps, int driverLaps)
    {
        var timeDiff = "0.000";

        if (leaderLaps.HasValue && leaderTime.HasValue && leaderTime.Value < raceTime)
        {
            if (driverLaps < leaderLaps)
            {
                timeDiff = $"+ {leaderLaps.Value - driverLaps} lap(s)";
            }
            else
            {
                var diff = TimeSpan.FromSeconds(raceTime - leaderTime.Value);

                timeDiff = diff.Minutes > 0 ? diff.ToString(@"\+m\:ss\.fff") : diff.ToString(@"\+s\.fff");
            }
        }

        return timeDiff;
    }

    /// <summary>
    /// Build time difference between leader and race time of current driver
    /// </summary>
    /// <param name="fastestLapTime">Fastest lap time</param>
    /// <param name="currentLapTime">Personal best lap time</param>
    /// <returns>Time difference as string</returns>
    private string BuildTimeDifference(uint fastestLapTime, uint currentLapTime)
    {
        var timeDiff = string.Empty;

        if (fastestLapTime < currentLapTime)
        {
            var diff = TimeSpan.FromMilliseconds(currentLapTime - fastestLapTime);

            timeDiff = diff.Minutes > 0 ? diff.ToString(@"\+m\:ss\.fff") : diff.ToString(@"\+s\.fff");
        }

        return timeDiff;
    }

    #endregion // Methods
}