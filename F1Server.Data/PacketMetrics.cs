namespace F1Server.Data;

/// <summary>
/// Metric per packet type
/// </summary>
public class PacketMetrics
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public PacketMetrics()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="sourceMetrics">Source object</param>
    public PacketMetrics(PacketMetrics sourceMetrics)
    {
        Received = sourceMetrics.Received;
        TotalProcessingTime = sourceMetrics.TotalProcessingTime;
    }

    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Packets received
    /// </summary>
    public ulong Received { get; set; }

    /// <summary>
    /// Total processing time in milliseconds
    /// </summary>
    public double TotalProcessingTime { get; set; }

    /// <summary>
    /// Average packet processing time
    /// </summary>
    public double AvgProcessingTime => Received > 0 ? (TotalProcessingTime / (Received * 1.0)) : 0;

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Reset data
    /// </summary>
    public void Reset()
    {
        Received = 0UL;
        TotalProcessingTime = 0L;
    }

    #endregion // Methods
}