using F1Server.Core.Enumerations;

namespace F1Server.WebApi;

/// <summary>
/// Represents the base data for a session, including identifiers and session-specific details
/// </summary>
internal class SessionBaseData
{
    #region Properties

    /// <summary>
    /// Gets or sets the unique identifier for the current session
    /// </summary>
    public long SessionId { get; set; }

    /// <summary>
    /// Gets or sets the type of session associated with the current context
    /// </summary>
    public SessionType SessionType { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for a track
    /// </summary>
    public long TrackId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the game version
    /// </summary>
    public long GameVersionId { get; set; }

    /// <summary>
    /// Gets or sets the formula type used to define the calculation logic
    /// </summary>
    public Formula FormulaType { get; set; }

    #endregion // Properties
}