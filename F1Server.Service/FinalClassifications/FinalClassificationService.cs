using F1Server.Data.ViewData;
using F1Server.Db.Entity;
using F1Server.Db.Entity.Repositories;
using F1Server.Service.Core;

using Microsoft.EntityFrameworkCore;

namespace F1Server.Service.FinalClassifications;

/// <summary>
/// Business logic of the final classification - reads the classification of a session and calculates the race time
/// and lap time differences of its drivers
/// </summary>
public class FinalClassificationService
{
    #region Constants

    /// <summary>
    /// Time difference reported for a driver that is not behind the leader
    /// </summary>
    private const string NoRaceTimeDifference = "0.000";

    /// <summary>
    /// Format string of a time difference of less than one minute
    /// </summary>
    private const string SecondsDifferenceFormat = @"\+s\.fff";

    /// <summary>
    /// Format string of a time difference of at least one minute
    /// </summary>
    private const string MinutesDifferenceFormat = @"\+m\:ss\.fff";

    #endregion // Constants

    #region Methods

    /// <summary>
    /// Loads the final classification of a session ordered by the finish position of its drivers
    /// </summary>
    /// <param name="sessionId">Database id of the session</param>
    /// <returns>Final classification of the session</returns>
    public async Task<List<FinalClassificationViewData>> GetFinalClassificationsOfSessionAsync(long? sessionId)
    {
        var finalClassifications = new List<FinalClassificationViewData>();

        using (var dbFactory = RepositoryFactory.CreateInstance())
        {
            var finalQuery = dbFactory.GetRepository<FinalClassificationRepository>()?.GetQuery();

            var dbFinals = finalQuery == null
                               ? null
                               : await finalQuery.Include(f => f.Participant)
                                                 .Where(f => f.SessionId == sessionId)
                                                 .OrderBy(f => f.FinishPosition)
                                                 .ToListAsync()
                                                 .ConfigureAwait(false);

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
                                                 TotalRaceTime = RaceTimeFormatter.FormatTotalRaceTime(finalClassification.TotalRaceTime),
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

        return finalClassifications;
    }

    #endregion // Methods

    #region Private methods

    /// <summary>
    /// Build time difference between fastest lap in session and personal best lap time
    /// </summary>
    /// <param name="fastestSessionLapTime">Fastest lap time</param>
    /// <param name="finalClassifications">Time table</param>
    private static void BuildTimeDifferenceFastestLap(uint fastestSessionLapTime, List<FinalClassificationViewData> finalClassifications)
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
    private static string BuildTimeDifference(double? leaderTime, double raceTime, int? leaderLaps, int driverLaps)
    {
        var timeDiff = NoRaceTimeDifference;

        if (leaderLaps.HasValue && leaderTime.HasValue && leaderTime.Value < raceTime)
        {
            timeDiff = driverLaps < leaderLaps
                           ? $"+ {leaderLaps.Value - driverLaps} lap(s)"
                           : FormatTimeDifference(TimeSpan.FromSeconds(raceTime - leaderTime.Value));
        }

        return timeDiff;
    }

    /// <summary>
    /// Build time difference between leader and race time of current driver
    /// </summary>
    /// <param name="fastestLapTime">Fastest lap time</param>
    /// <param name="currentLapTime">Personal best lap time</param>
    /// <returns>Time difference as string</returns>
    private static string BuildTimeDifference(uint fastestLapTime, uint currentLapTime)
    {
        var timeDiff = string.Empty;

        if (fastestLapTime < currentLapTime)
        {
            timeDiff = FormatTimeDifference(TimeSpan.FromMilliseconds(currentLapTime - fastestLapTime));
        }

        return timeDiff;
    }

    /// <summary>
    /// Formats a time difference, differences of less than one minute are reported without their minutes part
    /// </summary>
    /// <param name="timeDifference">Time difference</param>
    /// <returns>Formatted time difference</returns>
    private static string FormatTimeDifference(TimeSpan timeDifference)
    {
        return timeDifference.Minutes > 0
                   ? timeDifference.ToString(MinutesDifferenceFormat)
                   : timeDifference.ToString(SecondsDifferenceFormat);
    }

    #endregion // Private methods
}