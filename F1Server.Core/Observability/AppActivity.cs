using System.Diagnostics;

namespace F1Server.Core.Observability;

/// <summary>
/// Application telemetry object
/// </summary>
public static class AppActivity
{
    #region Fields

    /// <summary>
    /// Application source name WebAPI
    /// </summary>
    public static readonly ActivitySource ApiSource = new("F1-Telemetry-WebAPI", "1.0");

    /// <summary>
    /// Application source name server
    /// </summary>
    public static readonly ActivitySource SrvSource = new("F1-Telemetry", "1.0");

    #endregion // Fields
}