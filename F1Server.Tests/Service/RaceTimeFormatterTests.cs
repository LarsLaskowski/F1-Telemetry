using F1Server.Service.Core;

namespace F1Server.Tests.Service;

/// <summary>
/// Contains unit tests verifying that the <see cref="RaceTimeFormatter"/> keeps the hours of a race time and only
/// omits them for races shorter than one hour
/// </summary>
[TestClass]
public class RaceTimeFormatterTests
{
    #region Methods

    /// <summary>
    /// Verifies that a race shorter than one hour is formatted without an hours part
    /// </summary>
    [TestMethod]
    public void RaceTimeFormatterFormatTotalRaceTimeBelowOneHourOmitsHours()
    {
        var raceTime = RaceTimeFormatter.FormatTotalRaceTime(81);

        Assert.AreEqual("01:21.000", raceTime, "A race shorter than one hour should be formatted without hours!");
    }

    /// <summary>
    /// Verifies that a race of exactly one hour is formatted with an hours part
    /// </summary>
    [TestMethod]
    public void RaceTimeFormatterFormatTotalRaceTimeOfExactlyOneHourKeepsHours()
    {
        var raceTime = RaceTimeFormatter.FormatTotalRaceTime(3600);

        Assert.AreEqual("1:00:00.000", raceTime, "A race of exactly one hour should be formatted with hours!");
    }

    /// <summary>
    /// Verifies that a race longer than one hour keeps its hours instead of dropping them
    /// </summary>
    [TestMethod]
    public void RaceTimeFormatterFormatTotalRaceTimeAboveOneHourKeepsHours()
    {
        var raceTime = RaceTimeFormatter.FormatTotalRaceTime(5430);

        Assert.AreEqual("1:30:30.000", raceTime, "A race longer than one hour should be formatted with hours!");
    }

    /// <summary>
    /// Verifies that the milliseconds of a race time are preserved
    /// </summary>
    [TestMethod]
    public void RaceTimeFormatterFormatTotalRaceTimeKeepsMilliseconds()
    {
        var raceTime = RaceTimeFormatter.FormatTotalRaceTime(3700.456);

        Assert.AreEqual("1:01:40.456", raceTime, "The milliseconds of a race time should be preserved!");
    }

    /// <summary>
    /// Verifies that a race time of more than one day is formatted with the total number of hours
    /// </summary>
    [TestMethod]
    public void RaceTimeFormatterFormatTotalRaceTimeAboveOneDayReturnsTotalHours()
    {
        var raceTime = RaceTimeFormatter.FormatTotalRaceTime(TimeSpan.FromHours(25));

        Assert.AreEqual("25:00:00.000", raceTime, "A race time of more than one day should be formatted with the total number of hours!");
    }

    #endregion // Methods
}