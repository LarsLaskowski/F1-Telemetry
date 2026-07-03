using F1Server.Core.Enumerations;

namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface participant data of session (F1 2023)
/// </summary>
public interface IParticipantData2023 : IParticipantData2021
{
    #region Properties

    /// <summary>
    /// Players show online name setting
    /// </summary>
    bool IsShowOnlineNames { get; set; }

    /// <summary>
    /// Platform of player
    /// </summary>
    Platforms Platform { get; set; }

    #endregion // Properties
}