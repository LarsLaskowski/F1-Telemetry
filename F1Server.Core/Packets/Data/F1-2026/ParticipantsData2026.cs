using F1Server.Core.Data;
using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Data class for each participants packet - F1 2026
/// </summary>
public class ParticipantsData2026 : IParticipantsBase
{
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    public ParticipantsData2026()
    {
        Participants = new ParticipantData2026[ConstData.F12026MaxCars];
    }

    #endregion // Constructors

    #region IParticipantsBase

    /// <inheritdoc/>
    public ushort ActiveCars { get; set; }

    /// <inheritdoc/>
    public IParticipantDataBase[] Participants { get; set; }

    #endregion // IParticipantsBase
}