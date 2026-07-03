namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface participant data of session (F1 2021)
/// </summary>
public interface IParticipantData2021 : IParticipantDataBase
{
    #region Properties

    /// <summary>
    /// Identifier for network players
    /// </summary>
    ushort NetworkId { get; set; }

    /// <summary>
    /// Is my team?
    /// </summary>
    bool IsMyTeam { get; set; }

    #endregion // Properties
}