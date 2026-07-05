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
    Console = 1,

    /// <summary>
    /// Open telemetry standard
    /// </summary>
    OpenTelemetry = 3
}