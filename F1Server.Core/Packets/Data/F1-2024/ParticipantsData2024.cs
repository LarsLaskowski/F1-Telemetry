using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class for each participants packet - F1 2024
/// </summary>
public class ParticipantsData2024 : IParticipantsBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public ParticipantsData2024()
    {
        Participants = new ParticipantData2024[22];
    }

    #endregion // Constructors

    #region IParticipantsBase

    /// <summary>
    /// Active cars
    /// </summary>
    public ushort ActiveCars { get; set; }

    /// <summary>
    /// Data about participants in current session
    /// </summary>
    public IParticipantDataBase[] Participants { get; set; }

    #endregion // IParticipantsBase
}