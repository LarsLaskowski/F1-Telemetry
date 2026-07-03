namespace F1Server.Core.Packets.Interfaces;

/// <summary>
/// Interface for all packets of type participants
/// </summary>
public interface IParticipantsBase
{
    #region Properties

    /// <summary>
    /// Active cars
    /// </summary>
    ushort ActiveCars { get; set; }

    /// <summary>
    /// Data about participants in current session
    /// </summary>
    IParticipantDataBase[] Participants { get; set; }

    #endregion // Properties
}