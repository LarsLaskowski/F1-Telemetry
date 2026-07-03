using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class for each participants packet - F1 2021
/// </summary>
public class ParticipantsData2021 : IParticipantsBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public ParticipantsData2021()
    {
        Participants = new ParticipantData2021[22];
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