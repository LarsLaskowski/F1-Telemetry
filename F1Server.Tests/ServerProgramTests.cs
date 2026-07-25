using F1Server.Data;

namespace F1Server.Tests;

/// <summary>
/// Tests of the statistics console output in <see cref="Program"/>
/// </summary>
[TestClass]
public class ServerProgramTests
{
    #region Static methods

    /// <summary>
    /// Invokes <see cref="Program.WriteStatistics"/> and captures the console output it writes
    /// </summary>
    /// <param name="statistics">Statistic data passed to the method</param>
    /// <param name="useLogging">Whether the packet logging statistics have to be written</param>
    /// <returns>Captured console output</returns>
    private static string InvokeWriteStatistics(TelemetryStatistics statistics, bool useLogging)
    {
        var originalOut = Console.Out;

        try
        {
            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);

                Program.WriteStatistics(statistics, useLogging);

                return writer.ToString();
            }
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    #endregion // Static methods

    #region Methods

    /// <summary>
    /// Test to verify that the statistics output contains the average processing time per packet
    /// </summary>
    [TestMethod]
    public void ProgramWriteStatisticsWithProcessedPacketsWritesAverageProcessingTime()
    {
        var statistics = new TelemetryStatistics
                         {
                             TotalPacketProcessingTime = 15,
                             TotalPacketsProcessed = 4
                         };

        var output = InvokeWriteStatistics(statistics, false);

        Assert.Contains($"Average packet processing time     : {3.75:F2} ms", output, "Expected average packet processing time not found!");
    }

    /// <summary>
    /// Test to verify that the statistics output contains the average log time per packet while logging is active
    /// </summary>
    [TestMethod]
    public void ProgramWriteStatisticsWithLoggingWritesAverageLogTime()
    {
        var statistics = new TelemetryStatistics
                         {
                             TotalPacketLogTime = 9,
                             TotalPacketsLogged = 6
                         };

        var output = InvokeWriteStatistics(statistics, true);

        Assert.Contains($"Average packet log time            : {1.5:F2} ms", output, "Expected average packet log time not found!");
    }

    /// <summary>
    /// Test to verify that the statistics output omits the average log time while logging is inactive
    /// </summary>
    [TestMethod]
    public void ProgramWriteStatisticsWithoutLoggingOmitsAverageLogTime()
    {
        var statistics = new TelemetryStatistics();

        var output = InvokeWriteStatistics(statistics, false);

        Assert.DoesNotContain("Average packet log time", output, "Unexpected average packet log time found!");
    }

    /// <summary>
    /// Test to verify that the statistics output writes zero averages while no packet has been processed or logged
    /// </summary>
    [TestMethod]
    public void ProgramWriteStatisticsWithoutPacketsWritesZeroAverages()
    {
        var statistics = new TelemetryStatistics();

        var output = InvokeWriteStatistics(statistics, true);

        Assert.Contains($"Average packet processing time     : {0d:F2} ms", output, "Average packet processing time must be zero without processed packets!");
        Assert.Contains($"Average packet log time            : {0d:F2} ms", output, "Average packet log time must be zero without logged packets!");
    }

    #endregion // Methods
}