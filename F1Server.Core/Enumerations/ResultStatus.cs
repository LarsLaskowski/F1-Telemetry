namespace F1Server.Core.Enumerations;

/// <summary>
/// Current result status
/// </summary>
public enum ResultStatus
{
    /// <summary>
    /// Not set
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Invalid
    /// </summary>
    Invalid = 1,

    /// <summary>
    /// Inactive
    /// </summary>
    Inactive,

    /// <summary>
    /// Active
    /// </summary>
    Active,

    /// <summary>
    /// Finished
    /// </summary>
    Finished,

    /// <summary>
    /// Disqualified
    /// </summary>
    Disqualified,

    /// <summary>
    /// Not classified
    /// </summary>
    NotClassified,

    /// <summary>
    /// Retired
    /// </summary>
    Retired,

    /// <summary>
    /// Did not finished
    /// </summary>
    DidNotFinished
}