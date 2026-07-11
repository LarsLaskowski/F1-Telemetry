using F1Server.Data;

namespace F1Server.Tests;

/// <summary>
/// Test of the atomic packet counters in <see cref="TelemetryStatistics"/>
/// </summary>
[TestClass]
public class TelemetryStatisticsTests
{
    #region Properties

    /// <summary>
    /// Gets or sets the test context which provides information about and functionality for the current test run
    /// </summary>
    public TestContext TestContext { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Test to verify that a single increment raises both packet counters by one
    /// </summary>
    [TestMethod]
    public void TelemetryStatisticsIncrementPacketsReceivedRaisesBothCounters()
    {
        var statistics = new TelemetryStatistics();

        statistics.IncrementPacketsReceived();

        Assert.AreEqual(1L, statistics.PacketsReceivedTotal, "Total packet counter must be incremented!");
        Assert.AreEqual(1L, statistics.PacketsReceivedCurrentSession, "Current session packet counter must be incremented!");
    }

    /// <summary>
    /// Test to verify that concurrent increments from multiple threads lose no counts
    /// </summary>
    [TestMethod]
    public void TelemetryStatisticsIncrementPacketsReceivedParallelLosesNoCounts()
    {
        const int workerCount = 4;
        const int incrementsPerWorker = 100000;

        var statistics = new TelemetryStatistics();

        var workers = new Task[workerCount];

        for (var worker = 0; worker < workerCount; worker++)
        {
            workers[worker] = Task.Run(() => IncrementStatistics(statistics, incrementsPerWorker));
        }

        Task.WaitAll(workers, TestContext.CancellationToken);

        Assert.AreEqual((long)workerCount * incrementsPerWorker, statistics.PacketsReceivedTotal, "Total packet counter must not lose increments!");
        Assert.AreEqual((long)workerCount * incrementsPerWorker, statistics.PacketsReceivedCurrentSession, "Current session packet counter must not lose increments!");
    }

    /// <summary>
    /// Test to verify that a session change moves the current session counter to the last session counter
    /// </summary>
    [TestMethod]
    public void TelemetryStatisticsCheckChangeSessionMovesCurrentCounterToLastSession()
    {
        var statistics = new TelemetryStatistics();

        statistics.CheckChangeSession(1, 2025);

        statistics.IncrementPacketsReceived();
        statistics.IncrementPacketsReceived();

        statistics.CheckChangeSession(2, 2025);

        Assert.AreEqual(2L, statistics.PacketsReceivedLastSession, "Last session packet counter must contain the previous session count!");
        Assert.AreEqual(0L, statistics.PacketsReceivedCurrentSession, "Current session packet counter must be reset on session change!");
        Assert.AreEqual(2L, statistics.PacketsReceivedTotal, "Total packet counter must be unaffected by a session change!");
    }

    /// <summary>
    /// Increments the packet counters of the given statistics the given number of times
    /// </summary>
    /// <param name="statistics">Statistics to increment</param>
    /// <param name="count">Number of increments</param>
    private static void IncrementStatistics(TelemetryStatistics statistics, int count)
    {
        for (var increment = 0; increment < count; increment++)
        {
            statistics.IncrementPacketsReceived();
        }
    }

    #endregion // Methods
}