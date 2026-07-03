using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class for each participants packet - F1 2025
/// </summary>
public class ParticipantsData2025 : IParticipantsBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public ParticipantsData2025()
    {
        Participants = new ParticipantData2025[22];
    }

    #endregion // Constructors

    #region IParticipantsBase

    /// <inheritdoc/>
    public ushort ActiveCars { get; set; }

    /// <inheritdoc/>
    public IParticipantDataBase[] Participants { get; set; }

    #endregion // IParticipantsBase
}