using System.Diagnostics;

namespace F1ReplayClient.Data;

/// <summary>
/// Collected timings of a running replay, used to show where the processing time of a packet is spent
/// </summary>
internal class ReplayTimingData
{
    #region Properties

    /// <summary>
    /// Accumulated time in stopwatch ticks spent reading the packet files
    /// </summary>
    public long ReadTicks { get; set; }

    /// <summary>
    /// Accumulated time in stopwatch ticks spent analyzing the packets
    /// </summary>
    public long AnalyzeTicks { get; set; }

    /// <summary>
    /// Accumulated time in stopwatch ticks spent establishing the connection to the server
    /// </summary>
    public long ConnectTicks { get; set; }

    /// <summary>
    /// Accumulated time in stopwatch ticks spent sending the packets to the server
    /// </summary>
    public long SendTicks { get; set; }

    /// <summary>
    /// Number of established connections, a value above one means the connection was dropped and rebuilt
    /// </summary>
    public int Connects { get; set; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Creates the status text with the throughput and the average time per packet of every step
    /// </summary>
    /// <param name="processedFiles">Number of processed files</param>
    /// <param name="elapsed">Elapsed time since the replay was started</param>
    /// <returns>Status text</returns>
    public string CreateStatusText(int processedFiles, TimeSpan elapsed)
    {
        if (processedFiles == 0)
        {
            return string.Empty;
        }

        var packetsPerSecond = elapsed.TotalSeconds > 0
                                   ? processedFiles / elapsed.TotalSeconds
                                   : 0;

        return $"{packetsPerSecond:F1} packets/s | read {TicksToMilliseconds(ReadTicks, processedFiles):F3} ms | analyze {TicksToMilliseconds(AnalyzeTicks, processedFiles):F3} ms | connect {TicksToMilliseconds(ConnectTicks, processedFiles):F3} ms | send {TicksToMilliseconds(SendTicks, processedFiles):F3} ms | connects {Connects}";
    }

    /// <summary>
    /// Converts accumulated stopwatch ticks into the average number of milliseconds per packet
    /// </summary>
    /// <param name="ticks">Accumulated stopwatch ticks</param>
    /// <param name="processedFiles">Number of processed files</param>
    /// <returns>Average milliseconds per packet</returns>
    private static double TicksToMilliseconds(long ticks, int processedFiles)
    {
        return ticks * 1000.0D / Stopwatch.Frequency / processedFiles;
    }

    #endregion // Methods
}