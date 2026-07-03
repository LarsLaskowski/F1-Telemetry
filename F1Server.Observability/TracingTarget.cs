namespace F1Server.Observability;

/// <summary>
/// Tracing target framework
/// </summary>
public enum TracingTarget
{
    /// <summary>
    /// No tracing target set
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// Export to console
    /// </summary>
    Console,

    /// <summary>
    /// Export to zipkin
    /// </summary>
    [Obsolete]
    Zipkin,

    /// <summary>
    /// Open telemtry standard
    /// </summary>
    OpenTelemetry
}