namespace F1Server.Service.Core;

/// <summary>
/// Formatter for total race times
/// </summary>
public static class RaceTimeFormatter
{
    #region Constants

    /// <summary>
    /// Format string for the minutes, seconds and milliseconds part of a race time
    /// </summary>
    private const string MinutesFormat = @"mm\:ss\.fff";

    #endregion // Constants

    #region Static methods

    /// <summary>
    /// Format a total race time given in seconds, races shorter than one hour keep the compact minutes format
    /// </summary>
    /// <param name="totalRaceTimeSeconds">Total race time in seconds</param>
    /// <returns>Formatted total race time</returns>
    public static string FormatTotalRaceTime(double totalRaceTimeSeconds)
    {
        return FormatTotalRaceTime(TimeSpan.FromSeconds(totalRaceTimeSeconds));
    }

    /// <summary>
    /// Format a total race time, races shorter than one hour keep the compact minutes format
    /// </summary>
    /// <param name="totalRaceTime">Total race time</param>
    /// <returns>Formatted total race time</returns>
    public static string FormatTotalRaceTime(TimeSpan totalRaceTime)
    {
        var minutesPart = totalRaceTime.ToString(MinutesFormat);

        return totalRaceTime.TotalHours < 1 ? minutesPart : $"{(int)totalRaceTime.TotalHours}:{minutesPart}";
    }

    #endregion // Static methods
}