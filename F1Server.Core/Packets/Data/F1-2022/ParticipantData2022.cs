using F1Server.Core.Packets.Interfaces;

namespace F1Server.Core.Packets.Data;

/// <summary>
/// Implementation of the participant data 2022
/// </summary>
public class ParticipantData2022 : IParticipantData2022
{
    #region IParticipantDataBase

    /// <inheritdoc/>
    public bool IsAIControlled { get; set; }

    /// <inheritdoc/>
    public ushort DriverId { get; set; }

    /// <inheritdoc/>
    public ushort TeamId { get; set; }

    /// <inheritdoc/>
    public ushort RaceNumber { get; set; }

    /// <inheritdoc/>
    public ushort Nationality { get; set; }

    /// <inheritdoc/>
    public string DriverName { get; set; }

    /// <inheritdoc/>
    public bool IsPublicTelemetry { get; set; }

    #endregion // IParticipantDataBase

    #region IParticipantData2021

    /// <inheritdoc/>
    public ushort NetworkId { get; set; }

    /// <inheritdoc/>
    public bool IsMyTeam { get; set; }

    #endregion // IParticipantData2021
}