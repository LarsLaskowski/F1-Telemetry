using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Session data for F1 2022
/// </summary>
public interface ISessionData2022 : ISessionData2021
{
    #region Properties

    /// <summary>
    /// Game mode
    /// </summary>
    GameMode GameMode { get; }

    /// <summary>
    /// Ruleset
    /// </summary>
    RuleSet RuleSet { get; }

    /// <summary>
    /// Local time of day - minutes since midnight
    /// </summary>
    uint LocalTimeOfDay { get; }

    /// <summary>
    /// Length of session
    /// </summary>
    SessionLength SessionLength { get; }

    #endregion // Properties
}